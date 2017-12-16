using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.UI.Misc {
    class RequestRecipeArgument : EventArgs {
        public string RecipeRecord { get; set; }
        public string Callback { get; set; }
    }
}
