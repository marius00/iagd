using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Core;
using log4net.Layout.Pattern;

namespace IAGrim.Utilities {
    /// <summary>
    /// Masks the windows username from logs
    /// </summary>
    class LoggerUsernameConverter : PatternLayoutConverter {
        private readonly string _localAppdata = System.Environment.GetEnvironmentVariable("LocalAppData");
        private readonly string _userProfile = System.Environment.GetEnvironmentVariable("UserProfile");
        private static string _currentUser;
        private static string _currentUserInversePath;

        private static string CurrentUser {
            get {
                if (string.IsNullOrEmpty(_currentUser)) {
                    var user = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    if (!string.IsNullOrEmpty(user)) {
                        user = user.Substring(1 + user.LastIndexOf("\\"));
                        _currentUser = $"\\{user}\\"; // Only interesting if its part of a path. Eg if the username is "a" we wont filter every single time the letter is 'a', which would actually just expose it further.
                        _currentUserInversePath = $"/{user}/"; // file:// will have the inverse path
                    }
                }

                return _currentUser;
            }
        }

        protected override void Convert(TextWriter writer, LoggingEvent loggingEvent) {
            string message = loggingEvent.RenderedMessage;

            message = message.Replace(_localAppdata, "%LocalAppData%") // Appdata is the most likely culprit
                .Replace(_userProfile, "%UserProfile%") // Userprofile may occur when logging path to "My games"
                .Replace(CurrentUser, "\\:filtered\\") // Userprofile may occur when logging path to "My games"
                .Replace(_currentUserInversePath, "/:filtered/");


            writer.Write(message);
        }
    }
}