namespace GrimDawnItemStats;

/// <summary>
/// Computes the real, seed-applied stat values for a Grim Dawn item by replaying the game's
/// shared MINSTD random stream over the item's rollable stats in the exact draw order used by
/// the game engine.
///
/// Draw order (per store, then field order within a store):
/// Char → flat added-damage min/max pairs → damage %-modifiers → retaliation flat min/max pairs →
/// retaliation Dur (Slow/DoT) block → retaliation %-modifiers → retaliation Reflex (CC) block →
/// Defense (block modifier/amount-modifier first, then resists) → conversions → Skill (deferred,
/// rolls last). Offensive damage modifiers and flat added damage take the item's
/// <c>attributeScalePercent</c> (float32) after jitter; retaliation and block never scale;
/// conversions use a float multiplicative jitter. Shield block fields are loaded raw with no
/// jitter at all (0 draws). Retaliation Dur's DurationMin and Reflex's Chance companions are
/// fixed (0 draws, echoed at base value).
///
/// Most callers should use the high-level <see cref="Calculator.Calculate"/> entry point,
/// which accepts plain string→string dictionaries and returns the rolled values in draw order.
/// </summary>
public static class ItemStatEngine
{
    /// <summary>One raw stat field for an item record.</summary>
    /// <param name="Stat">The field name.</param>
    /// <param name="TextValue">Textual payload (conversion in/out types live here; else empty).</param>
    /// <param name="Value">Numeric value (0 for text-only stats).</param>
    public readonly record struct InputStat(string Stat, string TextValue, double Value);

    /// <summary>A contribution whose own <c>{Field}Chance</c> makes it a separate "N% Chance of ..."
    /// proc line rather than being summed into <see cref="Result.Stats"/>'s merged total for that
    /// field. Occurs when an affix source carries its own chance for a damage type that another
    /// source also contributes to as plain (non-chance) damage.</summary>
    public readonly record struct ProcLine(string Field, double? Min, double? Max, double? DurationMin, double Chance);

    /// <summary>Result of a roll.</summary>
    /// <param name="Stats">Field → final value. Rolled stats are computed; fixed fields are echoed
    /// at their base value.</param>
    /// <param name="UnmodeledFields">Rollable stat fields present on the item that the engine does
    /// not model. Their presence means downstream draws may be desynced — treat the output as
    /// approximate when this is non-empty.</param>
    /// <param name="ProcLines">Separate proc lines split off from the merged totals (see
    /// <see cref="ProcLine"/>).</param>
    public sealed record Result(
        IReadOnlyDictionary<string, double> Stats,
        IReadOnlyList<string> UnmodeledFields,
        IReadOnlyList<ProcLine>? ProcLines = null);

    private const double BaseJitterPercent = 20.0;

    private enum Kind { Char, Flat, SlowFlat, Dmg, Leech, OffReflex, OffSlow, OffReduc, RetalFlat, RetalDur, RetalMod, RetalReflex, Def, Conv, Skill }

    private readonly record struct OrderEntry(Kind Kind, string Field, bool Scales);

    // ---- Store field orders, in the sequence the game's per-store loaders draw them ----

    private static readonly string[] Char =
    {
        "characterStrength", "characterDexterity", "characterIntelligence", "characterLife", "characterMana",
        "characterStrengthModifier", "characterDexterityModifier", "characterIntelligenceModifier", "characterLifeModifier",
        "characterManaModifier", "characterLifeMultModifier", "characterOffensiveAbility", "characterDefensiveAbility",
        "characterOffensiveAbilityModifier", "characterDefensiveAbilityModifier", "characterLifeRegen", "characterLifeRegenModifier",
        "characterManaRegenModifier", "characterConstitutionModifier", "characterHealIncreasePercent", "characterTotalSpeedModifier",
        "characterAttackSpeedModifier", "characterAttackSpeedMaxModifier", "characterSpellCastSpeedModifier", "characterSpellCastSpeedMaxModifier",
        "characterRunSpeedModifier", "characterRunSpeedMaxModifier", "characterDefensiveBlockRecoveryReduction", "characterEnergyAbsorptionPercent",
        "characterDodgePercent", "characterDeflectProjectile", "characterManaLimitReserve", "characterManaLimitReserveModifier",
    };

    // Flat added-damage fields; each present {field}Min / {field}Max is one jittered draw (min
    // then spread), then scaled. Drawn after Char, before the %-modifiers.
    private static readonly string[] Flat =
    {
        // offensivePhysicalMin/Max is fixed (0 draws, weapon base damage) only when the item's own
        // Class is a Weapon* class. For armor/jewelry the same field name is a real flat pair
        // (jittered + scaled, "N-M Physical Damage"); gated dynamically on Class in Compute().
        "offensivePhysical",
        "offensiveBonusPhysical", "offensivePierce", "offensiveFire", "offensiveCold", "offensiveLightning",
        "offensivePoison", "offensiveLife", "offensiveAether", "offensiveChaos", "offensiveElemental",
    };

    // offensiveSlow{Type}Min (+ Max, DurationMin): the per-tick damage-over-time flat value paired
    // with offensiveSlow{Type}Modifier. Draws as its own flat-tier entry right after the regular
    // Flat block and before the whole Dmg-modifier list — same (min, spread) mechanism as Flat,
    // scaled by attributeScalePercent. DurationMin is fixed (0 draws); the display total is
    // scaled_min * DurationMin (or a scaled_min..scaled_max range when a Max/spread is present).
    private static readonly string[] SlowFlat =
    {
        "offensiveSlowPhysical", "offensiveSlowBleeding", "offensiveSlowFire", "offensiveSlowCold",
        "offensiveSlowLightning", "offensiveSlowPoison", "offensiveSlowLife", "offensiveSlowAether", "offensiveSlowChaos",
        // Leech-over-time DoTs: same scaled min[,spread] * DurationMin mechanism as the damage-type
        // DoTs above (e.g. "72 Energy Leech over 3 Seconds").
        "offensiveSlowLifeLeach", "offensiveSlowManaLeach",
    };

    // Offensive crowd-control-on-hit. {field}Min is a duration in SECONDS (one jittered draw, NO
    // scale); {field}Chance is fixed (0 draws). Mechanically identical to RetalReflex.
    private static readonly string[] OffReflex =
    {
        "offensiveStun", "offensiveKnockdown", "offensiveSleep", "offensiveFreeze", "offensivePetrify",
    };

