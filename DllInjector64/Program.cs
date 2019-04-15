using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DllInjector64 {
    class Program {
        static int Main(string[] args) {
            if (IntPtr.Size != 8) {
                Console.WriteLine("This application must be compiled as 64 bit.");
                return 1;
            }
            if (args.Length == 0) {
                Console.WriteLine("Expected command line parameters: PID DllName");
                Console.WriteLine($"PID: {DllInjectorCopy.FindProcessForWindow("Grim Dawn").FirstOrDefault()}");
                return 1;
            }

            uint pid;
            if (!uint.TryParse(args[0], out pid)) {
                Console.WriteLine($"The PID argument must be a positive numeric value");
                return 1;
            }

            if (!File.Exists(args[1])) {
                Console.WriteLine($"Cannot find the file \"{args[1]}\"");
                return 1;
            }

            try {
                var p = Process.GetProcessById((int) pid);
                if (!DllInjectorCopy.Is64BitProcess(p)) {
                    Console.WriteLine("The target process is not 64bit");
                    return 1;
                }
            }
            catch (ArgumentException ex) {
                Console.WriteLine(ex.Message);
                return 1;
            }

            try {
                var result = DllInjectorCopy.NewInject(pid, args[1]);
                if (result == IntPtr.Zero) {
                    Console.WriteLine("Unknown error injecting into target process");
                    return 1;
                }
                Console.WriteLine(result);
                return 0;
            }
            catch (ArgumentException ex) {
                Console.WriteLine(ex.Message);
                return 1;
            }
        }
    }
}
