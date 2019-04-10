using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DllInjector64 {
    class DllInjectorCopy {

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


        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWow64Process([In] IntPtr hProcess, [Out] out bool lpSystemInfo);

        public static bool Is64BitProcess(Process process) {
            if (!Environment.Is64BitOperatingSystem)
                return false;

            bool isWow64Process;
            if (!IsWow64Process(process.Handle, out isWow64Process))
                throw new Win32Exception(Marshal.GetLastWin32Error());

            return !isWow64Process;
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

        #endregion

    }

    public class Win32 {


        public static readonly uint MEM_COMMIT = 0x1000;
        public static readonly uint MEM_RESERVE = 0x2000;
        public static readonly uint PAGE_READWRITE = 0x04;
        public static readonly uint PAGE_EXECUTE_READWRITE = 0x40;
        public static readonly uint PROCESS_CREATE_THREAD = 0x0002;
        public static readonly uint PROCESS_VM_OPERATION = 0x0008;
        public static readonly uint PROCESS_VM_READ = 0x0010;
        public static readonly uint PROCESS_VM_WRITE = 0x0020;
        public static readonly uint PROCESS_QUERY_INFORMATION = 0x0400;


        public const UInt32 SE_PRIVILEGE_ENABLED_BY_DEFAULT = 0x00000001;
        public const UInt32 SE_PRIVILEGE_ENABLED = 0x00000002;
        public const UInt32 SE_PRIVILEGE_REMOVED = 0x00000004;
        public const UInt32 SE_PRIVILEGE_USED_FOR_ACCESS = 0x80000000;
        public const UInt32 TOKEN_ADJUST_PRIVILEGES = 0x0020;
        private const Int32 ANYSIZE_ARRAY = 1;

        private const uint Synchronize = 0x100000;
        private const uint StandardRightsRequired = 0x000F0000;
        public const uint AllAccess = StandardRightsRequired | Synchronize | 0xFFFF;

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(uint dwDesiredAccess, int bInheritHandle, uint dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, FreeType dwFreeType);

        [Flags]
        public enum FreeType {
            Decommit = 0x4000,
            Release = 0x8000,
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] buffer, uint size, out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttribute, IntPtr dwStackSize, IntPtr lpStartAddress,
            IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);


        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);

        [DllImport("kernel32.dll")]
        public static extern bool GetExitCodeThread(IntPtr hThread, out uint lpExitCode);


        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AdjustTokenPrivileges(IntPtr TokenHandle, [MarshalAs(UnmanagedType.Bool)]bool DisableAllPrivileges, ref TOKEN_PRIVILEGES NewState, UInt32 Zero, IntPtr Null1, IntPtr Null2);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool OpenProcessToken(IntPtr ProcessHandle, UInt32 DesiredAccess, out IntPtr TokenHandle);


        [StructLayout(LayoutKind.Sequential)]
        public struct LUID {
            public uint LowPart;
            public int HighPart;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct LUID_AND_ATTRIBUTES {
            public LUID Luid;
            public UInt32 Attributes;
        }

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool LookupPrivilegeValue(string lpSystemName, string lpName, out LUID lpLuid);


        [StructLayout(LayoutKind.Sequential)]
        public struct TOKEN_PRIVILEGES {
            public UInt32 PrivilegeCount;
            public LUID Luid;
            public UInt32 Attributes;
        }
    }
}
