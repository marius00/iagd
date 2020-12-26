using System;

namespace IAGrim.Backup.Cloud.CefSharp.Events {
    class AuthResultEvent : EventArgs {
        public AuthResultEvent(string user, string token) {
            User = user;
            Token = token;
        }
        public string Token { get; }
        public string User { get; }
    } 
}
