using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Utilities {
    public class TempFile : IDisposable {
        public readonly string filename;

        public TempFile() {
            this.filename = Path.GetTempFileName();
        }

        public void Dispose() {
            try {
                File.Delete(filename);
            }
            catch (IOException) {
                /* Oh well.. */
            }
        }
    }
}
