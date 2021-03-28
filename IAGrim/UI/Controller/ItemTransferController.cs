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
using System.Linq;
using System.Windows.Forms;
using IAGrim.Settings;

namespace IAGrim.UI.Controller {
    internal class ItemTransferController {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ItemTransferController));
        private readonly IPlayerItemDao _dao;
        private readonly Action<string> _setFeedback;
        private readonly Action<string> _setTooltip;
        private readonly SettingsService _settingsService;
        private readonly SplitSearchWindow _searchWindow;
        private readonly CefBrowserHandler _browser;
        private readonly TransferStashService _transferStashService;
        private readonly ItemStatService _itemStatService;

        public ItemTransferController(
            CefBrowserHandler browser,
            Action<string> feedback,
            Action<string> setTooltip,
            SplitSearchWindow searchWindow,
            IPlayerItemDao playerItemDao,
            TransferStashService transferStashService,
            ItemStatService itemStatService, 
            SettingsService settingsService
            ) {
            _browser = browser;
            _setFeedback = feedback;
            _setTooltip = setTooltip;
            _searchWindow = searchWindow;
            _dao = playerItemDao;
            _transferStashService = transferStashService;
            _itemStatService = itemStatService;
            _settingsService = settingsService;
        }

        List<PlayerItem> GetItemsForTransfer(StashTransferEventArgs args) {
            List<PlayerItem> items = new List<PlayerItem>();

            // Detect the record type (long or string) and add the item(s)
            if (args.HasValidId) {
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

                var message = RuntimeSettings.Language.GetTag("iatag_feedback_item_does_not_exist");
                _setFeedback(message);
                _browser.ShowMessage(message, UserFeedbackLevel.Danger);

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

            _itemStatService.ApplyStatsToPlayerItems(items, true); // For item class? 'IsStackable' maybe?
            try {
                _transferStashService.Deposit(transferFile, items, maxItemsToTransfer, out error);
                _dao.Update(items, true);

                var numItemsAfterTransfer = items.Sum(item => item.StackCount);
                long numItemsTransferred = numReceived - numItemsAfterTransfer;

                if (!string.IsNullOrEmpty(error)) {
                    Logger.Warn(error);
                    _browser.ShowMessage(error, UserFeedbackLevel.Danger);
                }

                return new TransferStatus {
                    NumItemsTransferred = (int)numItemsTransferred,
                    NumItemsRequested = numItemsRequested
                };
            }
            catch (TransferStashService.DepositException) {
                _browser.ShowMessage(RuntimeSettings.Language.GetTag("iatag_feedback_unable_to_deposit"), UserFeedbackLevel.Danger);
                return new TransferStatus {
                    NumItemsTransferred = 0,
                    NumItemsRequested = numItemsRequested
                };
            }
        }

        private string GetTransferFile() {
            GDTransferFile mfi = _searchWindow.ModSelectionHandler.SelectedMod;
            bool fileExists = !string.IsNullOrEmpty(mfi.Filename) && File.Exists(mfi.Filename);

            
            if (_settingsService.GetPersistent().TransferAnyMod || !fileExists) {
                StashPicker picker = new StashPicker(_browser);
                if (picker.ShowDialog() == DialogResult.OK) {
                    return picker.Result;
                }

                Logger.Info(RuntimeSettings.Language.GetTag("iatag_no_stash_abort"));
                _setFeedback(RuntimeSettings.Language.GetTag("iatag_no_stash_abort"));
                _browser.ShowMessage(RuntimeSettings.Language.GetTag("iatag_no_stash_abort"), UserFeedbackLevel.Danger);

                return string.Empty;
            }

            return mfi.Filename;
        }

        private bool CanTransfer() {
            bool secureTransfers = _settingsService.GetLocal().SecureTransfers.GetValueOrDefault(true);

            return RuntimeSettings.StashStatus == StashAvailability.CLOSED
                   || !secureTransfers
                   || (RuntimeSettings.StashStatus == StashAvailability.ERROR
                       && MessageBox.Show(
                           RuntimeSettings.Language.GetTag("iatag_stash_status_error"),
                           "Warning",
                           MessageBoxButtons.YesNo,
                           MessageBoxIcon.Warning) == DialogResult.Yes);

        }

        /// <summary>
        /// Transfer item request from sub control
        /// MUST BE CALLED ON SQL THREAD
        /// </summary>
        public void TransferItem(StashTransferEventArgs args) {
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

                    Logger.InfoFormat("Successfully deposited {0} out of {1} items", result.NumItemsTransferred, result.NumItemsRequested);
                    args.NumTransferred = result.NumItemsTransferred;
                    args.IsSuccessful = true;
                    
                    if (result.NumItemsTransferred > 0) {
                        var message = RuntimeSettings.Language.GetTag("iatag_stash3_success", result.NumItemsTransferred, result.NumItemsRequested);
                        _browser.ShowMessage(message, UserFeedbackLevel.Success);
                    } else if (result.NumItemsTransferred == 0) {
                        _setTooltip(RuntimeSettings.Language.GetTag("iatag_stash3_failure"));
                        _browser.ShowMessage(RuntimeSettings.Language.GetTag("iatag_stash3_failure"), UserFeedbackLevel.Warning);

                    }
                }
                else {
                    Logger.Warn("Could not find any items for the requested transfer");
                    _browser.ShowMessage(RuntimeSettings.Language.GetTag("iatag_feedback_unable_to_deposit"), UserFeedbackLevel.Warning);
                }
            }
            else if (RuntimeSettings.StashStatus == StashAvailability.OPEN) {
                var message = RuntimeSettings.Language.GetTag("iatag_deposit_stash_open");

                _setFeedback(message);
                _setTooltip(message);
                _browser.ShowMessage(message, UserFeedbackLevel.Warning);
            }
            else if (RuntimeSettings.StashStatus == StashAvailability.SORTED) {
                _setFeedback(RuntimeSettings.Language.GetTag("iatag_deposit_stash_sorted"));
                _browser.ShowMessage(RuntimeSettings.Language.GetTag("iatag_deposit_stash_sorted"), UserFeedbackLevel.Warning);
            }

            else if (RuntimeSettings.StashStatus == StashAvailability.UNKNOWN) {
                _setFeedback(RuntimeSettings.Language.GetTag("iatag_deposit_stash_unknown_feedback"));
                _setTooltip(RuntimeSettings.Language.GetTag("iatag_deposit_stash_unknown_tooltip"));
                _browser.ShowMessage(RuntimeSettings.Language.GetTag("iatag_deposit_stash_unknown_feedback"), UserFeedbackLevel.Warning);
            }
        }
    }
}