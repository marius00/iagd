using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.UI.Misc {

    internal class StashTransferEventArgs : EventArgs {
        public StashTransferEventArgs(object[] identifier, bool transferAll) {
            this.InternalId = identifier;
            this.TransferAll = transferAll;
        }

        public string Prefix => InternalId[1] as string;
        public string BaseRecord => InternalId[0] as string;
        public string Suffix => InternalId[2] as string;
        public string Materia => InternalId[3] as string;
        public string Mod => InternalId[4] as string;
        public Boolean IsHardcore => (bool)InternalId[5];

        public bool HasValidId => InternalId != null && InternalId.Length == 6 && BaseRecord != null;
        private object[] InternalId { get; }
        public bool TransferAll { get; }

        public override string ToString() {
            return $"StashTransferEventArgs[Prefix:{Prefix} Base:{BaseRecord} Suffix:{Suffix} Materia:{Materia}, Mod:{Mod}, IsHardcore:{IsHardcore}]";
        }

        public bool IsSuccessful { get; set; }
        public int NumTransferred { get; set; }
    }
}
