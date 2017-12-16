using DataAccess;
using EvilsoftCommons;
using IAGrim.Parser.Arc;
using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Parser.Arz {


    public class ArzParser {
        private static ILog logger = LogManager.GetLogger(typeof(ArzParser));
        

        
        /// <summary>
        /// Load string table from the Grim Dawn client
        /// </summary>
        /// <param name="fs"></param>
        /// <param name="start"></param>
        /// <param name="numBytes"></param>
        private static List<string> LoadStringTable(FileStream fs, uint start, uint numBytes) {
            var StringTable = new List<string>();
            fs.Seek(start, SeekOrigin.Begin);

            uint end = start + numBytes;
            while (fs.Position < end) {
                uint count = IOHelper.ReadUInteger(fs);
                for (int i = 0; i < count; i++) {
                    string s = IOHelper.ReadString(fs);                    
                    StringTable.Add(s);
                    
                }
            }

            return StringTable;
        }



        private static IItem ExtractItem(Record record, List<string> stringTable, bool skipLots) {

            int tmp = 0;

            string itemName = stringTable[(int)record.StringIndex];

            // Skip effects/procs/etc
            if (skipLots && !itemName.StartsWith("records/items/") && !itemName.StartsWith("records/storyelements/")
                && !itemName.StartsWith("records/skills/") && !itemName.StartsWith("records/creatures/npcs/npcgear"))
                return null;

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
                /*if (types.Contains(recordstring))*/
                {
                    for (uint n = 0; n < numEntries; n++) {
                        uint pos = 8 + 4 * n;
                        if (type == 1) {
                            float f = IOHelper.GetFloat(data, offset + pos);

                            if (Math.Abs(f) > 0.01)
                                item.Stats.Add(new ItemStat { Stat = recordstring, Value = f });
                        }
                        else if (type == 2) {
                            // Could technically continue to be stored as an int.. 
                            string val = stringTable[(int)IOHelper.GetUInt(data, offset + pos)];
                            if (!string.IsNullOrEmpty(val)) {
                                item.Stats.Add(new ItemStat { Stat = recordstring, TextValue = val });
                            }
                        }
                        else {
                            uint val = IOHelper.GetUInt(data, offset + pos);
                            float testonly = IOHelper.GetFloat(data, offset + pos);
                            if (val > 0)
                                item.Stats.Add(new ItemStat { Stat = recordstring, Value = (int)val });
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
        /// <param name="fs"></param>
        /// <param name="start"></param>
        /// <param name="numRecords"></param>
        private static List<IItem> LoadRecords(FileStream fs, uint start, uint numRecords, List<string> stringTable, bool skipLots) {
            fs.Seek(start, SeekOrigin.Begin);

            List<Record> tempRecords = new List<Record>();



            // Read all the records
            for (int i = 0; i < numRecords; i++) {
                Record record = ReadRecord(stringTable, fs);
                tempRecords.Add(record);                
            }



            // Read and uncompress the data
            for (int i = 0; i < tempRecords.Count; i++) {
                Decompress(fs, tempRecords[i]);
            }

            // Done with FS

            //var types = InterestingSkills;
            List<IItem> items = new List<IItem>();


            // Parse the uncompressed data
            while (tempRecords.Count > 0) {
                var item = ExtractItem(tempRecords[0], stringTable, skipLots);
                if (item != null) {
                    items.Add(item);
                }
                tempRecords.RemoveAt(0);
            }
            
            return new List<IItem>( items );            
        }


        private static void Decompress(FileStream fs, Record record) {
            fs.Seek(record.Offset + 24, SeekOrigin.Begin); // 24?
            if (fs.Read(record.Compressed, 0, record.Compressed.Length) != record.Compressed.Length) {
                logger.Warn("Could not read an entire record..");
            }
            else {
                record.Uncompressed = LZ4.LZ4Codec.Decode(record.Compressed, 0, record.Compressed.Length, record.Uncompressed.Length);
                record.Compressed = null;
            }
        }

        private static Record ReadRecord(List<string> stringTable, FileStream fs) {
            Record record = new Record();
            record.StringIndex = IOHelper.ReadUInteger(fs);
            record.Type = IOHelper.ReadString(fs);
            string itemName = stringTable[(int)record.StringIndex];

            record.Offset = IOHelper.ReadUInteger(fs);
            uint SizeCompressed = IOHelper.ReadUInteger(fs);
            record.Compressed = new byte[SizeCompressed];

            record.SizeUncompressed = IOHelper.ReadUInteger(fs);

            fs.Seek(8, SeekOrigin.Current);

            return record;
        }
        
        public static List<IItemTag> ParseArcFile(string file) {
            // Load the ARC data (item names etc)
            if (!string.IsNullOrEmpty(file)) {
                var decompresser = new Arc.Decompress(file, true);
                decompresser.decompress();

                List<IItemTag> tags = new List<IItemTag>();

                foreach (var s in decompresser.strings) {
                    if (s.ToLower().EndsWith(".txt")) {
                        tags.AddRange(decompresser.GetTags(s));
                        logger.Debug($"Loading tags from {s}");
                    }
                }
                /*
                if (decompresser.hasFile("tags_items.txt")) {
                    tags.AddRange(decompresser.GetTags("tags_items.txt"));
                    logger.InfoFormat("Loaded {0} item tags from Grim Dawn.", tags.Count);
                }
                else {
                    logger.WarnFormat("The Arc file at \"{0}\" does not contain tags_items.txt for item names.", file);
                }

                if (decompresser.hasFile("tags_skills.txt")) {
                    tags.AddRange(decompresser.GetTags("tags_skills.txt"));
                    logger.InfoFormat("Loaded {0} skill tags from Grim Dawn.", tags.Count);
                }
                else {
                    logger.WarnFormat("The Arc file at \"{0}\" does not contain tags_skills.txt for skill names.", file);
                }
                */

                logger.Debug($"Loaded {tags.Count} tags");
                return tags;
            }
            else {
                logger.Warn("Could not locate text_en.arc");
            }

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


                var stringTable = LoadStringTable(fs, header.StringTableStart, header.StringTableSize);
                logger.InfoFormat("Loaded {0} strings from Grim Dawn.", stringTable.Count);

                logger.Info("Attempting to parse items from Grim Dawn");
                var items = LoadRecords(fs, header.RecordTableStart, header.RecordTableEntryCount, stringTable, skipLots);
                logger.InfoFormat("Loaded {0} items from Grim Dawn.", items.Count);

                return items;
            }
        }
        

        
    }
}
