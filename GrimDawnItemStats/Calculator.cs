using System.Globalization;

namespace GrimDawnItemStats;

/// <summary>
/// High-level entry point for computing an item's real, seed-applied stats.
///
/// You supply the raw stat fields for the item's base record and (optionally) its prefix and
/// suffix affix records as plain <c>field → value</c> dictionaries — exactly the data you already
/// have from parsing the game database. Every value is passed as a string: numeric fields are
/// parsed internally, and the handful of text fields (e.g. the conversion in/out damage types)
/// are used as-is.
///
/// The result is the list of rolled stats in the game's draw order. It is a list rather than a
/// dictionary so the original sequence is preserved and so the rare "chance of" proc lines (which
/// can repeat a field name) are not lost.
/// </summary>
public static class Calculator
{
    /// <summary>One rolled stat: the DB field name and its final, seed-applied value.</summary>
    public readonly record struct StatEntry(string Field, double Value);

    /// <summary>
    /// Rolls the item's stats from its raw stat dictionaries and seed.
    /// </summary>
    /// <param name="baseStats">The base item record's fields (<c>field → value</c>). Required.</param>
    /// <param name="prefix">The prefix affix record's fields, or <c>null</c> if the item has none.</param>
    /// <param name="suffix">The suffix affix record's fields, or <c>null</c> if the item has none.</param>
    /// <param name="seed">The item's seed.</param>
    /// <returns>The rolled stats in draw order.</returns>
    public static IReadOnlyList<StatEntry> Calculate(
        IReadOnlyDictionary<string, string> baseStats,
        IReadOnlyDictionary<string, string>? prefix,
        IReadOnlyDictionary<string, string>? suffix,
        uint seed)
    {
        if (baseStats is null) throw new ArgumentNullException(nameof(baseStats));

        var result = ItemStatEngine.Compute(
            ToInputStats(baseStats),
            seed,
            prefixStats: prefix is null ? null : ToInputStats(prefix),
            suffixStats: suffix is null ? null : ToInputStats(suffix));

        var entries = new List<StatEntry>();
        var emitted = new HashSet<string>(StringComparer.Ordinal);

        // Modeled fields first, in the exact order the game draws them.
        foreach (var key in ItemStatEngine.DrawOrderKeys)
            if (result.Stats.TryGetValue(key, out var v) && emitted.Add(key))
                entries.Add(new StatEntry(key, v));

        // Any remaining fields (fixed / zero-draw values echoed at base) after the ordered block.
        foreach (var kv in result.Stats)
            if (emitted.Add(kv.Key))
                entries.Add(new StatEntry(kv.Key, kv.Value));

        // Chance-bearing "N% Chance of ..." proc contributions are separate lines that are not
        // merged into the totals above; append them so no rolled value is lost.
        if (result.ProcLines is { Count: > 0 })
            foreach (var p in result.ProcLines)
                if (p.Min is { } min)
                    entries.Add(new StatEntry(p.Field, min));

        return entries;
    }

    private static IEnumerable<ItemStatEngine.InputStat> ToInputStats(IReadOnlyDictionary<string, string> stats)
    {
        foreach (var kv in stats)
        {
            if (double.TryParse(kv.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var num))
                yield return new ItemStatEngine.InputStat(kv.Key, "", num);
            else
                yield return new ItemStatEngine.InputStat(kv.Key, kv.Value ?? "", 0.0);
        }
    }
}
