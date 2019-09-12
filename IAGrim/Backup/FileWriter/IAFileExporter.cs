using EvilsoftCommons;
using IAGrim.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;

namespace IAGrim.Backup.FileWriter {

    public class IAFileExporter : FileExporter {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(IAFileExporter));
        readonly List<int> _supportedFileVer = new List<int> { 1, 2, 3 };
        private readonly string _filename;

        public IAFileExporter(string filename) {
            this._filename = filename;
        }

        public List<PlayerItem> Read(byte[] bytes) {
            List<PlayerItem> items = new List<PlayerItem>();
            int pos = 0;

            int fileVer = IOHelper.GetShort(bytes, pos); pos += 2;


            
            if (!_supportedFileVer.Contains(fileVer)) {
                throw new InvalidDataException($"This format of IAStash files is not supported, expected {string.Join(",", _supportedFileVer)}, got {fileVer}");
            }

            if (_supportedFileVer.Max() != fileVer) {
                Logger.Warn($"Parsing a legacy IA file of version v{fileVer}, the latest is version v{_supportedFileVer.Max()}");
            }

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

                //seed is ok, stack count is not
                pi.UNKNOWN = IOHelper.GetUInt(bytes, pos); pos += 4;
                pi.EnchantmentSeed = IOHelper.GetUInt(bytes, pos); pos += 4;
                pi.MateriaCombines = IOHelper.GetShort(bytes, pos); pos += 2;
                pi.StackCount = IOHelper.GetUInt(bytes, pos); pos += 4;
                pi.IsHardcore = bytes[pos++] == 1;

                pos++; // isExpansion
                //pi.IsExpansion1 = bytes[pos++] == 1;

                pi.Mod = ReadString();

                if (fileVer == 1) {
                    // Old OID
                    pos += 8;
                }
                else if (fileVer >= 2) {
                    pi.AzurePartition = ReadString();
                    pi.AzureUuid = ReadString();
                    pi.AzureHasSynchronized = !string.IsNullOrEmpty(pi.AzurePartition);
                }
                if (fileVer >= 3) {
                    pi.AzureHasSynchronized = bytes[pos++] == 1;
                }

                items.Add(pi);
            }

            return items;
        }

        public void Write(IList<PlayerItem> items) {
            using (FileStream fs = new FileStream(_filename, FileMode.Create)) {
                IOHelper.Write(fs, (short)_supportedFileVer.Max());
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

                    IOHelper.WriteBytePrefixed(fs, pi.AzurePartition);
                    IOHelper.WriteBytePrefixed(fs, pi.AzureUuid);
                    IOHelper.Write(fs, pi.AzureHasSynchronized);
                }
            }
        }
    }
}
