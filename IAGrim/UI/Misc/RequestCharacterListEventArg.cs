using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.Backup.Cloud.Dto;

namespace IAGrim.UI.Misc {
    class RequestCharacterListEventArg : EventArgs {
        public List<CharacterListDto> Characters { get; set; }
    }
}
