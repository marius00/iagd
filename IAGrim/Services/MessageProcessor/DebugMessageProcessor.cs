using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.UI.Misc;
using log4net;
using EvilsoftCommons;
using log4net.Repository.Hierarchy;

namespace IAGrim.Services.MessageProcessor {
    class DebugMessageProcessor : IMessageProcessor {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DebugMessageProcessor));

        public void Process(MessageType type, byte[] data) {
#if DEBUG
            int t = (int)type;
            switch (type) {

                case MessageType.TYPE_GameEngine_AddItemToTransfer_01:
                case MessageType.TYPE_GameEngine_AddItemToTransfer_02:
                    //logger.InfoFormat("TYPE_GameEngine_AddItemToTransfer(index: {0})", IOHelper.GetInt(bt.Data, 0));
                    break;

                case MessageType.TYPE_GameEngine_GetTransferSack:
                    Logger.InfoFormat("GameEngine::GetTransferSack({0})", IOHelper.GetInt(data, 0));
                    break;


                // TODO: Not the right place.. but so be it, right?
                case MessageType.TYPE_SaveManager:
                    Logger.Info("TYPE_SaveManager received");
                    break;

                    // Very spammy
                case MessageType.TYPE_InterceptDirectRead:
                    // Logger.Info($"TYPE_InterceptDirectRead: {IOHelper.GetInt(data, 0)}");
                    break;
                case MessageType.TYPE_LoadPlayerTransfer:
                    Logger.Info($"TYPE_LoadPlayerTransfer");
                    break;
                case MessageType.TYPE_ReadPlayerTransfer:
                    Logger.Info($"TYPE_ReadPlayerTransfer");
                    break;
            }
            
            var map = new Dictionary<int, string> {
                {50001, "Hooked_InventorySack_AddItem_02" },
                {50002, "Hooked_InventorySack_InventorySackParam" },
                {50003, "Hooked_InventorySack_InventorySack" },
                {50004, "Hooked_InventorySack_Deconstruct" },
                {50005, "Hooked_InventorySack_Sort" },

            };
            if (t >= 50000) {
                if (map.ContainsKey(t))
                    Logger.Debug($"Got message {map[t]}");
                else
                    Logger.Debug($"Got message {t}");
            }
#endif
        }
    }
}
