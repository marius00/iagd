using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvilsoftCommons;
using EvilsoftCommons.Exceptions;
using IAGrim.Parsers.Arz;
using IAGrim.Properties;
using IAGrim.Settings;
using IAGrim.StashFile;
using IAGrim.Utilities;
using log4net;

namespace IAGrim.Parsers.TransferStash {
    class SafeTransferStashWriter {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SafeTransferStashWriter));
        private readonly SettingsService _settings;

        public SafeTransferStashWriter(SettingsService settings) {
            _settings = settings;
        }

        /// <summary>
        /// Write the GD Stash file
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="stash"></param>
        /// <returns></returns>
        public bool SafelyWriteStash(string filename, Stash stash) {
            try {
                var tempName = $"{filename}-{DateTime.UtcNow.ToTimestamp()}.ia";

                // Store the stash file in a temporary location
                var dataBuffer = new DataBuffer();

                stash.Write(dataBuffer);
                DataBuffer.WriteBytesToDisk(tempName, dataBuffer.Data);

                // Get the current backup number
                
                var backupNumber = _settings.GetLocal().BackupNumber;

                _settings.GetLocal().BackupNumber = (backupNumber + 1) % 100;

                // Back up the existing stash and replace with new stash file
                var backupLocation = Path.Combine(GlobalPaths.BackupLocation, $"transfer.{backupNumber:00}.gs_");
                File.Copy(filename, backupLocation, true);
                File.Copy(tempName, filename, true);
                Logger.Info($"The previous stash file has been backed up as {backupLocation}");

                // Delete the temporary file
                if (File.Exists(tempName)) {
                    File.Delete(tempName);
                }

                return true;
            }
            catch (UnauthorizedAccessException ex) {
                Logger.Warn(ex.Message);
                Logger.Warn(ex.StackTrace);
                ExceptionReporter.ReportException(ex, "SafelyWriteDatabase");
                return false;
            }
        }

    }
}