    // Offensive slow/debuff duration effects: one jittered draw for the value; {field}DurationMin
    // fixed, {field}Chance fixed if present. Speed slows (TotalSpeed/AttackSpeed/SpellCastSpeed/
    // RunSpeed) scale by attributeScalePercent; ability reductions (Offensive/DefensiveAbility) do
    // not. The scale flag is per-field (see BuildOrder).
    private static readonly (string Field, bool Scaled)[] OffSlow =
    {
        ("offensiveSlowTotalSpeed", true), ("offensiveSlowAttackSpeed", true),
        ("offensiveSlowSpellCastSpeed", true), ("offensiveSlowRunSpeed", true),
        ("offensiveSlowOffensiveAbility", false), ("offensiveSlowDefensiveAbility", false),
    };

    // Damage %-modifiers. Each offensiveSlow{Type}Modifier is drawn as a consecutive (value,
    // duration) pair with its offensiveSlow{Type}DurationModifier sibling (both scaled). Physical/
    // Bleeding/Fire/Cold/Lightning/Life/Poison have a DurationModifier field; Aether/Chaos only
    // have a DurationMin (which is fixed).
    private static readonly string[] Dmg =
    {
        "offensiveTotalDamageModifier", "offensiveCritDamageModifier",
        "offensivePhysicalModifier", "offensivePierceModifier", "offensiveFireModifier", "offensiveColdModifier", "offensiveLightningModifier",
        "offensivePoisonModifier", "offensiveLifeModifier", "offensiveAetherModifier", "offensiveChaosModifier", "offensiveElementalModifier",
        "offensiveSlowPhysicalModifier", "offensiveSlowPhysicalDurationModifier",
        "offensiveSlowBleedingModifier", "offensiveSlowBleedingDurationModifier",
        "offensiveSlowFireModifier", "offensiveSlowFireDurationModifier",
        "offensiveSlowColdModifier", "offensiveSlowColdDurationModifier",
        "offensiveSlowLightningModifier", "offensiveSlowLightningDurationModifier",
        "offensiveSlowPoisonModifier", "offensiveSlowPoisonDurationModifier",
        "offensiveSlowLifeModifier", "offensiveSlowLifeDurationModifier",
        "offensiveSlowAetherModifier", "offensiveSlowChaosModifier",
    };

    // offensive{Type}Modifier fields that use the same chance/non-chance proc-line split as
    // Flat/SlowFlat (see ProcLine): a source carrying its own {Field}Chance becomes a separate
    // proc line instead of being summed into the merged total. Restricted to the type-specific
    // modifiers; Total/CritDamageModifier are excluded.
    private static readonly HashSet<string> ModifierChanceSplitFields = new(StringComparer.Ordinal)
    {
        "offensivePhysicalModifier", "offensivePierceModifier", "offensiveFireModifier", "offensiveColdModifier",
        "offensiveLightningModifier", "offensivePoisonModifier", "offensiveLifeModifier", "offensiveAetherModifier",
        "offensiveChaosModifier", "offensiveElementalModifier",
    };

    // offensiveLifeLeech: one jittered draw, Min-only, NOT scaled. Draws after all Dmg %-modifiers
    // and before the retaliation store. Displays "{v}% of Attack Damage converted to Health".
    private static readonly string[] Leech = { "offensiveLifeLeech" };

    // Offensive resistance/damage-reduction debuffs: value is one jittered draw (NO scale),
    // DurationMin fixed (0 draws). Draw after offensiveLifeLeech / the speed block, before the
    // retaliation store.
    private static readonly string[] OffReduc =
    {
        "offensivePhysicalReductionPercent", "offensiveElementalReductionPercent",
        "offensiveTotalDamageReductionPercent", "offensiveTotalDamageReductionAbsolute",
        "offensiveTotalResistanceReductionPercent", "offensiveTotalResistanceReductionAbsolute",
        "offensivePhysicalResistanceReductionPercent", "offensivePhysicalResistanceReductionAbsolute",
        "offensiveElementalResistanceReductionPercent", "offensiveElementalResistanceReductionAbsolute",
    };

    // Retaliation flat damage: same (min, spread) pair mechanism as Flat, but retaliation never
    // scales. Drawn after the damage %-modifiers, before Defense.
    private static readonly string[] RetalFlat =
    {
        "retaliationPhysical", "retaliationPierce", "retaliationFire", "retaliationCold", "retaliationLightning",
        "retaliationPoison", "retaliationLife", "retaliationAether", "retaliationChaos", "retaliationElemental",
    };

    // Retaliation slow/DoT block: one jittered draw (no scale). Drawn after RetalFlat, before
    // RetalMod. {field}DurationMin is fixed (0 draws, echoed base).
    private static readonly string[] RetalDur =
    {
        "retaliationSlowPhysical", "retaliationSlowPierce", "retaliationSlowFire", "retaliationSlowCold",
        "retaliationSlowLightning", "retaliationSlowPoison", "retaliationSlowLife", "retaliationSlowAether",
        "retaliationSlowChaos", "retaliationSlowBleeding",
    };

    private static readonly string[] RetalDurPct = { "retaliationSlowAttackSpeed", "retaliationSlowRunSpeed" };

    // Retaliation %-modifiers, drawn right after the RetalDur block. retaliationDamageMultModifier
    // is the one exception: it draws AFTER the Defense store (see RetalDamageMultPostDef).
    private static readonly string[] RetalMod =
    {
        "retaliationTotalDamageModifier",
        "retaliationPhysicalModifier", "retaliationPierceModifier", "retaliationFireModifier", "retaliationColdModifier",
        "retaliationLightningModifier", "retaliationPoisonModifier", "retaliationLifeModifier", "retaliationAetherModifier",
        "retaliationChaosModifier", "retaliationElementalModifier",
    };

    // retaliationDamageMultModifier draws after the Defense store, not with the rest of RetalMod.
    private static readonly string[] RetalDamageMultPostDef = { "retaliationDamageMultModifier" };

    // Retaliation crowd-control block: one jittered draw (no scale). Drawn after RetalMod, before
    // Defense. {field}Chance is fixed (0 draws, echoed base).
    private static readonly string[] RetalReflex =
    {
        "retaliationStun", "retaliationFreeze", "retaliationConfusion",
    };

