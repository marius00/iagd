using EvilsoftCommons.Exceptions;
using EvilsoftCommons.Misc;
using IAGrim.UI;
using IAGrim.Utilities;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace IAGrim.Parsers.Arz {

    /// <summary>
    /// Background worker for parsing Grim Dawn database
    /// To be used in conjunction with a UI
    /// </summary>
    class ParsingUiBackgroundWorker : IDisposable {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ParsingUiBackgroundWorker));
        private BasicBackgroundWorker _bw;
        private readonly ArzParser _arzParser;

        public class ParsingUiBackgroundWorkerArgument {
            public string Path;
            public bool ExpansionOnlyMod;
            public string LocalizationFile;
            public bool TagsOnly;
        }

        public ParsingUiBackgroundWorker(RunWorkerCompletedEventHandler completed, ParsingUiBackgroundWorkerArgument args, ArzParser parser) {
            _arzParser = parser;
            _bw = new BasicBackgroundWorker(Execute, completed, args);
        }

        private void Execute(BackgroundWorker worker, DoWorkEventArgs args) {
            if (Thread.CurrentThread.Name == null)
                Thread.CurrentThread.Name = "ParserUI";


            
            try {
                ParsingUiBackgroundWorkerArgument arg = args.Argument as ParsingUiBackgroundWorkerArgument;
                if (!string.IsNullOrEmpty(arg.Path) && Directory.Exists(arg.Path)) {
                    if (arg.TagsOnly) {
                        _arzParser.LoadArcTags(arg.Path, arg.ExpansionOnlyMod);
                    }
                    else {
                        _arzParser.LoadArzDb(arg.Path, arg.LocalizationFile, arg.ExpansionOnlyMod);
                        if (!arg.ExpansionOnlyMod && Directory.Exists(Path.Combine(arg.Path, "gdx1"))) {
                            _arzParser.LoadArzDb(Path.Combine(arg.Path, "gdx1"), arg.LocalizationFile, true);
                        }
                    }
                }
                else {
                    Logger.Warn("Could not find the Grim Dawn install location");
                }
            }
            catch (UnauthorizedAccessException ex) {
                Logger.Warn(ex.Message);
                Logger.Warn(ex.StackTrace);
                Logger.Warn("Access denied trying to read a file. This may happen if Grim Dawn is currently running.");
                MessageBox.Show(string.Format("Access denied while parsing Grim Dawn\n\nEnsure that Grim Dawn is not currently running.", ex.Message), 
                    "Error", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
            }
            catch (IOException ex) {
                MessageBox.Show($"{ex.Message}\n\nPerhaps Grim Dawn is running?", 
                    "Error", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
            }
            catch (Exception ex) {
                Logger.Warn(ex.Message);
                Logger.Warn(ex.StackTrace);
                ExceptionReporter.ReportException(ex);
                throw;
            }
            
        }

        public void Dispose() {
            _bw?.Dispose();
            _bw = null;
        }
    }
}
