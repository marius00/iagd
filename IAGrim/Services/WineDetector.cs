using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace IAGrim.Services {
    class WineDetector {
        [DllImport("ntdll.dll", EntryPoint = "wine_get_version")]
        private static extern IntPtr wine_get_version();

        public static bool IsRunningInWine() {
            try {
                // Attempt to get the function pointer
                IntPtr functionPointer = wine_get_version();
                // If it succeeds, the function exists, so it's likely Wine
                return functionPointer != IntPtr.Zero;
            }
            catch (DllNotFoundException) {
                // ntdll.dll not found (unlikely on Windows/Wine)
                return false;
            }
            catch (EntryPointNotFoundException) {
                // The function wine_get_version was not found (likely on native Windows)
                return false;
            }
            catch (Exception) {
                // Handle other potential exceptions
                return false;
            }
        }
    }
}