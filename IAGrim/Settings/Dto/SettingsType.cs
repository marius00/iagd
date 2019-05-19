using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Settings.Dto {
    /// <summary>
    /// Local settings are specific to the current install, on the current computer.
    /// An example of a transient settings is .Net installation detection, which is very computer specific.
    /// Program position is also Local, as a new computer may run with a different resolution or monitor count.
    /// 
    /// Persistent settings could be database migrations applied, personal preferences for IA, etc. 
    /// </summary>
    public enum SettingsType {
        Local, Persistent
    }
}
