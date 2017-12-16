using EvilsoftCommons;
using IAGrim.UI.Misc;
using log4net;

namespace IAGrim.Services.MessageProcessor
{
    internal class PlayerPositionTracker : IMessageProcessor
    {
        private readonly ILog Logger = LogManager.GetLogger(typeof(PlayerPositionTracker));

        public void Process(MessageType type, byte[] data)
        {
            switch (type)
            {
                case MessageType.TYPE_ControllerPlayerStateIdleRequestMoveAction:
                case MessageType.TYPE_ControllerPlayerStateIdleRequestNpcAction:
                case MessageType.TYPE_ControllerPlayerStateMoveToRequestNpcAction:
                case MessageType.TYPE_ControllerPlayerStateMoveToRequestMoveAction:
                {
                    GrimStateTracker.LastKnownPosition = new GrimStateTracker.WorldVector {
                        X = IOHelper.GetFloat(data, 4),
                        Y = IOHelper.GetFloat(data, 8),
                        Z = IOHelper.GetFloat(data, 12),
                        Zone = IOHelper.GetInt(data, 0)
                    };

                    //Logger.Debug($"Received a TYPE_Move({IOHelper.GetFloat(data, 4)}, {IOHelper.GetFloat(data, 8)}, {IOHelper.GetFloat(data, 12)}, {IOHelper.GetInt(data, 0)}");
                }

                    //Logger.DebugFormat("Received a TYPE_Move({0}, {1}, {2}, {5}), ({3},{4})", x, y, z, data[data.Length - 2], data[data.Length - 1], zone);
                    break;

                case MessageType.TYPE_OPEN_PRIVATE_STASH:
                case MessageType.TYPE_OPEN_CLOSE_TRANSFER_STASH:
                    if (data.Length == 1)
                    {
                        bool isOpen = data[0] != 0;
                        if (isOpen)
                            GrimStateTracker.LastStashPosition = GrimStateTracker.LastKnownPosition;
                    }
                    else
                    {
                        Logger.WarnFormat(
                            "Received a Open/Close stash message from hook, expected length 1, got length {0}",
                            data.Length);
                    }
                    break;
            }
        }
    }
}