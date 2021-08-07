using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using log4net;
using Newtonsoft.Json;

namespace IAGrim.Utilities.HelperClasses {
    public class RestService : IDisposable {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RestService));
        private readonly HttpClient _client;

        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Culture = System.Globalization.CultureInfo.InvariantCulture,
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
        };

        public RestService(HttpClient client) {
            _client = client;

            _client.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public T Get<T>(string url) {
            var result = _client.GetAsync(url).Result;
            if (result.IsSuccessStatusCode) {
                string body = result.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<T>(body, _settings);
            }
            else {
                Logger.Warn($"Got response code {result.StatusCode} on GET {url}");
                throw new HttpException();
            }
        }

        public HttpStatusCode VerifyGet(string url) {
            return _client.GetAsync(url).Result.StatusCode;
        }

        public T Post<T>(string url, string json) {
            var result = _client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json")).Result;
            if (result.IsSuccessStatusCode) {
                var responseJson = result.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<T>(responseJson, _settings);
            }
            else {
                Logger.Warn($"Got response code {result.StatusCode} while posting json");
                throw new HttpException();
            }
        }

        public bool Post(string url, string json) {
            var result = _client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json")).Result;
            if (!result.IsSuccessStatusCode) {
                Logger.Warn($"POST {url} returned {result.StatusCode}, {result.Content}");
            }
            return result.IsSuccessStatusCode;
        }

        public bool Delete(string url, string json) {
            var result = _client.DeleteAsync(url).Result;
            return result.IsSuccessStatusCode;
        }

        public void Dispose() {
            _client?.Dispose();
        }
    }
}