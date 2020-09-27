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
        };

        private readonly MessageType[] Errors = new MessageType[] {
            MessageType.TYPE_ERROR_HOOKING_PRIVATE_STASH,
            MessageType.TYPE_ERROR_HOOKING_TRANSFER_STASH,
            MessageType.TYPE_ERROR_HOOKING_SAVETRANSFER_STASH
        };

        private ILog logger = LogManager.GetLogger(typeof(StashStatusHandler));

        private enum InternalStashStatus {
            Open, Closed, Unknown, Crafting
        };

        private InternalStashStatus TransferStashStatus = InternalStashStatus.Unknown;
        private InternalStashStatus PrivateStashStatus = InternalStashStatus.Unknown;
        private bool IsClosed => TransferStashStatus == InternalStashStatus.Closed && PrivateStashStatus == InternalStashStatus.Closed;
            
        

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
                        PrivateStashStatus = InternalStashStatus.Open;
                        logger.Debug("Private Stash opened");
                        break;

                    case MessageType.TYPE_DISPLAY_CRAFTER:
                        if (PrivateStashStatus != InternalStashStatus.Open) {
                            PrivateStashStatus = InternalStashStatus.Crafting;
                            RuntimeSettings.StashStatus = StashAvailability.CRAFTING;
                        }
                        break;

                    case MessageType.TYPE_CAN_USE_DISMANTLE:
                        if (PrivateStashStatus != InternalStashStatus.Open) {
                            PrivateStashStatus = InternalStashStatus.Crafting;
                            RuntimeSettings.StashStatus = StashAvailability.CRAFTING;
                        }
                        break;

                    case MessageType.TYPE_Display_Transmute:
                        if (PrivateStashStatus != InternalStashStatus.Open) {
                            PrivateStashStatus = InternalStashStatus.Crafting;
                            RuntimeSettings.StashStatus = StashAvailability.CRAFTING;
                        }

                        logger.Debug("TYPE_Display_Transmute");
                        break;
                        
                    case MessageType.TYPE_OPEN_CLOSE_TRANSFER_STASH:
                        bool isOpen = ((int)data[0]) != 0;
                        TransferStashStatus = isOpen ? InternalStashStatus.Open : InternalStashStatus.Closed;
                        break;

                    case MessageType.TYPE_SAVE_TRANSFER_STASH:
                        PrivateStashStatus = InternalStashStatus.Closed;
                        TransferStashStatus = InternalStashStatus.Closed;
                        break;
                }

                switch (type) {
                    case MessageType.TYPE_InventorySack_Sort:
                        RuntimeSettings.StashStatus = StashAvailability.SORTED;
                        break;

                    case MessageType.TYPE_OPEN_PRIVATE_STASH:
                    case MessageType.TYPE_OPEN_CLOSE_TRANSFER_STASH:
                    case MessageType.TYPE_SAVE_TRANSFER_STASH:
                        // Sorted can still be open, but lets treat it as sorted since we can't transfer items without knowing position
                        if (RuntimeSettings.StashStatus != StashAvailability.SORTED && !IsClosed)
                            RuntimeSettings.StashStatus = StashAvailability.OPEN;

                        else if (IsClosed) {
                            RuntimeSettings.StashStatus = StashAvailability.CLOSED;
                            logger.Debug("Closed stash");
                        }

                        break;
                }
            }
        }
    }
}
