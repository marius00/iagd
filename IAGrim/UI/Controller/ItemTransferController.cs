using IAGrim.Database;
using IAGrim.Database.Interfaces;
using IAGrim.Parsers.Arz;
using IAGrim.UI.Misc.CEF;
using IAGrim.Utilities;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using IAGrim.UI.Misc;
using IAGrim.Settings;
using System.Windows.Forms;
using static IAGrim.UI.StashPicker;

namespace IAGrim.UI.Controller {
    internal class ItemTransferController {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ItemTransferController));
        private readonly IPlayerItemDao _dao;
        private readonly Action<string> _setFeedback;
        private readonly CefBrowserHandler _browser;
        private readonly TransferStashService _transferStashService;
        private readonly SettingsService _settingsService;

        public ItemTransferController(
            CefBrowserHandler browser,
            Action<string> feedback,
            IPlayerItemDao playerItemDao,
            TransferStashService transferStashService,
            SettingsService settingsService
            ) {
            _browser = browser;
            _setFeedback = feedback;
            _dao = playerItemDao;
            _transferStashService = transferStashService;
            _settingsService = settingsService;
        }


        List<PlayerItem> GetItemsForTransfer(StashTransferEventArgs args) {
            List<PlayerItem> items = new List<PlayerItem>();

            // Detect the record type (long or string) and add the item(s)
            if (args.HasValidId) {
                var pid = args.PlayerItemId;
                if (pid.HasValue) {
                    var item = _dao.GetById(pid.Value);
                    items.Add(item);
                }
                else {
                    IList<PlayerItem> tmp = _dao.GetByRecord(args.Prefix, args.BaseRecord, args.Suffix, args.Materia, args.Mod, args.IsHardcore);
                    if (tmp.Count > 0) {
                        if (!args.TransferAll)
                            Logger.Warn("Error transferring item, transfer all was false, but no player item id was located.");
                        else {
                            items.AddRange(tmp);
                        }
                    }
                }
            }

            if (items.Contains(null)) {
                Logger.Warn("Attempted to transfer NULL item.");

                var message = RuntimeSettings.Language.GetTag("iatag_feedback_item_does_not_exist");
                _setFeedback(message);
                _browser.ShowMessage(message, UserFeedbackLevel.Danger);

                return null;
            }

            return items;
        }

        struct TransferStatus {
            public int NumItemsTransferred;
        }


        private TransferStatus TransferItems(List<PlayerItem> items, StashPickerResult modOverride) {
            int numItemsReceived = (int)items.Sum(item => Math.Max(1, item.StackCount));
            _transferStashService.Deposit(items, modOverride);
            _dao.Update(items, true);

            return new TransferStatus {
                NumItemsTransferred = (int)numItemsReceived,
            };
        }


        /// <summary>
        /// Transfer item request from sub control
        /// MUST BE CALLED ON SQL THREAD
        /// </summary>
        public void TransferItem(StashTransferEventArgs args) {
            Logger.Debug($"Item transfer requested, arguments: {args}");

            List<PlayerItem> items = GetItemsForTransfer(args);

            StashPickerResult modOverride = null;
            if (items?.Count > 0) {
                if (_settingsService.GetPersistent().TransferAnyMod) {
                    StashPicker picker = new StashPicker(_browser, _dao, _settingsService);
                    if (picker.ShowDialog() == DialogResult.OK) {
                        modOverride = picker.Result;
                    }
                }

                var result = TransferItems(items, modOverride);
                args.NumTransferred = result.NumItemsTransferred;
                args.IsSuccessful = true;
                var message = RuntimeSettings.Language.GetTag("iatag_stash3_success", result.NumItemsTransferred);
                _browser.ShowMessage(message, UserFeedbackLevel.Success);
            }
            else {
                Logger.Warn("Could not find any items for the requested transfer");
                _browser.ShowMessage(RuntimeSettings.Language.GetTag("iatag_feedback_unable_to_deposit"), UserFeedbackLevel.Warning);

            }
        }
    }
}