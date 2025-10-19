
using EvilsoftCommons.Exceptions;
using IAGrim.Backup.Cloud.CefSharp.Events;
using IAGrim.Database.Interfaces;
using IAGrim.UI.Misc.CEF;
using IAGrim.Utilities.HelperClasses;
using log4net;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.Caching;

namespace IAGrim.Backup.Cloud.Service {
    /// <summary>
    /// Not reuseable. Create a new instance for new attempts.
    /// </summary>
    public class AuthService : IDisposable {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(AuthService));
        private const string CacheKey = "IAGDIsCloudAuthenticated";
        private readonly AuthenticationProvider _authenticationProvider;
        private readonly IPlayerItemDao _playerItemDao;
        private Thread? _pollingThread = null;
        private volatile bool _isDisposing = false;
        private string _pollingId;
        public event EventHandler? OnAuthCompletion;

        public enum AccessStatus {
            Authorized,
            Unauthorized,
            Unknown
        }

        public AuthService(AuthenticationProvider authenticationProvider,
            IPlayerItemDao playerItemDao) {
            _authenticationProvider = authenticationProvider;
            _playerItemDao = playerItemDao;
        }

        public static AccessStatus IsTokenValid(string user, string token) {
            try {
                using (var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(5) }) {
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
                        Logger.Warn("Server response 500 verifying access token towards backup service");
                        return AccessStatus.Unknown;
                    }
                    else {
                        Logger.Error($"Got Status {result} verifying authentication token");
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

        public void Logout() {
            try {
                if (_authenticationProvider.HasToken()) {
                    Logger.Info("Logging out of cloud backups");
                    var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization",
                        _authenticationProvider.GetToken());
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Api-User",
                        _authenticationProvider.GetUser());

                    var status = httpClient.GetAsync(Uris.LogoutUrl).Result.StatusCode;

                    if (status == HttpStatusCode.OK) {
                        Logger.Info($"Successfully logged out of cloud backups");
                    }
                    else {
                        Logger.Info(
                            $"Failed logging out of cloud backups, status code {status} (this is OK, client still deletes token and gets logged out)");
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

            if (_authenticationProvider.HasToken()) {
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

        public string Authenticate(bool embedded) {
            _pollingId = Guid.NewGuid().ToString();
            if (!embedded) {
                Process.Start(new ProcessStartInfo { FileName = Uris.LoginPageUrl + $"?token={_pollingId}", UseShellExecute = true });
            }

            // If this is called twice.. well.. guess we'll just spam then..
            _pollingThread = new Thread(PollForAccessTokenStatus);
            _pollingThread.Start();
            return _pollingId;
        }

        private void PollForAccessTokenStatus() {
            ExceptionReporter.EnableLogUnhandledOnThread();
            if (Thread.CurrentThread.Name == null) {
                Thread.CurrentThread.Name = "PollForAccessTokenStatus";
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            }

            int numErrors = 0;
            int numRuns = 0;
            using (var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(5) }) {
                try {
                    while (!_isDisposing && numRuns++ < 240 /* ~8 minutes */) {
                        Thread.Sleep(2000);

                        var content = new FormUrlEncodedContent(new[] {
                            new KeyValuePair<string, string>("token", _pollingId),
                        });

                        var result = httpClient.PostAsync(Uris.TokenPollUri, content).Result;
                        var status = result.StatusCode;

                        string body = result.Content.ReadAsStringAsync().Result;
                        if (status == HttpStatusCode.OK) {
                            Logger.Info($"Got Status {result} verifying authentication token");
                            Dictionary<string, object> dataMap = JsonConvert.DeserializeObject<Dictionary<string, object>>(body);
                            if (dataMap["status"].ToString() == "COMPLETED") {
                                Logger.Info("Cloud reports login succeeded");

                                _authenticationProvider.SetToken(dataMap["email"].ToString(), dataMap["token"].ToString());

                                // This somewhat needlessly introduces the System.Runtime.Caching package
                                MemoryCache.Default.Set(CacheKey, true, DateTimeOffset.Now.AddDays(1));
                                OnAuthCompletion?.Invoke(this, new AuthResultEvent(dataMap["email"].ToString(), dataMap["token"].ToString()));

                                return;
                            }
                            else {
                                Logger.Info("Login still pending..");
                            }
                        }
                        else {
                            Logger.Error($"Got Status {result} polling login status");

                            if (numErrors++ > 5) {
                                return;
                            }
                        }
                    }
                }
                catch (Exception ex) {
                    Logger.Warn(ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Logout
        /// </summary>
        public void UnAuthenticate( /* args: This <-> All */) {
            _authenticationProvider.SetToken(string.Empty, string.Empty);
            MemoryCache.Default.Set(CacheKey, AccessStatus.Unauthorized, DateTimeOffset.Now.AddDays(1));
        }

        public AuthenticationProvider GetAuthProvider() {
            return CheckAuthentication() == AccessStatus.Authorized ? _authenticationProvider : null;
        }

        public RestService GetRestService() {
            if (CheckAuthentication() != AccessStatus.Authorized) return null;

            var token = _authenticationProvider.GetToken();

            var handler = new HttpClientHandler() {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            var httpClient = new HttpClient(handler);
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", token);
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Api-User", _authenticationProvider.GetUser());
            httpClient.Timeout = TimeSpan.FromSeconds(5);
            return new RestService(httpClient);
        }

        public void Dispose() {
            try {
                _isDisposing = true;
                _pollingThread = null;
            }
            catch (Exception ex) {
                Logger.Warn(ex.Message, ex);
            }
        }
    }
}