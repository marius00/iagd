using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using log4net;

namespace EvilsoftCommons.SQL {
    /// <summary>
    /// Will initialize, launch and stop a portable PostgreSQL installation.
    /// Will attempt to terminate PostgreSQL upon destruction.
    /// </summary>
    public class PostgresIntegrator : IDisposable {
        #region Attributes
        private static ILog logger = LogManager.GetLogger(typeof(PostgresIntegrator));
        private uint Port;
        private string _workingDirectory;
        private string PostgresLocation;


        /// <summary>
        /// The "working directory" for PSQLD (not your application)
        /// Usually under appdata
        /// </summary>
        private string WorkingDirectory {
            get {
                if (!Directory.Exists(_workingDirectory))
                    Directory.CreateDirectory(_workingDirectory);
                if (!Directory.Exists(_workingDirectory))
                    Directory.CreateDirectory(_workingDirectory);
                if (!Directory.Exists(_workingDirectory))
                    Directory.CreateDirectory(_workingDirectory);
                return _workingDirectory;
            }
            set {
                _workingDirectory = value;
            }
        }

        private string DataDirectory {
            get {
                string dir = Path.Combine(WorkingDirectory, "pgdata");
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                return dir;
            }
        }

        private string LogDirectory {
            get {
                string dir = Path.Combine(WorkingDirectory, "logs");
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                return dir;
            }
        }

        private ProcessStartInfo BasicStartInfo {
            get {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.EnvironmentVariables["PGDATA"] = DataDirectory;
                startInfo.EnvironmentVariables["PGLOG"] = LogDirectory;
                startInfo.RedirectStandardOutput = false;
                startInfo.RedirectStandardError = false;
                startInfo.RedirectStandardInput = false;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
                return startInfo;
            }
        }
        #endregion

        private void InitializeDatabase() {
            foreach (var file in Directory.EnumerateFiles(DataDirectory))
                return;


            logger.Info("Initializing PostgreSQL database...");
            ProcessStartInfo startInfo = BasicStartInfo;
            startInfo.FileName = Path.Combine(PostgresLocation, "bin", "initdb.exe");
            startInfo.Arguments = String.Format("-A trust -E utf8 --locale=C");

            if (Run(startInfo))
                logger.Info("Initialized PostgreSQL database");
            else
                logger.Warn("Failed to initialize PostgreSQL database");
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="workingDirectory">Working directory for postgres (any folder is valid, preferably appdata)</param>
        /// <param name="port">Post to listen on</param>
        /// <param name="postgresLocation">Absolute or relative path to Postgres</param>
        public PostgresIntegrator(string workingDirectory, uint port, string postgresLocation = "psqld") {
            this.Port = port;
            this.WorkingDirectory = workingDirectory;
            this.PostgresLocation = postgresLocation;
            InitializeDatabase();
            InitPsqld(port);
        }


        ~PostgresIntegrator() {
            this.Dispose();
        }

        private void InitPsqld(uint port) {
            logger.Info("Starting PostgreSQL Daemon...");
            ProcessStartInfo startInfo = BasicStartInfo;
            startInfo.FileName = Path.Combine(PostgresLocation, "bin", "postgres.exe");
            startInfo.Arguments = String.Format("-p {0}", port);
            if (Run(startInfo)) {
                logger.Info("Started PostgreSQL Daemon...");
            }
            else {
                logger.Warn("Could not start PostgreSQL Daemon...");
            }
        }

        private bool Run(ProcessStartInfo info, bool waitForExit = false) {
            Process p = new Process();
            p.StartInfo = info;
            p.EnableRaisingEvents = true;
            try {
                bool r = p.Start();
                if (waitForExit)
                    p.WaitForExit();

                p.Close();
                return r;
            }
            catch (Exception ex) {
                logger.WarnFormat("Failed launching executeable: {0} {1}", info.FileName, info.Arguments);
                logger.Warn(ex.Message);
                logger.Warn(ex.StackTrace);
                //ExceptionReporter.ReportException(ex);
            }
            return false;

        }


        private void StopPsqld() {
            logger.Info("Stopping PostgreSQL Daemon...");
            ProcessStartInfo startInfo = BasicStartInfo;
            startInfo.EnvironmentVariables["PGDATA"] = string.Empty;
            startInfo.FileName = Path.Combine(PostgresLocation, "bin", "pg_ctl.exe");
            startInfo.Arguments = String.Format("-D \"{0}\" stop", DataDirectory);
            if (Run(startInfo))
                logger.Info("Stopped PostgreSQL Daemon...");
            else
                logger.Warn("Failed to stop PostgreSQL Daemon...");

        }

        /// <summary>
        /// Wrapper class for temporary backup files, with deconstructor for automatic deletion
        /// </summary>
        public class BackupFile {
            public BackupFile(string filename) { this.Filename = filename; }
            public string Filename { get; protected set; }

            ~BackupFile() {
                if (!string.IsNullOrEmpty(Filename) && File.Exists(Filename)) {
                    try {
                        File.Delete(Filename);
                    }
                    catch (Exception ex) {
                        logger.WarnFormat("Error deleting temporary file \"{0}\"", Filename);
                        logger.Warn(ex.Message);
                        logger.Warn(ex.StackTrace);
                    }
                }
                GC.SuppressFinalize(this);
            }
        }



        public void RestorePostgresBackup(string filename) {
            throw new NotSupportedException("Backup restores is currently not supported.");
        }

        public BackupFile ExecutePostgresBackup() {

            string filename = Path.GetTempFileName();
            try {
                ExecutePostgresBackup(filename);
            }
            catch (Exception ex) {
                logger.Warn(ex.Message);
                logger.Warn(ex.StackTrace);
                try {
                    File.Delete(filename);
                }
                catch (Exception ex2) {
                    logger.WarnFormat("Could not delete temporary file \"{0}\"", filename);
                    logger.Warn(ex2.Message);
                }

                return null;
            }

            return new BackupFile(filename);

        }


        public bool ExecutePostgresBackup(string filename) {            
            ProcessStartInfo startInfo = BasicStartInfo;
            startInfo.EnvironmentVariables["PGDATA"] = string.Empty;
            startInfo.FileName = Path.Combine(PostgresLocation, "bin", "pg_dump.exe");
            startInfo.Arguments = string.Format("--host localhost --port {1} --format tar --blobs --verbose --file \"{0}\" app", filename, Port);
            return Run(startInfo);
        }

        public void Dispose() {
            StopPsqld();
            GC.SuppressFinalize(this);
        }
    }
    
}
