using IAGrim.UI.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Services.MessageProcessor {
    interface IMessageProcessor {
        void Process(MessageType type, byte[] data);
    }
}