    // Defense store field order. defensiveBlockModifier/AmountModifier/ProtectionModifier load
    // first, then absorption, resists and the rest. Every entry is a single-value jittered draw
    // (default 20% jitter), NOT scaled by attributeScalePercent. The resistance caps (71-90,
    // defensive*MaxResist) draw no RNG and are fixed, so they are omitted from this list.
    private static readonly string[] Def =
    {
        "defensiveBlockModifier", "defensiveBlockAmountModifier", "defensiveProtectionModifier", // 01-03
        "defensiveAbsorptionModifier",                                                           // 04
        "defensivePhysical", "defensivePierce", "defensiveFire", "defensiveCold", "defensiveLightning", // 05-09
        "defensivePoison", "defensiveLife", "defensiveAether", "defensiveChaos",                 // 10-13
        "defensiveElementalResistance", "defensiveBleeding",                                     // 14-15
        "defensiveSlowLifeLeach", "defensiveSlowManaLeach",                                      // 16-17 (leech resist)
        "defensiveManaBurn", "defensiveAllResistance",                                           // 18-19
        "defensivePhysicalModifier", "defensivePierceModifier", "defensiveFireModifier", "defensiveColdModifier", // 20-23
        "defensiveLightningModifier", "defensivePoisonModifier", "defensiveLifeModifier", "defensiveAetherModifier", // 24-27
        "defensiveChaosModifier", "defensiveElementalModifier", "defensiveBleedingModifier",     // 28-30
        "defensiveSlowLifeLeachModifier", "defensiveSlowManaLeachModifier",                      // 31-32
        // DoT-resistance durations (33-43): "{v}% Reduction in {DoT} Duration"
        "defensivePhysicalDuration", "defensiveFireDuration", "defensiveColdDuration", "defensiveLightningDuration", // 33-36
        "defensivePoisonDuration", "defensiveLifeDuration", "defensiveAetherDuration", "defensiveChaosDuration", // 37-40
        "defensiveBleedingDuration", "defensiveSlowLifeLeachDuration", "defensiveSlowManaLeachDuration", // 41-43
        // DoT-resistance duration modifiers (44-54): "+{v}% {DoT} Duration Reduction"
        "defensivePhysicalDurationModifier", "defensiveFireDurationModifier", "defensiveColdDurationModifier", // 44-46
        "defensiveLightningDurationModifier", "defensivePoisonDurationModifier", "defensiveLifeDurationModifier", // 47-49
        "defensiveAetherDurationModifier", "defensiveChaosDurationModifier", "defensiveBleedingDurationModifier", // 50-52
        "defensiveSlowLifeLeachDurationModifier", "defensiveSlowManaLeachDurationModifier",      // 53-54
        "defensiveDisruption", "defensiveStun", "defensiveStunModifier", "defensiveFreeze",      // 55-58
        "defensiveTrap", "defensivePetrify", "defensiveSleep", "defensiveSleepModifier",         // 59-62
        "defensiveKnockdown", "defensiveKnockdownModifier", "defensiveTaunt", "defensiveFear",   // 63-66
        "defensiveConfusion", "defensiveConvert", "defensiveTotalSpeedResistance", "defensiveCrowdControl", // 67-70
        // (71-90 defense caps = fixed)
        "defensiveReflect", "defensiveReflectModifier", "defensivePercentCurrentLife", "defensivePercentReflectionResistance", // 91-94
    };

    // Shield block fields are read straight from the raw table with no jitter at all -> fixed,
    // 0 draws, echoed as-is.
    private static readonly string[] BlockFixed =
    {
        "defensiveBlock", "defensiveBlockChance", "blockAbsorption", "blockRecoveryTime",
    };

    private static readonly string[] Conv = { "conversionPercentage", "conversionPercentage2" };

    // Skill store, drawn LAST (deferred). Each field is one jittered draw. Only a few of these
    // appear on real items; the rest are listed for draw-order completeness. The per-field
    // {field}Chance companion is fixed (0 draws), see IsFixed().
    private static readonly string[] Skill =
    {
        "skillCooldownReduction", "skillManaCostReduction", "skillComboChargeSpendReduction",
        "skillProjectileSpeedModifier", "skillCooldownReductionModifier", "skillManaCostReductionModifier",
    };

    // These skill fields draw EARLY when affix-sourced (see DrawSkillEarly). Every other skill
    // field draws at the deferred Skill position, like the base record's own skill fields.
    private static readonly string[] SkillEarly = { "skillCooldownReduction", "skillManaCostReduction" };

    // crit/total damage modifiers do NOT take item scale.
    private static readonly HashSet<string> NonScaling = new(StringComparer.Ordinal)
    {
        "offensiveCritDamageModifier", "offensiveTotalDamageModifier",
    };

    private static readonly List<OrderEntry> Order = BuildOrder();

    /// <summary>Every field the engine models (single fields + flat Min/Max pairs).</summary>
    private static readonly HashSet<string> Modeled = BuildModeled();

    private static List<OrderEntry> BuildOrder()
    {
        var order = new List<OrderEntry>();
        foreach (var f in Char) order.Add(new(Kind.Char, f, false));
        foreach (var f in Flat) order.Add(new(Kind.Flat, f, true));
        foreach (var f in SlowFlat) order.Add(new(Kind.SlowFlat, f, true));
        foreach (var f in Dmg) order.Add(new(Kind.Dmg, f, !NonScaling.Contains(f)));
        foreach (var f in Leech) order.Add(new(Kind.Leech, f, false));        // no scale
        foreach (var f in OffReflex) order.Add(new(Kind.OffReflex, f, false)); // crowd control, no scale
        foreach (var (f, scaled) in OffSlow) order.Add(new(Kind.OffSlow, f, scaled)); // speed scales, ability does not
        foreach (var f in OffReduc) order.Add(new(Kind.OffReduc, f, false));  // no scale
        foreach (var f in RetalFlat) order.Add(new(Kind.RetalFlat, f, false));
        foreach (var f in RetalDur) order.Add(new(Kind.RetalDur, f, false));
        foreach (var f in RetalMod) order.Add(new(Kind.RetalMod, f, false));
        foreach (var f in RetalDurPct) order.Add(new(Kind.RetalDur, f, false));
        foreach (var f in RetalReflex) order.Add(new(Kind.RetalReflex, f, false));
        foreach (var f in Def) order.Add(new(Kind.Def, f, false));
        foreach (var f in RetalDamageMultPostDef) order.Add(new(Kind.RetalMod, f, false));
        foreach (var f in Conv) order.Add(new(Kind.Conv, f, false));
        foreach (var f in Skill) order.Add(new(Kind.Skill, f, false));
        return order;
    }

    /// <summary>The stat keys the engine emits, in draw order — used to order the output of
    /// <see cref="Calculator.Calculate"/>.</summary>
    internal static readonly IReadOnlyList<string> DrawOrderKeys = BuildDrawOrderKeys();

