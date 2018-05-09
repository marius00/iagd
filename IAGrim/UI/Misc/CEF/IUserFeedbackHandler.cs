using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.UI.Misc.CEF {
    public interface IUserFeedbackHandler {
        void ShowMessage(string message, UserFeedbackLevel level);
        void ShowMessage(string message);
    }
}
