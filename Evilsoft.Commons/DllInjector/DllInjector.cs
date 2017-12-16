using log4net;
using EvilsoftCommons.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;

namespace EvilsoftCommons.DllInjector {
    public class DllInjector {        
        static ILog logger = LogManager.GetLogger(typeof(DllInjector));

/*
        [DllImport(@"ItemAssistantHook.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool InjectDLL(ulong ProcessID);*/


        private static DllInjector _instance;
        public static DllInjector Instance {
            get {
                if (_instance == null) {
                    _instance = new DllInjector();
                }
                return _instance;
            }
        }
        

        public static bool adjustDebugPriv(uint pid) {

            IntPtr hProcess = Win32.OpenProcess(Win32.AllAccess, 1, pid);

            if (IntPtr.Zero == hProcess) {
                return false;
            }

            Win32.TOKEN_PRIVILEGES tp = new Win32.TOKEN_PRIVILEGES();
            tp.PrivilegeCount = 1;
            tp.Attributes = Win32.SE_PRIVILEGE_ENABLED;

            if (!Win32.LookupPrivilegeValue(null, "SeDebugPrivilege", out tp.Luid)) {
                Win32.CloseHandle(hProcess);
                return false;
            }

            IntPtr hToken;
            if (!Win32.OpenProcessToken(hProcess, Win32.TOKEN_ADJUST_PRIVILEGES, out hToken)) {
                Win32.CloseHandle(hProcess);
                return false;
            }

            if (!Win32.AdjustTokenPrivileges(hToken, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero)) {
                Win32.CloseHandle(hProcess);
                Win32.CloseHandle(hToken);
                return false;
            }

            Win32.CloseHandle(hProcess);
            Win32.CloseHandle(hToken);
            return true;
        }


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


        public static string GetProcessPath(uint pid) {
            try {
                Process proc = Process.GetProcessById((int)pid);
                return proc.MainModule.FileName.ToString();
            }
            catch (Exception ex) {
                logger.Warn(ex.Message);
                logger.Warn(ex.StackTrace);
                ExceptionReporter.ReportException(ex, "GetProcessPath");
                return string.Empty;
            }
        }


        #region DLL Ejection/Unloading
        public static bool FreeLibraryEx(IntPtr hProcess, IntPtr hRemoteModule) {
            uint dwResult = 0;
            IntPtr hKernel32 = Win32.GetModuleHandle("kernel32.dll");
            if (hKernel32 != IntPtr.Zero) {
                IntPtr dwFreeLibrary = Win32.GetProcAddress(hKernel32, "FreeLibrary");
                if (dwFreeLibrary != IntPtr.Zero) {
                    IntPtr dwThreadId = IntPtr.Zero;
                    IntPtr hThread = Win32.CreateRemoteThread(hProcess, IntPtr.Zero, IntPtr.Zero, dwFreeLibrary, (IntPtr)hRemoteModule, 0, dwThreadId);


                    if (hThread != IntPtr.Zero) {
                        uint dwWaitResult = Win32.WaitForSingleObject(hThread, 0xFFFFFFFF);
                        if (dwWaitResult == 0)
                            Win32.GetExitCodeThread(hThread, out dwResult);

                        Win32.CloseHandle(hThread);
                    }
                }
            }

            return dwResult != 0;
        }
        #endregion

        
        public static IntPtr Allocate(IntPtr hProcess, uint dwSize) {
            return Win32.VirtualAllocEx(hProcess, IntPtr.Zero, (IntPtr)dwSize, Win32.MEM_COMMIT | Win32.MEM_RESERVE, Win32.PAGE_EXECUTE_READWRITE);
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
        #endregion


        public static bool Free(IntPtr hProcess, IntPtr dwAddress) {
            return Win32.VirtualFreeEx(hProcess, dwAddress, 0, Win32.FreeType.Release);
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

        public static bool UnloadDll(uint nProcessId, IntPtr remoteModule) {
            IntPtr hndProc = Win32.OpenProcess(Win32.PROCESS_CREATE_THREAD | Win32.PROCESS_VM_OPERATION | Win32.PROCESS_VM_READ | Win32.PROCESS_VM_WRITE | Win32.PROCESS_QUERY_INFORMATION, 0, nProcessId);
            try {
                return FreeLibraryEx(hndProc, remoteModule);
            }
            finally {
                Win32.CloseHandle(hndProc);
            }
        }


        public bool Inject(uint pToBeInjected, string sDllPath) {
            long flags = 0xFFF | 0x000F0000L | 0x00100000L;
            //long oldFlags = (0x2 | 0x8 | 0x10 | 0x20 | 0x400);
            IntPtr hndProc = Win32.OpenProcess((uint)flags, 0, pToBeInjected);
            if (hndProc == IntPtr.Zero) {
                return false;
            }

            try {
                IntPtr stringAddress = Win32.VirtualAllocEx(hndProc, (IntPtr)null, (IntPtr)sDllPath.Length, (Win32.MEM_COMMIT | Win32.MEM_RESERVE), Win32.PAGE_READWRITE);

                if (stringAddress == IntPtr.Zero) {
                    return false;
                }

                byte[] bytes = Encoding.ASCII.GetBytes(sDllPath);

                int numBytesWritten;
                if (Win32.WriteProcessMemory(hndProc, stringAddress, bytes, (uint)bytes.Length, out numBytesWritten) == 0) {
                    return false;
                }

                IntPtr lpLLAddress = Win32.GetProcAddress(Win32.GetModuleHandle("kernel32.dll"), "LoadLibraryA");

                if (lpLLAddress == IntPtr.Zero) {
                    return false;
                }
                if (Win32.CreateRemoteThread(hndProc, (IntPtr)null, IntPtr.Zero, lpLLAddress, stringAddress, 0, (IntPtr)null) == IntPtr.Zero) {
                    return false;
                }
            } finally {
                Win32.CloseHandle(hndProc);
            }

            return true;
        }
    }
}
