using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using CefSharp;
using EvilsoftCommons.Exceptions;
using IAGrim.Backup.Azure.CefSharp.Events;
using IAGrim.Backup.Azure.Constants;
using IAGrim.UI.Misc;
using IAGrim.UI.Misc.CEF;
using IAGrim.Utilities.HelperClasses;
using log4net;

namespace IAGrim.Backup.Azure.Service {
    public class AzureAuthService {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(AzureAuthService));
        private const string CacheKey = "IAGDIsAzureAuthenticated";
        private readonly ICefBackupAuthentication _authentication;
        private readonly AuthenticationProvider _authenticationProvider;

        public enum AccessStatus {
            Authorized,
            Unauthorized,
            Unknown
        }

        public AzureAuthService(ICefBackupAuthentication authentication, AuthenticationProvider authenticationProvider) {
            _authentication = authentication;
            _authenticationProvider = authenticationProvider;
            _authentication.OnSuccess += AuthenticationOnOnSuccess;
        }

        private void AuthenticationOnOnSuccess(object sender, EventArgs eventArgs) {
            var args = eventArgs as AuthResultEvent;
            (sender as IBrowser)?.CloseBrowser(true);

            if (IsTokenValid(args.Token) == AccessStatus.Authorized) {
                _authenticationProvider.SetToken(args.Token);
                MemoryCache.Default.Set(CacheKey, true, DateTimeOffset.Now.AddDays(1));
            }
        }

        private AccessStatus IsTokenValid(string token) {
            try {
                // TODO : This is being spammed to holy hell, probably incurring vast costs.
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Simple-Auth", token);
                var result = httpClient.GetAsync(AzureUris.TokenVerificationUri).Result.StatusCode;

                if (result == HttpStatusCode.OK) {
                    Logger.Info($"Got Status {result} verifying authentication token");
                    return AccessStatus.Authorized;
                }
                else if (result == HttpStatusCode.Unauthorized || result == HttpStatusCode.Forbidden) {
                    return AccessStatus.Unauthorized;
                }
                else if (result == HttpStatusCode.InternalServerError) {
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
            catch (AggregateException ex) {
                Logger.Warn(ex.Message, ex);
                return AccessStatus.Unknown;
            }
            catch (WebException ex) {
                Logger.Warn(ex.Message, ex);
                return AccessStatus.Unknown;
            }
        }

        public AccessStatus CheckAuthentication() {
            if (MemoryCache.Default[CacheKey] is AccessStatus cached) {
                return cached;
            }

            if (_authenticationProvider.HasToken()) {
                var result = IsTokenValid(_authenticationProvider.GetToken());
                MemoryCache.Default.Set(CacheKey, result, DateTimeOffset.Now.AddDays(1));

                if (result == AccessStatus.Unauthorized) {
                    Logger.Warn("Existing authentication token is invalid, clearing authentication provider");
                    _authenticationProvider.SetToken(string.Empty);
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

            _authentication.Open(AzureUris.AuthenticateUrl);
        }

        /// <summary>
        /// Logout
        /// </summary>
        public void UnAuthenticate(/* args: This <-> All */) {
            _authenticationProvider.SetToken(string.Empty);
            MemoryCache.Default.Set(CacheKey, AccessStatus.Unauthorized, DateTimeOffset.Now.AddDays(1));
        }

        public RestService GetRestService() {
            if (CheckAuthentication() == AccessStatus.Authorized) {
                var token = _authenticationProvider.GetToken();

                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Simple-Auth", token);
                return new RestService(httpClient);
            }

            return null;
        }
    }
}
