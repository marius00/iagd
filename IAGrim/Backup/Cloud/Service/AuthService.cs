using System;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using CefSharp;
using EvilsoftCommons.Exceptions;
using IAGrim.Backup.Cloud.CefSharp.Events;
using IAGrim.Database.Interfaces;
using IAGrim.UI.Misc.CEF;
using IAGrim.Utilities.HelperClasses;
using log4net;
using Newtonsoft.Json;

namespace IAGrim.Backup.Cloud.Service {
    public class AuthService {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(AuthService));
        private const string CacheKey = "IAGDIsCloudAuthenticated";
        private readonly ICefBackupAuthentication _authentication;
        private readonly AuthenticationProvider _authenticationProvider;
        private readonly IPlayerItemDao _playerItemDao;

        public enum AccessStatus {
            Authorized,
            Unauthorized,
            Unknown
        }

        public AuthService(ICefBackupAuthentication authentication, AuthenticationProvider authenticationProvider, IPlayerItemDao playerItemDao) {
            _authentication = authentication;
            _authenticationProvider = authenticationProvider;
            _playerItemDao = playerItemDao;
            _authentication.OnSuccess += AuthenticationOnSuccess;
        }

        private void AuthenticationOnSuccess(object sender, EventArgs eventArgs) {
            var args = eventArgs as AuthResultEvent;
            (sender as IBrowser)?.CloseBrowser(true);

            if (IsTokenValid(args.User, args.Token) == AccessStatus.Authorized) {
                Logger.Info($"Token validated for {args.User}");
                _authenticationProvider.SetToken(args.User, args.Token);
                MemoryCache.Default.Set(CacheKey, true, DateTimeOffset.Now.AddDays(1));
            }
            else {
                Logger.Warn($"Token for {args.User} failed validation");
            }
        }

        private static AccessStatus IsTokenValid(string user, string token) {
            try {
                using (var httpClient = new HttpClient {Timeout = TimeSpan.FromSeconds(5)}) {
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", token);
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Api-User", user);


                    var result = httpClient.GetAsync(Uris.TokenVerificationUri).Result;
                    var status = result.StatusCode;

                    if (status == HttpStatusCode.OK) {
                        Logger.Info($"Got Status {result} verifying authentication token");
                        return AccessStatus.Authorized;
                    }
                    else if (status == HttpStatusCode.Unauthorized || status == HttpStatusCode.Forbidden) {
                        Logger.Warn("Got unauthorized validating access token");
                        return AccessStatus.Unauthorized;
                    }
                    else if (status == HttpStatusCode.InternalServerError) {
                        ExceptionReporter.ReportIssue("Server response 500 verifying access token");
                        Logger.Warn("Server response 500 verifying access token towards backup service");
                        return AccessStatus.Unknown;
                    }
                    else {
                        Logger.Error($"Got Status {result} verifying authentication token");
                        ExceptionReporter.ReportIssue($"Server response {result} verifying access token");
                        return AccessStatus.Unknown;
                    }
                }
            }
            catch (AggregateException ex) {
                Logger.Warn(ex.Message, ex);
                return AccessStatus.Unknown;
            }
            catch (WebException ex) {
                Logger.Warn(ex.Message, ex);
                return AccessStatus.Unknown;
            }
        }

        class MigrateResponseType {
            public string Token { get; set; }
            public string Email { get; set; }
        }