    private static List<string> BuildDrawOrderKeys()
    {
        var keys = new List<string>();
        foreach (var e in Order)
        {
            if (e.Kind is Kind.Flat or Kind.RetalFlat or Kind.SlowFlat) { keys.Add(e.Field + "Min"); keys.Add(e.Field + "Max"); }
            else if (e.Kind == Kind.RetalDur) { keys.Add(e.Field + "Min"); keys.Add(e.Field + "DurationMin"); keys.Add(e.Field + "Chance"); }
            else if (e.Kind == Kind.RetalReflex) { keys.Add(e.Field + "Min"); keys.Add(e.Field + "Chance"); }
            else if (e.Kind == Kind.OffReflex) { keys.Add(e.Field + "Min"); keys.Add(e.Field + "Chance"); }
            else if (e.Kind == Kind.OffSlow) { keys.Add(e.Field + "Min"); keys.Add(e.Field + "DurationMin"); keys.Add(e.Field + "Chance"); }
            else if (e.Kind == Kind.Leech) keys.Add(e.Field + "Min");
            else if (e.Kind == Kind.OffReduc) { keys.Add(e.Field + "Min"); keys.Add(e.Field + "DurationMin"); }
            else keys.Add(e.Field);
        }
        return keys;
    }

    private static HashSet<string> BuildModeled()
    {
        var m = new HashSet<string>(StringComparer.Ordinal);
        foreach (var e in Order)
        {
            if (e.Kind is Kind.Flat or Kind.RetalFlat or Kind.SlowFlat) { m.Add(e.Field + "Min"); m.Add(e.Field + "Max"); }
            else if (e.Kind == Kind.RetalDur) { m.Add(e.Field + "Min"); m.Add(e.Field + "DurationMin"); m.Add(e.Field + "Chance"); }
            else if (e.Kind == Kind.RetalReflex) { m.Add(e.Field + "Min"); m.Add(e.Field + "Chance"); }
            else if (e.Kind == Kind.OffReflex) { m.Add(e.Field + "Min"); m.Add(e.Field + "Chance"); }
            else if (e.Kind == Kind.OffSlow) { m.Add(e.Field + "Min"); m.Add(e.Field + "DurationMin"); m.Add(e.Field + "Chance"); }
            else if (e.Kind == Kind.Leech) m.Add(e.Field + "Min");
            else if (e.Kind == Kind.OffReduc) { m.Add(e.Field + "Min"); m.Add(e.Field + "DurationMin"); }
            else m.Add(e.Field);
        }
        return m;
    }

    // Fields that are present on items but never draw (echoed at base value).
    private static readonly HashSet<string> Fixed = new(StringComparer.Ordinal)
    {
        "characterBaseAttackSpeed", "characterManaRegen", "characterConstitution", "characterAttackSpeed",
        "characterSpellCastSpeed", "characterRunSpeed", "characterIncreasedExperience", "characterIncreasedGold",
        "characterLightRadius", "characterGlobalReqReduction", "characterLevelReqReduction", "characterModifierPoints",
        // (characterHealIncreasePercent jitters and lives in the Char list — it is not fixed.)
        "defensiveProtection",          // armor: 0 draws
    };

    private static bool IsFixed(string f)
    {
        if (Fixed.Contains(f)) return true;
        if (f.StartsWith("character", StringComparison.Ordinal) && f.EndsWith("ReqReduction", StringComparison.Ordinal)) return true;
        // weapon base damage: 0 draws, shown at base range.
        if (f.StartsWith("offensiveBase", StringComparison.Ordinal) && (f.EndsWith("Min", StringComparison.Ordinal) || f.EndsWith("Max", StringComparison.Ordinal))) return true;
        if (BlockFixed.Contains(f)) return true;
        // offensiveSlow*DurationMin: fixed (0 draws), same as the retaliation Dur block's DurationMin.
        if (f.StartsWith("offensiveSlow", StringComparison.Ordinal) && f.EndsWith("DurationMin", StringComparison.Ordinal)) return true;
        // offensive{X}RatioMin (e.g. offensivePierceRatioMin -> "100% Armor Piercing"): fixed, 0 draws.
        if (f.StartsWith("offensive", StringComparison.Ordinal) && f.EndsWith("RatioMin", StringComparison.Ordinal)) return true;
        // Per-field proc {value}Chance companions (offensive*/retaliation*/skill*) are loaded but
        // never jittered (0 draws); they prefix the modeled value line as "{chance}% Chance of ...".
        if (f.EndsWith("Chance", StringComparison.Ordinal) && !f.EndsWith("GlobalChance", StringComparison.Ordinal)
            && (f.StartsWith("offensive", StringComparison.Ordinal) || f.StartsWith("retaliation", StringComparison.Ordinal)
                || f.StartsWith("skill", StringComparison.Ordinal)))
            return true;
        // Grouped-proc config: offensive/retaliationGlobalChance ("N% Chance of:" group header) and
        // each participating effect's *Global flag group several proc effects under one shared roll
        // instead of each effect's own Chance; they consume no RNG.
        if (f == "offensiveGlobalChance" || f == "retaliationGlobalChance") return true;
        if ((f.StartsWith("offensive", StringComparison.Ordinal) || f.StartsWith("retaliation", StringComparison.Ordinal))
            && f.EndsWith("Global", StringComparison.Ordinal))
            return true;
        return false;
    }

    private static readonly string[] CosmeticPrefixes =
    {
        "augment", "modif", "item", "drop", "physics", "mesh", "bitmap", "baseTexture", "bumpTexture", "glowTexture", "shader",
        "weaponTrail", "hitSound", "swipeSound", "blockSound", "attackEffect", "basicProjectile", "actor", "casts", "maxTransparency",
        "outline", "scale", "templateName", "Class", "FileDescription", "levelRequirement", "itemLevel", "armorClassification",
        "characterBaseAttackSpeedTag", "armorFemaleMesh", "armorMaleMesh", "decoration", "attributeScalePercent", "petBonusName",
    };

    private static readonly string[] StatPrefixes =
    {
        "offensive", "defensive", "retaliation", "character", "skill", "conversion", "blockAbsorption", "blockRecovery",
    };

    /// <summary>
    /// A rollable stat field the engine does not model — its presence risks desyncing the shared
    /// stream, so downstream computed values may be wrong.
    /// </summary>
    private static bool IsConcerning(string f)
    {
        if (CosmeticPrefixes.Any(p => f.StartsWith(p, StringComparison.Ordinal))) return false;
        if (Modeled.Contains(f) || IsFixed(f)) return false;
        if (f.StartsWith("conversion", StringComparison.Ordinal)) return false; // handled explicitly
        return StatPrefixes.Any(p => f.StartsWith(p, StringComparison.Ordinal));
    }

