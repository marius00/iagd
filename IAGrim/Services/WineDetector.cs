using log4net;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace IAGrim.Services {
    class WineDetector {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(WineDetector));
        [DllImport("ntdll.dll", EntryPoint = "wine_get_version")]
        private static extern IntPtr wine_get_version();

        public static bool IsRunningInWine() {
            try {
                // Attempt to get the function pointer
                IntPtr functionPointer = wine_get_version();
                // If it succeeds, the function exists, so it's likely Wine
                return functionPointer != IntPtr.Zero;
            }
            catch (DllNotFoundException ex) {
                // ntdll.dll not found (unlikely on Windows/Wine)
                Logger.Info("Could not find ntdll exe, environment unknown", ex);
                return false;
            }
            catch (EntryPointNotFoundException ex) {
                // The function wine_get_version was not found (likely on native Windows)
                Logger.Info("Could not find wine_get_version, environment likely windows", ex);
                return false;
            }
            catch (Exception ex) {
                // Handle other potential exceptions
                Logger.Warn("Unknown error detecting environment", ex);
                return false;
            }
        }
    }
}