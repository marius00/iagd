using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.UI.Misc;
using log4net;
using IAGrim.Utilities.HelperClasses;
using IAGrim.Utilities;

namespace IAGrim.Services.MessageProcessor {
    class CloudDetectorProcessor : IMessageProcessor {
        private ILog Logger = LogManager.GetLogger(typeof(CloudDetectorProcessor));
        private readonly Action<string> _setFeedback;
        public CloudDetectorProcessor(Action<string> feedback) {
            this._setFeedback = feedback;
        }

        public void Process(MessageType type, byte[] data) {

            switch (type) {
                case MessageType.TYPE_CloudGetNumFiles:
                case MessageType.TYPE_CloudRead:
                case MessageType.TYPE_CloudWrite:
                    Logger.WarnFormat("GD calling {0}, cloud saving is still enabled ingame.", type);
                    Logger.Warn("Go to settings INSIDE GRIM DAWN and disable cloud saving.");
                    _setFeedback(GlobalSettings.Language.GetTag("iatag_feedback_cloud_save_enabled_ingame"));
                    GlobalSettings.StashStatus = StashAvailability.CLOUD;
                    break;
            }
        }
    }
}
