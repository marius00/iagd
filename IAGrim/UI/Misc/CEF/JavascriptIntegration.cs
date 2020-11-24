using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.Utilities;
using Newtonsoft.Json;

namespace IAGrim.UI.Misc.CEF {
    class JavascriptIntegration {
        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Culture = System.Globalization.CultureInfo.InvariantCulture,
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        public string RequestStats() {
            return "stuff";
        }

        public string GetTranslationStrings() {
            var lang = RuntimeSettings.Language;
            Dictionary<string, string> translations = new Dictionary<string, string> {
                {"app.tab.items", lang.GetTag("iatag_html_tab_header_items")},
                {"app.tab.crafting", lang.GetTag("iatag_html_tab_header_crafting")},
                {"app.tab.components", lang.GetTag("iatag_html_tab_header_components")},
                {"app.tab.videoGuide", lang.GetTag("iatag_html_tab_header_videoguide")},
                {"app.tab.videoGuideUrl", lang.GetTag("iatag_html_tab_header_videoguide_url")},
                {"app.tab.discord", lang.GetTag("iatag_html_tab_header_discord")},
                {"items.label.noItemsFound", lang.GetTag("iatag_html_items_no_items")},
                {"items.label.youCanCraftThisItem", lang.GetTag("iatag_html_items_youcancraftthisitem")},
                {"items.label.cloudOk", lang.GetTag("iatag_html_cloud_ok")},
                {"items.label.cloudError", lang.GetTag("iatag_html_cloud_err")},
                {"item.buddies.singular", lang.GetTag("iatag_html_items_buddy_alsohasthisitem1")},
                {"item.buddies.plural", lang.GetTag("iatag_html_items_buddy_alsohasthisitem3")},
                {"item.buddies.singularOnly", lang.GetTag("iatag_html_items_buddy_alsohasthisitem4")},
                {"items.label.doubleGreen", lang.GetTag("iatag_html_items_affix2")},
                {"items.label.tripleGreen", lang.GetTag("iatag_html_items_affix3")},
                {"item.label.bonusToAllPets", lang.GetTag("iatag_html_bonustopets")},
                {"item.label.grantsSkill", lang.GetTag("iatag_html_items_grantsskill")},
                {"item.label.grantsSkillLevel", lang.GetTag("iatag_html_items_level")},
                {"item.label.levelRequirement", lang.GetTag("iatag_html_levlerequirement")},
                {"item.label.levelRequirementAny", lang.GetTag("iatag_html_any")},
                {"item.label.transferSingle", lang.GetTag("iatag_html_transfer")},
                {"item.label.transferAll", lang.GetTag("iatag_html_transferall")},
                {"crafting.header.recipeName", lang.GetTag("iatag_html_badstate_title")},
                {"crafting.header.currentlyLacking", lang.GetTag("iatag_html_crafting_lacking")},
                {"item.augmentPurchasable", lang.GetTag("iatag_html_augmentation_item")},
                {"app.copyToClipboard", lang.GetTag("iatag_html_copytoclipboard")},
                {"item.label.setbonus", lang.GetTag("iatag_html_setbonus")},
            };

            // Attempting to return a Dictionary<..> object will only work if this object is bound with "async: true"
            return JsonConvert.SerializeObject(translations, _settings);
        }
    }
}
