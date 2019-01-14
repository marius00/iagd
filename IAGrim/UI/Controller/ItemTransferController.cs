using IAGrim.Database;
using IAGrim.Database.Interfaces;
using IAGrim.Parsers.Arz;
using IAGrim.Services;
using IAGrim.Services.MessageProcessor;
using IAGrim.UI.Misc;
using IAGrim.UI.Misc.CEF;
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

namespace IAGrim.UI.Controller
{
    internal class ItemTransferController {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ItemTransferController));
        private readonly IPlayerItemDao _dao;
        private readonly Action<string> _setFeedback;
        private readonly Action<string> _setTooltip;
        private readonly ISettingsReadController _settingsController;
        private readonly SearchWindow _searchWindow;
        private readonly DynamicPacker _dynamicPacker;
        private readonly CefBrowserHandler _browser;
        private readonly StashManager _stashManager;
        private readonly ItemStatService _itemStatService;

        public ItemTransferController(
                CefBrowserHandler browser,
                Action<string> feedback,
                Action<string> setTooltip,
                ISettingsReadController settingsController,
                SearchWindow searchWindow,
                DynamicPacker dynamicPacker,
                IPlayerItemDao playerItemDao,
                StashManager stashManager,
                ItemStatService itemStatService
            ) {
            _browser = browser;
            _setFeedback = feedback;
            _setTooltip = setTooltip;
            _settingsController = settingsController;
            _searchWindow = searchWindow;
            _dynamicPacker = dynamicPacker;
            _dao = playerItemDao;
            _stashManager = stashManager;
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
                _logger.Warn("Attempted to transfer NULL item.");

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
            int numItemsReceived = (int)items.Sum(item => Math.Max(1, item.StackCount));
            int numItemsRequested = Math.Min(maxItemsToTransfer, numItemsReceived);

            _itemStatService.ApplyStatsToPlayerItems(items); // For item class? 'IsStackable' maybe?
            _stashManager.Deposit(transferFile, items, maxItemsToTransfer, out error);
            _dao.Update(items, true);

            var numItemsAfterTransfer = items.Sum(item => item.StackCount);
            long numItemsTransferred = numReceived - numItemsAfterTransfer;

            if (!string.IsNullOrEmpty(error)) {
                _logger.Warn(error);
                _browser.ShowMessage(error, UserFeedbackLevel.Error);
            }

            return new TransferStatus {
                NumItemsTransferred = (int)numItemsTransferred,
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

                _logger.Info(GlobalSettings.Language.GetTag("iatag_no_stash_abort"));
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
            _logger.Debug($"Item transfer requested, arguments: {args}");

            if (CanTransfer()) {
                List<PlayerItem> items = GetItemsForTransfer(args);

                if (items?.Count > 0) {
                    string file = GetTransferFile();
                    if (string.IsNullOrEmpty(file)) {
                        _logger.Warn("Could not find transfer file, transfer aborted.");
                        return;
                    }

                    _logger.Debug($"Found {items.Count} items to transfer");
                    var result = TransferItems(file, items, (int) args.Count);

                    _logger.InfoFormat("Successfully deposited {0} out of {1} items", result.NumItemsTransferred,
                        result.NumItemsRequested);
                    try {
                        /*var message = string.Format(GlobalSettings.Language.GetTag("iatag_stash3_success"),
                            result.NumItemsTransferred, result.NumItemsRequested);
                        _setFeedback(message);
                        _browser.ShowMessage(message, UserFeedbackLevel.Success);*/
                    }
                    catch (FormatException ex) {
                        _logger.Warn(ex.Message);
                        _logger.Warn(ex.StackTrace);
                        _logger.Warn("Language pack error, iatag_stash3_success is of invalid format.");
                    }
                    if (result.NumItemsTransferred == 0) {
                        _setTooltip(GlobalSettings.Language.GetTag("iatag_stash3_failure"));
                        _browser.ShowMessage(GlobalSettings.Language.GetTag("iatag_stash3_failure"), UserFeedbackLevel.Warning);
                    }

                    // Lets do this last, to make sure feedback reaches the user as fast as possible
                    _searchWindow.UpdateListview();
                }
                else {
                    _logger.Warn("Could not find any items for the requested transfer");
                }
            }
            else if (GlobalSettings.StashStatus == StashAvailability.OPEN) {
                // "InstaTransfer" is an experimental feature
                if (Properties.Settings.Default.InstaTransfer) { // disabled for now
                    List<PlayerItem> items = GetItemsForTransfer(args);
                    int numDepositedItems = 0;
                    int numItemsToTransfer = items?.Count ?? 0;

                    if (items == null) {
                        _logger.Info("Item previously deposited (ghost item)");
                        _browser.ShowMessage("Item previously deposited (ghost item)", UserFeedbackLevel.Warning);
                        _searchWindow.UpdateListviewDelayed();
                    }
                    else {
                        foreach (var item in items) {
                            if (TransferToOpenStash(item)) {
                                _dao.Remove(item);
                                numDepositedItems++;
                            }
                        }

                        var message = string.Format(GlobalSettings.Language.GetTag("iatag_stash3_success"), numDepositedItems, numItemsToTransfer);
                        _setFeedback(message);
                        _browser.ShowMessage(message, UserFeedbackLevel.Success);

                        _logger.InfoFormat("Successfully deposited {0} out of {1} items", numDepositedItems, numItemsToTransfer);
                        if (numDepositedItems > 0) {
                            _searchWindow.UpdateListviewDelayed();
                        }
                    }
                }
                else {
                    var message = GlobalSettings.Language.GetTag("iatag_deposit_stash_open");
                    _setFeedback(message);
                    _setTooltip(message);
                    _browser.ShowMessage(message, UserFeedbackLevel.Warning);
                }
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

        public bool TransferToOpenStash(PlayerItem pi) {
            List<byte> buffer = ItemInjectCallbackProcessor.Serialize(pi);
            var stashTab = Properties.Settings.Default.StashToDepositTo;
            if (stashTab == 0) {
                // Find real tab.. Related to hotfix v1.0.4.0
                stashTab = StashManager.GetNumStashPages(GetTransferFile());
            }

            var position = _dynamicPacker.Insert(pi.BaseRecord, (uint)pi.Seed);
            if (position == null) {
                _logger.Warn("Item insert canceled, no valid position found.");
                _setFeedback(GlobalSettings.Language.GetTag("iatag_deposit_stash_full"));
                return false;
            }
            buffer.InsertRange(0, BitConverter.GetBytes(position.X * 32));
            buffer.InsertRange(4, BitConverter.GetBytes(position.Y * 32));
            buffer.InsertRange(0, BitConverter.GetBytes(stashTab));

            using (NamedPipeClientStream pipeStream = new NamedPipeClientStream(".", "gdiahook", PipeDirection.InOut, PipeOptions.Asynchronous)) {
                // The connect function will indefinitely wait for the pipe to become available
                // If that is not acceptable specify a maximum waiting time (in ms)
                try {
                    pipeStream.Connect(250);
                } catch (TimeoutException) {
                    return false;
                }

                pipeStream.Write(buffer.ToArray(), 0, buffer.Count);
                _logger.Debug("Wrote item to pipe");
                _setFeedback(GlobalSettings.Language.GetTag("iatag_deposit_pipe_success"));
            }

            return true;
        }
    }
}
