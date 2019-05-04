using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IAGrim.Services {
    static class GrimStateTracker {
        public class WorldVector {
            public float X  { get; set; }
            public float Y { get; set; }
            public float Z { get; set; }
            public int Zone { get; set; }

            
            public override string ToString() {
                return $"[{X}, {Z}, {Y}, {Zone}]";
            }
        }
        public static WorldVector LastKnownPosition { get; set; }
        public static WorldVector LastStashPosition { get; set; }

        public static bool IsFarFromStash {
            get {
                if (LastKnownPosition == null || LastStashPosition == null)
                    return false;

                float distSq = (LastKnownPosition.X - LastStashPosition.X) * (LastKnownPosition.X - LastStashPosition.X)
                               + (LastKnownPosition.Z - LastStashPosition.Z) * (LastKnownPosition.Z - LastStashPosition.Z);
                return distSq >= MinDistance * MinDistance;
            }
        }

        public static int MinDistance => 15;

        public static int? Distance {
            get {
                if (LastKnownPosition == null || LastStashPosition == null)
                    return null;

                float distSq = (LastKnownPosition.X - LastStashPosition.X) * (LastKnownPosition.X - LastStashPosition.X)
                               + (LastKnownPosition.Z - LastStashPosition.Z) * (LastKnownPosition.Z - LastStashPosition.Z);
                return (int)Math.Sqrt(distSq);
            }
        }
    }
}
