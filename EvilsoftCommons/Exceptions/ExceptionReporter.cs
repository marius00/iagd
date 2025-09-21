#if !DEBUG
#define RELEASE
#endif

// Just leave it on for now
//#define RELEASE


using log4net;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices.JavaScript;
using System.Text;

namespace EvilsoftCommons.Exceptions {

    public class ExceptionReporter {
        static readonly ILog Logger = LogManager.GetLogger(typeof(ExceptionReporter));

        public static string Uuid { protected get; set; }

        public static string UrlStats {
            get;
            set;
        }


        public static void EnableLogUnhandledOnThread() {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyHandler);
        }

        private static void MyHandler(object sender, UnhandledExceptionEventArgs args) {
            Exception e = (Exception)args.ExceptionObject;
            Logger.Fatal(e.Message);
            Logger.Fatal(e.StackTrace);
        }


        public static void ReportUsage() {
#if !DEBUG
            try {
                string postData = string.Format("version={0}&uuid={1}", Uri.EscapeDataString(VersionString), Uuid);
                HttpWebRequest httpWReq = (HttpWebRequest) WebRequest.Create(UrlStats);
                Encoding encoding = new UTF8Encoding();
                byte[] data = encoding.GetBytes(postData);

                httpWReq.Method = "POST";
                httpWReq.ContentType = "application/x-www-form-urlencoded";
                httpWReq.ContentLength = data.Length;

                using (Stream stream = httpWReq.GetRequestStream()) {
                    stream.Write(data, 0, data.Length);
                }

                // threshold
                using (HttpWebResponse response = (HttpWebResponse) httpWReq.GetResponse()) {
                    if (response.StatusCode != HttpStatusCode.OK) {
                        Logger.Warn("Failed to send anonymous usage statistics to developer.");
                        return;
                    }

                    string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                    Logger.Info("Sent anonymous usage statistics to developer.");
                }
            }
            catch (Exception ex) {
                Logger.Fatal(ex.Message);
                Logger.Fatal(ex.StackTrace);
            }
#endif
        }

        public static string VersionString {
            get {
                try {
                    var version = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
                    return $"{version.Major}.20{version.Minor}.{version.Build:D4}.{version.Revision:D4}";
                }
                catch (Exception ex) {
                    Logger.Warn("Error getting assembly version, automatic updates may not function correctly.");
                    Logger.Warn(ex.Message);
                    Logger.Warn(ex.StackTrace);

                    var version = Assembly.GetExecutingAssembly().GetName().Version;
                    return $"{version.Major}.20{version.Minor}.{version.Build:D4}.{version.Revision:D4}";
                }
            }
        }

        private static DateTime VersionToDateTime(Version version) {
            int year = 2000 + version.Minor;

            int month;
            int day;
            if (version.Build.ToString().Substring(0, 1) == "1") {
                month = int.Parse(version.Build.ToString().Substring(0, 2));

                day = int.Parse(version.Build.ToString().Substring(2));
            }
            else {

                month = int.Parse(version.Build.ToString().Substring(0, 1));
                day = int.Parse(version.Build.ToString().Substring(1));
            }

            int hour;
            int minute;
            if (version.Revision.ToString().Length == 3) {
                hour = int.Parse(version.Revision.ToString().Substring(0, 1));
                minute = int.Parse(version.Revision.ToString().Substring(1));
            }
            else {
                hour = int.Parse(version.Revision.ToString().Substring(0, 2));
                minute = int.Parse(version.Revision.ToString().Substring(2));
            }

            return new DateTime(year, month, day, hour, minute, 0);
        }

        public static DateTime BuildDate {
            get {
                try {
                    return VersionToDateTime(System.Reflection.Assembly.GetEntryAssembly().GetName().Version);
                }
                catch (Exception ex) {
                    Logger.Warn("Error getting assembly version, automatic updates may not function correctly.");
                    Logger.Warn(ex.Message);
                    Logger.Warn(ex.StackTrace);

                    return VersionToDateTime(Assembly.GetExecutingAssembly().GetName().Version);
                }
            }
        }


    }
}
