using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IAGrim.Parsers.GameDataParsing.Model {

    class WinformsProgressBar {
        private ProgressBar _progressBar;
        public ProgressTracker Tracker { get; } = new ProgressTracker();

        public WinformsProgressBar(ProgressBar progressBar) {
            _progressBar = progressBar;
            _progressBar.Maximum = 100;

            Tracker.OnProgressChanged += (_, __) => {
                if (_progressBar.InvokeRequired) {
                    Action action = () => _progressBar.Value = Tracker.Progress;
                    _progressBar.Invoke(action);
                }
                else {
                    Action action = () => _progressBar.Value = Tracker.Progress;
                    action.Invoke();
                }
            };
        }
    }
}
