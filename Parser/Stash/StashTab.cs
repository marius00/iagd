using System;
using System.Collections.Generic;
using System.Linq;
using IAGrim.StashFile;

namespace IAGrim.Parser.Stash {
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
            "records/items/questitems/quest_dynamite.dbr",
            "records/items/crafting/materials/craft_skeletonkey.dbr",
            "records/items/crafting/materials/craft_sacrifice.dbr",
            "records/items/crafting/materials/craft_ironexchange_02.dbr",
            "records/items/crafting/materials/craft_ironexchange_01.dbr",
            "records/items/crafting/materials/craft_celestiallotus.dbr",
            "records/items/crafting/materials/craft_eldritchessence.dbr",
            "records/items/crafting/materials/craft_aethercrystalcluster.dbr",
            "records/items/crafting/consumables/potion_freezeresist01.dbr",
            "records/items/crafting/blueprints/other/craft_potion_chaosresist.dbr",
            "records/items/crafting/blueprints/other/craft_potion_lightningresist.dbr",
            "records/items/crafting/blueprints/other/craft_potion_freezeresist.dbr",
            "records/items/crafting/consumables/potion_aetherresist01.dbr",
            "records/items/crafting/blueprints/other/craft_potion_coldresist.dbr",
            "records/items/crafting/blueprints/other/craft_potion_aetherresist.dbr",
            "records/items/crafting/blueprints/other/craft_potion_fireresist.dbr",
            "records/items/crafting/blueprints/other/craft_potion_poisonresist.dbr",
            "records/items/crafting/blueprints/other/craft_potion_royaljellyointment.dbr",
            "records/items/crafting/blueprints/other/craft_potion_vitalityresist.dbr",
            "records/items/crafting/consumables/potion_chaosresist01.dbr",
            "records/items/crafting/consumables/potion_coldresist01.dbr",
            "records/items/crafting/consumables/potion_fireresist01.dbr",
            "records/items/crafting/consumables/potion_lightningresist01.dbr",
            "records/items/crafting/consumables/potion_poisonresist01.dbr",
            "records/items/crafting/consumables/potion_royaljellyointment.dbr",
            "records/items/crafting/consumables/potion_vitalityresist01.dbr",
            "records/items/crafting/blueprints/other/craft_potionb103.dbr",
            "records/items/crafting/consumables/potion_bleedresist01.dbr"

        };

        private Block _block = new Block();

        public uint Width = 10u;

        public uint Height = 18u;

        public List<Item> Items { get; private set; }

        public StashTab() {
            this.Items = new List<Item>();
        }

        public static bool CanStack(string slot) {
            return StackableSlots.Contains(slot);
        }

        public bool Read(GDCryptoDataBuffer pCrypto) {
            uint num = 0;
            bool flag = !Block.ReadStart(out this._block, pCrypto) || !pCrypto.ReadCryptoUInt(out this.Width) || !pCrypto.ReadCryptoUInt(out this.Height) || !pCrypto.ReadCryptoUInt(out num);
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
                bool flag3 = !this._block.ReadEnd(pCrypto);
                result = !flag3;
            }
            return result;
        }

        public void Write(DataBuffer pBuffer) {
            this._block.WriteStart(0, pBuffer);
            pBuffer.WriteUInt(this.Width);
            pBuffer.WriteUInt(this.Height);
            if ((this.Items == null) || (this.Items.Count < 1)) {
                pBuffer.WriteUInt(0);
            }
            else {
                pBuffer.WriteUInt((uint)this.Items.Count);
                foreach (var item in this.Items) {
                    item.Write(pBuffer);
                }
            }
            this._block.WriteEnd(pBuffer);
        }


    }

}