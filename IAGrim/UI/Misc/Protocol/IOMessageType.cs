using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.UI.Misc.Protocol {
    internal enum IOMessageType {
        ShowHelp,
        ShowMessage,
        ShowCharacterBackups,
        SetState,
        SetAggregateItemData,
        SetItems,
        SetCollectionItems,
        ShowModFilterWarning,
    }
}
