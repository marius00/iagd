using EvilsoftCommons;
using IAGrim.UI.Misc;
using log4net;

namespace IAGrim.Services.MessageProcessor {
    internal class PlayerPositionTracker : IMessageProcessor {
        private readonly bool _debugPlayerPositions;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(PlayerPositionTracker));

        public PlayerPositionTracker(bool debugPlayerPositions) {
            _debugPlayerPositions = debugPlayerPositions;
        }

        public void Process(MessageType type, byte[] data) {
             switch (type) {
                case MessageType.TYPE_ControllerPlayerStateIdleRequestMoveAction: {
                    GrimStateTracker.LastKnownPosition = new GrimStateTracker.WorldVector {
                        X = IOHelper.GetFloat(data, 4),
                        Y = IOHelper.GetFloat(data, 8),
                        Z = IOHelper.GetFloat(data, 12),
                        Zone = IOHelper.GetInt(data, 0)
                    };

                    if (_debugPlayerPositions) {
                        Logger.Debug(GrimStateTracker.LastKnownPosition);
                    }
                }
                    break;

                case MessageType.TYPE_ControllerPlayerStateIdleRequestNpcAction:
                case MessageType.TYPE_ControllerPlayerStateMoveToRequestNpcAction: {
                    GrimStateTracker.LastKnownPosition = new GrimStateTracker.WorldVector {
                        X = IOHelper.GetFloat(data, 4),
                        Y = IOHelper.GetFloat(data, 8),
                        Z = IOHelper.GetFloat(data, 12),
                        Zone = IOHelper.GetInt(data, 0)
                    };

                    if (_debugPlayerPositions) {
                        Logger.Debug(GrimStateTracker.LastKnownPosition);
                    }

                    break;
                }

                case MessageType.TYPE_ControllerPlayerStateMoveToRequestMoveAction: {
                    GrimStateTracker.LastKnownPosition = new GrimStateTracker.WorldVector {
                        X = IOHelper.GetFloat(data, 4),
                        Y = IOHelper.GetFloat(data, 8),
                        Z = IOHelper.GetFloat(data, 12),
                        Zone = IOHelper.GetInt(data, 0)
                    };

                    if (_debugPlayerPositions) {
                        Logger.Debug(GrimStateTracker.LastKnownPosition);
                    }
                }
                    break;

                case MessageType.TYPE_OPEN_PRIVATE_STASH:
                case MessageType.TYPE_OPEN_CLOSE_TRANSFER_STASH:
                    if (data.Length == 1) {
                        bool isOpen = data[0] != 0;
                        if (isOpen)
                            GrimStateTracker.LastStashPosition = GrimStateTracker.LastKnownPosition;
                    }
                    else {
                        Logger.WarnFormat(
                            "Received a Open/Close stash message from hook, expected length 1, got length {0}",
                            data.Length);
                    }

                    break;
            }
        }
    }
}