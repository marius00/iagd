﻿namespace StatTranslator
{
    internal class Randomizer {
        const int a = 16807;
        const int m = 0x7FFFFFFF;
        const int q = 0x1F31D;
        const int r = 2836;
        int previous;

        //generate
        public int Generate() {
            int hi = previous / q;
            int lo = previous - q * hi;  // previous % q
            int t = a * lo - r * hi;

            //int t = a * (previous % q) - r * (previous / q);
            previous = (t >= 0) ? t : t + m;

            return previous;
        }

        public Randomizer() {
            previous = 0;
        }

        public Randomizer(int seed) {
            Seed(seed);
        }

        public void Seed(int seed) {
            previous = seed;
            Generate();
        }

        public float GenerateFloat(float min, float max) {
            float f = (float)Generate() / (float)m;
            return f * (max - min) + min;
        }

        public int GenerateInt(int min, int max) {
            return min + (Generate() % (max - min + 1));

        }

        public int GenerateSeed() {
            return Generate();

        }
    }
}