        private AccessStatus Migrate() {
            try {
                Logger.Info("Starting migration to new backup provider");
                var httpClient = new HttpClient(new HttpClientHandler {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                }) {Timeout = TimeSpan.FromSeconds(15)};

                var url = Uris.MigrateUrl + "?token=" + _authenticationProvider.GetLegacyToken();
                var result = httpClient.GetAsync(url).Result;
                var status = result.StatusCode;

                if (status == HttpStatusCode.OK) {
                    Logger.Info($"Got Status {result} migrating authentication token");
                    var body = result.Content.ReadAsStringAsync().Result;
                    var accessToken = JsonConvert.DeserializeObject<MigrateResponseType>(body);
                    _authenticationProvider.SetToken(accessToken.Email, accessToken.Token);

                    return AccessStatus.Authorized;
                }
                else if (status == HttpStatusCode.Unauthorized || status == HttpStatusCode.Forbidden) {
                    Logger.Warn("Migration failed, unauthorized");
                    return AccessStatus.Unauthorized;
                }
                else if (status == HttpStatusCode.InternalServerError) {
                    ExceptionReporter.ReportIssue("Server response 500 migrating access token");
                    Logger.Warn("Server response 500 migrating access token towards backup service");
                    return AccessStatus.Unknown;
                }
                else {
                    Logger.Error($"Got Status {result} migrating authentication token");
                    ExceptionReporter.ReportIssue($"Server response {result} migrating access oken");

                    return AccessStatus.Unknown;
                }
            }
            catch (AggregateException ex) {
                Logger.Warn(ex.Message, ex);
                return AccessStatus.Unknown;
            }
            catch (WebException ex) {
                Logger.Warn(ex.Message, ex);
                return AccessStatus.Unknown;
            }
        }

        public void Logout() {
            try {
                if (_authenticationProvider.HasToken()) {
                    Logger.Info("Logging out of cloud backups");
                    var httpClient = new HttpClient {Timeout = TimeSpan.FromSeconds(5)};
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", _authenticationProvider.GetToken());
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Api-User", _authenticationProvider.GetUser());

                    var status = httpClient.GetAsync(Uris.LogoutUrl).Result.StatusCode;

                    if (status == HttpStatusCode.OK) {
                        Logger.Info($"Successfully logged out of cloud backups");
                    }
                    else {
                        Logger.Info($"Failed logging out of cloud backups, status code {status} (this is OK, client still deletes token and gets logged out)");
                    }
                }
            }
            catch (Exception ex) {
                Logger.Warn(ex.Message, ex);
            }
            finally {
                UnAuthenticate();
            }
        }

        public AccessStatus CheckAuthentication() {
            if (MemoryCache.Default[CacheKey] is AccessStatus cached) {
                return cached;
            }

            if (_authenticationProvider.CanMigrate()) {
                // We have a legacy access token, can migrate to the new cloud solution.
                return Migrate();
            }
            else if (_authenticationProvider.HasToken()) {
                var result = IsTokenValid(_authenticationProvider.GetUser(), _authenticationProvider.GetToken());
                MemoryCache.Default.Set(CacheKey, result, DateTimeOffset.Now.AddDays(1));

                if (result == AccessStatus.Unauthorized) {
                    Logger.Warn("Existing authentication token is invalid, clearing authentication provider");
                    _authenticationProvider.SetToken(string.Empty, string.Empty);
                    _playerItemDao.ResetOnlineSyncState();
                }

                return result;
            }
            else {
                MemoryCache.Default.Set(CacheKey, false, DateTimeOffset.Now.AddDays(1));
            }

            return AccessStatus.Unauthorized;
        }

        public void Authenticate() {
            // Open chromium
            // Redirect to protected page
            // Protected page redirects to magic://something
            // Chromium hook notifies authentication complete
            // Close chromium popup
            // Mark as authenticated

            _authentication.Open(Uris.LoginPageUrl);
        }

        /// <summary>
        /// Logout
        /// </summary>
        public void UnAuthenticate( /* args: This <-> All */) {
            _authenticationProvider.SetToken(string.Empty, string.Empty);
            MemoryCache.Default.Set(CacheKey, AccessStatus.Unauthorized, DateTimeOffset.Now.AddDays(1));
        }

        public RestService GetRestService() {
            if (CheckAuthentication() == AccessStatus.Authorized) {
                var token = _authenticationProvider.GetToken();

                HttpClientHandler handler = new HttpClientHandler() {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                };

                var httpClient = new HttpClient(handler);
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", token);
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Api-User", _authenticationProvider.GetUser());
                httpClient.Timeout = TimeSpan.FromSeconds(5);
                return new RestService(httpClient);
            }

            return null;
        }
    }
}