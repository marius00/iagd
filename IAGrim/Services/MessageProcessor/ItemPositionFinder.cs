using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.UI.Misc;
using EvilsoftCommons;
using log4net;
using IAGrim.Utilities.RectanglePacker;
using IAGrim.Utilities;

namespace IAGrim.Services.MessageProcessor {
    class ItemPositionFinder : IMessageProcessor {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ItemPositionFinder));
        private DynamicPacker _dynamicPacker;
        public ItemPositionFinder(DynamicPacker packer) {
            this._dynamicPacker = packer;
            GlobalSettings.StashStatusChanged += GlobalSettings_StashStatusChanged;
        }

        ~ItemPositionFinder() {
            GlobalSettings.StashStatusChanged -= GlobalSettings_StashStatusChanged;
        }

        private void GlobalSettings_StashStatusChanged(object sender, EventArgs e) {
            if (GlobalSettings.StashStatus == Utilities.HelperClasses.StashAvailability.OPEN)
                _dynamicPacker.Clear();
        }

        public void Process(MessageType type, byte[] data) {
            switch (type) {
                case MessageType.TYPE_Stash_Item_BasicInfo: {
                        uint pos = 0;
                        int stash = IOHelper.GetInt(data, (int)pos);
                        pos += 4;

                        int source = data[pos];
                        pos += 1;

                        uint x = ((uint)Math.Ceiling(IOHelper.GetFloat(data, pos))) / 32;
                        pos += 4;

                        uint y = ((uint)Math.Ceiling(IOHelper.GetFloat(data, pos))) / 32;
                        pos += 4;

                        uint seed = IOHelper.GetUInt(data, pos);
                        pos += 4;

                        string baseRecord = IOHelper.GetPrefixString(data, (int)pos);
                        if (stash == 3) {
                            _dynamicPacker.Insert(baseRecord, seed, x, y);
                            Logger.Debug($"[Src{source}] Item 3 construct: X:{x} Y:{y} Seed:{seed} {baseRecord}");
                        }
                        else {
                            Logger.Debug($"[Src{source}] Stash {stash} construct: X:{x} Y:{y} Seed:{seed} {baseRecord}");
                        }
                    }
                    break;
            }
        }
    }
}
