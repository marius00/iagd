using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.Settings.Dto;

namespace IAGrim.UI.Misc.CEF {
    class FeatureSuggestionArgs : EventArgs {
        public FeatureRecommendation Feature { get; set; }
        public bool HasFeature { get; set; }
    }
}
