using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using log4net;

namespace IAGrim.Services {
    /// <summary>
    /// Responsible for dispatching requests to the listener inside GD.
    /// The callee is responsible for sending data that can be understood.
    ///
    /// </summary>
    class ItemReplicaDispatchService {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ItemReplicaDispatchService));
        private volatile bool _isGrimDawnRunning = false;

        public bool DispatchItemSeedInfoRequest(List<byte> buffer) {
            if (buffer == null) {
                return false;
            }

            using (NamedPipeClientStream pipeStream = new NamedPipeClientStream(".", "gdiahook", PipeDirection.InOut, PipeOptions.Asynchronous)) {
                try {
                    pipeStream.Connect(250);
                }
                catch (TimeoutException) {
                    // Typically: The pipe does not exist
                    Logger.Debug("Timed out connecting to GD");
                    _isGrimDawnRunning = false;
                    return false;
                }
                catch (IOException ex) {
                    // Typical scenario: The pipe exists, but nobody are accepting connections.
                    Logger.Warn("IOException connecting to GD", ex);
                    _isGrimDawnRunning = false;
                    return false;

                }
                catch (Exception ex) {
                    Logger.Warn("Exception connecting to GD", ex);
                    return false;
                }

                pipeStream.Write(buffer.ToArray(), 0, buffer.Count);
#if DEBUG
                // Logger.Debug("Wrote item to pipe");
#endif
            }

            return true;
        }


        public void SetIsGrimDawnRunning(bool b) {
            _isGrimDawnRunning = b;
        }

        public bool GetIsGrimDawnRunning => _isGrimDawnRunning;

    }
}
