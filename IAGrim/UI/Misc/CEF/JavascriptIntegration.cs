using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace IAGrim.UI.Misc.CEF {
    class JavascriptIntegration {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(JavascriptIntegration));

        public string RequestStats() {
            Logger.Debug("Request bla bla");
            return "stuff";
        }
    }
}
