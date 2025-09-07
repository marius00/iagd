using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace EvilsoftCommons.Cloud {
    public interface ICloudBackup {
        void Update();
    }
}
