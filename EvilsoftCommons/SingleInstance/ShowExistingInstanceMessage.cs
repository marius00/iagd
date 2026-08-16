using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace EvilsoftCommons.SingleInstance {

    /// <summary>
    /// A window message asking an already running instance to show itself.
    ///
    /// This is what a second launch does instead of exiting silently: nothing ever listened on the named pipe
    /// <see cref="SingleInstance.PassArgumentsToFirstInstance"/> writes to, so starting IA while it sat minimized
    /// in the tray looked like nothing happened at all.
    /// </summary>
    public static class ShowExistingInstanceMessage {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ShowExistingInstanceMessage));

        private const uint MessageFilterAllow = 1;
        private const uint AllowAnyProcess = 0xffffffff; // ASFW_ANY

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern uint RegisterWindowMessage(string lpString);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ChangeWindowMessageFilterEx(IntPtr hWnd, uint msg, uint action, IntPtr changeInfo);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllowSetForegroundWindow(uint dwProcessId);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        /// <summary>
        /// Every process registering the same string gets the same message id back, which is how the two
        /// instances agree on one without sharing any state. 0 means the registration failed.
        /// </summary>
        public static readonly uint Id = RegisterWindowMessage("IAGD_ShowExistingInstance_F3693953");

        /// <summary>
        /// Asks the running instance to bring its window up.
        ///
        /// The message goes to the windows of the other IA process rather than to HWND_BROADCAST: a broadcast
        /// is rejected with ERROR_ACCESS_DENIED and never arrives, which was measured, not assumed. Every
        /// top-level window of that process is posted to, since the one belonging to the main window is not
        /// identifiable from the outside; the rest ignore an unknown registered message.
        /// </summary>
        public static void Notify() {
            if (Id == 0) {
                Logger.Warn("Could not register the show-window message, the running instance will not be notified");
                return;
            }

            // The running instance can only take the foreground if a process that currently has it says so.
            AllowSetForegroundWindow(AllowAnyProcess);

            var posted = 0;
            foreach (var hwnd in WindowsOfTheOtherInstance()) {
                if (PostMessage(hwnd, Id, IntPtr.Zero, IntPtr.Zero)) {
                    posted++;
                }
            }

            if (posted == 0) {
                Logger.Warn("Found no window to notify, the running instance stays hidden");
            }
        }

        /// <summary>
        /// Lets the message through UIPI, so a regular second instance can still reach an IA running as admin.
        /// </summary>
        public static void AllowReceiving(IntPtr hwnd) {
            if (Id == 0 || hwnd == IntPtr.Zero) {
                return;
            }

            if (!ChangeWindowMessageFilterEx(hwnd, Id, MessageFilterAllow, IntPtr.Zero)) {
                Logger.Warn("Failed to remove the UIPI filter for the show-window message");
            }
        }

        private static List<IntPtr> WindowsOfTheOtherInstance() {
            var windows = new List<IntPtr>();

            try {
                var self = Process.GetCurrentProcess();
                var pids = new HashSet<uint>();

                foreach (var process in Process.GetProcessesByName(self.ProcessName)) {
                    using (process) {
                        // A different user session is a different desktop, its windows are not ours to touch.
                        if (process.Id != self.Id && process.SessionId == self.SessionId) {
                            pids.Add((uint) process.Id);
                        }
                    }
                }

                if (pids.Count == 0) {
                    return windows;
                }

                // Includes hidden windows, which is the entire point: IA may be minimized to the tray.
                EnumWindows((hwnd, _) => {
                    GetWindowThreadProcessId(hwnd, out var owner);
                    if (pids.Contains(owner)) {
                        windows.Add(hwnd);
                    }

                    return true;
                }, IntPtr.Zero);
            }
            catch (Exception ex) {
                Logger.Warn("Could not locate the window of the running instance", ex);
            }

            return windows;
        }
    }
}
