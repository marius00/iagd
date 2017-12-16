using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Utilities {
    class TemporaryCopy : IDisposable {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(TemporaryCopy));

        private readonly bool _deleteFile;
        private string _tempfile;

        public TemporaryCopy(string file) {
            this._tempfile = Path.GetTempFileName();
            File.Copy(file, _tempfile, true);
            File.SetAttributes(_tempfile, FileAttributes.Normal);
            Logger.InfoFormat("Created a temporary file at {0} to prevent read-issues with running GD instance.", _tempfile);
            _deleteFile = true;
        }

        /// <summary>
        /// Temp-copy class
        /// If create-copy is false, the class is simply used as a wrapper around a regular file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="createCopy"></param>
        public TemporaryCopy(string file, bool createCopy) {
            if (createCopy) {
                this._tempfile = Path.GetTempFileName();
                File.Copy(file, _tempfile, true);
                File.SetAttributes(_tempfile, FileAttributes.Normal);
                Logger.InfoFormat("Created a temporary file at {0} to prevent read-issues with running GD instance.",
                    _tempfile);
            }
            else {
                _tempfile = file;
            }

            _deleteFile = createCopy;
        }

        public string Filename => _tempfile;

        ~TemporaryCopy() {
            Dispose();
        }

        public void Dispose() {
            if (!string.IsNullOrEmpty(_tempfile) && _deleteFile) {
                try {
                    File.Delete(_tempfile);
                } catch (Exception ex) {
                    Logger.Debug(ex.Message);
                    Logger.Debug(ex.StackTrace);
                    Logger.Debug("This doesn't really matter.");
                }
                _tempfile = null;
            }
        }
    }
}
