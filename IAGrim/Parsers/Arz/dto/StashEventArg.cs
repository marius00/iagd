using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Parsers.Arz.dto {
    class StashEventArg : EventArgs {
        public StashEventArg(string filename) {
            this.Filename = filename;
        }

        /// <summary>
        /// Filename of the stash file
        /// </summary>
        public string Filename { get; }
    }
}
