namespace GrimDawnItemStats;

/// <summary>
/// The value-rolling ("jitter") functions the game applies to a base stat value using the
/// shared <see cref="MinstdRandom"/> stream.
///
/// These are integer-uniform rolls, NOT the multiplicative <c>base*(1±jitter)</c> formula
/// often quoted on forums (that only approximates the endpoints).
/// </summary>
public static class Jitter
{
    /// <summary>
    /// The jitter used by the Char / Damage / Retaliation / Defense attribute stores. Integer
    /// roll with a minimum ±1 spread.
    /// </summary>
    /// <param name="value">The base stat value.</param>
    /// <param name="jitterPercent">Jitter magnitude in percent (20 for base-record stats).</param>
    /// <param name="rng">The shared stream. A draw is consumed unless value/percent is 0.</param>
    public static double Char(double value, double jitterPercent, MinstdRandom rng)
    {
        if (value == 0.0 || jitterPercent == 0.0)
            return value; // no draw consumed

        int spread = (int)(value * jitterPercent * 0.01);
        if (spread == 0)
            spread = 1; // minimum ±1

        int s = rng.Next();
        double rolled = (double)(RangeMod(s, spread)) - spread + value;
        if (Math.Abs(rolled) < 1.0)
            return value; // snap-back (the draw was still consumed)
        return rolled;
    }

    /// <summary>
    /// The jitter used by the Skill attribute store. Same as <see cref="Char"/> but WITHOUT the
    /// min-1 clamp, and it still consumes a draw when the percent is 0 (as long as the value is
    /// non-zero).
    /// </summary>
    public static double Skill(double value, double jitterPercent, MinstdRandom rng)
    {
        if (value == 0.0)
            return value; // no draw consumed

        int spread = (int)(value * jitterPercent * 0.01);
        int s = rng.Next(); // draw even when spread == 0

        double rolled = (double)(RangeMod(s, spread)) - spread + value;
        if (Math.Abs(rolled) < 1.0)
            return value;
        return rolled;
    }

    /// <summary>
    /// Applies an item's offensive scale (<c>attributeScalePercent</c> + affix
    /// <c>lootRandomizerScale</c>) to an already-jittered value. The game computes
    /// <c>(jittered * (100 + scale)) / 100</c> entirely in <b>float32</b> precision (C++
    /// <c>float</c>) then truncates. This exact form matters: <c>jittered * (1 + scale/100)f</c>
    /// mis-truncates exact-integer cases (e.g. <c>90*130/100 = 117.0f</c> correct, whereas
    /// <c>90 * 1.3f = 116.9999924 → 116</c> wrong). Applied only to scale-eligible offensive
    /// damage modifiers and flat added damage.
    /// </summary>
    public static double ApplyScale(double jittered, double scalePercent)
    {
        float num = (float)((float)jittered * (float)(100.0 + scalePercent));
        return (int)(num / 100.0f);
    }

    /// <summary>
    /// The multiplicative float jitter used for damage-conversion percentages. One draw per valid
    /// conversion slot. <c>factor = s*2^-31*2j + (1-j)</c> where <c>j = percent/100</c>, computed
    /// in float32; the result is clamped to [0, 100].
    /// </summary>
    /// <param name="value">The base conversion percentage.</param>
    /// <param name="jitterPercent">Jitter magnitude in percent (20 for base-record slots).</param>
    /// <param name="rng">The shared stream. One draw is consumed per call.</param>
    public static double Conversion(double value, double jitterPercent, MinstdRandom rng)
    {
        if (jitterPercent <= 0.0)
            return value;

        double j = jitterPercent * 0.01;
        int s = rng.Next();
        float factor = (float)(s * Math.Pow(2.0, -31) * (2.0 * j) + (1.0 - j));
        double rolled = value * factor;
        if (rolled < 0.0) return 0.0;
        if (rolled > 100.0) return 100.0;
        return rolled;
    }

    /// <summary>
    /// Computes <c>s % (2*spread + 1)</c> using the game's unsigned semantics. When the
    /// modulus is 0 the game treats it as 1 (so the result is 0).
    /// </summary>
    private static int RangeMod(int s, int spread)
    {
        uint modulus = unchecked((uint)(2 * spread + 1));
        if (modulus == 0)
            modulus = 1;
        return (int)((uint)s % modulus);
    }
}
