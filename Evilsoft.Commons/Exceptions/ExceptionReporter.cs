#if !DEBUG
#define RELEASE
#endif

// Just leave it on for now
//#define RELEASE


using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;

namespace EvilsoftCommons.Exceptions {

    public class ExceptionReporter {
        static readonly ILog Logger = LogManager.GetLogger(typeof(ExceptionReporter));

        public static string Uuid { protected get; set; }

        //public static string URL_HOST { get; set; }
        public static string UrlCrashreport {
            get;
            set;
        }
        public static string UrlStats {
            get;
            set;
        }

        public static bool LogExceptions { get; set; }

        private const int MaxReportsPerCooldown = 3;
        private const int ReportCooldownMs = 60000;


        public static long Timestamp => ((DateTime.UtcNow.Ticks - DateTime.Parse("01/01/1970 00:00:00").Ticks) / 10000000);

        private static TimeSpan TimeApplicationRunning => DateTime.Now - System.Diagnostics.Process.GetCurrentProcess().StartTime;

        private static List<long> ReportTicks = new List<long>();
        private static bool CanReport {
            get {
                return ReportTicks.Count(m => m + ReportCooldownMs > Timestamp) < MaxReportsPerCooldown;
            }
        }

        private static void RemoveOneTick() {
            if (ReportTicks.Count > 0)
                ReportTicks.RemoveAt(ReportTicks.Count - 1);

        }
        private static void AddReportTick() {
            ReportTicks.Add(Timestamp);
            if (ReportTicks.Count > 1000)
                ReportTicks.RemoveRange(0, ReportTicks.Count - MaxReportsPerCooldown * 10);
        }


        /// <summary>
        /// Clean the username from exception messages
        /// </summary>
        /// <param name="message"></param>
        private static string CleanUsername(string message) {
            return message?.Replace(Environment.UserName, ":filtered");
        }


        public static void EnableLogUnhandledOnThread() {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyHandler);
        }

        private static void MyHandler(object sender, UnhandledExceptionEventArgs args) {
            Exception e = (Exception)args.ExceptionObject;
            Logger.Fatal(e.Message);
            Logger.Fatal(e.StackTrace);
            ReportException(e);
        }

        private static int _maxForcedReports = 2;


        //[System.Diagnostics.Conditional("RELEASE")]
        public static void ReportUsage() {

            try {
                string postData = string.Format("version={0}&winver=0&uuid={1}",
                    Uri.EscapeDataString(VersionString),
                    Uuid);



                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(UrlStats);

                Encoding encoding = new UTF8Encoding();

                byte[] data = encoding.GetBytes(postData);

                httpWReq.Method = "POST";
                httpWReq.ContentType = "application/x-www-form-urlencoded";
                httpWReq.ContentLength = data.Length;

                using (Stream stream = httpWReq.GetRequestStream()) {
                    stream.Write(data, 0, data.Length);
                }
                // threshold
                using (HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse()) {
                    if (response.StatusCode != HttpStatusCode.OK) {
                        Logger.Info("Failed to send anonymous usage statistics to developer.");
                        return;
                    }

                    string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                    Logger.Info("Sent anonymous usage statistics to developer.");

                    AddReportTick();
                }
            }
            catch (Exception ex) {
                Logger.Fatal(ex.Message);
                Logger.Fatal(ex.StackTrace);
            }

        }

