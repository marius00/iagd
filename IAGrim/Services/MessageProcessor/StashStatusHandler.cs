using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.UI.Misc;
using log4net;
using IAGrim.Utilities;
using IAGrim.Utilities.HelperClasses;
using System.Windows.Forms;
using EvilsoftCommons.Exceptions;

namespace IAGrim.Services.MessageProcessor {
    class StashStatusHandler : IMessageProcessor {
        private readonly MessageType[] Relevants = new MessageType[] {
            MessageType.TYPE_OPEN_PRIVATE_STASH,
            MessageType.TYPE_OPEN_CLOSE_TRANSFER_STASH,
            MessageType.TYPE_InventorySack_Sort,
            MessageType.TYPE_SAVE_TRANSFER_STASH,
            MessageType.TYPE_ERROR_HOOKING_SAVETRANSFER_STASH,
            MessageType.TYPE_DISPLAY_CRAFTER,
            MessageType.TYPE_RequestRestrictedSack
        };
        private readonly MessageType[] Errors = new MessageType[] {
            MessageType.TYPE_ERROR_HOOKING_PRIVATE_STASH,
            MessageType.TYPE_ERROR_HOOKING_TRANSFER_STASH,
            MessageType.TYPE_ERROR_HOOKING_SAVETRANSFER_STASH
        };

        private ILog logger = LogManager.GetLogger(typeof(StashStatusHandler));

        private enum StashStatus {
            Open, Closed, Unknown, Crafting
        };

        private StashStatus TransferStashStatus = StashStatus.Unknown;
        private StashStatus PrivateStashStatus = StashStatus.Unknown;
        private bool IsClosed => TransferStashStatus == StashStatus.Closed && PrivateStashStatus == StashStatus.Closed;
            
        

        public void Process(MessageType type, byte[] data) {
            if (Errors.Contains(type)) {
                var error = Errors.FirstOrDefault(m => m == type);
                
                logger.Fatal($"GD Hook reports error detecting {error}, unable to detect stash status");
                ExceptionReporter.ReportIssue($"DLL Incompatability: {error}");
                MessageBox.Show("Alert dev - Possible version incompatibility", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (Relevants.Contains(type)) {
                if (data.Length != 1 && type == MessageType.TYPE_OPEN_CLOSE_TRANSFER_STASH)
                    logger.WarnFormat("Received a Open/Close stash message from hook, expected length 1, got length {0}", data.Length);


                switch (type) {
                    case MessageType.TYPE_OPEN_PRIVATE_STASH:
                        PrivateStashStatus = StashStatus.Open;
                        logger.Debug("Private Stash opened");
                        break;

                    case MessageType.TYPE_DISPLAY_CRAFTER:
                        if (PrivateStashStatus != StashStatus.Open) {
                            PrivateStashStatus = StashStatus.Crafting;
                            GlobalSettings.StashStatus = StashAvailability.CRAFTING;
                        }
                        break;

                    case MessageType.TYPE_OPEN_CLOSE_TRANSFER_STASH:
                        bool isOpen = ((int)data[0]) != 0;
                        TransferStashStatus = isOpen ? StashStatus.Open : StashStatus.Closed;
                        break;

                    case MessageType.TYPE_SAVE_TRANSFER_STASH:
                        PrivateStashStatus = StashStatus.Closed;
                        TransferStashStatus = StashStatus.Closed;
                        break;
                }

                switch (type) {
                    case MessageType.TYPE_InventorySack_Sort:
                        GlobalSettings.StashStatus = StashAvailability.SORTED;
                        break;

                    case MessageType.TYPE_OPEN_PRIVATE_STASH:
                    case MessageType.TYPE_OPEN_CLOSE_TRANSFER_STASH:
                    case MessageType.TYPE_SAVE_TRANSFER_STASH:
                        // Sorted can still be open, but lets treat it as sorted since we can't transfer items without knowing position
                        if (GlobalSettings.StashStatus != StashAvailability.SORTED && !IsClosed)
                            GlobalSettings.StashStatus = StashAvailability.OPEN;

                        else if (IsClosed) {
                            GlobalSettings.StashStatus = StashAvailability.CLOSED;
                            logger.Debug("Closed stash");
                        }

                        break;
                }
            }
        }
    }
}
