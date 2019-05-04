using IAGrim.Database;
using IAGrim.Database.Interfaces;
using IAGrim.Parsers.Arz;
using IAGrim.Services;
using IAGrim.Services.MessageProcessor;
using IAGrim.UI.Misc;
using IAGrim.UI.Misc.CEF;
using IAGrim.UI.Tabs;
using IAGrim.Utilities;
using IAGrim.Utilities.HelperClasses;
using IAGrim.Utilities.RectanglePacker;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Windows.Forms;

namespace IAGrim.UI.Controller {
    internal class ItemTransferController {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ItemTransferController));
        private readonly IPlayerItemDao _dao;
        private readonly Action<string> _setFeedback;
        private readonly Action<string> _setTooltip;
        private readonly ISettingsReadController _settingsController;
        private readonly SplitSearchWindow _searchWindow;
        private readonly DynamicPacker _dynamicPacker;
        private readonly CefBrowserHandler _browser;
        private readonly TransferStashService _transferStashService;
        private readonly ItemStatService _itemStatService;

        public ItemTransferController(
            CefBrowserHandler browser,
            Action<string> feedback,
            Action<string> setTooltip,
            ISettingsReadController settingsController,
            SplitSearchWindow searchWindow,
            DynamicPacker dynamicPacker,
            IPlayerItemDao playerItemDao,
            TransferStashService transferStashService,
            ItemStatService itemStatService
        ) {
            _browser = browser;
            _setFeedback = feedback;
            _setTooltip = setTooltip;
            _settingsController = settingsController;
            _searchWindow = searchWindow;
            _dynamicPacker = dynamicPacker;
            _dao = playerItemDao;
            _transferStashService = transferStashService;
            _itemStatService = itemStatService;
        }

        List<PlayerItem> GetItemsForTransfer(StashTransferEventArgs args) {
            List<PlayerItem> items = new List<PlayerItem>();

            // Detect the record type (long or string) and add the item(s)
            if (args.Id != null && args.Id > 0) {
                items.Add(_dao.GetById(args.Id.Value));
            }
            else if (args.HasValidId) {
                IList<PlayerItem> tmp = _dao.GetByRecord(args.Prefix, args.BaseRecord, args.Suffix, args.Materia);
                if (tmp.Count > 0) {
                    if (args.Count == 1)
                        items.Add(tmp[0]);
                    else {
                        items.AddRange(tmp);
                    }
                }
            }

            if (items.Contains(null)) {
                Logger.Warn("Attempted to transfer NULL item.");

                var message = GlobalSettings.Language.GetTag("iatag_feedback_item_does_not_exist");
                _setFeedback(message);
                _browser.ShowMessage(message, UserFeedbackLevel.Error);

                return null;
            }

            return items;
        }

        struct TransferStatus {
            public int NumItemsRequested;
            public int NumItemsTransferred;
        }

        private TransferStatus TransferItems(string transferFile, List<PlayerItem> items, int maxItemsToTransfer) {
            var numReceived = items.Sum(item => Math.Max(1, item.StackCount));

            // Remove all items deposited (may or may not be less than the requested amount, if no inventory space is available)
            string error;
            int numItemsReceived = (int) items.Sum(item => Math.Max(1, item.StackCount));
            int numItemsRequested = Math.Min(maxItemsToTransfer, numItemsReceived);

            _itemStatService.ApplyStatsToPlayerItems(items); // For item class? 'IsStackable' maybe?
            _transferStashService.Deposit(transferFile, items, maxItemsToTransfer, out error);
            _dao.Update(items, true);

            var numItemsAfterTransfer = items.Sum(item => item.StackCount);
            long numItemsTransferred = numReceived - numItemsAfterTransfer;

            if (!string.IsNullOrEmpty(error)) {
                Logger.Warn(error);
                _browser.ShowMessage(error, UserFeedbackLevel.Error);
            }

            return new TransferStatus {
                NumItemsTransferred = (int) numItemsTransferred,
                NumItemsRequested = numItemsRequested
            };
        }

        private string GetTransferFile() {
            GDTransferFile mfi = _searchWindow.ModSelectionHandler.SelectedMod;
            bool fileExists = !string.IsNullOrEmpty(mfi.Filename) && File.Exists(mfi.Filename);

            if (_settingsController.TransferAnyMod || !fileExists) {
                StashPicker picker = new StashPicker();
                if (picker.ShowDialog() == DialogResult.OK) {
                    return picker.Result;
                }

                Logger.Info(GlobalSettings.Language.GetTag("iatag_no_stash_abort"));
                _setFeedback(GlobalSettings.Language.GetTag("iatag_no_stash_abort"));
                _browser.ShowMessage(GlobalSettings.Language.GetTag("iatag_no_stash_abort"), UserFeedbackLevel.Error);

                return string.Empty;
            }

            return mfi.Filename;
        }

        private bool CanTransfer() {
            return GlobalSettings.StashStatus == StashAvailability.CLOSED
                   || !_settingsController.SecureTransfers
                   || (GlobalSettings.StashStatus == StashAvailability.ERROR
                       && MessageBox.Show(
                           GlobalSettings.Language.GetTag("iatag_stash_status_error"),
                           "Warning",
                           MessageBoxButtons.YesNo,
                           MessageBoxIcon.Warning) == DialogResult.Yes);
        }

        /// <summary>
        /// Transfer item request from sub control
        /// MUST BE CALLED ON SQL THREAD
        /// </summary>
        public void TransferItem(object ignored, EventArgs _args) {
            StashTransferEventArgs args = _args as StashTransferEventArgs;
            Logger.Debug($"Item transfer requested, arguments: {args}");

            if (CanTransfer()) {
                List<PlayerItem> items = GetItemsForTransfer(args);

                if (items?.Count > 0) {
                    string file = GetTransferFile();
                    if (string.IsNullOrEmpty(file)) {
                        Logger.Warn("Could not find transfer file, transfer aborted.");
                        return;
                    }

                    Logger.Debug($"Found {items.Count} items to transfer");
                    var result = TransferItems(file, items, (int) args.Count);

                    Logger.InfoFormat("Successfully deposited {0} out of {1} items", result.NumItemsTransferred,
                        result.NumItemsRequested);
                    try {
                        var message = string.Format(GlobalSettings.Language.GetTag("iatag_stash3_success"),
                            result.NumItemsTransferred, result.NumItemsRequested);
                        _browser.ShowMessage(message, UserFeedbackLevel.Success);
                    }
                    catch (FormatException ex) {
                        Logger.Warn(ex.Message);
                        Logger.Warn(ex.StackTrace);
                        Logger.Warn("Language pack error, iatag_stash3_success is of invalid format.");
                    }

                    if (result.NumItemsTransferred == 0) {
                        _setTooltip(GlobalSettings.Language.GetTag("iatag_stash3_failure"));
                        _browser.ShowMessage(GlobalSettings.Language.GetTag("iatag_stash3_failure"), UserFeedbackLevel.Warning);
                    }

                    // Lets do this last, to make sure feedback reaches the user as fast as possible
                    _searchWindow.UpdateListView();
                }
                else {
                    Logger.Warn("Could not find any items for the requested transfer");
                }
            }
            else if (GlobalSettings.StashStatus == StashAvailability.OPEN) {
                var message = GlobalSettings.Language.GetTag("iatag_deposit_stash_open");
                _setFeedback(message);
                _setTooltip(message);
                _browser.ShowMessage(message, UserFeedbackLevel.Warning);
            }
            else if (GlobalSettings.StashStatus == StashAvailability.SORTED) {
                _setFeedback(GlobalSettings.Language.GetTag("iatag_deposit_stash_sorted"));
                _browser.ShowMessage(GlobalSettings.Language.GetTag("iatag_deposit_stash_sorted"), UserFeedbackLevel.Warning);
            }

            else if (GlobalSettings.StashStatus == StashAvailability.UNKNOWN) {
                _setFeedback(GlobalSettings.Language.GetTag("iatag_deposit_stash_unknown_feedback"));
                _setTooltip(GlobalSettings.Language.GetTag("iatag_deposit_stash_unknown_tooltip"));
                _browser.ShowMessage(GlobalSettings.Language.GetTag("iatag_deposit_stash_unknown_feedback"), UserFeedbackLevel.Warning);
            }
        }
    }
}