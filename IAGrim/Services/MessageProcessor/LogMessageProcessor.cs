using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.UI.Misc;
using log4net;
using EvilsoftCommons;

namespace IAGrim.Services.MessageProcessor {
    class LogMessageProcessor : IMessageProcessor {
        private ILog logger = LogManager.GetLogger(typeof(LogMessageProcessor));
        private bool doLog = false;

        void Handle(byte[] data) {
            int priority = IOHelper.GetInt(data, 0);
            int origin = IOHelper.GetInt(data, 4);
            int strlen = data.Length >= 12 ? IOHelper.GetInt(data, 8) : -1;
            int strlen2 = data.Length >= 16 ? IOHelper.GetInt(data, 12) : -1;
            logger.Debug($"Priority: {priority}, Origin: {origin}, Len: {strlen}/{strlen2}");


        }
        void HandleX(byte[] data) {
            int pos = 0;
            int param01 = IOHelper.GetInt(data, pos); pos += 4;
            int param02 = IOHelper.GetInt(data, pos); pos += 4;
            int param03 = IOHelper.GetInt(data, pos); pos += 4;
            int param04 = IOHelper.GetInt(data, pos); pos += 4;
            int param05 = IOHelper.GetInt(data, pos); pos += 4;
            int param06 = IOHelper.GetInt(data, pos); pos += 4;
            int param07 = IOHelper.GetInt(data, pos); pos += 4;
            int param08 = IOHelper.GetInt(data, pos); pos += 4;

            logger.Debug($"{param01} @ {param02} @ {param03} @ {param04} @ {param05} @ {param06} @ {param07} @ {param08}");


        }
        public void Process(MessageType type, byte[] data) {
            switch (type) {
                case MessageType.TYPE_HookedCombatManager_ApplyDamage:
                    logger.Debug("TYPE_HookedCombatManager_ApplyDamage");
                    doLog = true;
                    break;
                case MessageType.TYPE_HookedCombatManager_ApplyDamage_Exit:
                    logger.Debug("TYPE_HookedCombatManager_ApplyDamage_Exit");
                    doLog = false;
                    break;

                case MessageType.TYPE_LOG01:
                    logger.Debug("TYPE_LOG01");
                    Handle(data);
                    break;
                case MessageType.TYPE_LOG02:
                    if (doLog)
                        HandleX(data);
                    break;
                case MessageType.TYPE_LOG02_:
                    break;

                case MessageType.TYPE_DEBUG_DefenseAttribute_Jitter: {
                        float baseValue = IOHelper.GetFloat(data, 0);
                        float jitter = IOHelper.GetFloat(data, 4);
                        uint seed = IOHelper.GetUInt(data, 8);
                        if (seed == 0 && Math.Abs(baseValue + jitter) < 0.001)
                            break;
                        logger.Debug($"DefenseAttribute_Jitter({baseValue}, {jitter}, {seed})");
                    }
                    
                    break;
                case MessageType.TYPE_DEBUG_CharAttribute_AddJitter: {
                        float jitter = IOHelper.GetFloat(data, 0);
                        uint seed = IOHelper.GetUInt(data, 4);
                        logger.Debug($"CharAttribute_AddJitter({jitter}, {seed})");
                    }
                    break;
                case MessageType.TYPE_DEBUG_CharAttributeStore_Equipment_Load:
                    logger.Debug($"CharAttributeStore_Equipment_Load({data[0] == 1})");
                    break;
            }
        }
    }
}
