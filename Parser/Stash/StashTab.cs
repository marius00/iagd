using System;
using System.Collections.Generic;
using System.Linq;

namespace IAGrim.StashFile {
    public class StashTab {
        private static readonly string[] StackableSlots = { "ItemRelic", "OneShot_PotionHealth", "OneShot_PotionMana", "OneShot_Scroll" };

        // TODO: Load this dynamically
        // select s.id_databaseitem, i.name from databaseitemstat_v2 s, databaseitem_v2 i where stat = 'preventEasyDrops' and i.id_databaseitem = s.id_databaseitem -- These are stackable
        public static readonly string[] HardcodedRecords = {
            "records/items/crafting/materials/craft_bloodchthon.dbr",
            "records/items/crafting/materials/craft_manticore.dbr",
            "records/items/crafting/materials/craft_ancientheart.dbr",
            "records/items/crafting/materials/craft_royaljelly.dbr",
            "records/items/crafting/materials/craft_aethershard.dbr",
            "records/items/crafting/materials/craft_cultistseal.dbr",
            "records/items/crafting/materials/craft_taintedbrain.dbr",
            "records/items/questitems/scrapmetal.dbr",
            "records/items/materia/compa_aethercrystal.dbr",
            "records/items/crafting/materials/craft_ugdenbloom.dbr",
            "records/items/crafting/materials/craft_aetherialmissive.dbr",
            "records/items/crafting/materials/craft_aetherialmutagen.dbr",
            "records/items/crafting/materials/craft_heartofdarkness.dbr",
            "records/items/crafting/materials/craft_wendigospirit.dbr",
            "records/items/questitems/quest_dynamite.dbr"
        };

        public const int DEFAULT_WIDTH = 8;

        public const int DEFAULT_HEIGHT = 16;

        public Block Block = new Block();

        public uint Width = 8u;

        public uint Height = 16u;

        public List<Item> Items { get; private set; }

        public StashTab() {
            this.Items = new List<Item>();
        }

        public static bool CanStack(string slot) {
            return StackableSlots.Contains(slot);
        }

        /// <summary>
        /// Add an item to the tab
        /// If the item already exists, the stackcount is increased.
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(Item item, string slot) {
            var comparator = Items.Where(i => i.Equals(item));
            if ((CanStack(slot) || HardcodedRecords.Contains(item.BaseRecord)) && comparator.Any()) {
                Item existing = comparator.First();
                
                existing.StackCount += Math.Max(1, item.StackCount);

                // Prevent any stacksize from exceeding 100
                // This may not be correct for augments but is necessary for potions
                if (existing.StackCount > 100) {
                    item.StackCount = existing.StackCount - 100;
                    existing.StackCount = 100;
                    Items.Add(item);
                }
            }
            else {
                Items.Add(item);
            }
        }


        public bool Read(GDCryptoDataBuffer pCrypto) {
            uint num = 0;
            bool flag = !Block.ReadStart(out this.Block, pCrypto) || !pCrypto.ReadCryptoUInt(out this.Width) || !pCrypto.ReadCryptoUInt(out this.Height) || !pCrypto.ReadCryptoUInt(out num);
            bool result;
            if (flag) {
                result = false;
            }
            else {
                this.Items = new List<Item>();
                for (uint num2 = 0u; num2 < num; num2 += 1u) {
                    Item item = new Item();
                    bool flag2 = !item.Read(pCrypto);
                    if (flag2) {
                        result = false;
                        return result;
                    }
                    this.Items.Add(item);
                }
                bool flag3 = !this.Block.ReadEnd(pCrypto);
                result = !flag3;
            }
            return result;
        }

        public void Write(DataBuffer pBuffer) {
            this.Block.WriteStart(0, pBuffer);
            pBuffer.WriteUInt(this.Width);
            pBuffer.WriteUInt(this.Height);
            if ((this.Items == null) || (this.Items.Count < 1)) {
                pBuffer.WriteUInt(0);
            }
            else {
                pBuffer.WriteUInt((uint)this.Items.Count);
                for (int i = 0; i < this.Items.Count; i++) {
                    this.Items[i].Write(pBuffer);
                }
            }
            this.Block.WriteEnd(pBuffer);
        }


    }

}