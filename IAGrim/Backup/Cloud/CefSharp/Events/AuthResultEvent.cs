using IAGrim.Backup.Cloud.Service;
using System;
using static IAGrim.Backup.Cloud.Service.AuthService;

namespace IAGrim.Backup.Cloud.CefSharp.Events {
    class AuthResultEvent : EventArgs {
        public AuthResultEvent(string user, string token) {
            User = user;
            Token = token;
        }
        public string Token { get; }
        public string User { get; }

        public AccessStatus Status => AuthService.IsTokenValid(User, Token);
        public bool IsAuthorized => Status == AccessStatus.Authorized;
    } 
}
