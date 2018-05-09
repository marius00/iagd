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

        public AzureAuthService(ICefBackupAuthentication authentication, AuthenticationProvider authenticationProvider) {
            _authentication = authentication;
            _authenticationProvider = authenticationProvider;
            _authentication.OnSuccess += AuthenticationOnOnSuccess;
        }

        private void AuthenticationOnOnSuccess(object sender, EventArgs eventArgs) {
            var args = eventArgs as AuthResultEvent;
            (sender as IBrowser)?.CloseBrowser(true);

            if (IsTokenValid(args.Token)) {
                _authenticationProvider.SetToken(args.Token);
                MemoryCache.Default.Set(CacheKey, true, DateTimeOffset.Now.AddDays(1));
            }
        }

        private bool IsTokenValid(string token) {
            try {

                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Simple-Auth", token);
                var result = httpClient.GetAsync(AzureUris.TokenVerificationUri).Result.StatusCode;

                if (result == HttpStatusCode.OK) {
                    Logger.Info($"Got Status {result} verifying authentication token");
                    return true;
                }
                else {
                    Logger.Error($"Got Status {result} verifying authentication token");
                    return false;
                }
            }
            catch (AggregateException ex) {
                Logger.Warn(ex.Message, ex);
                return false;
            }
            catch (WebException ex) {
                Logger.Warn(ex.Message, ex);
                return false;
            }
        }

        public bool IsAuthenticated() {
            var cached = MemoryCache.Default[CacheKey] as bool?;
            if (cached.HasValue)
                return cached.Value;

            if (_authenticationProvider.HasToken()) {
                var result = IsTokenValid(_authenticationProvider.GetToken());
                MemoryCache.Default.Set(CacheKey, result, DateTimeOffset.Now.AddDays(1));

                if (!result) {
                    Logger.Warn("Existing authentication token is invalid, clearing authentication provider");
                    _authenticationProvider.SetToken(string.Empty);
                }

                return result;
            }
            else {
                MemoryCache.Default.Set(CacheKey, false, DateTimeOffset.Now.AddDays(1));
            }

            return false;
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
            throw new NotImplementedException();

            _authenticationProvider.SetToken(String.Empty);
        }

        public RestService GetRestService() {
            if (IsAuthenticated()) {
                var token = _authenticationProvider.GetToken();

                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Simple-Auth", token);
                return new RestService(httpClient);
            }

            return null;
        }
    }
}
