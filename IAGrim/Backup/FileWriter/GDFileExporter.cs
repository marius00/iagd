using EvilsoftCommons;
using IAGrim.Database;
using System;
using System.Collections.Generic;
using System.IO;

namespace IAGrim.Backup.FileWriter {

    public class GDFileExporter : FileExporter {
        const int SUPPORTED_FILE_VER = 1;
        private readonly string _filename;
        private readonly string _mod;

        public GDFileExporter(string filename, string mod) {
            this._filename = filename;
            this._mod = mod;
        }

        public List<PlayerItem> Read(byte[] bytes) {
            List<PlayerItem> items = new List<PlayerItem>();
            
            int pos = 0;

            int file_ver = IOHelper.GetInt(bytes, pos); pos += 4;
            if (file_ver != SUPPORTED_FILE_VER)
                throw new InvalidDataException($"This format of GDStash/Mambastash files is not supported, expected {SUPPORTED_FILE_VER}, got {file_ver}");

            string ReadString() {
                var s = IOHelper.GetBytePrefixedString(bytes, pos);
                pos += 1 + s?.Length ?? 0;
                return s;
            }

            int numItems = IOHelper.GetInt(bytes, pos); pos += 4;
            for (int i = 0; i < numItems; i++) {
                PlayerItem pi = new PlayerItem();

                pi.BaseRecord = ReadString();
                pi.PrefixRecord = ReadString();
                pi.SuffixRecord = ReadString();
                pi.ModifierRecord = ReadString();
                pi.TransmuteRecord = ReadString();
                pi.Seed = IOHelper.GetUInt(bytes, pos); pos += 4;
                pi.MateriaRecord = ReadString();
                pi.RelicCompletionBonusRecord = ReadString();
                pi.RelicSeed = IOHelper.GetUInt(bytes, pos); pos += 4;
                pi.EnchantmentRecord = ReadString();

                pi.UNKNOWN = IOHelper.GetUInt(bytes, pos); pos += 4;
                pi.EnchantmentSeed = IOHelper.GetUInt(bytes, pos); pos += 4;
                pi.MateriaCombines = IOHelper.GetUInt(bytes, pos); pos += 4;
                pi.StackCount = IOHelper.GetUInt(bytes, pos); pos += 4;
                pi.IsHardcore = bytes[pos++] == 1;
                //pi.IsExpansion1 = this.isExpansion1;
                pi.Mod = this._mod;
                string charName = ReadString();

                items.Add(pi);
            }

            return items;
        }

        public void Write(IList<PlayerItem> items) {
            using (FileStream fs = new FileStream(_filename, FileMode.Create)) {

                IOHelper.Write(fs, SUPPORTED_FILE_VER);
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
                    IOHelper.Write(fs, (uint)pi.MateriaCombines);

                    IOHelper.Write(fs, (uint)pi.StackCount);
                    IOHelper.Write(fs, pi.IsHardcore);
                    IOHelper.Write(fs, (byte)0); // Char name                    
                }
            }
        }
    }
}
