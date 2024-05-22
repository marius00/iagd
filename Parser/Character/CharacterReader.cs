using IAGrim.StashFile;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Parser.Character {
    public class CharacterReader {
        private GDCryptoDataBuffer reader;
        public string Name { get; private set; }
        public int Level { get; private set; }
        public GDInventory Inventory { get; private set; }
        private GDCharSkillList SkillList = new GDCharSkillList();
        public GDCharBio Bio { get; private set; } = new GDCharBio();

        public List<GDCharSkill> Skills {
            get {
                return SkillList.Skills;
            }
        }

        /// <summary>
        /// All equipment worn
        /// Excluding weapon set 2
        /// </summary>
        public List<string> Equipment {
            get {
                List<string> records = new List<string>();
                records.AddRange(Inventory.Equipment.Select(m => m.BaseRecord));
                records.AddRange(Inventory.Equipment.Select(m => m.PrefixRecord));
                records.AddRange(Inventory.Equipment.Select(m => m.SuffixRecord));

                // Just taking one weapon set into account, can't use both.
                // 
                records.AddRange(Inventory.Weapon1.Select(m => m.BaseRecord));
                records.AddRange(Inventory.Weapon1.Select(m => m.PrefixRecord));
                records.AddRange(Inventory.Weapon1.Select(m => m.SuffixRecord));

                return records;
            }
        }

        public CharacterReader() {
            Inventory = new GDInventory();
        }

        private void ReadHeader() {
            string classID;
            bool hardcore;
            byte sex;
            string name;

            reader.ReadCryptoWString(out name);
            Name = name;

            reader.ReadCryptoByte(out sex);
            if (!reader.ReadCryptoString(out classID))
                throw new FormatException("Could not read class id");
            Level = reader.ReadCryptoIntUnchecked();

            reader.ReadCryptoBool(out hardcore);
        }



        private void ReadCharacterInfo() {
            bool isInMainQuest;
            bool hasBeenInGame;
            byte difficulty;
            byte greatestCampaignDifficulty;
            uint money;
            byte greatestCrucibleDifficulty;
            int tributes;

            reader.ReadBlockStart(Constants.BLOCK);



            int version = reader.ReadCryptoIntUnchecked();
            if ((version != Constants.VERSION_3) && (version != Constants.VERSION_4)) {
                throw new FormatException("ERR_UNSUPPORTED_VERSION");
            }


            reader.ReadCryptoBool(out isInMainQuest);
            reader.ReadCryptoBool(out hasBeenInGame);
            reader.ReadCryptoByte(out difficulty);
            reader.ReadCryptoByte(out greatestCampaignDifficulty);

            reader.ReadCryptoUInt(out money);

            if (version == Constants.VERSION_4) {
                reader.ReadCryptoByte(out greatestCrucibleDifficulty);
                reader.ReadCryptoInt(out tributes);
            }

            byte compassState = reader.ReadCryptoByteUnchecked();
            int lootMode = reader.ReadCryptoIntUnchecked();
            byte skillWindowHelp = reader.ReadCryptoByteUnchecked();
            byte alternateConfig = reader.ReadCryptoByteUnchecked();
            byte alternateConfigEnabled = reader.ReadCryptoByteUnchecked();
            string texture = reader.ReadCryptoStringUnchecked();

            reader.ReadBlockEnd();
        }


        public void ReadSummary(string file) {
            ReadSummary(File.ReadAllBytes(file));
        }

        public void ReadSummary(byte[] data) {
            reader = new GDCryptoDataBuffer(data);
            int val = 0;

            reader.ReadCryptoKey();

            if (!reader.ReadCryptoInt(out val)) {
                throw new FormatException("ERR_UNSUPPORTED_VERSION");
            }

            if (val != 0x58434447)
                throw new FormatException("ERR_UNSUPPORTED_VERSION");

            reader.ReadCryptoInt(out val);
            if (val != 1)
                throw new FormatException("ERR_UNSUPPORTED_VERSION");


            ReadHeader();

            reader.ReadCryptoInt(out val, false);
            if (val != 0)
                throw new FormatException("ERR_UNSUPPORTED_VERSION");

            int version;
            reader.ReadCryptoInt(out version);
            if ((version != 6) && (version != 7)) {
                throw new FormatException("ERR_UNSUPPORTED_VERSION");
            }

            reader.ReadAndDiscardUID();
            ReadCharacterInfo();
            Bio.Read(reader);

            Inventory.Read(reader);
        }

        /// <summary>
        /// Must be called after reading summary
        /// </summary>
        public void ReadGrimcalc() {
            Debug.Assert(reader != null, "Please call ReadySummary prior to reading Grim Calc data");

            new GDCharStash().Read(reader);

            new GDCharRespawnList().Read(reader);
            new GDCharTeleportList().Read(reader);
            new GDCharMarkerList().Read(reader);
            new GDCharShrineList().Read(reader);
            SkillList.Read(reader);
        }


        private void AddRecords(List<string> regulars, List<string> greens, GDItem item) {
            if (!string.IsNullOrEmpty(item.PrefixRecord) || !string.IsNullOrEmpty(item.SuffixRecord)) {
                greens.Add(item.PrefixRecord);
                greens.Add(item.BaseRecord);
                greens.Add(item.SuffixRecord);
                greens.Add(item.ComponentRecord);
                greens.Add(item.AugmentRecord);
            }
            else {
                regulars.Add(item.BaseRecord);

                if (!string.IsNullOrEmpty(item.ComponentRecord))
                    regulars.Add(item.ComponentRecord);

                if (!string.IsNullOrEmpty(item.AugmentRecord))
                    regulars.Add(item.AugmentRecord);
            }
        }


        public string URL {
            get {
                List<string> records = new List<string>();
                List<string> greens = new List<string>();

                GDItem[] elems = Inventory.Equipment;
                foreach (var item in elems) {
                    AddRecords(records, greens, item);
                }
                
                elems = Inventory.Weapon1;
                foreach (var item in elems) {
                    AddRecords(records, greens, item);
                }


                var url = "?records=" + string.Join(",", records.Select(m => CleanRecord(m)));
                if (greens.Count > 0) {
                    url += "&cmb=" + string.Join(",", greens.Select(m => CleanRecord(m)));
                }

                return url;
            }
        }

        public void Print() {

            Console.WriteLine("Character: " + Name);
            Console.WriteLine("Level: " + Level);

            /*           int str = (int)(bio.getPhysique() - 50) / 8;
                       int agi = (int)(bio.getCunning() - 50) / 8;
                       int inte = (int)(bio.getSpirit() - 50) / 8;
                       string attributes = str + "/" + agi + "/" + inte;
                       Console.WriteLine("Attributes: " + attributes);*/

        }


        private string CleanRecord(string record) {
            return record.Substring(1 + record.LastIndexOf('/')).Replace(".dbr", "").Replace('%', '_');
        }
    }
}
