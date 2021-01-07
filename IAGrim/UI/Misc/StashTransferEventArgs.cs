using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.UI.Misc {

    internal class StashTransferEventArgs : EventArgs {
        public StashTransferEventArgs(object[] identifier, int count) {
            this.InternalId = identifier;
            this.Count = count;
        }

        public string Prefix => InternalId[1] as string;
        public string BaseRecord => InternalId[0] as string;
        public string Suffix => InternalId[2] as string;
        public string Materia => InternalId[3] as string;

        public bool HasValidId => InternalId != null && InternalId.Length == 4 && BaseRecord != null;
        private object[] InternalId { get; }
        public int Count { get; }

        public override string ToString() {
            return $"StashTransferEventArgs[Prefix:{Prefix} Base:{BaseRecord} Suffix:{Suffix} Materia:{Materia}]";
        }

        public bool IsSuccessful { get; set; }
        public int NumTransferred { get; set; }
    }
}
