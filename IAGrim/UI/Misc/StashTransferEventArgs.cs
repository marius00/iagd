using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.UI.Misc {

    internal class StashTransferEventArgs : EventArgs {

        public string Prefix => InternalId[1] as string;
        public string BaseRecord => InternalId[0] as string;
        public string Suffix => InternalId[2] as string;
        public string Materia => InternalId[3] as string;

        public bool HasValidId => InternalId != null && InternalId.Length == 4 && BaseRecord != null;
        public object[] InternalId { private get; set; }
        public int Count { get; set; }

        public override string ToString() {
            return $"StashTransferEventArgs[P:{Prefix},B:{BaseRecord},S:{Suffix},M:{Materia}]";
        }

        public bool IsSuccessful { get; set; }
        public int NumTransferred { get; set; }
    }
}
