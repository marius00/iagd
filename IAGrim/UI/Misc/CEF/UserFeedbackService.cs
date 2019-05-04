using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EvilsoftCommons;
using log4net;

namespace IAGrim.UI.Misc.CEF {
    public class UserFeedbackService {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(UserFeedbackService));
        private List<LogHistoryEntry> _history = new List<LogHistoryEntry>();
        private readonly CefBrowserHandler _cefBrowserHandler;

        public UserFeedbackService(CefBrowserHandler cefBrowserHandler) {
            _cefBrowserHandler = cefBrowserHandler;
        }

        public void Print(List<UserFeedback> feedbacks) {
            foreach (var entry in feedbacks) {

                if (!IsRecent(entry)) {
                    if (!string.IsNullOrEmpty(entry.URL)) {
                        _cefBrowserHandler.ShowMessage(entry.Message, entry.Level, entry.URL);
                        Logger.Info($"Feedback {entry.Message}");
                    }
                    else {
                        _cefBrowserHandler.ShowMessage(entry.Message, entry.Level);
                        Logger.Info($"Feedback {entry.Message}");
                    }

                    _history.Add(new LogHistoryEntry() {
                        Message = entry.Message,
                        Timestamp = DateTime.UtcNow
                    });
                }
            }
        }

        private bool IsRecent(UserFeedback entry) {
            ClearRecent();
            return _history.Any(e => entry.Message == e.Message);
        }

        private void ClearRecent() {
            var cutoff = DateTime.UtcNow.ToTimestamp() - 3000;
            _history.RemoveAll(entry => entry.Timestamp.ToTimestamp() < cutoff);
        }

        class LogHistoryEntry {
            public DateTime Timestamp { get; set; }
            public string Message { get; set; }
        }

        public void SetFeedback(string level, string feedback, string helpUrl) {
            _cefBrowserHandler.ShowMessage(feedback, UserFeedbackLevel.Info, helpUrl);
        }

        public void SetFeedback(string feedback) {
            _cefBrowserHandler.ShowMessage(feedback, UserFeedbackLevel.Info);
        }
    }
}
