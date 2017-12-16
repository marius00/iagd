using EvilsoftCommons;
using IAGrim.UI;
using IAGrim.UI.Controller;
using IAGrim.UI.Misc;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Services.MessageProcessor {
    class ModChangeProcessor : IMessageProcessor {
        private ILog logger = LogManager.GetLogger(typeof(ModChangeProcessor));
        private readonly ISettingsReadController settingsController;
        private readonly SearchWindow searchWindow;

        public ModChangeProcessor(ISettingsReadController settingsController, SearchWindow searchWindow) {
            this.settingsController = settingsController;
            this.searchWindow = searchWindow;
        }

        public void Process(MessageType type, byte[] data) {

            switch (type) {


                case MessageType.TYPE_GameInfo_IsHardcore:
                    logger.InfoFormat("TYPE_GameInfo_IsHardcore({0})", data[0] > 0);
                    if (settingsController.AutoUpdateModSettings) {
                        searchWindow.UpdateModSelection(data[0] > 0);
                    }

                    break;

                case MessageType.TYPE_GameInfo_SetModName:
                    logger.InfoFormat("TYPE_GameInfo_SetModName({0})", IOHelper.GetPrefixString(data, 0));
                    if (settingsController.AutoUpdateModSettings) {
                        searchWindow.UpdateModSelection(IOHelper.GetPrefixString(data, 0));
                    }
                    break;
            }
        }
    }
}
