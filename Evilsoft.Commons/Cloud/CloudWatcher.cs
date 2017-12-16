using log4net;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace EvilsoftCommons.Cloud {
    public class CloudWatcher {
        static ILog logger = LogManager.GetLogger(typeof(CloudWatcher));
        public HashSet<CloudProvider> Providers {
            get;
            protected set;
        }

        public CloudWatcher() {
            Providers = new HashSet<CloudProvider>();
            FindDropbox();
            FindOneDrive();
            FindGoogleDrive();
        }



        private void FindDropbox() {
            try {
                String roaming = System.Environment.GetEnvironmentVariable("AppData");
                String local = System.Environment.GetEnvironmentVariable("LocalAppData");

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
                string path = (string)y["path"];


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
                    string location = (string)registryKey.GetValue("UserFolder");
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
                    Guid FOLDERID_SkyDrive = new Guid("A52BBA46-E9E1-435f-B3D9-28DAA648C0F6");
                    string location = GetKnownFolderPath(FOLDERID_SkyDrive);
                    if (Directory.Exists(location)) {
                        Providers.Add(new CloudProvider {
                            Location = location,
                            Provider = CloudProviderEnum.ONEDRIVE
                        });
                    }
                }
                catch (Exception ex) {
                    logger.Warn(ex.Message);
                    logger.Warn(ex.StackTrace);
                }
            }

        }

        private void FindGoogleDrive() {
            string drivePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Google", "Drive");

            try {
                if (Directory.Exists(drivePath)) {
                    string[] files = Directory.GetFiles(drivePath, "sync_config.db", SearchOption.AllDirectories);
                    foreach (string dbFilePath in files) {
                        if (File.Exists(dbFilePath)) {
                            string csGdrive = @"Data Source=" + dbFilePath + ";Version=3;New=False;Compress=True;";
                            SQLiteConnection con = new SQLiteConnection(csGdrive);
                            con.Open();
                            try {
                                SQLiteCommand sqLitecmd = new SQLiteCommand(con);

                                //To retrieve the folder use the following command text
                                sqLitecmd.CommandText = "select * from data where entry_key='local_sync_root_path'";

                                SQLiteDataReader reader = sqLitecmd.ExecuteReader();
                                reader.Read();

                                //String retrieved is in the format "\\?\<path>" that's why I have used Substring function to extract the path alone.
                                var path = reader["data_value"].ToString().Substring(4);

                                if (Directory.Exists(path)) {
                                    Providers.Add(new CloudProvider {
                                        Location = path,
                                        Provider = CloudProviderEnum.GOOGLE_DRIVE
                                    });
                                }
                            }
                            finally {
                                con.Dispose();
                            }
                        }
                    }
                }
            }
            catch (Exception ex) {
                logger.Warn(ex.Message);
                logger.Warn(ex.StackTrace);
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
