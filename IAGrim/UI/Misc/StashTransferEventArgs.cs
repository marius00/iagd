using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.UI.Misc {

    internal class StashTransferEventArgs : EventArgs {

        public long? Id {
            get {
                long tmp;
                if (long.TryParse(InternalId[0].ToString(), out tmp))
                    return tmp;
                else
                    return null;
            }
        }

        public string Prefix => InternalId[2] as string;

        public string BaseRecord => InternalId[1] as string;
        public string Suffix => InternalId[3] as string;
        public string Materia => InternalId[4] as string;
        //public string Modifier => InternalId[5] as string;


        public bool HasValidId => InternalId != null && InternalId.Length == 5 && BaseRecord != null;

        public object[] InternalId { private get; set; }

        public int Count { get; set; }
        //public bool Scramble { get; set; }

        public override string ToString() {
            if (Id.HasValue)
                return $"StashTransferEventArgs[Id:{Id.Value}]";

            return $"StashTransferEventArgs[P:{Prefix},B:{BaseRecord},S:{Suffix},M:{Materia}]";
        }
    }
}
