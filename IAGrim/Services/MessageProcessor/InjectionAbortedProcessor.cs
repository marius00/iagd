using IAGrim.Settings;
using IAGrim.UI.Misc;
using log4net;
using System;

namespace IAGrim.Services.MessageProcessor {
    class InjectionAbortedProcessor : IMessageProcessor {
        private readonly ILog _logger = LogManager.GetLogger(typeof(InjectionAbortedProcessor));
        private readonly Action _feedback;
        public InjectionAbortedProcessor(Action feedback) {
            this._feedback = feedback;
        }
        public void Process(MessageType type, byte[] data, string dataString) {
            if (type == MessageType.TYPE_INJECTION_CANCELLED) {
                _logger.Info("GD Reports that the injection was aborted, still in the menu.");
                _feedback();
            }
        }
    }
}
