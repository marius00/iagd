using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.UI.Misc;
using EvilsoftCommons;
using log4net;
using IAGrim.Utilities.RectanglePacker;
using IAGrim.Utilities;

namespace IAGrim.Services.MessageProcessor {
    class ItemSpawnedProcessor : IMessageProcessor {
        private static ILog logger = LogManager.GetLogger(typeof(ItemSpawnedProcessor));


        public void Process(MessageType type, byte[] data) {
            switch (type) {
                case MessageType.TYPE_WorldSpawnItem: {
                        uint pos = 0;

                        int source = data[pos];
                        pos += 1;
                        
                        // Vec3f
                        int zone = IOHelper.GetInt(data, (int)pos); pos += 4;
                        float x = IOHelper.GetFloat(data, pos); pos += 4;
                        float y = IOHelper.GetFloat(data, pos); pos += 4;
                        float z = IOHelper.GetFloat(data, pos); pos += 4;

                        uint seed = IOHelper.GetUInt(data, pos);
                        pos += 4;

                        string baseRecord = IOHelper.GetPrefixString(data, (int)pos);

                        logger.Info($"[Src{source}] SpawnItem: X:{x} Z:{z} Zone:{zone} Seed:{seed} {baseRecord}");
                    }
                    break;
            }
        }
    }
}
