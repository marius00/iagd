using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.UI.Misc;
using log4net;
using EvilsoftCommons;

namespace IAGrim.Services.MessageProcessor {
    class GenericErrorHandler : IMessageProcessor {
        private ILog logger = LogManager.GetLogger(typeof(GenericErrorHandler));

        public void Process(MessageType type, byte[] data) {
            if (type == MessageType.TYPE_ERROR_HOOKING_GENERIC) {
                int method = IOHelper.GetInt(data, 0);
                logger.Error($"Error Hooking method \"{method}\"");
            }
        }
    }
}
