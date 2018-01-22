using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IAGrim.Parsers.GameDataParsing.Model {
    public class ProgressTracker {
        private int _currentValue;
        private int _maxValue;


        public int Progress {
            get {
                if (_maxValue > 0) {
                    return Math.Min((_currentValue * 100) / _maxValue, 100);
                }
                else {
                    return 0;
                }
            }
        }

        public int MaxValue {
            get {
                return _maxValue;
            }
            set {
                _maxValue = value;
                OnMaxValueChanged?.Invoke(this, null);
            }
        }

        public event EventHandler OnProgressChanged;
        public event EventHandler OnMaxValueChanged;

        public void Increment() {
            if (_currentValue + 1 <= _maxValue) {
                Interlocked.Increment(ref _currentValue);
                OnProgressChanged?.Invoke(this, null);
            }
        }

        public void MaxProgress() {
            Interlocked.Add(ref _currentValue, _maxValue - _currentValue);
        }
    }
}
