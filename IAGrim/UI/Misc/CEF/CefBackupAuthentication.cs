using System;

namespace IAGrim.UI.Misc.CEF {
    public interface ICefBackupAuthentication {
        void Open(string url);
        event EventHandler OnSuccess;
    }
}
