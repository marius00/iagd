using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.Utilities.HelperClasses;

namespace IAGrim.Utilities {
    class ActionCooldown {
        private Stopwatch? _stopwatch;
        private readonly long _cooldown;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cooldown">in milliseconds</param>
        public ActionCooldown(long cooldown) {
            _cooldown = cooldown;
        }
        public ActionCooldown(long cooldown, bool startTriggered) {
            _cooldown = cooldown;

            if (startTriggered) {
                Reset();
            }
        }

        public bool IsReady => _stopwatch == null || _stopwatch.ElapsedMilliseconds >= _cooldown;
        public bool IsOnCooldown => !IsReady;

        /// <summary>
        /// The cooldown is armed even when the action throws. Without that, a repeatable
        /// failure downgrades the cooldown to however fast the caller happens to poll.
        /// </summary>
        public void ExecuteIfReady(Action a) {
            if (IsReady) {
                try {
                    a.Invoke();
                }
                finally {
                    Reset();
                }
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
