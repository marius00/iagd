using IAGrim.UI.Controller;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.Database.Model;
using IAGrim.UI.Controller.dto;
using IAGrim.Utilities;

namespace IAGrim.UI.Misc {

    public class JSWrapper {
        private readonly JsonSerializerSettings _settings;

        public string ItemSetAssociations { get; private set; }

        public void SetItemSetAssociations(IList<ItemSetAssociation> elems) {
            this.ItemSetAssociations = JsonConvert.SerializeObject(elems, _settings);
        }

        
        public JSWrapper() {
            _settings = new JsonSerializerSettings {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Culture = System.Globalization.CultureInfo.InvariantCulture,
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };
        }


        public void UpdateCollectionItems(IList<CollectionItem> items) {
            this.CollectionItems = JsonConvert.SerializeObject(items, _settings);
        }

        public string CollectionItems { get; set; }

        public string Message => string.Empty;
    }
}