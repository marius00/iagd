using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using IAGrim.Backup.Azure.Constants;
using log4net;
using Newtonsoft.Json;

namespace IAGrim.Utilities.HelperClasses {
    public class RestService {
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
            var result = _client.GetStringAsync(url).Result;
            return JsonConvert.DeserializeObject<T>(result, _settings);
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
            return result.IsSuccessStatusCode;
        }
    }
}
