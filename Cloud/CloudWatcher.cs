using log4net;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace EvilsoftCommons.Cloud {
    public class CloudWatcher {
        static ILog logger = LogManager.GetLogger(typeof(CloudWatcher));
        public HashSet<CloudProvider> Providers { get; protected set; }

        public CloudWatcher() {
            Providers = new HashSet<CloudProvider>();
            FindDropbox();
            FindOneDrive();
        }


        private void FindDropbox() {
            try {
                string roaming = System.Environment.GetEnvironmentVariable("AppData");
                string local = System.Environment.GetEnvironmentVariable("LocalAppData");

                if (Directory.Exists(Path.Combine(roaming, "Dropbox")) && File.Exists(Path.Combine(roaming, "Dropbox", "info.json"))) {
                    FindDropbox(File.ReadAllText(Path.Combine(roaming, "Dropbox", "info.json")));
                }

                if (Directory.Exists(Path.Combine(local, "Dropbox")) && File.Exists(Path.Combine(local, "Dropbox", "info.json"))) {
                    FindDropbox(File.ReadAllText(Path.Combine(local, "Dropbox", "info.json")));
                }

                if (Directory.Exists(Path.Combine(roaming, "Dropbox")) && File.Exists(Path.Combine(roaming, "Dropbox", "host.db"))) {
                    var data = File.ReadAllLines(Path.Combine(roaming, "Dropbox", "host.db"));
                    if (data.Length >= 2) {
                        var path = ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(data[1]));
                        if (Directory.Exists(path)) {
                            Providers.Add(new CloudProvider {
                                Location = path,
                                Provider = CloudProviderEnum.DROPBOX
                            });
                        }
                    }
                }

                if (Directory.Exists(Path.Combine(local, "Dropbox")) && File.Exists(Path.Combine(local, "Dropbox", "host.db"))) {
                    var data = File.ReadAllLines(Path.Combine(local, "Dropbox", "host.db"));
                    if (data.Length >= 2) {
                        var path = ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(data[1]));
                        if (Directory.Exists(path)) {
                            Providers.Add(new CloudProvider {
                                Location = path,
                                Provider = CloudProviderEnum.DROPBOX
                            });
                        }
                    }
                }
            }
            catch (Exception ex) {
                logger.Warn(ex.Message);
                logger.Warn(ex.StackTrace);
            }
        }

        private void FindDropbox(string json) {
            var providers = JObject.Parse(json);
            foreach (var provider in providers) {
                var y = provider.Value;
                string path = (string) y["path"];


                if (Directory.Exists(path)) {
                    Providers.Add(new CloudProvider {
                        Location = path,
                        Provider = CloudProviderEnum.DROPBOX
                    });
                }
            }
        }

        private static string FindOnedriveByRegistry() {
            using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\OneDrive")) {
                if (registryKey != null) {
                    string location = (string) registryKey.GetValue("UserFolder");
                    if (!string.IsNullOrEmpty(location) && Directory.Exists(location)) {
                        logger.Info("OneDrive install location located using registry key");
                        return location;
                    }
                }
            }

            return string.Empty;
        }


        private void FindOneDrive() {
            var viaRegistry = FindOnedriveByRegistry();
            if (!string.IsNullOrEmpty(viaRegistry)) {
                Providers.Add(new CloudProvider {
                    Location = viaRegistry,
                    Provider = CloudProviderEnum.ONEDRIVE
                });
            }
            else {
                try {
                    Guid folderidOneDrive = new Guid("A52BBA46-E9E1-435f-B3D9-28DAA648C0F6");
                    string location = GetKnownFolderPath(folderidOneDrive);
                    if (Directory.Exists(location)) {
                        Providers.Add(new CloudProvider {
                            Location = location,
                            Provider = CloudProviderEnum.ONEDRIVE
                        });
                    }
                }
                catch (Exception ex) {
                    logger.Debug($"Error detecting OneDrive installation path (this is fine, don't worry): {ex.Message}");
                    logger.Debug(ex.StackTrace);
                }
            }
        }


        [DllImport("shell32.dll")]
        static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, IntPtr hToken, out IntPtr pszPath);

        public static string GetKnownFolderPath(Guid knownFolderId) {
            IntPtr pszPath = IntPtr.Zero;
            try {
                int hr = SHGetKnownFolderPath(knownFolderId, 0, IntPtr.Zero, out pszPath);
                if (hr >= 0)
                    return Marshal.PtrToStringAuto(pszPath);
                throw Marshal.GetExceptionForHR(hr);
            }
            finally {
                if (pszPath != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(pszPath);
            }
        }
    }
}