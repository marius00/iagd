using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.UI.Misc;
using log4net;
using IAGrim.Utilities.HelperClasses;
using IAGrim.Utilities;
using IAGrim.Settings;

namespace IAGrim.Services.MessageProcessor {
    class CloudDetectorProcessor : IMessageProcessor {
        private ILog Logger = LogManager.GetLogger(typeof(CloudDetectorProcessor));
        private readonly Action<string> _setFeedback;
        private readonly SettingsService _settingsService;
        public CloudDetectorProcessor(Action<string> feedback, SettingsService settings) {
            this._setFeedback = feedback;
            this._settingsService = settings;
        }

        public void Process(MessageType type, byte[] data, string dataString) {

            switch (type) {
                case MessageType.TYPE_CloudGetNumFiles:
                case MessageType.TYPE_CloudRead:
                case MessageType.TYPE_CloudWrite:
                    if (_settingsService.GetLocal().DisableInstaloot) { 
                        Logger.WarnFormat("GD calling {0}, cloud saving is still enabled ingame.", type);
                        Logger.Warn("Go to settings INSIDE GRIM DAWN and disable cloud saving.");
                        _setFeedback(RuntimeSettings.Language.GetTag("iatag_feedback_cloud_save_enabled_ingame"));
                        RuntimeSettings.StashStatus = StashAvailability.CLOUD;
                    }
                    break;

            }
        }
    }
}
