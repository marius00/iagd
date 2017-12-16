using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IAGrim.Services {
    class GrimStateTracker {
        public class WorldVector {
            public float X  { get; set; }
            public float Y { get; set; }
            public float Z { get; set; }
            public int Zone { get; set; }
        }
        public static WorldVector LastKnownPosition { get; set; }
        public static WorldVector LastStashPosition { get; set; }

        public static bool IsFarFromStash {
            get {
                if (LastKnownPosition == null || LastStashPosition == null)
                    return false;

                float distSq = (LastKnownPosition.X - LastStashPosition.X) * (LastKnownPosition.X - LastStashPosition.X) 
                    + (LastKnownPosition.Z - LastStashPosition.Z) * (LastKnownPosition.Z - LastStashPosition.Z);
                return distSq >= 25;
            }
        }
    }
}
