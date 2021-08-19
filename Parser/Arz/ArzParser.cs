using DataAccess;
using EvilsoftCommons;
using IAGrim.Parser.Arc;
using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace IAGrim.Parser.Arz {
    public class ArzParser {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ArzParser));

        /// <summary>
        /// Load string table from the Grim Dawn client
        /// </summary>
        /// <param name="fs"></param>
        /// <param name="start"></param>
        /// <param name="numBytes"></param>
        private static List<string> LoadStringTable(FileStream fs, uint start, uint numBytes) {
            List<string> stringTable = new List<string>();
            fs.Seek(start, SeekOrigin.Begin);

            uint end = start + numBytes;

            while (fs.Position < end) {
                uint count = IOHelper.ReadUInteger(fs);

                for (int i = 0; i < count; i++) {
                    string s = IOHelper.ReadString(fs);
                    stringTable.Add(s);
                }
            }

            return stringTable;
        }

        private static bool IsInteresting(string record) {
            string[] interesting = {
                "/scriptentities/",
                "/items/",
                "/storyelements/",
                "/skills/",
                "/npcgear/"
            };

            string[] notInteresting = {
                "/lootchests/",
                "/loottables/",
                "/lootaffixes/"
            };

            return interesting.Any(record.Contains) && !notInteresting.Any(record.Contains);
        }

        private static IItem ExtractItem(Record record, IReadOnlyList<string> stringTable, bool skipLots) {
            int tmp = 0;

            string itemName = stringTable[(int)record.StringIndex];

            // Skip effects/procs/etc
            if (skipLots && !IsInteresting(itemName)) {
                return null;
            }

            IItem item = new Item {
                Record = itemName,
                Stats = new List<IItemStat>(),
            };

            uint offset = 0;

            while (tmp < record.Uncompressed.Length / 4) {
                byte[] data = record.Uncompressed;
                ushort type = IOHelper.GetShort(data, offset + 0);
                ushort numEntries = IOHelper.GetShort(data, offset + 2);
                uint stringIndex = IOHelper.GetUInt(data, offset + 4);

                tmp += 2 + numEntries;

                // Store the interesting records
                string recordstring = stringTable[(int)stringIndex];
                {
                    for (uint n = 0; n < numEntries; n++) {
                        uint pos = 8 + 4 * n;

                        switch (type) {
                            case 1: {
                                float f = IOHelper.GetFloat(data, offset + pos);

                                if (Math.Abs(f) > 0.01) {
                                    item.Stats.Add(new ItemStat { Stat = recordstring, Value = f });
                                }

                                break;
                            }

                            case 2: {
                                // Could technically continue to be stored as an int.. 
                                string val = stringTable[(int)IOHelper.GetUInt(data, offset + pos)];

                                if (!string.IsNullOrEmpty(val)) {
                                    item.Stats.Add(new ItemStat { Stat = recordstring, TextValue = val });
                                }

                                break;
                            }

                            default: {
                                uint val = IOHelper.GetUInt(data, offset + pos);

                                if (val > 0) {
                                    item.Stats.Add(new ItemStat { Stat = recordstring, Value = (int)val });
                                }

                                break;
                            }
                        }
                    }
                }

                offset += 8 + (uint)numEntries * 4;
            }

            return item;
        }

        /// <summary>
        /// Load items from the Grim Dawn client
        /// </summary>
        private static List<IItem> LoadRecords(
            FileStream fs,
            uint start,
            uint numRecords,
            IReadOnlyList<string> stringTable,
            bool skipLots) {
            fs.Seek(start, SeekOrigin.Begin);

            List<Record> tempRecords = new List<Record>();

            // Read all the records
            for (int i = 0; i < numRecords; i++) {
                Record record = ReadRecord(fs);
                tempRecords.Add(record);
            }

            // Read and decompress the data
            foreach (Record record in tempRecords) {
                Decompress(fs, record);
            }

            // Done with FS

            //var types = InterestingSkills;
            List<IItem> items = new List<IItem>();

            // Parse the uncompressed data
            while (tempRecords.Count > 0) {
                IItem item = ExtractItem(tempRecords[0], stringTable, skipLots);

                if (item != null) {
                    items.Add(item);
                }

                tempRecords.RemoveAt(0);
            }

            return new List<IItem>(items);
        }

        private static void Decompress(FileStream fs, Record record) {
            fs.Seek(record.Offset + 24, SeekOrigin.Begin); // 24?

            if (fs.Read(record.Compressed, 0, record.Compressed.Length) != record.Compressed.Length) {
                Logger.Warn("Could not read an entire record..");
            }
            else {
                record.Uncompressed = LZ4.LZ4Codec.Decode(record.Compressed, 0, record.Compressed.Length, record.Uncompressed.Length);
                record.Compressed = null;
            }
        }

        private static Record ReadRecord(FileStream fs) {
            Record record = new Record();
            record.StringIndex = IOHelper.ReadUInteger(fs);
            record.Type = IOHelper.ReadString(fs);

            record.Offset = IOHelper.ReadUInteger(fs);
            uint sizeCompressed = IOHelper.ReadUInteger(fs);
            record.Compressed = new byte[sizeCompressed];

            record.SizeUncompressed = IOHelper.ReadUInteger(fs);

            fs.Seek(8, SeekOrigin.Current);

            return record;
        }

        public static List<IItemTag> ParseArcFile(string file) {
            // Load the ARC data (item names etc)
            if (!string.IsNullOrEmpty(file)) {
                Decompress decompresser = new Decompress(file, true);
                decompresser.decompress();

                List<IItemTag> tags = new List<IItemTag>();

                foreach (string s in decompresser.strings) {
                    if (s.ToLowerInvariant().EndsWith(".txt")) {
                        tags.AddRange(decompresser.GetTags(s));
                        Logger.Debug($"Loading tags from {s}");
                    }
                    else {
                        Logger.Debug($"Skipping tag file \"{s}\"");
                    }
                }

                Logger.Debug($"Loaded {tags.Count} tags");

                return tags;
            }

            Logger.Warn("Could not locate text_en.arc");

            return null;
        }

        public static List<IItem> LoadItemRecords(string arzFile, bool skipLots) {
            GRIMDAWN_ARZ_V3_HEADER header = new GRIMDAWN_ARZ_V3_HEADER();

            using (FileStream fs = new FileStream(arzFile, FileMode.Open)) {
                header.Unknown = IOHelper.ReadUShort(fs);
                header.Version = IOHelper.ReadUShort(fs);
                header.RecordTableStart = IOHelper.ReadUInteger(fs);
                header.RecordTableSize = IOHelper.ReadUInteger(fs);
                header.RecordTableEntryCount = IOHelper.ReadUInteger(fs);
                header.StringTableStart = IOHelper.ReadUInteger(fs);
                header.StringTableSize = IOHelper.ReadUInteger(fs);

                Debug.Assert(header.Unknown == 2 && header.Version == 3);

                List<string> stringTable = LoadStringTable(fs, header.StringTableStart, header.StringTableSize);
                Logger.InfoFormat("Loaded {0} strings from Grim Dawn.", stringTable.Count);

                Logger.Info("Attempting to parse items from Grim Dawn");
                List<IItem> items = LoadRecords(fs, header.RecordTableStart, header.RecordTableEntryCount, stringTable, skipLots);
                Logger.InfoFormat("Loaded {0} items from Grim Dawn.", items.Count);

                return items;
            }
        }
    }
}