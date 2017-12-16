using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.UI.Misc;
using log4net;
using IAGrim.Database;
using EvilsoftCommons;
using System.Diagnostics;
using IAGrim.Database.Interfaces;

namespace IAGrim.Services.MessageProcessor {
    public class ItemInjectCallbackProcessor : IMessageProcessor {
        private ILog logger = LogManager.GetLogger(typeof(ItemInjectCallbackProcessor));
        private readonly IPlayerItemDao playerItemDao;
        private Action UpdateItemView;

        public ItemInjectCallbackProcessor(Action updateListview, IPlayerItemDao playerItemDao) {
            this.UpdateItemView = updateListview;
            this.playerItemDao = playerItemDao;
        }

        public void Process(MessageType type, byte[] data) {
            switch (type) {
                case MessageType.TYPE_Custom_AddItemSucceeded:
                    logger.Info("Hook reports item injection successful.");
                    break;

                case MessageType.TYPE_Custom_AddItem:
                    logger.DebugFormat("TYPE_Custom_AddItem: {0}", IOHelper.GetInt(data, 0));
                    break;

                case MessageType.TYPE_Custom_AddItemFailed:
                    logger.Info("Hook reports item injection failed."); 
                    {
                        PlayerItem pi = Deserialize(data);
                        playerItemDao.Save(pi);
                        logger.Info("Item stored back in IA database.");
                        UpdateItemView();
                    }
                    break;
            }
        }


        public static PlayerItem Deserialize(byte[] buffer) {

            int pos = 0;
            uint seed = IOHelper.GetUInt(buffer, (uint)pos);
            pos += 4;

            uint relicSeed = IOHelper.GetUInt(buffer, (uint)pos);
            pos += 4;

            uint unknown = IOHelper.GetUInt(buffer, (uint)pos);
            pos += 4;

            uint enchantSeed = IOHelper.GetUInt(buffer, (uint)pos);
            pos += 4;

            uint materiaCombines = IOHelper.GetUInt(buffer, (uint)pos);
            pos += 4;

            // 4x unknown crap
            pos += 4 * 4;



            uint stacksize = Math.Max(1, IOHelper.GetUInt(buffer, (uint)pos));
            pos += 4;


            // 1x unknown crap
            pos += 4;



            // 1x string length
            pos += 4;

            string baseRecord = IOHelper.GetNullString(buffer, pos);
            pos += baseRecord.Length + 5;

            string prefixRecord = IOHelper.GetNullString(buffer, pos);
            pos += prefixRecord.Length + 5;

            string suffixRecord = IOHelper.GetNullString(buffer, pos);
            pos += suffixRecord.Length + 5;

            string modifierRecord = IOHelper.GetNullString(buffer, pos);
            pos += modifierRecord.Length + 5;

            string materiaRecord = IOHelper.GetNullString(buffer, pos);
            pos += materiaRecord.Length + 5;

            string relicCompletionBonusRecord = IOHelper.GetNullString(buffer, pos);
            pos += relicCompletionBonusRecord.Length + 5;

            string enchantmentRecord = IOHelper.GetNullString(buffer, pos);
            pos += enchantmentRecord.Length + 5;

            string transmuteRecord = IOHelper.GetNullString(buffer, pos);
            pos += transmuteRecord.Length + 5;


            var item = new PlayerItem {
                Seed = seed,
                RelicSeed = relicSeed,
                UNKNOWN = unknown,
                EnchantmentSeed = enchantSeed,
                MateriaCombines = materiaCombines,
                StackCount = stacksize,
                BaseRecord = baseRecord,
                PrefixRecord = prefixRecord,
                SuffixRecord = suffixRecord,
                ModifierRecord = modifierRecord,
                MateriaRecord = materiaRecord,
                RelicCompletionBonusRecord = relicCompletionBonusRecord,
                EnchantmentRecord = enchantmentRecord,
                TransmuteRecord = transmuteRecord
            };

            return item;
        }

        public static List<byte> Serialize(PlayerItem pi) {
            List<byte> buffer = new List<byte>();
            int unknown_leave_empty = 0;


            buffer.AddRange(BitConverter.GetBytes((int)pi.Seed));
            buffer.AddRange(BitConverter.GetBytes((int)pi.RelicSeed));
            buffer.AddRange(BitConverter.GetBytes((int)pi.UNKNOWN));
            buffer.AddRange(BitConverter.GetBytes((int)pi.EnchantmentSeed));
            buffer.AddRange(BitConverter.GetBytes((int)pi.MateriaCombines));
            buffer.AddRange(BitConverter.GetBytes(unknown_leave_empty));
            buffer.AddRange(BitConverter.GetBytes(unknown_leave_empty));

            buffer.AddRange(BitConverter.GetBytes(unknown_leave_empty));
            buffer.AddRange(BitConverter.GetBytes(unknown_leave_empty));
            buffer.AddRange(BitConverter.GetBytes((int)pi.StackCount));
            buffer.AddRange(BitConverter.GetBytes(unknown_leave_empty));


            buffer.AddRange(BitConverter.GetBytes(pi.BaseRecord.Length));
            buffer.AddRange(System.Text.Encoding.ASCII.GetBytes(pi.BaseRecord));
            buffer.Add(0);

            buffer.AddRange(BitConverter.GetBytes(pi.PrefixRecord?.Length ?? 0));
            if (!string.IsNullOrEmpty(pi.PrefixRecord))
                buffer.AddRange(System.Text.Encoding.ASCII.GetBytes(pi.PrefixRecord));
            buffer.Add(0);

            buffer.AddRange(BitConverter.GetBytes(pi.SuffixRecord?.Length ?? 0));
            if (!string.IsNullOrEmpty(pi.SuffixRecord))
                buffer.AddRange(System.Text.Encoding.ASCII.GetBytes(pi.SuffixRecord));
            buffer.Add(0);

            buffer.AddRange(BitConverter.GetBytes(pi.ModifierRecord?.Length ?? 0));
            if (!string.IsNullOrEmpty(pi.ModifierRecord))
                buffer.AddRange(System.Text.Encoding.ASCII.GetBytes(pi.ModifierRecord));
            buffer.Add(0);

            buffer.AddRange(BitConverter.GetBytes(pi.MateriaRecord?.Length ?? 0));
            if (!string.IsNullOrEmpty(pi.MateriaRecord))
                buffer.AddRange(System.Text.Encoding.ASCII.GetBytes(pi.MateriaRecord));
            buffer.Add(0);

            buffer.AddRange(BitConverter.GetBytes(pi.RelicCompletionBonusRecord?.Length ?? 0));
            if (!string.IsNullOrEmpty(pi.RelicCompletionBonusRecord))
                buffer.AddRange(System.Text.Encoding.ASCII.GetBytes(pi.RelicCompletionBonusRecord));
            buffer.Add(0);

            buffer.AddRange(BitConverter.GetBytes(pi.EnchantmentRecord?.Length ?? 0));
            if (!string.IsNullOrEmpty(pi.EnchantmentRecord))
                buffer.AddRange(System.Text.Encoding.ASCII.GetBytes(pi.EnchantmentRecord));
            buffer.Add(0);

            buffer.AddRange(BitConverter.GetBytes(pi.TransmuteRecord?.Length ?? 0));
            if (!string.IsNullOrEmpty(pi.TransmuteRecord))
                buffer.AddRange(System.Text.Encoding.ASCII.GetBytes(pi.TransmuteRecord));
            buffer.Add(0);

            return buffer;
        }
    }
}
