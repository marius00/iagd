using EvilsoftCommons;
using IAGrim.Database;
using System;
using System.Collections.Generic;
using System.IO;

namespace IAGrim.Backup.FileWriter {

    public class IAFileExporter : FileExporter {
        const short SUPPORTED_FILE_VER = 1;
        private readonly string filename;

        public IAFileExporter(string filename) {
            this.filename = filename;
        }

        public List<PlayerItem> Read() {
            List<PlayerItem> items = new List<PlayerItem>();
            byte[] bytes = File.ReadAllBytes(filename);
            int pos = 0;

            int file_ver = IOHelper.GetShort(bytes, pos); pos += 2;
            if (file_ver != SUPPORTED_FILE_VER)
                throw new InvalidDataException($"This format of IAStash files is not supported, expected {SUPPORTED_FILE_VER}, got {file_ver}");

            Func<string> readString = () => {
                var s = IOHelper.GetBytePrefixedString(bytes, pos);
                pos += 1 + s?.Length ?? 0;
                return s;
            };

            int numItems = IOHelper.GetInt(bytes, pos); pos += 4;
            for (int i = 0; i < numItems; i++) {
                PlayerItem pi = new PlayerItem();

                pi.BaseRecord = readString();
                pi.PrefixRecord = readString();
                pi.SuffixRecord = readString();
                pi.ModifierRecord = readString();
                pi.TransmuteRecord = readString();
                pi.Seed = IOHelper.GetUInt(bytes, pos); pos += 4;
                pi.MateriaRecord = readString();
                pi.RelicCompletionBonusRecord = readString();
                pi.RelicSeed = IOHelper.GetUInt(bytes, pos); pos += 4;
                pi.EnchantmentRecord = readString();

                //seed is ok, stack count is not
                pi.UNKNOWN = IOHelper.GetUInt(bytes, pos); pos += 4;
                pi.EnchantmentSeed = IOHelper.GetUInt(bytes, pos); pos += 4;
                pi.MateriaCombines = IOHelper.GetShort(bytes, pos); pos += 2;
                pi.StackCount = IOHelper.GetUInt(bytes, pos); pos += 4;
                pi.IsHardcore = bytes[pos++] == 1;

                pos++; // isExpansion
                //pi.IsExpansion1 = bytes[pos++] == 1;

                pi.Mod = readString();

                pi.OnlineId = IOHelper.GetLong(bytes, pos);
                if (pi.OnlineId == 0)
                    pi.OnlineId = null;

                pos += 8;

                items.Add(pi);
            }

            return items;
        }

        public void Write(IList<PlayerItem> items) {
            using (FileStream fs = new FileStream(filename, FileMode.Create)) {
                IOHelper.Write(fs, (short)SUPPORTED_FILE_VER);
                IOHelper.Write(fs, (int)items.Count);

                foreach (PlayerItem pi in items) {
                    IOHelper.WriteBytePrefixed(fs, pi.BaseRecord);
                    IOHelper.WriteBytePrefixed(fs, pi.PrefixRecord);
                    IOHelper.WriteBytePrefixed(fs, pi.SuffixRecord);
                    IOHelper.WriteBytePrefixed(fs, pi.ModifierRecord);
                    IOHelper.WriteBytePrefixed(fs, pi.TransmuteRecord);

                    IOHelper.Write(fs, (uint)pi.Seed);
                    IOHelper.WriteBytePrefixed(fs, pi.MateriaRecord);
                    IOHelper.WriteBytePrefixed(fs, pi.RelicCompletionBonusRecord);

                    IOHelper.Write(fs, (uint)pi.RelicSeed);
                    IOHelper.WriteBytePrefixed(fs, pi.EnchantmentRecord);

                    IOHelper.Write(fs, (uint)pi.UNKNOWN);
                    IOHelper.Write(fs, (uint)pi.EnchantmentSeed);
                    IOHelper.Write(fs, (short)pi.MateriaCombines);

                    IOHelper.Write(fs, (uint)pi.StackCount);
                    IOHelper.Write(fs, pi.IsHardcore);

                    IOHelper.Write(fs, pi.IsExpansion1);
                    IOHelper.WriteBytePrefixed(fs, pi.Mod);
                    IOHelper.Write(fs, pi.OnlineId ?? 0L);
                }
            }
        }
    }
}
