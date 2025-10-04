using log4net;
using EvilsoftCommons.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;

namespace EvilsoftCommons.DllInjector {
    public class DllInjector {

        public static HashSet<uint> FindProcessForWindow(String windowname) {
            // Find the windows
            HashSet<uint> clients = new HashSet<uint>();
            uint pid;
            IntPtr prevWindow = IntPtr.Zero;
            do {
                prevWindow = Win32.FindWindowEx(IntPtr.Zero, prevWindow, windowname, null);
                if (prevWindow != null && prevWindow != IntPtr.Zero) {
                    Win32.GetWindowThreadProcessId(prevWindow, out pid);
                    clients.Add(pid);
                }
            }

            while (prevWindow != IntPtr.Zero);


            return clients;
        }

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)][return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWow64Process([In] IntPtr hProcess, [Out] out bool lpSystemInfo);

        public static bool Is64BitProcess(Process process) {
            if (!Environment.Is64BitOperatingSystem)
                return false;

            bool isWow64Process;
            if (!IsWow64Process(process.Handle, out isWow64Process))
                throw new Win32Exception(Marshal.GetLastWin32Error());

            return !isWow64Process;
        }

    }
}