        //[System.Diagnostics.Conditional("RELEASE")]
        public static void ReportException(Exception e, string extra = "", bool forced = false) {
            // Try to get English error messages..
            var culture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            try {
                string innerMessage = string.Empty;
                string innerStacktrace = string.Empty;

                if (e.InnerException != null) {
                    innerMessage = CleanUsername(e.InnerException.Message);
                    innerStacktrace = CleanUsername(e.InnerException.StackTrace);
                }


                string postData = string.Join(
                    "&",
                    new [] {
                        "message=" + Uri.EscapeDataString(string.Format("{0} {1} {2}", e.GetType(), CleanUsername(e.Message), extra)),
                        "stacktrace=" + Uri.EscapeDataString(CleanUsername(e.StackTrace)),
                        "version=" + Uri.EscapeDataString(VersionString),
                        "winver=" + string.Format("{0}.{1}", Environment.OSVersion.Version.Major, Environment.OSVersion.Version.Minor),
                        "uuid=" + Uuid,
                        "running=" + TimeApplicationRunning,
                        "innerMessage=" + innerMessage,
                        "innerStacktrace=" + innerStacktrace,
                        "thread=" + Thread.CurrentThread.Name
                    }
                    );
                /*
                string postData = String.Format("message={0}&stacktrace={1}&version={2}&winver={3}&uuid={4}&running={5}&innerMessage={6}&innerStacktrace={7}",
                    Uri.EscapeDataString(string.Format("{0} {1} {2}", e.GetType(), CleanUsername(e.Message), extra)),
                    Uri.EscapeDataString(CleanUsername(e.StackTrace)),
                    Uri.EscapeDataString(VersionString),
                    string.Format("{0}.{1}", Environment.OSVersion.Version.Major, Environment.OSVersion.Version.Minor),
                    UUID.Value, TimeApplicationRunning, innerMessage, innerStacktrace);*/

                if (forced && _maxForcedReports-- > 0)
                    RemoveOneTick();

                if (LogExceptions) {
                    Logger.Info(string.Format("Outer exception: ", e.Message));
                    Logger.Info(e.StackTrace);

                    if (e.InnerException != null) {
                        Logger.Info(string.Format("Inner exception: ", innerMessage));
                        Logger.Info(innerStacktrace);
                    }
                }

                ReportCrash(postData);
            }
            finally {
                Thread.CurrentThread.CurrentCulture = culture;
            }
        }

        //[System.Diagnostics.Conditional("RELEASE")]
        public static void ReportIssue(string issue) {
            string postData = String.Format("message={0}&stacktrace={1}&version={2}&winver={3}&uuid={4}",
                Uri.EscapeDataString(issue),
                Uri.EscapeDataString("N/A"),
                Uri.EscapeDataString(VersionString),
                string.Format("{0}.{1}", Environment.OSVersion.Version.Major, Environment.OSVersion.Version.Minor),
                Uuid);

            ReportCrash(postData);
        }


        private static string VersionString {
            get {

                try {
                    var version = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
                    return //buildDate.ToString("dd-MM-yyyy hh:mm");
                        string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
                }
                catch (Exception ex) {
                    Logger.Warn("Error getting assembly version, automatic updates may not function correctly.");
                    Logger.Warn(ex.Message);
                    Logger.Warn(ex.StackTrace);

                    var version = Assembly.GetExecutingAssembly().GetName().Version;
                    return //buildDate.ToString("dd-MM-yyyy hh:mm");
                        string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);

                }
            }
        }


        private static void ReportCrash(string postData) {
            if (!CanReport) {
                Logger.Info("Crash report not sent, skill on cooldown..");
                return;
            }

            HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(UrlCrashreport);

            Encoding encoding = new UTF8Encoding();

            byte[] data = encoding.GetBytes(postData);

            httpWReq.Method = "POST";
            httpWReq.ContentType = "application/x-www-form-urlencoded";
            httpWReq.ContentLength = data.Length;

            try {
                using (Stream stream = httpWReq.GetRequestStream()) {
                    stream.Write(data, 0, data.Length);
                }
                // threshold
                using (HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse()) {
                    if (response.StatusCode != HttpStatusCode.OK) {
                        Logger.Info("Failed to upload crash report to developer.");
                        return;
                    }

                    string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();


                    Logger.Info("Uploaded crash report to developer.");
                    AddReportTick();
                }
            }
            catch (Exception ex) {
                Logger.Fatal(ex.Message);
                Logger.Fatal(ex.StackTrace);
            }
        }
        //#endif
    }
}
