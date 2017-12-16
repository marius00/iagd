using IAGrim.Database;
using System;
using System.Collections.Generic;

namespace IAGrim.Backup.FileWriter {
    public interface FileExporter {
        void Write(IList<PlayerItem> items);
        List<PlayerItem> Read();
    }
}
