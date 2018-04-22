using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.UI.Misc {
    public interface ICefBackupAuthentication {
        void Open(string url);
        event EventHandler OnSuccess;
    }
}
