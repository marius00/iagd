using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EvilsoftCommons.Exceptions;
using IAGrim.Utilities;
using log4net;

namespace IAGrim {
    public class StartupService {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(StartupService));
        public void Init() {

            var version = Assembly.GetExecutingAssembly().GetName().Version;
            DateTime buildDate = new DateTime(2000, 1, 1)
                .AddDays(version.Build)
                .AddSeconds(version.Revision * 2);

            Logger.InfoFormat("Running version {0}.{1}.{2}.{3} from {4}", version.Major, version.Minor, version.Build, version.Revision, buildDate.ToString("dd/MM/yyyy"));

            if (!DependencyChecker.CheckNet452Installed()) {
                MessageBox.Show("It appears .Net Framework 4.5.2 is not installed.\nIA May not function correctly", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (!DependencyChecker.CheckVs2013Installed()) {
                MessageBox.Show("It appears VS 2013 (x86) redistributable is not installed.\nPlease install it to continue using IA", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            if (!DependencyChecker.CheckVs2010Installed()) {
                MessageBox.Show("It appears VS 2010 (x86) redistributable is not installed.\nPlease install it to continue using IA", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Upgrade any settings if required
        /// This happens for just about every compile
        /// </summary>
        public static bool UpgradeSettings() {
            try {
                if (Properties.Settings.Default.CallUpgrade) {
                    Properties.Settings.Default.Upgrade();
                    Properties.Settings.Default.CallUpgrade = false;
                    Logger.Info("Settings upgraded..");

                    return true;
                }
            }
            catch (Exception ex) {
                Logger.Warn(ex.Message);
                Logger.Warn(ex.StackTrace);
                ExceptionReporter.ReportException(ex);
            }

            return false;
        }
    }
}
