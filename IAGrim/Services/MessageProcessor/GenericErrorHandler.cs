﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.UI.Misc;
using log4net;
using EvilsoftCommons;

namespace IAGrim.Services.MessageProcessor {
    class GenericErrorHandler : IMessageProcessor {
        private readonly ILog _logger = LogManager.GetLogger(typeof(GenericErrorHandler));

        public void Process(MessageType type, byte[] data, string dataString) {
            if (type == MessageType.TYPE_ERROR_HOOKING_GENERIC) {
                int method = IOHelper.GetInt(data, 0);
                _logger.Error($"Error Hooking method \"{method}\"");
                if (method == 11 || method == 12 || method == 13) { // Steam cloud sync
                    _logger.Info("If you are using the GOG version, this is normal.");
                }
            }
            else if (type == MessageType.TYPE_SUCCESS_HOOKING_GENERIC) {
                int method = IOHelper.GetInt(data, 0);
                _logger.Debug($"Success hooking method \"{(MessageType)method}\" ({method})");
            }
        }
    }
}
