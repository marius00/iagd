namespace GrimDawnItemStats;

/// <summary>
/// The Park–Miller "minimal standard" (MINSTD) linear congruential generator, implemented
/// with Schrage's method exactly as Grim Dawn does it (multiplier 16807, modulus 2147483647).
///
/// The item seed is advanced once when the generator is constructed, mirroring the single
/// priming iteration the game runs before any stat is rolled. Each <see cref="Next"/> call
/// then advances the shared stream one more step.
/// </summary>
public sealed class MinstdRandom
{
    // Schrage decomposition constants for a = 16807, m = 2147483647:
    //   q = m / a = 127773,  r = m % a = 2836.
    private const int A = 16807;      // 0x41a7
    private const int Q = 127773;     // 0x1f31d
    private const int R = 2836;       // 0x0b14
    private const int M = 2147483647; // 0x7fffffff

    private int _state;

    /// <summary>Seeds the stream and performs the single priming iteration the game does.</summary>
    public MinstdRandom(uint seed)
    {
        _state = Step(unchecked((int)seed));
    }

    /// <summary>The current internal state (after priming / last draw). Exposed for testing.</summary>
    public int State => _state;

    /// <summary>Advances the stream one step and returns the new state (a positive draw).</summary>
    public int Next()
    {
        _state = Step(_state);
        return _state;
    }

    /// <summary>
    /// One MINSTD/Schrage iteration. By construction every intermediate fits in Int32,
    /// matching the game's integer arithmetic exactly.
    /// </summary>
    public static int Step(int s)
    {
        int hi = s / Q;
        int lo = s % Q;
        int result = A * lo - R * hi;
        if (result < 0)
            result += M;
        return result;
    }
}
