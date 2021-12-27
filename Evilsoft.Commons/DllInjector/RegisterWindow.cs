using System;
using System.Runtime.InteropServices;
using log4net;
using System.Security.Principal;
using System.Text;

namespace EvilsoftCommons.DllInjector {

    public class RegisterWindow : IDisposable {
        private delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
        private const uint WM_COPYDATA = 0x004A;
        private Action<DataAndType> CustomWndProc;
        private WndProc m_wnd_proc_delegate;
        static ILog logger = LogManager.GetLogger(typeof(RegisterWindow));

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool ChangeWindowMessageFilterEx(IntPtr hWnd, uint msg, ChangeWindowMessageFilterExAction action, /*ref CHANGEFILTERSTRUCT changeInfo*/ IntPtr passNull);


        public enum ChangeWindowMessageFilterExAction : uint {
            Reset = 0, Allow = 1, DisAllow = 2
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct CHANGEFILTERSTRUCT {
            public uint size;
            public MessageFilterInfo info;
        }
        public enum MessageFilterInfo : uint {
            None = 0, AlreadyAllowed = 1, AlreadyDisAllowed = 2, AllowedHigher = 3
        };


        [System.Runtime.InteropServices.StructLayout(
            System.Runtime.InteropServices.LayoutKind.Sequential,
            CharSet = System.Runtime.InteropServices.CharSet.Unicode
        )]
        struct WNDCLASS {
            public uint style;
            public IntPtr lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
            public string lpszMenuName;
            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
            public string lpszClassName;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        static extern System.UInt16 RegisterClassW(
            [System.Runtime.InteropServices.In] ref WNDCLASS lpWndClass
        );

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr CreateWindowExW(
            UInt32 dwExStyle,
            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string lpClassName,
            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string lpWindowName,
            UInt32 dwStyle,
            Int32 x,
            Int32 y,
            Int32 nWidth,
            Int32 nHeight,
            IntPtr hWndParent,
            IntPtr hMenu,
            IntPtr hInstance,
            IntPtr lpParam
        );

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        public static extern System.IntPtr DefWindowProcW(
            IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam
        );

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        static extern bool DestroyWindow(
            IntPtr hWnd
        );


        private const int ERROR_CLASS_ALREADY_EXISTS = 1410;

        private IntPtr m_hwnd;
        public IntPtr hwnd {
            get { return m_hwnd; }
        }

        public class DataAndType {
            public DataAndType(COPYDATASTRUCT cps) {
                Type = (int)cps.cbData;
                Data = new byte[(int)cps.dwData];
                StringData = Marshal.PtrToStringUni(cps.lpData);


                System.Runtime.InteropServices.Marshal.Copy(cps.lpData, Data, 0, (int)cps.dwData);
            }

            public byte[] Data { get; }
            public int Type { get; }
            public string StringData { get; }
        }


        private IntPtr ProxyWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam) {
            if (msg == RegisterWindow.WM_COPYDATA) {
                COPYDATASTRUCT cps = (COPYDATASTRUCT)Marshal.PtrToStructure(lParam, typeof(COPYDATASTRUCT));
                DataAndType bt = new DataAndType(cps);
                CustomWndProc?.Invoke(bt);
            }

            return RegisterWindow.DefWindowProcW(hWnd, msg, wParam, lParam);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) {
            CustomWndProc = null;
            if (!false) {
                if (disposing) {
                    // Dispose managed resources
                }

                // Dispose unmanaged resources
                if (m_hwnd != IntPtr.Zero) {
                    DestroyWindow(m_hwnd);
                    m_hwnd = IntPtr.Zero;
                }

            }
        }

        public RegisterWindow(string className, Action<DataAndType> callback) {

            if (className == null) throw new System.Exception("class_name is null");
            if (className == String.Empty) throw new System.Exception("class_name is empty");


            CustomWndProc = callback;

            m_wnd_proc_delegate = ProxyWndProc;

            // Create WNDCLASS
            WNDCLASS wind_class = new WNDCLASS();
            wind_class.lpszClassName = className;
            wind_class.lpfnWndProc = System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(m_wnd_proc_delegate);

            UInt16 class_atom = RegisterClassW(ref wind_class);

            int last_error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();

            if (class_atom == 0 && last_error != ERROR_CLASS_ALREADY_EXISTS) {
                throw new System.Exception("Could not register window class");
            }

            // Create window
            m_hwnd = CreateWindowExW(
                0,
                className,
                String.Empty,
                0,
                0,
                0,
                0,
                0,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero
            );
            logger.Info("Created window with hwnd " + m_hwnd);

            // This should allow IA to receive messages from GD, even if IA is running as admin.
            if (!ChangeWindowMessageFilterEx(m_hwnd, WM_COPYDATA, ChangeWindowMessageFilterExAction.Allow, IntPtr.Zero)) {
                var isAdmin = (new WindowsPrincipal(WindowsIdentity.GetCurrent())).IsInRole(WindowsBuiltInRole.Administrator);
                logger.Warn("Failed to remove UIPI filter restrictions.");
                if (isAdmin)
                    logger.Fatal("Running as administrator, IA will not be able to communicate with GD due to UIPI filter.");
                else
                    logger.Info("Not running as administrator, UIPI filter not required.");
            }
        }
    }
}
