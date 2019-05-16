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

        public static IntPtr NewInject(uint nProcessId, string sDllPath) {
            IntPtr hndProc = Win32.OpenProcess(Win32.PROCESS_CREATE_THREAD | Win32.PROCESS_VM_OPERATION | Win32.PROCESS_VM_READ | Win32.PROCESS_VM_WRITE | Win32.PROCESS_QUERY_INFORMATION, 0, nProcessId);

            if (Path.GetFileName(sDllPath).Equals(sDllPath))
                throw new ArgumentException("The DLL path must be an absolute path to DLL");
            try {
                IntPtr remoteModule = LoadLibraryEx(hndProc, sDllPath);
                return remoteModule;
            }
            finally {
                Win32.CloseHandle(hndProc);
            }
        }

        #region DLL Injection/Loading
        public static IntPtr LoadLibraryEx(IntPtr hProcess, string sModule) {
            uint dwResult = 0;
            IntPtr hKernel32 = Win32.GetModuleHandle("kernel32.dll");
            if (hKernel32 != IntPtr.Zero) {
                IntPtr dwLoadLibrary = Win32.GetProcAddress(hKernel32, "LoadLibraryW");
                if (dwLoadLibrary != IntPtr.Zero) {
                    byte[] bBuffer = Encoding.Unicode.GetBytes(sModule);
                    uint dwBufferLength = (uint)bBuffer.Length + 2;
                    IntPtr dwCodeCave = Allocate(hProcess, dwBufferLength);
                    if (dwCodeCave != IntPtr.Zero) {
                        if (Write(hProcess, dwCodeCave, sModule, CharSet.Unicode)) {
                            IntPtr dwThreadId = IntPtr.Zero;
                            IntPtr hThread = Win32.CreateRemoteThread(hProcess, IntPtr.Zero, IntPtr.Zero, dwLoadLibrary, dwCodeCave, 0, dwThreadId);

                            if (hThread != IntPtr.Zero) {
                                uint dwWaitResult = Win32.WaitForSingleObject(hThread, 0xFFFFFFFF);
                                if (dwWaitResult == 0)
                                    Win32.GetExitCodeThread(hThread, out dwResult);

                                Win32.CloseHandle(hThread);
                            }
                        }

                        Free(hProcess, dwCodeCave);
                    }
                }
            }

            return (IntPtr)dwResult;
        }
        public static IntPtr Allocate(IntPtr hProcess, uint dwSize) {
            return Win32.VirtualAllocEx(hProcess, IntPtr.Zero, (IntPtr)dwSize, Win32.MEM_COMMIT | Win32.MEM_RESERVE, Win32.PAGE_EXECUTE_READWRITE);
        }
        public static bool Write(IntPtr hProcess, IntPtr dwAddress, string sString, CharSet sCharacterSet) {
            byte[] bBuffer;
            if (sCharacterSet == CharSet.None || sCharacterSet == CharSet.Ansi)
                bBuffer = Encoding.ASCII.GetBytes(sString);
            else
                bBuffer = Encoding.Unicode.GetBytes(sString);

            bool bResult = false;
            int dwBytesWritten = 0;
            uint dwStringLength = (uint)bBuffer.Length;

            bResult = Win32.WriteProcessMemory(hProcess, dwAddress, bBuffer, dwStringLength, out dwBytesWritten) != 0;

            return bResult && dwBytesWritten == dwStringLength;
        }

        public static bool Free(IntPtr hProcess, IntPtr dwAddress) {
            return Win32.VirtualFreeEx(hProcess, dwAddress, 0, Win32.FreeType.Release);
        }
        #endregion

    }
}
