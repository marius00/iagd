using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Utilities {
    class ActionCooldown {
        private Stopwatch _stopwatch;
        private readonly long _cooldown;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cooldown">in milliseconds</param>
        public ActionCooldown(long cooldown) {
            _cooldown = cooldown;
        }

        public bool IsReady => _stopwatch == null || _stopwatch.ElapsedMilliseconds >= _cooldown;
        public bool IsOnCooldown => !IsReady;

        public void ExecuteIfReady(Action a) {
            if (IsReady) {
                a.Invoke();
                Reset();
            }
        }

        public void Reset() {
            if (_stopwatch == null) {
                _stopwatch = new Stopwatch();
            }

            _stopwatch.Restart();
        }

        public override string ToString() {
            return $"AC[{_cooldown}]";
        }
    }
}
