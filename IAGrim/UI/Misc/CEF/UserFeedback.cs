using System.Collections.Generic;
using IAGrim.Utilities;

namespace IAGrim.UI.Misc.CEF {
    public class UserFeedback {
        public string Message { get; set; }
        public UserFeedbackLevel Level { get; set; } = UserFeedbackLevel.Info;
        public string URL { get; set; }

        public UserFeedback() { }

        public UserFeedback(string message) {
            this.Message = message;
        }

        public UserFeedback(UserFeedbackLevel level, string message) {
            this.Level = level;
            this.Message = message;
        }

        public UserFeedback(UserFeedbackLevel level, string message, string url) {
            this.Level = level;
            this.Message = message;
        }

        public static UserFeedback FromTag(string tag) {
            return new UserFeedback(RuntimeSettings.Language.GetTag(tag));
        }

        public static List<UserFeedback> FromTagSingleton(string tag) {
            return new List<UserFeedback> {
                new UserFeedback(RuntimeSettings.Language.GetTag(tag))
            };
        }

    }
}