    /// <summary>
    /// Rolls the item's stats from its raw stat rows and seed.
    /// </summary>
    /// <param name="stats">All stat rows for the base record.</param>
    /// <param name="seed">The item seed.</param>
    /// <param name="scalePercentOverride">If set, used as the offensive scale instead of the
    /// record's <c>attributeScalePercent</c>.</param>
    /// <param name="prefixStats">All stat rows for the item's prefix record, if any.</param>
    /// <param name="suffixStats">All stat rows for the item's suffix record, if any.</param>
    /// <remarks>
    /// <para><b>Affixes (prefix / suffix).</b> The per-field draw order is
    /// <c>Prefix → Suffix → Base (last)</c>. All three call the same jitter function, each with its
    /// OWN field value and its OWN <c>lootRandomizerJitter</c> percent (not the base record's
    /// constant 20%). If an affix record has no <c>lootRandomizerJitter</c> stat, its contribution
    /// to that field does not jitter at all (fixed, 0 draws, value echoed). The jittered
    /// contributions are SUMMED, then scaled once for scale-eligible fields using
    /// <c>sp = base attributeScalePercent + prefix lootRandomizerScale + suffix lootRandomizerScale</c>.
    /// </para>
    /// <para>All affix-touched field shapes are handled: scalar fields, flat / retaliation-flat
    /// (min, spread) pairs (summed component-wise per source, scale applied once to the summed
    /// min), retaliation Dur/Reflex pairs, per-source conversion jitters, the early-drawn affix
    /// skill-store fields, and the interleaved per-source Slow{X} (value, duration) pairs.</para>
    /// </remarks>
    public static Result Compute(
        IEnumerable<InputStat> stats,
        uint seed,
        double? scalePercentOverride = null,
        IEnumerable<InputStat>? prefixStats = null,
        IEnumerable<InputStat>? suffixStats = null)
    {
        var (values, text) = ParseStats(stats);
        bool hasPrefix = prefixStats is not null;
        bool hasSuffix = suffixStats is not null;
        var (pValues, pText) = hasPrefix ? ParseStats(prefixStats!) : (new(StringComparer.Ordinal), new Dictionary<string, string>(StringComparer.Ordinal));
        var (sValues, sText) = hasSuffix ? ParseStats(suffixStats!) : (new(StringComparer.Ordinal), new Dictionary<string, string>(StringComparer.Ordinal));

        // Absence of lootRandomizerJitter means that affix's own contribution never jitters (fixed,
        // 0 draws). Defaulting to 0 here (rather than the base record's 20%) reproduces that:
        // Jitter.Char/Skill treat a 0 percent as a no-draw.
        double pfxPct = pValues.GetValueOrDefault("lootRandomizerJitter", 0.0);
        double sfxPct = sValues.GetValueOrDefault("lootRandomizerJitter", 0.0);
        double affixScaleAdd = pValues.GetValueOrDefault("lootRandomizerScale", 0.0)
            + sValues.GetValueOrDefault("lootRandomizerScale", 0.0);

        double sp = scalePercentOverride
            ?? (values.GetValueOrDefault("attributeScalePercent", 0.0) + affixScaleAdd);

        var rng = new MinstdRandom(seed);
        var result = new Dictionary<string, double>(StringComparer.Ordinal);
        var affixPairFields = new List<string>();
        var procLines = new List<ProcLine>();
        var handledDur = new HashSet<string>(StringComparer.Ordinal);

        bool ScalarPresent(string f) => values.ContainsKey(f) || pValues.ContainsKey(f) || sValues.ContainsKey(f);
        bool isOffhand = text.TryGetValue("Class", out var itemCls) && itemCls == "WeaponArmor_Offhand";

        // Skill-store fields (skillCooldownReduction/skillManaCostReduction) sourced from an AFFIX
        // draw EARLY — at the start of the Damage store (before the item's first present non-Char
        // entry), prefix first then suffix — NOT at the deferred end-of-sequence Skill position
        // where the base record's own skill fields draw. The early value is the affix's
        // contribution, summed with the base's end-drawn part.
        bool pfxHasSkill = hasPrefix && SkillEarly.Any(pValues.ContainsKey);
        bool sfxHasSkill = hasSuffix && SkillEarly.Any(sValues.ContainsKey);
        bool skillEarlyDone = !(pfxHasSkill || sfxHasSkill);
        var skillEarly = new Dictionary<string, double>(StringComparer.Ordinal);
        void DrawSkillEarly()
        {
            if (skillEarlyDone) return;
            skillEarlyDone = true;
            if (pfxHasSkill)
                foreach (var f in SkillEarly)
                    if (pValues.TryGetValue(f, out var v))
                        skillEarly[f] = skillEarly.GetValueOrDefault(f) + Jitter.Skill(v, pfxPct, rng);
            if (sfxHasSkill)
                foreach (var f in SkillEarly)
                    if (sValues.TryGetValue(f, out var v))
                        skillEarly[f] = skillEarly.GetValueOrDefault(f) + Jitter.Skill(v, sfxPct, rng);
        }

        foreach (var e in Order)
        {
            switch (e.Kind)
            {
                case Kind.Flat:
                {
                    if (e.Field == "offensivePhysical"
                        && text.TryGetValue("Class", out var cls)
                        && cls.StartsWith("Weapon", StringComparison.Ordinal))
                        break; // weapon base physical: 0 draws, shown as base range (fixed)
                    string minF = e.Field + "Min", maxF = e.Field + "Max";
                    bool anyPresent = values.ContainsKey(minF) || values.ContainsKey(maxF)
                        || pValues.ContainsKey(minF) || pValues.ContainsKey(maxF)
                        || sValues.ContainsKey(minF) || sValues.ContainsKey(maxF);
                    if (!anyPresent) break;
                    DrawSkillEarly();
                    // The value-array pair is (min, spread) where spread = max(0, max - min).
                    // AddJitter jitters BOTH elements; the scale pass scales ONLY the min. So max is
                    // derived as jittered_min + jittered_spread, never jittered independently. For
                    // affixed items each present source (base/prefix/suffix) is its own
                    // independently-jittered (min, spread) pair using that source's own pct; the raw
                    // jittered mins are summed across sources and scale is applied ONCE to the total.
                    // The spread element is never scaled. On a caster off-hand
                    // (Class=WeaponArmor_Offhand, cannot attack) an affix's flat pair is discarded
                    // entirely (0 draws, no line).
                    string chF = e.Field + "Chance";
                    double fMin = 0.0, fSpread = 0.0;
                    bool anyDrawn = false;
                    void AccumulateFlat(Dictionary<string, double> src, double pct, bool active, bool isBase)
                    {
                        if (!active) return;
                        bool hasMin = src.ContainsKey(minF), hasMax = src.ContainsKey(maxF);
                        if (!hasMin && !hasMax) return;
                        if (!isBase && isOffhand) return; // off-hand: affix flat destroyed, no draw
                        double mn = src.GetValueOrDefault(minF, 0.0), mx = src.GetValueOrDefault(maxF, 0.0);
                        double jMn = Jitter.Char(mn, pct, rng);
                        double jSpread = Jitter.Char(Math.Max(0.0, mx - mn), pct, rng);
                        // The chance-bearing-source-is-its-own-line split only applies in an
                        // affix-merge context (hasPrefix||hasSuffix) — a base-only item's own
                        // per-field Chance is just its group's displayed chance (0 draws, no merge
                        // risk since there's only ever one source).
                        if ((hasPrefix || hasSuffix) && src.TryGetValue(chF, out var chance))
                        {
                            double sMin = Jitter.ApplyScale(jMn, sp);
                            double sMax = Math.Truncate(sMin + jSpread);
                            sMin = Math.Truncate(sMin);
                            procLines.Add(new ProcLine(e.Field, sMin, sMax == sMin ? null : sMax, null, chance));
                            return;
                        }
                        anyDrawn = true;
                        fMin += jMn;
                        fSpread += jSpread;
                    }
                    AccumulateFlat(values, BaseJitterPercent, true, true);
                    AccumulateFlat(pValues, pfxPct, hasPrefix, false);
                    AccumulateFlat(sValues, sfxPct, hasSuffix, false);
                    if (!anyDrawn) break;
                    fMin = Jitter.ApplyScale(fMin, sp);
                    result[minF] = Math.Truncate(fMin);
                    result[maxF] = Math.Truncate(fMin + fSpread);
                    break;
                }
                case Kind.SlowFlat:
                {
                    // Per-source sum-then-scale, same mechanism as Flat above; the display total is
                    // scale(sum of jittered mins) * DurationMin. DurationMin stays fixed (echoed via
                    // the fixed-field pass below). Off-hand focus items (Class=WeaponArmor_Offhand)
                    // do not draw or display their own SlowFlat pair at all, even when the row
                    // carries it — gated on Class, same as the Flat case.
                    if (isOffhand) break;
                    string minF = e.Field + "Min", maxF = e.Field + "Max", durF = e.Field + "DurationMin";
                    // A base Min with no DurationMin at all can't render "over N Seconds": 0 draws,
                    // not displayed.
                    if (values.ContainsKey(minF) && !values.ContainsKey(durF)) break;
                    bool anyPresent = values.ContainsKey(minF) || values.ContainsKey(maxF)
                        || pValues.ContainsKey(minF) || pValues.ContainsKey(maxF)
                        || sValues.ContainsKey(minF) || sValues.ContainsKey(maxF);
                    if (!anyPresent) break;
                    DrawSkillEarly();
                    string slowChF = e.Field + "Chance";
                    double fMin = 0.0, fSpread = 0.0;
                    bool anyDrawn = false;
                    void AccumulateSlowFlat(Dictionary<string, double> src, double pct)
                    {
                        bool hasMin = src.ContainsKey(minF), hasMax = src.ContainsKey(maxF);
                        if (!hasMin && !hasMax) return;
                        double mn = src.GetValueOrDefault(minF, 0.0), mx = src.GetValueOrDefault(maxF, 0.0);
                        double jMn = Jitter.Char(mn, pct, rng);
                        double jSpread = Jitter.Char(Math.Max(0.0, mx - mn), pct, rng);
                        // A source carrying its own {Field}Chance is a separate proc DoT line (own
                        // scale, own DurationMin), NOT summed into the merged non-chance bucket —
                        // same split as the Flat case above; only in an affix-merge context.
                        if ((hasPrefix || hasSuffix) && src.TryGetValue(slowChF, out var chance))
                        {
                            double sMin = Jitter.ApplyScale(jMn, sp);
                            double sMax = Math.Truncate(sMin + jSpread);
                            sMin = Math.Truncate(sMin);
                            double? srcDur = src.TryGetValue(durF, out var d) ? d : null;
                            procLines.Add(new ProcLine(e.Field, sMin, sMax == sMin ? null : sMax, srcDur, chance));
                            return;
                        }
                        anyDrawn = true;
                        fMin += jMn;
                        fSpread += jSpread;
                    }
                    AccumulateSlowFlat(values, BaseJitterPercent);
                    if (hasPrefix) AccumulateSlowFlat(pValues, pfxPct);
                    if (hasSuffix) AccumulateSlowFlat(sValues, sfxPct);
                    if (!anyDrawn) break;
                    fMin = Jitter.ApplyScale(fMin, sp);
                    result[minF] = Math.Truncate(fMin);
                    result[maxF] = Math.Truncate(fMin + fSpread);
                    break;
                }
                case Kind.RetalFlat:
                {
                    string minF = e.Field + "Min", maxF = e.Field + "Max";
                    bool anyPresent = values.ContainsKey(minF) || values.ContainsKey(maxF)
                        || pValues.ContainsKey(minF) || pValues.ContainsKey(maxF)
                        || sValues.ContainsKey(minF) || sValues.ContainsKey(maxF);
                    if (!anyPresent) break;
                    DrawSkillEarly();
                    // Same (min, spread) pair mechanism as Flat above, but retaliation never scales.
                    // Affix sources summed component-wise the same way as Flat.
                    double rMin = 0.0, rSpread = 0.0;
                    void AccumulateRetal(Dictionary<string, double> src, double pct, bool active, bool isBase)
                    {
                        if (!active) return;
                        bool hasMin = src.ContainsKey(minF), hasMax = src.ContainsKey(maxF);
                        if (!hasMin && !hasMax) return;
                        double mn = src.GetValueOrDefault(minF, 0.0), mx = src.GetValueOrDefault(maxF, 0.0);
                        rMin += Jitter.Char(mn, pct, rng);
                        rSpread += Jitter.Char(Math.Max(0.0, mx - mn), pct, rng);
                    }
                    AccumulateRetal(values, BaseJitterPercent, true, true);
                    AccumulateRetal(pValues, pfxPct, hasPrefix, false);
                    AccumulateRetal(sValues, sfxPct, hasSuffix, false);
                    result[minF] = Math.Truncate(rMin);
                    result[maxF] = Math.Truncate(rMin + rSpread);
                    break;
                }
                case Kind.RetalDur:
                case Kind.RetalReflex:
                case Kind.OffReflex:
                {
                    // Affix-sourced retaliation Dur/Reflex and offensive-CC pairs draw at this same
                    // position, each source with its own pct, contributions summed; DurationMin/
                    // Chance stay fixed (taken from whichever source has them, base first). OffReflex
                    // is mechanically identical to RetalReflex: Min = seconds, one jittered draw (no
                    // scale), Chance fixed.
                    string minF = e.Field + "Min";
                    bool present = values.ContainsKey(minF) || pValues.ContainsKey(minF) || sValues.ContainsKey(minF);
                    if (!present) break;
                    DrawSkillEarly();
                    double tot = 0.0;
                    if (values.TryGetValue(minF, out var bmn)) tot += Jitter.Char(bmn, BaseJitterPercent, rng);
                    if (hasPrefix && pValues.TryGetValue(minF, out var pmn)) tot += Jitter.Char(pmn, pfxPct, rng);
                    if (hasSuffix && sValues.TryGetValue(minF, out var smn)) tot += Jitter.Char(smn, sfxPct, rng);

                    result[minF] = e.Kind == Kind.RetalReflex ? tot : Math.Round(tot, MidpointRounding.AwayFromZero);
                    foreach (var comp in e.Kind == Kind.RetalDur ? new[] { "DurationMin", "Chance" } : new[] { "Chance" })
                    {
                        string cf = e.Field + comp;
                        if (values.TryGetValue(cf, out var cv0)) result[cf] = cv0;           // fixed, 0 draws
                        else if (pValues.TryGetValue(cf, out var cv1)) result[cf] = cv1;
                        else if (sValues.TryGetValue(cf, out var cv2)) result[cf] = cv2;
                    }
                    break;
                }
                case Kind.Conv:
                {
                    // Each source's conversionPercentage is jittered separately (base with the
                    // default 20%, an affix with its own lootRandomizerJitter), drawing in
                    // base → prefix → suffix order; contributions with the same In/Out type pair
                    // sum for display.
                    string sfx = e.Field.EndsWith("2", StringComparison.Ordinal) ? "2" : "";
                    string inKey = "conversionInType" + sfx, outKey = "conversionOutType" + sfx;
                    var acc = new Dictionary<(string, string), double>();
                    var accOrder = new List<(string, string)>();
                    void AccumulateConv(Dictionary<string, double> src, Dictionary<string, string> srcText, double pct, bool active)
                    {
                        if (!active) return;
                        double v = src.GetValueOrDefault(e.Field, 0.0);
                        srcText.TryGetValue(inKey, out var inType);
                        srcText.TryGetValue(outKey, out var outType);
                        if (string.IsNullOrEmpty(inType) || v == 0.0) return; // invalid: destroyed, no draw
                        DrawSkillEarly();
                        var key = (inType!, outType ?? "");
                        if (!acc.ContainsKey(key)) accOrder.Add(key);
                        acc[key] = acc.GetValueOrDefault(key) + Jitter.Conversion(v, pct, rng);
                    }
                    AccumulateConv(values, text, BaseJitterPercent, true);
                    AccumulateConv(pValues, pText, pfxPct, hasPrefix);
                    AccumulateConv(sValues, sText, sfxPct, hasSuffix);
                    if (accOrder.Count == 0) break;
                    result[e.Field] = acc[accOrder[0]];
                    if (accOrder.Count > 1)
                        affixPairFields.Add(e.Field + " (multiple distinct conversion type pairs)");
                    break;
                }
                case Kind.Leech:
                {
                    // offensiveLifeLeech: per-source jitter (own pct), summed, no scale, Min-only.
                    // Damage store is base-first (base then prefix then suffix).
                    string minF = e.Field + "Min";
                    if (!(values.ContainsKey(minF) || pValues.ContainsKey(minF) || sValues.ContainsKey(minF))) break;
                    DrawSkillEarly();
                    double tot = 0.0;
                    if (values.TryGetValue(minF, out var blv)) tot += Jitter.Char(blv, BaseJitterPercent, rng);
                    if (hasPrefix && pValues.TryGetValue(minF, out var plv)) tot += Jitter.Char(plv, pfxPct, rng);
                    if (hasSuffix && sValues.TryGetValue(minF, out var slv)) tot += Jitter.Char(slv, sfxPct, rng);
                    result[minF] = tot;   // no scale
                    break;
                }
                case Kind.OffSlow:
                {
                    // Offensive speed-slow / ability-reduction debuff: per-source jitter (own pct),
                    // summed; speed slows scale (e.Scales, applied once to the summed value), ability
                    // reductions do not. DurationMin/Chance fixed (0 draws, echoed from the first
                    // source that has them, base first).
                    string minF = e.Field + "Min";
                    if (!(values.ContainsKey(minF) || pValues.ContainsKey(minF) || sValues.ContainsKey(minF))) break;
                    DrawSkillEarly();
                    double tot = 0.0;
                    if (values.TryGetValue(minF, out var bsv)) tot += Jitter.Char(bsv, BaseJitterPercent, rng);
                    if (hasPrefix && pValues.TryGetValue(minF, out var psv)) tot += Jitter.Char(psv, pfxPct, rng);
                    if (hasSuffix && sValues.TryGetValue(minF, out var ssv)) tot += Jitter.Char(ssv, sfxPct, rng);
                    result[minF] = e.Scales ? Jitter.ApplyScale(tot, sp) : Math.Round(tot, MidpointRounding.AwayFromZero);
                    foreach (var comp in new[] { "DurationMin", "Chance" })
                    {
                        string cf = e.Field + comp;
                        if (values.TryGetValue(cf, out var cv0)) result[cf] = cv0;           // fixed, 0 draws
                        else if (pValues.TryGetValue(cf, out var cv1)) result[cf] = cv1;
                        else if (sValues.TryGetValue(cf, out var cv2)) result[cf] = cv2;
                    }
                    break;
                }
                case Kind.OffReduc:
                {
                    // Offensive reduction debuff: per-source jitter (own pct), summed, no scale;
                    // DurationMin fixed (0 draws, echoed from the first source that has it, base first).
                    string minF = e.Field + "Min", durF = e.Field + "DurationMin";
                    if (!(values.ContainsKey(minF) || pValues.ContainsKey(minF) || sValues.ContainsKey(minF))) break;
                    DrawSkillEarly();
                    double tot = 0.0;
                    if (values.TryGetValue(minF, out var brv)) tot += Jitter.Char(brv, BaseJitterPercent, rng);
                    if (hasPrefix && pValues.TryGetValue(minF, out var prv)) tot += Jitter.Char(prv, pfxPct, rng);
                    if (hasSuffix && sValues.TryGetValue(minF, out var srv)) tot += Jitter.Char(srv, sfxPct, rng);
                    result[minF] = tot;   // no scale
                    if (values.TryGetValue(durF, out var dv0)) result[durF] = dv0;         // fixed, 0 draws
                    else if (pValues.TryGetValue(durF, out var dv1)) result[durF] = dv1;
                    else if (sValues.TryGetValue(durF, out var dv2)) result[durF] = dv2;
                    break;
                }
                default:
                {
                    // Scalar kinds (Char/Dmg/Def/RetalMod/Skill): each source has its own value and
                    // own jitter pct, then summed. With no prefix/suffix supplied this reduces to the
                    // base-only behavior exactly (pv/sv are 0, and Jitter.* is a no-draw for a 0
                    // value). Draw order differs by store: Char/Skill/RetalMod draw
                    // Prefix → Suffix → Base (base last); Dmg and Def draw Base first, then Prefix,
                    // then Suffix.
                    if (!ScalarPresent(e.Field)) break;
                    if (e.Kind != Kind.Char) DrawSkillEarly();
                    double pv = pValues.GetValueOrDefault(e.Field, 0.0);
                    double sv = sValues.GetValueOrDefault(e.Field, 0.0);
                    double bv = values.GetValueOrDefault(e.Field, 0.0);
                    if (e.Kind == Kind.Dmg && handledDur.Contains(e.Field))
                        break; // already drawn as part of its Slow{X}Modifier pair below
                    if (e.Kind == Kind.Dmg
                        && e.Field.StartsWith("offensiveSlow", StringComparison.Ordinal)
                        && e.Field.EndsWith("Modifier", StringComparison.Ordinal)
                        && !e.Field.EndsWith("DurationModifier", StringComparison.Ordinal))
                    {
                        string durF = e.Field[..^"Modifier".Length] + "DurationModifier";
                        if (ScalarPresent(durF))
                        {
                            // Slow{X}Modifier + Slow{X}DurationModifier live in one object per source,
                            // so each source draws its (value, duration) PAIR consecutively —
                            // base(v,d), prefix(v,d), suffix(v,d) — not all values then all durations.
                            double vTot = 0.0, dTot = 0.0;
                            void AccumulatePair(Dictionary<string, double> src, double pct, bool active)
                            {
                                if (!active) return;
                                vTot += Jitter.Char(src.GetValueOrDefault(e.Field, 0.0), pct, rng);
                                dTot += Jitter.Char(src.GetValueOrDefault(durF, 0.0), pct, rng);
                            }
                            AccumulatePair(values, BaseJitterPercent, true);
                            AccumulatePair(pValues, pfxPct, hasPrefix);
                            AccumulatePair(sValues, sfxPct, hasSuffix);
                            result[e.Field] = e.Scales ? Jitter.ApplyScale(vTot, sp) : vTot;
                            result[durF] = Jitter.ApplyScale(dTot, sp);
                            handledDur.Add(durF);
                            break;
                        }
                    }
                    double pj, sj, bj;
                    if (e.Kind == Kind.Skill)
                    {
                        bool early = SkillEarly.Contains(e.Field) && (pfxHasSkill || sfxHasSkill);
                        if (early)
                        {
                            // Affix contributions were drawn early (see DrawSkillEarly); only the
                            // base record's own part draws here at the deferred Skill position.
                            bj = Jitter.Skill(bv, BaseJitterPercent, rng);
                            result[e.Field] = skillEarly.GetValueOrDefault(e.Field) + bj;
                            break;
                        }
                        pj = hasPrefix ? Jitter.Skill(pv, pfxPct, rng) : 0.0;
                        sj = hasSuffix ? Jitter.Skill(sv, sfxPct, rng) : 0.0;
                        bj = Jitter.Skill(bv, BaseJitterPercent, rng);
                    }
                    else if (e.Kind is Kind.Dmg or Kind.Def or Kind.RetalMod)
                    {
                        bj = Jitter.Char(bv, BaseJitterPercent, rng);
                        pj = hasPrefix ? Jitter.Char(pv, pfxPct, rng) : 0.0;
                        sj = hasSuffix ? Jitter.Char(sv, sfxPct, rng) : 0.0;
                    }
                    else
                    {
                        pj = hasPrefix ? Jitter.Char(pv, pfxPct, rng) : 0.0;
                        sj = hasSuffix ? Jitter.Char(sv, sfxPct, rng) : 0.0;
                        bj = Jitter.Char(bv, BaseJitterPercent, rng);
                    }
                    if (ModifierChanceSplitFields.Contains(e.Field) && (hasPrefix || hasSuffix))
                    {
                        // Chance split: draws already happened above (base/prefix/suffix, order
                        // unaffected) — a source carrying its own {Field}Chance is a separate proc
                        // line, NOT summed into the merged non-chance total.
                        string chF = e.Field + "Chance";
                        double tot = 0.0; bool any = false;
                        foreach (var (src, j) in new[] { (values, bj), (pValues, pj), (sValues, sj) })
                        {
                            if (!src.ContainsKey(e.Field)) continue;
                            if (src.TryGetValue(chF, out var chance) && chance > 0)
                                procLines.Add(new ProcLine(e.Field, e.Scales ? Jitter.ApplyScale(j, sp) : j, null, null, chance));
                            else { any = true; tot += j; }
                        }
                        if (any) result[e.Field] = e.Scales ? Jitter.ApplyScale(tot, sp) : tot;
                        break;
                    }
                    double total = pj + sj + bj;
                    result[e.Field] = e.Scales ? Jitter.ApplyScale(total, sp) : total;
                    break;
                }
            }
        }

        // Echo fixed (zero-draw) stat fields at their base/prefix/suffix value (base wins if more
        // than one source defines the same fixed field — summing fixed fields across sources is not
        // established, so this is a conservative best-effort choice).
        var unmodeled = new List<string>();
        var allFields = new HashSet<string>(values.Keys, StringComparer.Ordinal);
        allFields.UnionWith(pValues.Keys);
        allFields.UnionWith(sValues.Keys);
        foreach (var field in allFields)
        {
            if (result.ContainsKey(field)) continue;
            if (IsFixed(field) && StatPrefixes.Any(p => field.StartsWith(p, StringComparison.Ordinal)))
            {
                if (values.TryGetValue(field, out var bval)) result[field] = bval;
                else if (pValues.TryGetValue(field, out var pval)) result[field] = pval;
                else if (sValues.TryGetValue(field, out var sval)) result[field] = sval;
            }
            else if (IsConcerning(field))
                unmodeled.Add(field);
        }
        foreach (var f in affixPairFields) unmodeled.Add(f + " [affix pair field, not modeled]");
        unmodeled.Sort(StringComparer.Ordinal);

        return new Result(result, unmodeled, procLines);
    }

    private static (Dictionary<string, double> Values, Dictionary<string, string> Text) ParseStats(IEnumerable<InputStat> stats)
    {
        var values = new Dictionary<string, double>(StringComparer.Ordinal);
        var text = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var s in stats)
        {
            values[s.Stat] = s.Value;
            if (!string.IsNullOrEmpty(s.TextValue)) text[s.Stat] = s.TextValue;
        }
        return (values, text);
    }
}
