## Session update (July 7 2026, NEWEST #7 — affix-sourced offensiveSlow{Type}Min/Max flat DoT pair modeled: same per-source sum-then-scale mechanism as plain Flat; both remaining KnownGapItems (plaguebearer-blunderbuss, salazars-blade-dagger) now pass; single-affix 1061/1063->1154/1156, both-affix 388/388->405/405, 61/61 dotnet test green, KnownGapItems set now EMPTY)

Picked up the standing "affix-sourced Slow flat, not modeled" gap flagged by session "#6"'s
predecessor. Draw-aligned exactly on the two fixture items that carry it:
`plaguebearer-blunderbuss` (prefix `b_wpn004_melee2h_c` carries `offensiveSlowPoisonMin=14` +
`DurationMin=5`, no base/suffix Min; pfx pct=18, item sp=52 — `scale(cjit(14,pct=18))*5=105`
matches the real "105 Poison Damage over 5 Seconds" exactly) and `salazars-blade-dagger` (prefix
`b_wpn102...` carries `offensiveSlowLightningMin=10`+`DurationMin=3`; pfx pct=15, sp=38 —
`scale(cjit(10,pct=15))*3=36` matches "36 Electrocute Damage over 3 Seconds" exactly). Both confirm
the field uses the **identical per-source sum-then-scale mechanism already proven for the plain
`Flat` kind** (each present source's own (Min,Max) pair jittered independently with that source's
own pct, raw mins/spreads summed across sources, scale applied ONCE to the total) — no new formula
needed, just wiring it up. `DurationMin` stays FIXED (0 draws, echoed base-first), unchanged.

**Change**: removed the affix-loop's skip-guard for `offensiveSlow{Type}Min/Max` in
`gdvalidate.py` (previously always `a_skipped+=1; continue`d), added `'slowflat'` to the
Min/Max-presence entries-builder alongside `'flat'`/`'retalflat'`, and added a `kind=='slowflat'`
dispatch mirroring `'flat'`'s per-source accumulate-then-scale-once logic (spread never scaled,
duration taken from whichever source has it, base first) plus rendering via the existing
`slowflat_line()`. **Result**: single-affix **1061/1063 → 1154/1156** (+93, previously-skipped
items unlocked for free by the shared `concerning()` check), both-affix **388/388 → 405/405**
(+17), both still ~100% (2 residual single-affix mismatches, both the pre-existing unrelated
`offensivePhysicalModifier` gap on `b_wpn102_melee1h_a`-prefixed items, off by ~2, untouched by
this change). Base-only corpus unaffected (this was purely an affix-loop gap).

**C# parity**: `ItemStatEngine.cs`'s `Kind.SlowFlat` case (previously: if an affix touched the
field, flag `"(affix-sourced Slow flat, not modeled)"` and desync every later draw) replaced with
the same per-source accumulate/sum/scale-once logic as `Kind.Flat` (a local `AccumulateSlowFlat`
closure, base→prefix→suffix order). `ItemCorpusTests.cs`'s `KnownGapItems` set — which held exactly
these two items — is now **empty** (kept as an empty `HashSet` rather than removed outright, since
the test's `Assert.True(failures.Count>0, ...)` "flip when it starts passing" scaffolding is cheap
insurance for a future regression). **`dotnet test`: 61/61 pass, 0 skipped**, zero regressions.
Committed to the GrimDawnSeedStats repo (branch `more-work`, 1 commit).

**Remaining open** (unchanged, both minor pre-existing residuals, not chased this session): 2-3
items around `d206_ring` (multi-field desync, same shape as `d008_necklace`/`c027_focus`/
`c024_waist`) and the `b_wpn102_melee1h_a` prefix's `offensivePhysicalModifier` off-by-~2 pair
(`a05_gun1h001`, `a02_blunt002`). Both need more DB samples to isolate, same playbook as every
other field family solved this project (draw-align on the raw seed stream, not guess-and-check).

## Session update (July 7 2026, #6 — defensiveProtectionModifier ("Increases Armor by N%") field modeled and validated: draws first in the Defense store, right after defensiveBlockModifier/AmountModifier; standard cjit pct=20, unscaled; ported Python+C#; corpus base-only 1153/1169->1199/1216, single-affix 1022->1061/1063, both-affix 358->388/388; outlaw-murderer-cowl KnownGapItem removed, 61/61 dotnet test still green)

Picked up the last of the 3 `KnownGapItems` from session "#5" below: `defensiveProtectionModifier`
was completely absent from every field table in both `gdvalidate.py` and `ItemStatEngine.cs`. User
supplied a targeted SQL query (`DatabaseItemStat_v2` join `PlayerItem` on `Stat='defensiveProtectionModifier'`)
finding 133 real DB rows carrying the field, several of them base-only (no prefix/suffix), enough to
draw-align directly.

**Pinned via draw alignment on 5 base-only samples** (`c015_ring` seed 816311, `c018_necklace` seed
1341859, `c014_hands` seed 645919, `c035_hands` seed 411260, `c011_shield`/`c015_blunt` seeds
429169/223932/982001): brute-forced every permutation of the field against its neighboring DEF-store
fields (and, for the shield/blunt samples, against the already-existing `defensiveBlockModifier` field)
until all fields land on their exact real tooltip value simultaneously, not just "in range". Result:
`defensiveProtectionModifier` is a plain `cjit`-jittered field (default pct=20, NOT scaled by
`attributeScalePercent` — same as every other DEF field), and it draws **right after
defensiveBlockModifier/defensiveBlockAmountModifier, before every resist field** (defensivePhysical/
Pierce/.../TotalSpeedResistance). Displayed as `"Increases Armor by {v}%"`. One false lead ruled out
along the way on the shield samples: an apparent block-vs-protection order ambiguity turned out to be
a pre-existing missed draw (`retaliationTotalDamageModifier`, which the model already draws in
RETAL_MOD before Defense but which my hand-rolled verification scripts initially omitted) — once that
draw was included, both orderings produced identical values for that specific seed, and the real
discriminator (block first) came from the C# engine's existing comment ("confirmed empirically") plus
a clean fix on `c029_shield`/`d206_ring`-style residuals once placed there.

**Result** (`tools/gdvalidate.py`, `DEF` list + `fmt()` template dict updated — one line each):
- Base-only: fully-modeled 1169→**1216** (+47, previously-skipped items), items-all-correct
  1153/1169→**1199/1216** (98.6%→98.6%, net +46 passing). Two NEW residual mismatches on
  previously-untested items (`c029_shield` — actually the RETAL_MOD-before-Defense draw already
  covered it once wired in, no longer mismatching after the block-then-protection order; `d206_ring`
  — a pre-existing multi-field desync unrelated to this field, same "several fields wrong
  simultaneously" shape as the already-documented `d008_necklace`/`c027_focus`/`c024_waist` residuals,
  not chased further).
- Single-affix: 1022/1022→**1061/1063** (99.8%), both-affix: 358/358→**388/388 (100%)** — the affix
  loop's shared `concerning()` picked this up for free (the field was previously flagging every
  affix-corpus item that touched it as unmodeled/skipped, hiding 71 more items). Two NEW residual
  mismatches (`a05_gun1h001`+`b_wpn102_melee1h_a` seed 1225981, `a02_blunt002`+`b_wpn102_melee1h_a`
  seed 884789), both `offensivePhysicalModifier` off by ~2 on the SAME prefix — a distinct,
  pre-existing gap on that one prefix record, not a defensiveProtectionModifier regression.

**C# parity**: `ItemStatEngine.cs`'s `Def` array got `"defensiveProtectionModifier"` inserted right
after `"defensiveBlockAmountModifier"`; `TooltipFormatter.cs`'s template dict got
`["defensiveProtectionModifier"] = v => $"Increases Armor by {v}%"`. Since `Modeled`/`IsConcerning`
are both built from the same field-list arrays, no other C# change was needed. `outlaw-murderer-cowl`
(the `KnownGapItems` entry that existed specifically for this field, per session "#5") now passes and
was removed from the known-gap set. **`dotnet test`: 61/61 pass, 0 skipped** (same total as before —
one test flipped from known-fail to pass, not a new test).

**Remaining open** (unchanged from "#5", now down to 1 item pair): `plaguebearer-blunderbuss` /
`salazars-blade-dagger` — affix-sourced `offensiveSlow{Type}Min/Max` flat DoT pair, still needs 2-3
DB samples isolating an affix-only SlowFlat contribution to pin the per-source draw formula. The two
newly-surfaced residuals above (`d206_ring`, the `b_wpn102_melee1h_a` prefix pair) are candidates for
a future "more DB samples" session but are minor (2-3 items each) compared to the win banked here.

## Session update (July 7 2026, NEWEST #5 — affixed-item corpus wired into a real xUnit Theory: TooltipFormatter.cs ported from gdvalidate.py's fmt()/*_line() helpers, items.json extended with embedded baseStats/prefixStats/suffixStats, AffixedItems_ComputationPending replaced with a per-item assertion; 61/61 dotnet test green, up from 47 passed+1 skipped)

Picked up the standing TODO on `AffixedItems_ComputationPending` (the `[Fact(Skip=...)]` placeholder
in `ItemCorpusTests.cs`): the replay pipeline itself (`ItemStatEngine.Compute`) was already complete
and 100%-corpus-validated in the Python oracle, but nothing rendered its numeric output into tooltip
TEXT to compare against `Fixtures/items.json`'s `expectedTooltip` strings, so the 14 affixed items
sat unchecked.

**What was built:**
- `src/GrimDawnSeedStats/TooltipFormatter.cs` (new): ports `gdvalidate.py`'s `fmt()` (scalar field →
  tooltip line, via regex-matched field-name families: retaliation modifiers, offensive %-modifiers,
  Slow{X}Duration/Slow{X} lines, defensive resistances, plus a template dict for the ~25 one-off Char/
  Skill/Defense fields) and its five pair-shaped `*_line()` helpers (`FlatLine`, `RetalLine`,
  `SlowFlatLine`, `RetalDurLine`, `RetalReflexLine`, `ConversionLine`) 1:1, including the DmgName/
  SlowName/ResName/ConvName display-name tables (Poison→Acid, Life→Vitality, Pierce→Piercing for flat
  lines but Pierce→Pierce for %-modifier lines, etc). Bug caught during porting (not present in the
  Python source, which never hit this shape): min==max flat/retal/slowflat pairs need to collapse to a
  single-value line ("N Type Damage"), not "N-N Type Damage" — fixed in `FlatLine`/`RetalLine`/
  `SlowFlatLine`.
- `tests/GrimDawnSeedStats.Tests/Fixtures/items.json`: extended every one of the 15 fixture items with
  `baseStats`/`prefixStats`/`suffixStats` arrays (raw `{stat, textValue, value}` triples — the exact
  `DatabaseItemStat_v2` row shape `ItemStatEngine.InputStat` expects), dumped directly from
  `userdata-test.db` for all 39 distinct base/prefix/suffix `.dbr` records the corpus references (all
  39 found, zero misses).
- `tests/GrimDawnSeedStats.Tests/ItemFixture.cs`: added the `RawStat` DTO and `BaseStats`/
  `PrefixStats`/`SuffixStats` properties plus `ToBaseInputStats()`/`ToPrefixInputStats()`/
  `ToSuffixInputStats()` convenience converters to `ItemStatEngine.InputStat`.
- `tests/GrimDawnSeedStats.Tests/ItemCorpusTests.cs`: replaced the skip placeholder with
  `AffixedItem_ComputesExpectedTooltipLines`, a real `[Theory]` over `AffixedItems()` that calls
  `ItemStatEngine.Compute` with the embedded base+prefix+suffix stats, renders every implied tooltip
  line (`RenderLines()` dispatches computed `Result.Stats` entries by field shape — scalar via
  `TooltipFormatter.Format`, `{prefix}Min/Max` pairs via the *Line() helpers, matching gdvalidate.py's
  own dispatch), and asserts each rendered line's candidate phrasing(s) appear as a substring of the
  joined real `expectedTooltip` (same "any candidate is a substring match" pass criterion the Python
  oracle uses).

**Result: 11/14 affixed items pass exactly out of the box (the corpus has 15 items total, 1 of which
is affix-free and covered separately by `AffixFreeItem_ReplaysToInGameValues`). 3 hit genuine,
PRE-EXISTING modeling gaps
shared with the Python oracle** (not new bugs introduced by the formatter/test — confirmed by
grepping `gdvalidate.py`, which has no handling for either gap either):
- `plaguebearer-blunderbuss`, `salazars-blade-dagger`: an AFFIX (prefix or suffix) touches an
  `offensiveSlow{Type}Min/Max` flat-DoT field. `ItemStatEngine`'s `SlowFlat` case only implements the
  base-only single-source draw; no sample has confirmed the affix-side per-source draw formula yet
  (flagged into `UnmodeledFields` as `"... (affix-sourced Slow flat, not modeled)"`), so the RNG
  stream desyncs for every draw after it, producing wrong downstream values.
- `outlaw-murderer-cowl`: its suffix carries `defensiveProtectionModifier` ("Increases Armor by N%"),
  a field absent from EVERY field table in both `gdvalidate.py` and `ItemStatEngine.cs` (not Fixed,
  not Def, not anywhere) — a real, undrawn RNG consumption that desyncs the suffix's Pierce/Fire/
  Lightning resist draws after it.

These 3 are marked in a `KnownGapItems` set and asserted as "currently still mismatching" (fails the
test loudly if they start passing without the set being updated, rather than silently skipping) —
per-item detail and rationale is in the `AffixedItem_ComputesExpectedTooltipLines` XML doc comment.
**`dotnet test` (run directly against `tests/GrimDawnSeedStats.Tests/GrimDawnSeedStats.Tests.csproj`,
since `src/GrimDawnSeedStats.Cli/Program.cs` has a pre-existing, unrelated build break — confirmed
zero git diff on that file, not touched this session) is 61/61 passed, 0 failed, 0 skipped** (up from
47 passed + 1 skipped = 48 total before this session).

**Next step for either gap**: needs 2-3 more DB samples isolating each field family (an item where
ONLY an affix-sourced SlowFlat is present, or ONLY `defensiveProtectionModifier`) to pin the exact
draw position/formula the same way every other field family in this project was pinned — not
guessable from the 3 mismatching items alone since each has several other draws happening around the
gap.

## Session update (July 7 2026, #4 — Ghidra follow-up on the removed quirk records: confirmed there is NO engine override mechanism, root cause is stale/wrong data in userdata-test.db itself for those ~8 records, not a missed game rule)

Per user follow-up to "#3" below (which removed the hardcoded quirk tables): searched for a
GENERIC, non-record-keyed explanation before accepting the 8 quirk records as simply unmodeled.
Checked two candidate generic signals across all 8 records' own metadata fields
(`itemNameTag`/`mesh`/`bitmap`): whether the embedded item-number in those fields matches the
record's own filename (a sign of leftover copy-paste data). Only 2 of 8 show a mismatch
(`c025_shield.dbr`'s tag/bitmap literally say "C007"; `d110_focus.dbr`'s say "D105"/`d105_focus`)
— not a signal that generalizes to the other 6, and even where present it only flags "this record's
data might be stale," not a formula for the correct value. Also re-checked the `itemSetName`
angle (a sibling set record's own copy of a field overriding the equipped item's copy) — fits only
1 of 8 (`c016_axe2h`, already known from a prior session) and was already refuted for `d105_blunt`.

**Decompiled `CharAttribute::LoadBaseTable` (0x1800b77e0)** — the single generic per-field loader
shared by EVERY `char`-kind attribute (`characterLife`, `characterSpellCastSpeedModifier`, etc.):
it does a raw by-name lookup into the `LoadTable` (`vt+0x88`) and one trivial zero-value cleanup —
nothing else. No per-Class branch, no clamp/fallback, no set/upgrade-record cross-reference. This
matches the shape of every other loader already decompiled in prior sessions
(`DamageAttributeAbs::LoadFromTable`, `AttributePak::LoadFromTable`, etc.) — all of them are bare
reads. **There is no mechanism anywhere in the compiled engine that could make one specific
record's field resolve to a value different from what's literally stored for that record.**

**Conclusion**: the ~8 previously-quirked records' `DatabaseItemStat_v2.val1` simply does not match
what the live/shipped game actually uses for that exact DBR file — this is a **stale or incorrect
row in this specific `userdata-test.db` snapshot**, not an unmodeled game mechanic. Confirmed this
isn't expressible as a formula either: the 4 `BASE_OVERRIDE_QUIRKS` ratios don't share a factor
(200→150 and 402→302 are both ×0.75, but 15→20 is ×1.33 and 320→860 is ×2.69) — if it were a real
in-game rule (e.g. a percentage reduction applied uniformly to some record category) the ratios
would match; they don't, which is further evidence this is a data-quality artifact per-file, not a
rule to encode.

**Decision**: leave these records unmodeled (no quirk table, no generic detector) — there is no
generic, checkable condition to gate a fix on, and no formula to apply even if there were. If the
user later re-captures `userdata-test.db` and any of these records still mismatch, that would be
the point to revisit (would rule out "stale snapshot" and reopen the question); until then this is
closed as "data-quality, not a modeling gap." No code changes this session (investigation only) —
`gdvalidate.py`/`ItemStatEngine.cs` remain as left by session "#3" below (quirk tables removed).

## Session update (July 7 2026, #3 — user directive: NO per-record hardcoded quirk tables, anywhere; removed NODRAW_QUIRKS/BASE_OVERRIDE_QUIRKS/SINGLE_SIDED_QUIRKS from gdvalidate.py entirely (kept as no-op stubs); a C#-side port of the same tables (added earlier this session, see "#2" below) was reverted before landing)

**Explicit user instruction: hardcoding behavior to a specific DBR record path is not acceptable,
full stop** — every item follows the SAME engine rules; if a handful of records need a different
base value or draw count to validate, that means the general rule hasn't been found yet, not that
those specific records are exceptions. This directly contradicts the `NODRAW_QUIRKS`/
`BASE_OVERRIDE_QUIRKS`/`SINGLE_SIDED_QUIRKS` tables that had accumulated in `gdvalidate.py` over
several prior sessions (7 (record,field) entries total: `c019_necklace`, `d008_necklace`,
`c025_shield`, upgraded `c024_waist`, `c009_shield` ×2, `c016_axe2h`, `d105_blunt`, `d110_focus`)
— each was individually "confirmed" by brute-force grid search fitting every sampled seed for that
one record exactly, but NONE had a Ghidra-level explanation for why that specific record differs.

**Action taken**: removed all three dictionaries from `gdvalidate.py`; `quirk_skip`/
`quirk_force_single`/`quirk_base_override` are now permanent no-op stubs (`return False`/`return
False`/`return default`) rather than being deleted outright, so the call sites throughout the
base-only and affix loops don't need touching — but they now always fall through to the
un-overridden, un-skipped behavior. **Also reverted, before it ever landed**: this same session
had ported the (already-hardcoded) Python quirk tables into `ItemStatEngine.cs` as a
`baseRecordPath`-keyed lookup (see "#2" session note directly below) — that port was undone in
full (dictionaries, helper methods, and the `baseRecordPath` parameter all removed from
`ItemStatEngine.cs`) before any commit, per the same directive.

**Corpus impact (expected, accepted regression)**: base-only items-all-correct dropped from
1167/1169 back to **1153/1169** (the ~7 records the quirks used to paper over are mismatches
again); `dotnet test` still 47/47 (none of the removed quirks were exercised by the C# fixture
corpus, confirmed via grep before the C# port was even attempted). Affix pass unaffected (still
1022/1022 single, 358/358 both, 100%) since none of the quirked records are in the affix corpus.

**Standing rule for all future sessions**: do not add any dictionary/table keyed by a literal DBR
record path to either `gdvalidate.py` or `ItemStatEngine.cs`. If a record's fields don't validate
under the current field-family model, that is an open investigation item (get more seeds for that
exact record, look for a shared DBR property across similarly-behaving records — e.g. set
membership, upgrade tier, a specific flag field), not something to patch over with a per-path
lookup table, no matter how well the override fits the available seeds.

## Session update (July 7 2026, #2 — base-only coverage push: offensiveSlow{Type}Min/Max flat-DoT family + offensivePierceRatioMin modeled; skipped 332->218, fully-modeled 1055->1169, 1167/1169 (99.8%) exact; ported to C#, 47/47 tests still pass)

Picked up the user's ask to push the BASE-ONLY (no affix) corpus toward 100%. Diagnosed the 332
previously-skipped base-only items by field: the two biggest unmodeled families were
`offensiveSlow{Type}Min` (a per-tick flat DoT value, 64-68 occurrences per type across
Bleeding/Fire/Physical/Poison/etc) and `offensivePierceRatioMin` (53 occurrences, always "100% Armor
Piercing" verbatim).

**offensivePierceRatioMin: FIXED, 0 draws** — confirmed via c020_gun2h (tooltip literally "100%
Armor Piercing" matching the DB value exactly on every sample). Added to `is_fixed()`/`IsFixed()`
as a `offensive*RatioMin` suffix rule.

**offensiveSlow{Type}Min (+ Max, DurationMin): a new "flat DoT" tier.** Draw-aligned exactly on 3
samples (c025_axe Bleeding seed 1523045, c046_head Fire seed 790734, c207_medal Physical seed
708250, all 3 fields simultaneously exact): the field draws as its OWN (min,spread) cjit pair —
same mechanism as the existing offensive Flat block — positioned right AFTER the regular Flat
block and BEFORE the entire Dmg %-modifier list, NOT interleaved next to its own
offensiveSlow{Type}Modifier sibling despite DMG listing Modifier+DurationModifier adjacently. It
IS scaled by `attributeScalePercent` (confirmed: c207_medal's `9`min/`82`Modifier both landed
exactly at `sp=40`). Display total = `scaled_min * DurationMin` (DurationMin stays FIXED, 0
draws, per existing precedent), or `scaled_min..scaled_max` range × duration if a Max/spread is
present (confirmed pair mechanism exact on c018_ring, 2 seeds, min=4/max=12 base). Both "{Type}
Damage over N Seconds" (Bleeding/Fire) and bare "{Type} over N Seconds" (Physical/"Internal
Trauma") tooltip phrasings were observed — `slowflat_line()` offers both as accepted candidates
since which phrasing applies per-type isn't pinned down yet, only that one of them always is.

**One new per-record anomaly found** (not kept as a hardcoded quirk — see the "#3" session note
above this one, which removed the whole quirk-table mechanism): upgraded `c024_waist.dbr`'s
`offensiveSlowFireMin` (12 base) appears to never draw/display at all on its 2 sampled seeds
(1149892, 90619) — keeping the draw desyncs `offensiveSlowFireModifier`/
`defensiveElementalResistance` on both, removing it makes all 6 checked fields match exactly on
both. This was briefly added to `NODRAW_QUIRKS` this session, then reverted along with the rest of
the quirk-table mechanism per user direction; it's back to being an open, unexplained mismatch on
that one record rather than a silently-patched exception.

**Result**: `gdvalidate.py` base-only: skipped 332→**218**, fully-modeled 1055→**1169**, items-all-
correct **1167/1169 (99.8%)**, per-stat OK 6267→6982 (BAD=17, all on one single record,
`c027_focus.dbr`, 2 seeds — see below). The affix pass's shared `concerning()`/`is_fixed()` picked
up the same fixes for free (PierceRatioMin), but since the affix loop's entries-builder was never
taught to draw/sum a per-source `slowflat` contribution, a guard was added to keep those specific
fields treated as still-unmodeled there (matching pre-existing behavior) rather than silently
eating the draw and desyncing every later field on an affixed item — net result affix pass actually
IMPROVED for free: single-affix 951/951→**1022/1022**, both-affix 352/352→**358/358**, still 100%.

**Residual, NOT fixed this session**: `c027_focus.dbr` (2 seeds, 1029175/856303) — 8 stats each
mismatch simultaneously (offensiveLifeModifier, offensiveSlowPoisonModifier, offensiveSlowLifeModifier,
defensiveLife, defensiveAether, skillCooldownReduction, offensiveSlowPoison flat, retaliationPoison),
including `skillCooldownReduction` appearing SWAPPED between the two seeds' real values relative to
what's computed (16↔19) and `retaliationPoisonMin`'s real total (274) being IDENTICAL on both seeds
despite base=240 — smells like the same "stale DB base value" `BASE_OVERRIDE_QUIRKS` pattern seen
before (`c016_axe2h`/`d105_blunt`/`d110_focus`/`c009_shield`) possibly combined with a genuine
draw-position issue, but not chased this session (only 2 samples, diminishing returns vs. the size
of the win already banked). Next step if resumed: get more `c027_focus.dbr`-seed samples (same
playbook as the other `BASE_OVERRIDE_QUIRKS` entries) before guessing further.

**C# parity**: ported `SlowFlat` as a new `Kind` in `ItemStatEngine.cs` (inserted in `BuildOrder`
right after `Flat`, before `Dmg`; `Modeled` set updated; `IsFixed` got the `RatioMin` suffix rule).
Since the C# engine unifies base+affix in one `Compute()` call (unlike Python's split loops), the
affix-guard is expressed as: if a prefix/suffix record touches a `Slow{X}Min/Max` field, flag it
as an unmodeled "affix-sourced Slow flat, not modeled" field instead of processing it (mirrors the
Python guard exactly). The one new per-record quirk (`c024_waist` NODRAW) was NOT ported — grepped
the C# test fixture corpus, confirmed no fixture exercises `offensiveSlow*Min`/`PierceRatioMin` at
all yet, so this is a purely additive change with no port-drift risk. `dotnet test`: still **47
pass / 1 skip**, zero regressions.

## Session update (July 7 2026, NEWEST — MATERIA'S OWN STATS CONFIRMED FIXED/NO-JITTER (user's hypothesis validated); RelicSeed column discovered; still no code changed)

User recalled that Materia stats used to have variance years ago but are now believed hardcoded/fixed.
Verified this directly against the DB and it's correct:

- **All 108 `records/items/materia/*.dbr` rows in `userdata-test.db` have ZERO `lootRandomizerJitter`
  field** (queried every one via `DatabaseItemStat_v2`) — unlike every Prefix/Suffix/base record,
  which always carries one. No jitter field present means no RNG draw is spent on the record at all.
- **Confirmed exactly on `compa_markofillusions.dbr` (PlayerItem 3326)**: its component tooltip block
  reads "+15% Elemental Damage / +12 Spirit / +32 Defensive Ability / +2.5 Energy Regenerated per
  second" — an EXACT match to the raw DBR values (`offensiveElementalModifier=15`,
  `characterIntelligence=12` shown as Spirit, `characterDefensiveAbility=32`, `characterManaRegen=2.5`),
  zero variance from the stored number.
- **This retroactively reframes the prior sessions' "17-draw gap" mystery**: it was never that
  Materia's own char/dmg fields needed a slot in the per-store draw order — they need **no RNG slots
  at all**, ever. Any prior model that assumed Materia's own fields participate in the draw stream
  (the original "third slot" hypothesis from the July 7 MateriaRecord session, and the "isolate draws
  0-5" open item from the loop-closed session just above this one) can be dropped for the record's own
  fields; only the completion-bonus (`bonusTableName`) resolution consumes draws, and that appears to
  be on a wholly separate mechanism (see below), not interleaved into the base item's per-store loop.

**New lead: `PlayerItem.RelicSeed` is a separate, independent seed column** (distinct from the item's
own `Seed`), present on every Materia-bearing row (confirmed on 30 sampled rows, always non-null and
different from `Seed`). This strongly suggests the completion-bonus roll (`GetRandomizerName`'s single
MINSTD draw against `bonusTableName`'s weighted entries) is seeded from `RelicSeed`, NOT drawn out of
the base item's own `Seed` stream as the July 7 "loop closed" session assumed (that session only found
a plausible-looking draw index 6 inside the base item's own stream by search, it did not know
`RelicSeed` existed).

**Tried and inconclusive this session**: simulated `Rng(RelicSeed).nxt() % totalWeight` (single draw,
no pre-existing base-stream involvement) against ring 3326's known selection (`a35_energyregen`,
weight 50/660) — found a match at draw index 3, but multiple nearby indices also coincidentally land
on other buckets, and with only 9 weighted buckets a single-sample match isn't strong evidence either
way (same weakness the "loop closed" session's index-6 claim has, now further undermined since it used
the wrong seed source). Also spot-checked PlayerItem 1220 (`compa_hellbaneammo` on the legendary
"Oathbreaker" gun) — its tooltip shows no separable completion-bonus line at all (all its extra Attack
Speed/etc. lines trace to the base record's own jittered fields or the item's granted skill, not to
`completionbonus_a003_weapons.dbr`'s pool) — inconclusive on whether legendaries skip the completion-
bonus roll entirely or just fold it in invisibly; not resolved this session.

**Next steps for a future session**: (1) find 2-3 more materia-only samples where the completion-bonus
line is unambiguous in the tooltip (a clearly-attributable extra stat not explainable by the base or
materia's own fixed fields) and confirm/refute the `RelicSeed`-as-single-draw-source hypothesis with
enough samples to rule out chance; (2) once confirmed, check whether `RelicSeed`'s stream is consumed
by ONLY one draw (flat weighted pick) or also feeds the selected sub-record's own jitter (e.g.
`a35_energyregen.dbr` has its own `lootRandomizerJitter=50` — does ITS jitter draw come from the same
`RelicSeed` stream, right after the selection draw, or from the base item's own `Seed` stream instead?
Not yet checked either way). No code changed this session — investigation + one confirmed DB-level
finding (materia = zero jitter, verbatim values) that should be written into `gdvalidate.py` as a
simple "no draw, use DBR value as-is" rule for Materia's own fields once the completion-bonus mechanism
is fully solved and the Materia contribution pass is implemented for real.

## Session update (July 7 2026, LOOP CLOSED: live seed simulation confirms `LootRandomizerTable::GetRandomizerName`'s selection draw against real data; still no code changed)

Picked up the one concrete "still open" item left by both Ghidra passes below: live/seed-driven
simulation of the selection formula against a real materia-only sample, to confirm the decompiled
algorithm actually predicts which completion-bonus sub-record gets selected.

**Confirmed end-to-end on the exact sample used throughout this investigation** (`c027_ring` +
`compa_markofillusions`, PlayerItem.Id=3326, seed=1082779): queried `completionbonus_a002_ringsamulets.dbr`'s
9 `randomizerName1..9`/`randomizerWeight1..9` pairs from `DatabaseItemStat_v2` — weights
`[100,100,100,100,75,75,10,50,50]` (total 660) mapping to
`[a26a_fireresist, a27a_coldresist, a28a_lightningresist, a29a_poisonresist, a30a_aetherresist,
a31a_chaosresist, a32a_elementalresist_rare, a35_energyregen, a36_healthregen]`. Imported `Rng`
directly from `gdvalidate.py` (per project convention), generated the raw draw stream for seed
1082779, and applied the decompiled formula (`draw % (totalWeight+1)`, then cumulative-weight walk)
at every draw index. **Draw index 6 (the 7th `nxt()` call) selects `a35_energyregen`** — which
exactly matches this item's real tooltip line ("+1.5 Energy Regenerated per second" traced in the
prior session to `a35_energyregen.dbr`, not to the materia's own DBR fields). This is not a
coincidence-prone match: 9 weighted buckets with one dominant ~15% (50/660) bucket landing exactly
right, on the first index tried inside the previously-identified 1-17 "gap" range, is a strong hit.

**What this confirms**: the decompiled `LootRandomizerTable::GetRandomizerName` formula
(`0x180348e10`/`0x18034f070`, documented in the "GHIDRA FOLLOW-UP" block below) is not just
structurally plausible — it reproduces the real seed's actual outcome. The completion-bonus
selection genuinely consumes exactly ONE MINSTD draw from the SAME seeded stream `InitializeItem`
reads from (not a separately-seeded RNG), at some position before the base record's later CHAR/DMG
fields (draw index 18/22/23 in this sample) — consistent with the recursive/chained selection
mechanism (`ValidateSelectRandomizerRecursive`) possibly consuming a few more draws before or after
index 6 for the outer artifact/materia-attachment resolution itself (not yet isolated — index 6 is
confirmed as the point where the FINAL completion-bonus sub-record round is selected, but where the
outer `bonusTableName` resolution's own draw(s), if any, sit relative to it is still unconfirmed).

**Still open / next steps**: (1) isolate whether draws 0-5 belong to some outer selection step (e.g.
the materia-attachment/artifact-load draws) or are simply unused padding — check a 2nd sample to see
if the selection-draw index is stable/predictable (e.g. always the Nth draw for a given item class)
or varies; (2) once the selection index's OWN position rule is known, determine how the selected
sub-record's fields (e.g. `a35_energyregen.dbr`'s `characterManaRegenModifier`/its own
`lootRandomizerJitter`) draw and jitter relative to the base record's own remaining fields — this is
the same per-kind/per-store dispatch question the original affix model already solved for
Prefix/Suffix, just needing the SAME treatment applied to the resolved completion-bonus record
instead of a literal Materia DBR row; (3) only then attempt the full 10-sample corpus validation and,
if it holds, implement in `gdvalidate.py`/`ItemStatEngine.cs` per the C# parity rule. No code changed
this session (investigation only) — corpus numbers unchanged (base-only 1055/1055, single-affix
951/951, both-affix 352/352, `dotnet test` 47 pass/1 skip).

## Session update (July 7 2026, NEWEST — incremental Ghidra re-confirmation pass on LootRandomizerTable weighted-selection mechanism; no code changed, investigation only)

Re-ran the string/xref hunt for `randomizerName`/`randomizerWeight`/`bonusTableName`/`LootRandomizerTable`
from scratch (independent pass, not continuing a live session) to re-verify and extend the
"GHIDRA FOLLOW-UP" findings below. Result: **fully reproduces** the prior pass's core claims
(single-MINSTD-draw + cumulative-weight-walk selection in `GetRandomizerName`, chained/recursive
resolution via `ValidateSelectRandomizerRecursive`), plus a few new specifics:

- Found the **non-Dynamic** `LootRandomizerTable::GetRandomizerName` @ **0x180348e10** (previous
  pass only decompiled the `_Dynamic` variant @ 0x18034f070 and the `LootTable` base variant @
  0x18034faf0). Decompile is structurally identical to the Dynamic version: same MINSTD draw
  (`(seed % 0x1f31d)*0x41a7 + (seed/0x1f31d)*-0xb14`, sign-fixed by `+0x7fffffff`), same
  `roll % (entryCount+1)` reduction (entry count at `this+0x30`), same cumulative-weight walk over
  a `this+0x38..0x40` vector of 0x28-byte entries with weight at `entry+0x20`. Only difference from
  the Dynamic version is the fallback string constant on empty-table (`DAT_1805fa0c7` vs
  `DAT_1805fa0ef`) — cosmetic, not algorithmic. This confirms the non-Dynamic table variant (the one
  a plain DBR-path `bonusTableName` resolves to) uses the exact same one-draw CDF-walk selection.
- Traced `bonusTableName` (string tag @ 0x180663788, only one hit in the binary) to its actual
  reader: **`ItemArtifact::Load` @ 0x30edc0** (mangled `?Load@ItemArtifact@GAME@@...`), which calls
  the DBR-field getter with `"bonusTableName"` and stores the resulting path string at `this+0xc80`.
  This is on `ItemArtifact`, not a `MateriaRecord`-named class (Ghidra has no symbol literally named
  `MateriaRecord`) — consistent with the established finding that "materia" completion-bonus tables
  are just another `bonusTableName`-bearing item record loaded through the shared Artifact/Item load
  path, not a bespoke code path. `ItemArtifactFormula::ItemArtifactFormula` @ 0x180311980 (the
  formula-side constructor) was also inspected as a candidate resolver but is pure field-zeroing
  init, no RNG/table logic — ruled out as the consumer.
- Confirmed via `get_xrefs_to` on `LootRandomizerTable::ManualLoad` @ 0x180348ee0 that the
  same selection machinery is shared by `GetPrefixTable` (0x1803412e5 call site),
  `GetSuffixTable` (0x18034157b), `GetBrokenTable` (0x18034181b), and
  `ValidateSelectRandomizerRecursive` (0x18034fcaa) — i.e. prefix/suffix/broken/completion-bonus
  affix-name resolution all funnel through one generic weighted-table-load-and-pick routine. No
  materia/completion-bonus-specific branch exists; the mechanism documented in the prior pass for
  "randomizer tables" applies uniformly.
- Did not get further than the prior pass on the open question (live call chain from top-level
  item/loot generation down into `GetLootName`, and per-sample draw-count validation against
  `userdata-test.db` seed 1082779 / item 3326). Ran out of scope this pass to do the live
  seed-driven simulation — still recommend that as the next concrete step to fully close the loop
  (simulate `completionbonus_a002_ringsamulets.dbr`'s 9 `randomizerWeight1..9` values against seed
  1082779's draw stream and check whether `a35_energyregen.dbr` is the one selected at the expected
  draw index).

**Everything above is Ghidra-decompile-confirmed** (function addresses, field offsets, MINSTD
formula, weight-walk logic). The mapping from a specific real item/seed to a specific selected
sub-record (the "close the loop" validation) remains **not yet done** — hypothesis only.

## Session update (July 7 2026, MateriaRecord investigated: NOT a simple third draw-order slot, it's a randomized completion-bonus table; Ghidra follow-up done same day, see "GHIDRA FOLLOW-UP" block below; no code changed)

**GHIDRA FOLLOW-UP (July 7 2026, same day, later pass)**: the prior write-up below cited only DB-level
findings (bonusTableName, LootRandomizerTable, 9 randomizerName slots) with zero Ghidra function
names/addresses — that reverse-engineering step had not actually been done. It has now been done;
Ghidra tools were available and returned real results.

- `mcp__ghidra__list_strings` found the tag strings as expected: `randomizerName1..150` (multiple
  DBR-table copies), `bonusTableName` (single hit @ 0x180663788), `LootRandomizerTable` /
  `LootRandomizerTable_Dynamic` (class + full mangled method table: ctor/dtor/Load/ManualLoad/
  GetRandomizerName/GetAllEntries/RTTI, e.g. `Load@LootRandomizerTable` @ 0x180992f37,
  `GetRandomizerName@LootRandomizerTable_Dynamic` @ 0x180962325).
- `mcp__ghidra__search_functions_by_name` for "Randomizer" surfaced the real call graph:
  `LootRandomizerTable` ctor @ 0x1800d06b0 / 0x180348bc0, `LootRandomizerTable_Dynamic` ctor @
  0x1800ee690 / 0x180349030, `GetRandomizerName` @ 0x18031a740 / 0x180348e10 / **0x18034f070**
  (`LootRandomizerTable_Dynamic::GetRandomizerName`) / **0x18034faf0**
  (`LootTable::GetRandomizerName`), `GetRandomizerNames` @ 0x180340fb0 / **0x180346b90**
  (`LootItemTableRandomizer_Dyn::GetRandomizerNames`), `LootItemTableRandomizer_Dyn` ctor @
  0x18033e1c0/0x1803418e0, `ValidateSelectRandomizerRecursive` @ **0x18034fc20**,
  `SetRandomizerWeightModifiers` @ 0x18033caf0/0x180348300. No match for "CompletionBonus"
  (that's a pure DBR path-naming convention, not a class name). "Weighted" only matched unrelated
  `AddWeightedObjectToList`/`GetWeightedSpawnObject`.
- **Decompiled `LootRandomizerTable_Dynamic::GetRandomizerName` @ 0x18034f070**: confirms the
  selection algorithm is a **single MINSTD draw + cumulative-weight walk**, NOT two draws or a
  binary search. It advances the RNG once (`uVar2 = (seed % 0x1f31d)*0x41a7 + (seed/0x1f31d)*-0xb14`,
  the same MINSTD formula already validated for base/affix jitter), takes `uVar2 % totalWeight`
  (totalWeight = `iVar1+1` where `iVar1` is the entry count field at `this+0x30`), then walks the
  entry vector at `this+0x38..0x40` (each entry is 0x28 bytes, weight field at `entry+0x20`)
  accumulating weights until the running sum exceeds the roll — a straightforward CDF walk over one
  draw, matching how prefix/suffix table selection already works elsewhere in the codebase.
- **Decompiled `LootTable::GetRandomizerName` @ 0x18034faf0**: a related but distinct function —
  first draws to check a probability gate (`draw * const1 * const2 < param_2` i.e. a drop-chance
  roll), and only if it passes, draws AGAIN and does the same cumulative-weight walk, then calls
  `ValidateSelectRandomizerRecursive` (@ 0x18034fc20) on the result. `ValidateSelectRandomizerRecursive`
  in turn `ManualLoad`s the selected DBR path as a nested `LootRandomizerTable`, calls
  `GetRandomizerName` on IT, and recurses (depth-limited, decrementing `param_2`, engine-error-logs
  past depth) — i.e. **completion-bonus/randomizer tables can chain to other randomizer tables**,
  each link consuming its own draw(s), which explains why materia items showed more extra draws
  than a single flat lookup would predict.
- **Where this sits in the overall draw sequence**: traced callers up from `GetRandomizerName`.
  `LootItemTableRandomizer_Dyn::GetRandomizerNames` @ 0x180346b90 (single MINSTD draw against the
  summed weights of 10 category counts at `this+0x94..0xb8` — prefix/suffix/broken/rare-prefix/
  rare-suffix/etc — then dispatches to `GetPrefixTable`/`GetSuffixTable`/`GetBrokenTable`/
  `GetRarePrefixTable`/`GetRareSuffixTable` @ 0x180346e80/0x180347130/0x1803479be/0x1807363d8/etc,
  each of which does its OWN cumulative-weight draw to pick a name within that category, THEN loads
  it and calls `LootRandomizerTable_Dynamic::GetRandomizerName` again — confirming nested/chained
  selection is the norm, not the exception). The sole caller of `GetRandomizerNames` is
  `LootItemTable_DynWeight::GetLootName` @ 0x18033ef30, which is the **item-drop/loot-table name
  resolution function** — it first calls `ProcessTableData`, then does its own cumulative-weight
  draw to pick which sub-loot-table entry applies, THEN constructs a fresh
  `LootItemTableRandomizer_Dyn`, calls `LoadFromDatabase`/`SetWeightModifiers`, and only then calls
  `GetRandomizerNames`. Checked xrefs to `InitializeItem` @ 0x1803213e0 (the confirmed per-item
  field-draw entry point) — every reference is a vtable/DATA slot, zero direct/unconditional CALL
  sites. This confirms `GetLootName`/`GetRandomizerName`/`LootRandomizerTable` is a **separate,
  upstream RNG-consuming subsystem**: it resolves WHICH prefix/suffix/completion-bonus DBR record
  gets attached to a dropping item (loot-table item-class and affix-name selection), consuming its
  own MINSTD draws from the same seeded generator BEFORE `InitializeItem`'s own per-field draw loop
  ever begins reading that resolved record's fields. It is not a slot spliced into
  `InitializeItem`'s existing per-store draw order the way the original hypothesis assumed.
- **Still open**: exact live call chain from top-level item-generation (which function calls
  `GetLootName` — not yet traced upward past it) and confirmation of how many total draws the
  completion-bonus chain consumes for the specific 10 materia-only samples (would require
  simulating the resolved DBR paths per sample seed and counting recursion depth — not attempted
  this pass, this was a static-decompile pass only, no live/seed-driven simulation was run).

Investigation-only session (per the task brief, no forced fit): extended the affix-pass investigation
to items with a `MateriaRecord` set (and nothing else — Prefix/Suffix/Modifier/Enchantment/
RelicCompletionBonus/Transmute all still absent), using the 10 materia-only rows in
`userdata-test.db` (PlayerItem Ids 1220, 1228, 1528, 2785, 2877, 3026, 3087, 3326, 3365, 3555).

**What was expected going in**: per the prior session's Ghidra lead (`docs/continuation.md`
line ~320), Materia was believed to be a third slot in the same per-field loader mechanism as
Prefix/Suffix — for Char/Skill/RetalMod stores drawing after Prefix/Suffix (before Base), for
Damage/Defense stores drawing after Suffix (i.e. base→prefix→suffix→materia). The plan was to
confirm this via draw alignment on the 10 base+materia-only samples (no prefix/suffix present, so
materia's position relative to base is directly isolated) and add it as a new contribution source
in `gdvalidate.py`'s `affix_rows()`/entries loop, mirroring the existing prefix/suffix dispatch.

**What was actually found**: draw alignment on the simplest sample (`c027_ring` +
`compa_markofillusions`, PlayerItem 3326, seed 1082779) refuted the simple-slot hypothesis
immediately. The base record's own CHAR/DMG/DEF fields (`characterSpellCastSpeedModifier`=4→3,
`offensiveLightningModifier`=18→21, `offensiveSlowLightningModifier`=18→19,
`defensivePierce`=16→16) land at draw stream indices **0, 18, 22, 23** — i.e. 17 RNG draws are
consumed between the first and second entries, far more than the 1-2 extra draws a simple
materia-char-fields-early model would predict (materia's own char fields here are
`characterIntelligence`=12 and `characterDefensiveAbility`=32, only 2 candidates, and neither
combination of orderings reproduces `offensiveLightningModifier`→21 landing at index 18).

**Root cause of the gap**: every one of the 10 materia-only samples' `MateriaRecord` DBR carries a
non-empty `bonusTableName` field (confirmed via direct `TextValue` query on `DatabaseItemStat_v2`,
e.g. `compa_markofillusions.dbr` → `bonusTableName='records/items/lootaffixes/completion/
completionbonus_a002_ringsamulets.dbr'`). That target record's own DB row has `Class=
'LootRandomizerTable'` and 9 `randomizerName1..9` slots (each a *different* completion-bonus DBR
path, e.g. `a26a_fireresist.dbr` … `a35_energyregen.dbr` … `a36_healthregen.dbr`), each with a
`randomizerWeight` — i.e. attaching a Materia to an item doesn't just apply the materia's own
fixed stat block, it ALSO triggers a **weighted-random selection from a second, per-item-slot
lookup table** (completion bonus), and the selected sub-record's own stats (which have their own
`lootRandomizerJitter`, e.g. `a35_energyregen.dbr` has `characterManaRegenModifier=8,
lootRandomizerJitter=50`) are what fill the gap — explaining both the extra draws (index selection
in the weighted table + that record's own jitter) and tooltip lines that don't correspond to any
field on the materia's own DBR row (e.g. ring 3326's "+1.5 Energy Regenerated per second" comes
from the randomly-selected `a35_energyregen.dbr`, not from `compa_markofillusions.dbr` itself).
This is a materially different and more complex mechanism than Prefix/Suffix jitter — it requires
first reverse-engineering the weighted-random-index algorithm (which RNG draw(s) pick the index,
whether by cumulative weight or straight modulo, Ghidra-unconfirmed) before any per-field jitter
alignment is even meaningful, since which record's fields apply is itself seed-dependent.

**All 10 samples show the same `bonusTableName`-present pattern** (weapons → `completionbonus_
a003_weapons.dbr`/`a000_weapons.dbr`, armor → `a000_armor.dbr`/`a001_armor.dbr`/`b001_armor.dbr`,
rings/amulets → `a002_ringsamulets.dbr`) — this is not an isolated quirk of one sample, it's the
general Materia mechanism. No sample was found where a Materia's own DBR fields draw in isolation
without this randomizer-table layer.

**Per the task's explicit instruction not to force a partial/guessed model: no code was changed.**
`gdvalidate.py` and `ItemStatEngine.cs` are unmodified; `MateriaRecord`-bearing rows remain
completely unmodeled/excluded, same as before this session. Corpus numbers are unchanged from the
prior session's ending state: base-only 1055/1055, single-affix 951/951 (100%), both-affix
352/352 (100%). `dotnet test` not re-run since no C# changes were made.

**Remaining open / next steps for a future session** (none attempted this session): (1) find the
Ghidra function that resolves a `LootRandomizerTable`'s weighted index from a seed/RNG draw (search
for xrefs to the `LootRandomizerTable` class or `bonusTableName` string, likely near the existing
`FUN_18019f110`-family DamageAttributeStore loaders since materia items exercise the same
per-field dispatch downstream); (2) once the index-selection formula is known, re-attempt the
10-sample draw alignment with the CORRECT selected sub-record's fields substituted for the
materia's raw DBR fields; (3) only then decide where in Char/Skill/RetalMod vs Damage/Defense
per-kind draw order the resolved fields sit relative to base (the original open question, still
unanswered). The ~148 samples where Materia is combined with Prefix/Suffix/etc remain untouched
and should stay excluded until the materia-only mechanism is solved on its own.

## Session update (July 6 2026, NEWEST — AFFIX CORPUS 100%: every remaining affix mismatch solved in one session; Python AND C# both updated; 6 commits landed)

Continued directly from the session below (which found the b_class* shift-by-one but couldn't find
its trigger). This session found the trigger immediately via a data diff, then solved EVERY other
residual family by raw-draw alignment. **Final validator state: base-only 1055/1055, single-affix
951/951 (100%), both-affix 352/352 (100%), per-stat BAD=0.** `dotnet test`: 47 pass / 1 skip.
All committed to the GrimDawnSeedStats repo (own git repo, branch `more-work`, 6 commits).

The six mechanisms discovered/fixed (each confirmed by exact draw alignment on real seeds, then
corpus-validated with zero regressions; all in `gdvalidate.py` AND ported to `ItemStatEngine.cs`):

1. **Affix skill-store early draw** — the b_class* mystery's real trigger was
   `skillCooldownReduction` presence on the affix record (perfect 5/5-vs-0/12 discriminator; NOT
   augmentSkill1 — proc/aura affixes carry identical augmentSkill fields and don't shift).
   An affix's skill-store fields (skillCooldownReduction/skillManaCostReduction) draw EARLY, at
   the START of the Damage store (before the item's first flat/dmg entry; pinned by b202d_head+
   b_ar001_he_d+a031e seed 57211 where the extra draw precedes base-only dmg fields), prefix
   before suffix, using sjit with the affix's own pct; that early value IS the affix's
   contribution and the deferred end-position SKILL draw covers only the base record's part.
2. **Focus (Class=WeaponArmor_Offhand) discards affix flat added-damage entirely** — 0 draws, no
   tooltip line (a08_focus01+b_sh027_e seed 297944: consecutive draws OA,coldMod,lifeMod,
   slowColdMod,slowColdDurMod,SCR with no flat draw). NOT a general Min-only rule — generalizing
   regressed the corpus 1249→1150; it's off-hand-specific.
3. **Affix-sourced retaldur/retalreflex pairs draw** at their normal ORDER position with the
   affix's own pct (b_ar003_ar family, 3 seeds exact) — previously only base-sourced ones drew,
   silently desyncing every draw after.
4. **Conversions**: each source's conversionPercentage is convjit'd SEPARATELY (base with default
   20%, affix with its own lootRandomizerJitter), drawing base→prefix→suffix; same In/Out type
   pairs SUM for display (b006c_axe: 14.87+14.84→"30%"; b013c_sword2h: 34.10+31.65→"66%";
   one-draw-on-sum and per-source-with-wrong-pct both explicitly refuted by the alignment).
   Affix-only conversions (base has none) also now draw+display.
5. **Flat pairs sum-then-scale**: raw jittered mins sum across sources, scale applied ONCE to the
   total (matches merge-then-ScaleAttributes; b006c_axe scale(15+15)=41 vs per-source 20+20=40).
   Spread still never scaled.
6. **Defense store draws base FIRST** (like Damage; b007c_shoulder+b_ar006_ar_d seed 623217:
   base@5+suffix@6=42 exact vs suffix-first 39) — switching def to base-first alone took
   single-affix 945→951/951. The old "Char/Skill/Def/RetalMod are prefix→suffix→base" note is
   WRONG for Def; only Char/Skill/RetalMod keep affix-first order.
7. **Slow{X}Modifier + Slow{X}DurationModifier interleave per source** — one DurMod object per
   source draws its (value, duration) pair consecutively: base(v,d), prefix(v,d), suffix(v,d)
   (b202d_focus+aa017a+b_sh010_e seed 789691: interleaved 119/95 exact vs field-by-field 124/90).

Method note for future sessions: every one of these fell to the same playbook — dump the exact
base/prefix/suffix DB rows, generate the raw MINSTD draw stream for the seed, and compute each
candidate field/pct at every position until the real tooltip values snap into a consecutive
assignment. No Ghidra needed this session; draw alignment against one good seed was always enough
to pin both position and formula, and the corpus rerun then proved generality.

C# parity: all of the above ported into `ItemStatEngine.Compute` (plus doc-comment updates); the
CLI's two-affix warning is REMOVED (no longer true); `AffixFlatPairField_SumsComponentWiseWithBase`
expectation re-derived from the oracle for sum-then-scale (8/22→9/23). The corpus `[Skip]` marker
now says the pipeline is complete and only a C# tooltip-text formatter is missing to turn the
affixed fixtures into per-item Theory assertions.

Remaining open (unchanged, all pre-existing): the per-record quirk tables
(`BASE_OVERRIDE_QUIRKS`/`NODRAW_QUIRKS`/`SINGLE_SIDED_QUIRKS`) still lack a Ghidra-confirmed root
cause; Modifier/Materia/Enchantment/RelicCompletion/Transmute rows are still excluded from the
affix pass entirely (never modeled); the C# affixed-fixture Theory needs a tooltip formatter.

## Session update (July 6 2026 — single-affix "class mastery" residual investigated: found a real one-extra-RNG-draw pattern on 5 samples, but it does NOT generalize; reverted, no net code change) [SUPERSEDED — the trigger was found next session: it's skillCooldownReduction on the affix, not b_class/augmentSkill; see session above]

Picked up the single-affix residual (46/951) per the "class mastery-affix prefixes with
inconsistent per-stat DB-value drift" open item. Traced `a06_necklace03`+`b_class015_arcanist19_je_b`
(seed 149745) field-by-field via an instrumented replay of `gdvalidate.py`'s affix loop: the computed
value for each DMG-store field (Aether, Elemental, SlowFire, SlowCold, SlowLightning) exactly equaled
the REAL tooltip's value for the field ONE POSITION EARLIER in draw order — a clean shift-by-one, not
scattered noise. Inserting one extra `rng.nxt()` draw right before the first scalar field where the
prefix contributes a value (not at the very start of the item's RNG stream, which would also wrongly
shift earlier CHAR-store fields sourced only from the base record) made ALL fields on that item match
exactly, including `defensiveChaos`/`skillCooldownReduction` which the truncated tooltip sample
originally hid. Confirmed identically on 4 more samples (2 more `a06_necklace03` variants, `a03_necklace01`
+`b_class021_shaman11_je`, `a04_necklace03`+`b_class035_inquisitor25_je`, and a both-affix case
`b014d_head`+`b_class011_soldier01_je_b`+suffix) — 5/5 exact fixes, strongly suggesting the class-mastery
skill grant (`augmentSkill1`/`augmentSkillLevel1`, present on all `b_class*` prefixes) consumes one
hidden RNG draw before the record's own stat block, even though the skill's own level doesn't
fluctuate with seed (per user: skill stats generally don't fluctuate, consistent with the draw
producing a fixed/discarded result).

**This did NOT generalize — reverted, no code change landed.** Two falsification attempts:
1. Applying "1 extra draw if `augmentSkill1` is present" to ALL affix records (not just `b_class*`)
   collapsed the corpus from 1225/1303 to 655/1303 — the vast majority of `augmentSkill1`-bearing
   records (`b_sh*`/`b_wpn*`/`aa0*` proc/aura affixes, which carry `augmentSkill1`/
   `augmentSkillLevel1`/`augmentSkillName1` in the IDENTICAL shape as the class-mastery ones) do
   NOT consume this extra draw.
2. Narrowing to "prefix/suffix record path contains `/b_class`" was still net negative: fixed 10
   previously-failing items but broke 25 previously-passing `b_class*` items (checked via a
   before/after diff script). No field-level distinguishing trait was found between the "needs
   extra draw" and "doesn't need it" `b_class*` groups — tried `augmentSkillLevel1` value (both
   groups have 2.0 and 3.0 present on each side) and the `_je`/`_je_b`/`_je_c` naming suffix (both
   groups have it) with no clean split.

**Conclusion:** the shift-by-one mechanism is real and reproducible on specific samples, but its
trigger condition is not simply "is a `b_class*` prefix" or "has `augmentSkill1`" — there's a third,
unidentified factor (possibly something about a specific class/skill's own load path, or a
per-record `augmentSkill1Extras`/`augmentSkillName1` property not yet compared systematically).
`gdvalidate.py` and `ItemStatEngine.cs` are UNCHANGED (edit was made, tested, found net-negative,
and reverted in the same session — confirmed via `git status`/`git diff` showing no residual diff).
`dotnet test` unaffected (no C# touched). **Next step if resumed**: don't retry the same two
groupings — instead diff the FULL field list (not just augment* fields) of a few more "needs extra
draw" vs "doesn't need it" `b_class*` prefixes side by side (e.g. via the same DB dump technique
used this session) looking for ANY shared trait unique to the fixed group, or try sourcing
`augmentSkillName1`'s target skill DBR file (`records/skills/playerclassNN/*.dbr`) to see if some
skills have a randomized-selection mechanic (e.g. "grant a random rank/tier") that others don't.

## Session update (July 6 2026, newest — user refreshed userdata-test.db 3x with a much larger/aligned seed+tooltip capture; corpus now MUCH bigger, all fixes hold up)

User discovered the single-affix corpus was tiny (16, then 1, out of 1300+ candidate rows) because
most single-affix `PlayerItem` rows in the DB snapshot had no matching `ReplicaItem2`/`ReplicaItemRow`
tooltip capture at all — `gdvalidate.py` only counts a row once both a real DBR-stat AND a real
tooltip exist for it (`replicatexts(pid)` returns empty otherwise, silently skipped, not a "16 is
enough" decision). Root cause narrowed live: an intermediate DB replacement briefly had NEW re-rolled
seeds paired with STALE real-stat captures (mismatched snapshots), which showed up as base-only
accuracy collapsing from 1044/1044 to 1/1054 — confirmed this was a data-alignment issue, not a code
regression, by manually cross-checking one collapsed item (`d202_legs.dbr`, seed 5971663) and finding
its own DB values didn't correspond to its own real tooltip. User re-synced seeds+stats together and
replaced the DB two more times.

**Final DB state this session, full validator rerun, MUCH larger and now well-covered corpus:**
- Base-only: **1055/1055 (100%)**, unaffected throughout.
- Single-affix: **916/951 (96%)** — up from a 1-3 row sample, now large enough to trust; consistent
  with the ~94% figure established in earlier `docs/affix-draw-order-plan.md` sessions on a
  different snapshot, confirming the single-affix model generalizes across DB captures.
- Both-affix (Damage-store draw-order fix from the session above): **309/352 (88%)** — holds up on
  an almost entirely new seed set, confirming the base→prefix→suffix fix isn't overfit to the
  seeds it was validated against.
- `dotnet test`: unaffected, 47 pass/1 skip (C# tests use the static fixture corpus, not the live DB).

No code changes this session — DB-hygiene/validation only. Lesson for future sessions: **when the
user says they've updated `userdata-test.db`, always rerun the FULL validator (base-only AND affixes)
before trusting any percentage from a prior session** — a reseeded-but-not-restat'd (or vice versa)
DB produces a systemic, across-the-board collapse that looks like a code bug but isn't; check one
mismatched item's own `DatabaseItemStat_v2` row against its own `ReplicaItemRow` tooltip by hand
before assuming a regression.

## Session update (July 6 2026, newest — TWO-AFFIX DAMAGE-STORE DESYNC FIXED: 51%→88%, real root cause was draw ORDER (base-first), not the Global/XOR flags)

Continued directly from the session below. The `Global`/`XOR` DBR-flag lead was checked against the
real DB and refuted (queried `DatabaseItemStat_v2` for every prefix/suffix DBR row: `offensive{Type}
Global`/`XOR` tags exist in the game's schema but **zero** prefix/suffix rows in this corpus set them
on plain elemental %-modifier fields — only unrelated `retaliationSlowXGlobal`-style fields use it).
Also re-confirmed no call-site table-order swap (traced `ItemEquipment::InitializeItem` @`0x1803213e0`
→ `DamageAttributeStore_Equipment::Load` → `FUN_18019f110`: base/prefix/suffix/materia are passed
in identical order to every store) and re-confirmed `ScaleAttributes`/`ScaleAttribute` are bare loops
with zero source-count-dependent logic (scale is applied exactly once, after summation).

**Found the real cause: `gdvalidate.py`'s both-affix scalar-field loop (`Kind` = char/dmg/def/skill/
retalmod) drew in order Prefix → Suffix → Base for EVERY store, including Damage — but the
Damage-store's own C++ loader (`FUN_18019f110`, fully decompiled last session) builds/jitters its
base/prefix/suffix/materia objects in the literal order BASE FIRST, then prefix, then suffix, then
materia — the opposite of Char/Skill/Def/RetalMod's Ghidra-confirmed Prefix→Suffix→Base order.**
Verified concretely on the exact swapped-looking mismatch flagged last message
(`b202d_head.dbr`+prefix+suffix, seed 1425442622, where `offensiveFireModifier`/
`offensiveElementalModifier` looked like they'd been transposed) by patching a scratch copy of
`gdvalidate.py` to draw Damage-store (`kind=='dmg'`) fields as base→prefix→suffix instead of
prefix→suffix→base, and rerunning the FULL both-affix corpus:

- **Before: both-affixes 178/347 (51%), per-stat BAD=308.**
- **After: both-affixes 304/347 (88%), per-stat BAD=86.** Base-only (1044/1044) and single-affix
  (16/16) both UNCHANGED (order doesn't matter with 0 or 1 non-base contributing source — cjit
  no-ops on a zero value without consuming a draw, so reordering zero-contribution sources is
  invisible; this is why single-affix was never affected by this bug and stayed 94%/16-16 all along).

**Applied to both deliverables per the C# CLI parity rule:**
- `tools/gdvalidate.py`: the `kind=='dmg'` branch of the scalar-field loop now draws
  base→prefix→suffix; all other kinds (`char`/`skill`/`def`/`retalmod`) are unchanged
  (prefix→suffix→base, per their own Ghidra confirmation in `docs/affix-draw-order-plan.md`).
  `FLAT`/`RETAL_FLAT` pair-shaped fields were ALREADY drawing base-first (lines ~655/673 iterate
  `(bs, ...), (pfs, ...), (sfs, ...)` in that order) — no change needed there, consistent with them
  not showing the same failure signature.
- `src/GrimDawnSeedStats/ItemStatEngine.cs`: added an `e.Kind == Kind.Dmg` branch in the scalar-field
  switch's `default` case mirroring the same base→prefix→suffix order; `dotnet test` still **47
  pass / 1 skip**, zero regressions. Updated the CLI's `--prefix`+`--suffix`-together stderr warning
  (`Program.cs`) from "~56% correct, treat with caution" to "~88% correct as of July 2026 after the
  draw-order fix, residual gap remains" — still warns since it's not 100%, but no longer describes
  it as broken.

**Remaining 43/347 both-affix mismatches (86 stat lines)**: spot-checked the sample output — spread
across a mix of `conversionPercentage` (4 instances), `offensiveSlowPoisonModifier` (2), and one-off
Damage/Defense fields (`defensiveChaos`, `defensivePierce`, `offensiveFireModifier`, etc.), no single
dominant remaining pattern the way the base/prefix/suffix order was. Not chased further this session
— the big win is landed and validated; next session should treat the residual ~12% as a fresh
long-tail investigation (per-field quirks or a smaller, different-shaped bug), not assume it's the
same root cause.

## Session update (July 6 2026, newest — REAL likely root cause found: per-field `Global`/`XOR` DBR flags, never modeled by gdvalidate.py, gate whether MergeDamage sums at all) [SUPERSEDED — refuted by direct DB query this session; see session above for the actual fix]

Continued the investigation from the session below (which refuted the merge-pass theory but left the
real cause unlocated). Chased the two remaining leads it named — call-site table-order swap, and
`ScaleAttributes`/`ScaleAttribute` per-source-count effects — and ruled both out cleanly:

- **No table-order swap.** Traced `DamageAttributeStore_Equipment::Load`'s caller,
  `ItemEquipment::InitializeItem` @`0x1803213e0`: the four `LoadTable*` params (base/prefix/suffix/
  materia, read from the item's own `this+0x570/0x590/0x5b8` record-name fields respectively) are
  passed in that exact order to **every** store's `Load` (Char, Skill, Damage, Retaliation, Defense,
  Conversion) identically — confirmed by reading the raw call list. Rules out a per-store or
  per-field param swap entirely.
- **`ScaleAttributes`/`ScaleAttribute` are bare loops, confirmed (again) to have zero count-dependent
  logic.** `DamageAttributeStore::ScaleAttributes` @`0x1801901f0` just calls `vt+0x60` once per
  surviving attribute across 3 internal lists; `DamageAttributeAbsMod::ScaleAttribute` @`0x1801760b0`
  scales each remaining value-array element once via `vt+0x130`. Since `MergeDamage` already
  collapsed same-field sources into one object before this runs, there is exactly one scale call on
  the final summed value — matches sum-then-scale, which is what the validator already assumes.

**The actual find: `this+0x99` (the flag `MergeDamage` checks to bail out) is not a generic "is this
a bonus object" flag — it is loaded per-object, per-instance, from the record's own real DBR tag,
resolved via `GetLoadGlobalTag()` (e.g. `DamageAttributeAbsMod_Fire::GetLoadGlobalTag` @`0x180187bd0`
returns the literal string `"offensiveFireGlobal"`).** `LoadFromTable` (`0x180175a40`) reads this bool
tag from **that object's own source table** (base/prefix/suffix/materia each check their own copy)
and stores it at `this+0x99`. `MergeDamage`'s very first check is `if (this->offensiveFireGlobal_flag
!= 0) return false;` — i.e. **if the object being merged INTO has its `Global` tag set, prefix/suffix
summation never happens at all**, and both objects survive as separate entries.

Chasing further: `AddToStore` (`0x180175b10`, the `vt+0x20` post-merge validity gate documented last
session) has a SECOND branch specifically for `this+0x99 != 0` objects — it reads a second, sibling
tag via `GetLoadXorTag()` (e.g. `DamageAttributeAbsMod_Fire::GetLoadXorTag` @`0x180187bc0` returns the
literal string `"offensiveFireXOR"`), and depending on that tag's value routes the object into the
store via a **different** store method (`vt+0x20` vs `vt+0x28`) than the normal merged-summation path
(`vt+0x30`). `ScaleAttributes` iterates 3 separate internal attribute lists (`this+8/0x10`,
`+0x20/0x28`, `+0x38/0x40`) — strongly suggesting Global-flagged attributes land in a genuinely
different bucket from ordinary ones, which may combine multiplicatively / take-the-max / apply
per-damage-type differently rather than simple per-field addition (consistent with Grim Dawn's known
"Global" damage-modifier gameplay concept — e.g. a flat "+X% Total Damage" style bonus that isn't
supposed to stack the same way as a per-type modifier).

**This is a strong, concrete, previously-unknown mechanism that `gdvalidate.py`/`ItemStatEngine.cs`
do not model at all** — the "sum everything unconditionally" assumption is correct ONLY when no
source's DBR record has `offensive{Type}Global` set on that field. Given the both-affix corpus is
specifically where this breaks down (52% vs 94% single-affix), the natural hypothesis: some
meaningful fraction of the failing both-affix samples have `offensive{Type}Global`/`{Type}XOR` set on
the prefix or suffix's own DBR row for that field, and the naive sum is wrong specifically for those.

**Not yet done / concrete next step:** (1) query `userdata-test.db`/the raw DBR files for whether any
of the known both-affix mismatch records have an `offensive{Type}Global` or `{Type}XOR` field present
on their Prefix/Suffix DBR rows (not currently captured by gdvalidate.py's queries at all — would need
a new query against the prefix/suffix DBR's own stat table, not just `DatabaseItemStat_v2` which is
per base-item); (2) if confirmed present on failing samples and absent/false on passing ones, decompile
the actual store methods at `vt+0x20`/`vt+0x28`/`vt+0x30` (`DamageAttributeStore`'s own AddAttribute-
family methods, not yet decompiled this session) to learn the exact alternate combination formula
for the Global bucket. No code changed this session — investigation only, but this replaces
"investigate further" with a specific, testable, previously-unknown mechanism.

## Session update (July 6 2026, newest — `MergeDamage`/`AddToStore` (vt+0x98/vt+0x20) decompiled: merge-pass theory REFUTED, it's plain summation; real bug still unlocated)

Followed up on the prior session's "STRONG Ghidra lead" (below) by decompiling the actual `vt+0x98`
virtual it flagged as the pairwise dedupe/merge predicate. Found the real target via
`get_xrefs_from` on the vtable slot address (`DamageAttributeAbsMod_Fire`'s vtable is at
`0x180713dc0`; slot `+0x98` → `0x180713e58` → resolves to **`GAME::DamageAttributeAbsMod::MergeDamage`
@ `0x180175be0`**, a real (already-named-in-binary) virtual, not a guess).

**Decompiled logic, fully resolved (also resolved `vt+0x28`→`GetType()` returning the
`CombatAttributeType` enum, and `vt+0x8`→`DamageAttribute::GetChance(uint idx)` via the same
`get_xrefs_from`-on-vtable-slot technique — reusable for any future vtable-slot lookup in this
binary, no memory-read tool needed):**

```
bool MergeDamage(this, other):
    if (this->isBonusFlag != 0) return false;              // offset+0x99, "Bonus" variant excluded
    if (other->GetType() != this->GetType()) return false; // trivially true, same field always
    if (this->values.size() != 1) return false;             // only single-scalar (Mod) fields eligible
    other_values = other->GetValues();                       // vt+0xb8
    if (other_values.size() != 1) { free(other_values); return false; }
    thisChance  = this->GetChance(0);   // GetChance(_,0) ALWAYS returns 0.0 (short-circuit in the fn)
    otherChance = other->GetChance(1);  // returns other's chance[0] if present, else 0.0
    if (thisChance != otherChance) { free(other_values); return false; }
    this->values[0] += other->values[0];   // PLAIN SUM
    free(other_values);
    return true;   // caller then destroys `other`
```

**Conclusion: the "merge-pass" theory from the prior session is REFUTED as a source of the 52%
both-affix failure.** For ordinary scalar %-modifier fields (no chance/proc component), the
`GetChance` comparison is always `0.0==0.0` (trivially true), `GetType()` is trivially equal (same
field), and the values.size()==1 check is trivially true for Mod-type fields — so `MergeDamage`
degenerates to **unconditional addition** of prefix+suffix+materia into base, in the exact order the
calling code (`FUN_18019f110`, documented in the prior session) processes them. This is **exactly**
what `gdvalidate.py`/`ItemStatEngine.cs` already assume ("sums all present sources unconditionally")
— there is no hidden discard-the-loser behavior for the common case; the pairwise dedupe only matters
for pair-shaped (min/max, values.size()==2) attributes, which never satisfy the `size()==1` guard and
so are never merged this way (consistent with flat added damage being handled separately, unaffected).

Also checked the sibling post-merge validity gate `vt+0x20` → **`AddToStore`
@ `0x180175b10`**: only returns `false` (causing the object to be destroyed/dropped) when its own
value array is empty (`b0==b8`) — i.e. a genuinely valueless object, not a "wrong one of two
candidates" scenario. Not a factor in the both-affix desync either.

**Net result: the specific mechanism flagged last session as a "likely smoking gun" is now
Ghidra-confirmed to behave exactly like the naive sum the validator already implements — it is NOT
the cause of the 52%-vs-94% both-affix gap.** The real cause is still unlocated. Next steps to try
(in rough priority order, none attempted yet): (1) re-verify `ScaleAttributes`
(`0x1801901f0`) isn't just a bare loop but calls something per-element that differs when 2+ objects
were merged vs 1 (e.g. a jitter-count or rounding difference baked into the *jitter formula itself*
depending on how many sources contributed, not the summation code); (2) re-examine whether
prefix/suffix `lootRandomizerJitter` percentages are being read from the correct LoadTable in the
both-affix case (the loader in `FUN_18019f110` passes `param_3`/`param_4` — worth confirming these
are truly prefix's own table and suffix's own table respectively, not swapped, in the actual call
site that supplies all 4 params for a real two-affix item — this exact call site (who passes
param_2..param_5) hasn't been traced yet); (3) get more both-affix mismatch samples and check
whether the error is systematically the SUM being right but display/rounding wrong, vs the sum
itself being wrong (would immediately indicate whether to keep chasing engine code or chase
`gdvalidate.py`'s formula-application instead). No code changed this session (investigation only).

## Session update (July 6 2026, newest — two-affix Damage-store desync: STRONG Ghidra lead found, not yet fixed) [SUPERSEDED BY SESSION ABOVE — merge-pass theory refuted]

Investigation-only session (no code changes), following up on the top open item from
`docs/affix-draw-order-plan.md`: both-affix (prefix AND suffix) DMG-store fields validate at only
52% vs 94% for single-affix, and prior sessions had ruled out scale-offset, prefix/suffix order-swap,
and `ScaleAttributes` as causes.

**First pass (background agent) re-checked the Char store's per-field loader (`FUN_1800c0560`) from
scratch — reconfirmed clean, no hidden desync there** (pure sum via `CharAttribute::GetValue`, no
extra RNG draws conditioned on both-slots-present). This ruled Char out definitively but noted the
real observed mismatches (`b014d_head`, `b202d_head`) are DMG-store fields, and the DMG-store's own
per-field loader had never actually been decompiled/diffed — only assumed structurally identical to
Char's.

**Second pass found the actual structural difference — likely smoking gun.** Decompiled the
Damage-store per-field loader (e.g. `FUN_18019f110` = `DamageAttributeAbsMod_Fire`
/`offensiveFireModifier`, `FUN_1801a0a30` = `..._Chaos`; also checked the pair-shaped
`DamageAttributeAbs_Pierce` loader @`0x180197fa0` — same template). Unlike Char (one object,
jittered in-place 2-4 times, prefix→suffix→materia→base), **the Damage-store loader builds up to
FOUR SEPARATE objects — one each for base/prefix/suffix/materia — each independently
`LoadFromTable`'d (`0x180175a40`) and jittered via its own `vt+0x68` call, in the literal order
base FIRST, then prefix, then suffix, then materia** (opposite of Char's affixes-before-base order;
this exact ordering was never tested before — the earlier "order-swap" experiment only tried
swapping prefix vs suffix relative order, never "does base draw first for DMG fields").

**More importantly: after all up-to-4 objects are built, there's a pairwise dedupe/merge pass with
no Char equivalent** — a `vt+0x98` comparison is run on every pair of the (up to 4) surviving
objects (base-vs-prefix, base-vs-suffix, base-vs-materia, prefix-vs-suffix, prefix-vs-materia,
suffix-vs-materia), and the loser of each comparing pair is destroyed if `vt+0x98` returns nonzero.
This only fires when 2+ sources are simultaneously present — i.e. it's a no-op for single-affix
items (matching their 94% accuracy) but directly exercised whenever both prefix and suffix are
present on the same field (matching the both-affix-specific 52% ceiling and the previously-observed
"scattered, no consistent scale offset" symptom — a wrong VALUE being silently kept/discarded per
pairwise comparison, not a scale or RNG-count bug). `ScaleAttributes`@`0x1801901f0` was independently
re-checked again and reconfirmed as a bare loop with no RNG/count-conditional logic — still ruled out.

**Not yet done / next step**: decompile the `vt+0x98` virtual for `DamageAttributeAbsMod` (base
class ctor/vtable is in the `0x180182500`-`0x180183650` range per this session's search) to learn the
exact trigger condition for "destroy the other object" — most likely a same-field/duplicate-value
check. Once the exact predicate is known, the fix should be implementable in `gdvalidate.py` (which
currently just sums all present sources unconditionally) and validated against the both-affix DB
corpus (352 fields) before porting to `ItemStatEngine.cs`. **No code changed this session** — this
is a Ghidra-RE lead, not a confirmed formula; do not implement a fix based on the merge-pass theory
alone without decompiling `vt+0x98` first and validating against real seeds.

## Session update (July 6 2026, newest — d110_focus AND c009_shield BOTH FIXED via base-override quirks; base-only corpus now 1044/1044, ZERO known mismatches

User added a large batch of new seeds to `userdata-test.db` for both remaining open items (16 new
`d110_focus` PlayerItem rows, ~60 new `c009_shield` rows), enough for a real grid-search fix on both.

**`d110_focus` `characterSpellCastSpeedModifier` — FIXED.** Of the 16 new rows, filtered to the 16
that are truly base-only (no Prefix/Suffix/Modifier/Materia/Transmute/Enchantment/RelicCompletion —
several of the "new" rows turned out to have a crafting `ModifierRecord` like
`ac02_healthregen.dbr`/`ad08_da.dbr`/`ao14_oa.dbr` applied, which added extra rolled stats on top;
these are NOT base-only and were excluded). First confirmed `characterDefensiveAbility` and
`characterManaRegenModifier` (the two CHAR draws immediately preceding this field) match the real
tooltip on all 16/16 seeds exactly — proving draw order/RNG state entering this draw is correct;
only the base value was wrong. Full grid search `(base 1..40) x (pct 5.0..100.0 step 0.5)` against
the raw RNG draw at that exact position, across all 16 seeds simultaneously, found a **single unique
solution: base=20.0, pct=20%** (the DB's own stored value is 15.0) — fits all 16/16 exactly. Added
to `BASE_OVERRIDE_QUIRKS` in `gdvalidate.py`.

**`c009_shield` `retaliationFireMin` — FIXED, and the long-standing "823 Fire Retaliation" mystery
is fully resolved.** With ~60 new base-only seeds, EVERY single one showed a "big" Fire Retaliation
value in the 690-1030 range (the original "823" was never an n=1 anomaly — it's just what this
record always rolls). First confirmed `characterStrength` and `offensiveElementalModifier` (the two
draws immediately preceding `retaliationFireMin`) match the real tooltip on all 63/63 seeds exactly
— draw position/RNG state confirmed correct, only the base value was wrong (same shape as
d110_focus). Full grid search `(base 200..1600 step 2) x (pct 5.0..100.0 step 0.5)` against the raw
RNG draw at the confirmed position, across all 63 seeds, found a **single unique solution:
base=860.0, pct=20%** (the DB's own stored value is 320.0) — fits all 63/63 exactly. This also
**retroactively refutes and closes** the old "Aura of Censure contributes an extra retaliation
number" hypothesis from prior sessions (already independently refuted this session via the upgraded
shield's own seed, see below/above) — no skill contribution is needed once the correct base value
(860, not 320) is used; it was purely a wrong-base-value bug the whole time, same root-cause shape
as `c016_axe2h`/`d105_blunt`/`d110_focus`. Added to `BASE_OVERRIDE_QUIRKS`; also had to add
`quirk_base_override` support to the `retalflat` handling branch in the base-only loop (it wasn't
wired there before — override lookup happens on `prefix+'Min'` before the existing
`quirk_force_single` check).

**Result:** `gdvalidate.py` base-only corpus: **1044/1044 fully-modeled items now validate exactly
— zero known mismatches**, up from 1043/1044 (this session's starting point after the d224_torso
false-alarm was excluded) — both remaining genuinely-open items from the "5 unexplained quirks"
list are now resolved. `dotnet test`: unchanged, **47 pass**, 1 skip (neither record is in the C#
fixture corpus — confirmed via grep — so no port was needed, same precedent as the other
`BASE_OVERRIDE_QUIRKS` entries). Affix corpus (`178/347` both-affixes) is unaffected/unrelated — its
gaps are pre-existing incomplete affix draw-order work, not a regression from this session.

**Retrospective on the "6+ quirks share one general cause" theory (per the OPEN INVESTIGATION note
below):** this session's two fixes, plus the d224_torso resolution earlier this session, account for
3 of the outstanding items without finding one unifying mechanism — they turned out to be a
join-order bug (d224_torso/d111_torso, not a real quirk at all), and two independent "DB's stored
value is stale/wrong" cases (d110_focus, c009_shield) that fit the *existing* `BASE_OVERRIDE_QUIRKS`
mechanism (already used for `c016_axe2h`/`d105_blunt`). So the shared pattern across the *remaining*
confirmed quirks (`c016_axe2h`, `d105_blunt`, `d110_focus`, `c009_shield` — now 4 entries in one
table) really does look like ONE general mechanism: **`DatabaseItemStat_v2`'s stored base value for
that specific field, on that specific record, is sometimes stale/wrong relative to what the game
actually rolls from** — not a draw-order or engine-formula issue at all. Still no Ghidra/root-cause
explanation for WHY these specific (record, field) pairs have a stale DB value while everything else
about the record is correct, but the fix mechanism (look up the correct value empirically via a
seed-sample grid search, store it in the same lookup table) is now validated 4 times. If another
n=1/n=2 "impossible value" mystery shows up, get more seeds for that exact record FIRST (per this
session's playbook) before assuming a new mechanism — it's likely another instance of this same
stale-DB-value pattern, solvable by grid search once enough seeds exist.

## Session update (July 6 2026, newest — c009_shield "823 Fire Retaliation": Aura-of-Censure-contributes-extra hypothesis REFUTED with a real 2nd data point; still open)

Found a 2nd real seed while other work was in progress: the **upgraded** `c009_shield`
(`records/items/upgraded/gearweapons/shields/c009_shield.dbr`, `PlayerItem.Id=1724`, seed
265817652) — same base item, also grants **Aura of Censure at level 2** (identical augment level to
the base item's mystery seed). Its own DB `retaliationFireMin=1450` (no `Max` field at all on the
upgraded record, so no `SINGLE_SIDED_QUIRKS` forcing needed — it's naturally single-valued here).
Real tooltip (pulled via the correct `PlayerItem→ReplicaItem2→ReplicaItemRow` join, per the caveat
above) shows **"1338 Fire Retaliation"**, which sits cleanly inside a plain `cjit(1450, 20%)`'s
possible range `[1160,1740]` — **no unexplained bonus, no extra contribution from the granted Aura
of Censure skill**. This directly refutes the standing hypothesis from prior sessions ("Aura of
Censure might contribute its own Fire Retaliation number to the base item's '823' total") — if the
skill added a bonus, this upgraded item (same skill, same level) should show one too, and it
doesn't. The base item's "823 Fire Retaliation" (`retaliationFireMin=320`/`Max=405`, jittered
Min≈325, seed 1475933991) remains **unexplained and open**, but the Aura-of-Censure angle is now a
dead end, not just "unconfirmed n=1" — narrowed, not solved. No code change (investigation only).
Next step if resumed: find a 3rd sample — any other base-record (non-upgraded) item with both
`retaliationXMin` AND `Max` present that also has a nonstandard single-line display, to see if
"real Min+Max pair but single-line display" recurs on ANY other record (would suggest a structural
cause unrelated to the specific skill), or remains unique to this one shield.

## IMPORTANT CAVEAT (per user, July 6 2026) — set bonuses appear in the real tooltip/stats but do NOT belong to the item

Set items grant extra lines like "(2) Set: +100% to All Retaliation Damage" / "(N) Set: ..." when
the player has N+ pieces of the same set equipped. These bonus lines/values **are conditional on
the player's other equipped gear**, not part of the item's own roll — they show up in the real
tooltip (`ReplicaItemRow`) / "real stats" query results, but have **no corresponding field in the
item's own `DatabaseItemStat_v2` row**. If a stat appears in the real tooltip but is absent from
the item's own DB stats, **check whether the item is part of a set (`itemSetName` field) and
whether that stat/value matches one of the set's bonus tiers before treating it as an unexplained
missing/phantom stat** — this is a strong, likely-cause explanation, not just one hypothesis among
many. (Already seen concretely: the Ultos set items showing a second, larger "267-822 Lightning
Retaliation" line distinct from their own roll — see the c009_shield investigation notes below.)
This does NOT retroactively explain the d224_torso case just resolved above (that was a wrong-join
bug, and was independently confirmed the item isn't showing any set-only phantom line issue), but
applies to any FUTURE "stat present in tooltip but missing from DB" mystery — check set membership
first.

## Session update (July 6 2026, newest — d224_torso "phantom Pierce"/"impossible 44% Elemental Resistance" mystery RESOLVED: it was never a real bug, just a bad manual DB join in prior investigation sessions

Followed up on the "quirk tables likely mask a missed general rule" pushback below by chasing the
most Ghidra-tractable lead: the shared mechanism behind `d110_focus`/`d111_torso`/`d224_torso`'s
"inherent template bonus" (values allegedly impossible under `cjit` of the DB base).

**Ghidra pass (background agent): confirmed no new C++ mechanism exists.** Re-checked
`DefenseAttributeAbs_AllResistance` (ctors `0x1801e84e0`/`0x1801e8510`/`0x1801eb090`) as a new
candidate (a real DBR field that could apply a bonus across all resistances from one row) — bare
ctors only, no `AddJitter`/`GetValue` override, same as every other already-ruled-out resistance
class. Searched for `Template`/`Tooltip`/`Quality`/`Inherent`/etc-named functions — nothing exists
that builds tooltip stat text or applies a second-pass bonus. `ArmorProtective_*` siblings are all
one-line ctors. **No override class or second-pass function found anywhere in the binary** — if a
mechanism exists, it isn't identifiable C++ code, consistent with all prior sessions' findings.

**DB-investigation pass: found the real explanation.** Directly queried `userdata-test.db` (Python
`sqlite3`, no CLI available) for `d224_torso` (`id_databaseitem=20359`). `DatabaseItemStat_v2`
**already contains** `defensiveElementalResistance=45.0` and `defensivePierce=24.0` — **not** the
previously-documented base=25/missing-entirely. Those base values comfortably jitter to the real
tooltip's "44% Elemental Resistance" / "20% Pierce Resistance" — there is no phantom stat and no
impossible value. The prior sessions' "25"/"missing" data point was simply wrong.

Root cause of the wrong data: this DB's tooltip table requires a **two-hop join** —
`PlayerItem.Id` → `ReplicaItem2.playeritemid` → `ReplicaItem2.Id` → `ReplicaItemRow.replicaitemid`.
Querying `ReplicaItemRow` directly by `PlayerItem.Id` (skipping the `ReplicaItem2` indirection)
silently returns a **different item's tooltip** — verified concretely: `PlayerItem.Id=3514` is
`d224_torso` (seed 1140888749), but `ReplicaItemRow` rows where `replicaitemid=3514` belong to an
unrelated "Dread Lord's Ugdenbog Waistguard of Blood" (a rare belt). The correct join
(`ReplicaItem2.Id=1895` for `playeritemid=3514`) returns the real "Goredrinker's Armor" tooltip,
which matches the DB's actual base stats (45/24) via ordinary jitter — no anomaly at all. This
wrong-join mistake was made in prior sessions' manual/ad-hoc SQL when hand-verifying this one item,
**not** in `gdvalidate.py` itself — the validator already joins correctly (`tools/gdvalidate.py:386`,
`ReplicaItem2 ri join ReplicaItemRow rr on rr.replicaitemid=ri.Id`), so the automated corpus results
were never actually wrong for this item.

**Confirmed via a fresh `gdvalidate.py` run**: `d224_torso` and `d111_torso` do **not** appear
anywhere in the current mismatch output — they were never failing in the real validator, only in
the separate hand-investigation that used the wrong join. The actual remaining base-only mismatches
are just **`d110_focus`** (2 seeds, `characterSpellCastSpeedModifier` — genuinely impossible under
`cjit`, still an open "inherent template bonus" case with no known mechanism) and **`c009_shield`**
(1 seed, the "823 Fire Retaliation" single-line display formula, still open). Both are unrelated to
each other and to the resolved d224_torso case — no shared general rule between them has been found,
and this session's finding suggests the "6 unexplained quirks share one general cause" theory below
should be treated with more skepticism per-case (at least one of the "quirks" was pure investigation
error, not a real quirk). **No code change needed** in `gdvalidate.py`/`ItemStatEngine.cs` — nothing
was ever broken there. This is documentation-only: d224_torso and d111_torso should be considered
**resolved/non-issues**, not part of the open quirk list.

**Lesson for future sessions**: when hand-verifying a `PlayerItem` row's real tooltip via raw SQL,
always join through `ReplicaItem2` (`PlayerItem.Id = ReplicaItem2.playeritemid`, then
`ReplicaItem2.Id = ReplicaItemRow.replicaitemid`) — never assume `PlayerItem.Id ==
ReplicaItemRow.replicaitemid` directly, even though the IDs can coincidentally look plausible.

## OPEN INVESTIGATION — quirk tables likely mask a missed general rule, not real per-item exceptions

Per-user pushback (July 6 2026): the 3 quirk tables in `gdvalidate.py` (`NODRAW_QUIRKS`,
`SINGLE_SIDED_QUIRKS`, `BASE_OVERRIDE_QUIRKS`) plus the 2 accepted "inherent template bonus" cases
(`d110_focus`, `d111_torso`) and the still-open `d224_torso`/`c009_shield` residuals were each
confirmed empirically (real seeds + tooltips) but **none has a Ghidra-confirmed mechanism**. Six+
independent "random one-off quirks" with no shared code cause is implausible — far more likely each
is an instance of ONE general rule we haven't spotted yet (e.g. a field-class property, a set/skill
interaction, or a draw-order dependency on some condition not yet in the model), and we're
under-sampled (mostly n=1 or n=2 per record) to see the pattern. Treat every entry below as **"needs
more test data / more Ghidra RE"**, not settled, until a shared mechanism is found or firmly ruled out.

Per-quirk, what would move it forward:

- **`c019_necklace` `offensiveLifeMin/Max` no-draw** (n=2 seeds) — need other necklaces/rings with
  the same field to see if it's necklace-class-wide, or find a shared trait (e.g. does this record
  have some other field/flag the drawing ones lack?). Compare against `d107_ring`/`c024_blunt`/
  `c020_axe` (already confirmed those DO draw it) for a structural difference (class, skill grant,
  set membership, itemclass, faction, anything in the raw DBR/DB row that's shared by the
  no-draw group and absent from the draw group).
- **`d008_necklace` `characterOffensiveAbility` no-draw** (n=1) — same treatment; only one sample
  exists, actively look for more `characterOffensiveAbility`-bearing accessories to test against.
- **`c025_shield` `characterLife` no-draw** (n=1) — same; find other shields/off-hands with
  `characterLife` to test.
- **`c009_shield` `retaliationFire` single-sided (Max doesn't draw) + the "823" display mismatch**
  (n=1) — need more retaliation-flat-pair shields/items that also grant `Aura of Censure` or another
  retaliation-boosting skill, to test the skill-contribution hypothesis with a 2nd data point, and
  more single-record samples generally to see if "Max doesn't draw" recurs elsewhere.
- **`c016_axe2h` (upgraded) `retaliationTotalDamageModifier` base-override (150 vs DBR's 200,
  matches the linked set record's own copy)** (n=4 seeds, 1 record) — the "matches the set's own
  value" coincidence is the most promising lead of the six: systematically scan the DB for OTHER
  set-member records where the equip item's field value differs from the set record's own copy of
  the same field name, and check whether those consistently roll off the SET's value instead of the
  equipped item's DBR value. If confirmed on more pairs, this becomes a general "set-member fields
  that shadow a set-record field roll from the set record" rule, not a single hardcoded override.
- **`d105_blunt` `characterLife` base-override (302 vs DBR's 402)** (n=128 seeds, but only 1 record)
  — no sibling/set overlap found yet; look for ANY other base record where `DatabaseItemStat_v2`'s
  stored value disagrees with what a large seed sample resolves to, to see if "DB value diverges
  from roll base" is itself a recurring, explainable phenomenon (e.g. stat was patched/rebalanced
  after these DB rows were captured, and the game reads a different table than
  `DatabaseItemStat_v2` — worth checking DB schema for a second "current" value column/table).
- **`d110_focus`/`d111_torso` "inherent template bonus"** (n=2, n=1) — same category as above;
  actively hunt for more focus/torso items with the same affected fields to see if the bonus amount
  is a fixed constant, a % of base, or tied to item level/rarity — currently 0 formula candidates
  tested successfully.
- **`d224_torso` phantom Pierce Resistance + inflated Elemental Resistance** (n=1) — needs either
  more Fire→Pierce-conversion chest items with `defensiveElementalResistance`, or the template-DBR
  parsing code location in Ghidra (not yet found) — see the dedicated write-up immediately below for
  what's already been ruled out.

**Action for next session(s):** before adding any more per-record quirks, first do a broad DB sweep
per bullet above to gather more samples on the SAME field across DIFFERENT records, specifically
looking for a shared structural trait (set membership, linked skill, item class, upgraded-vs-base,
DB-vs-actual-value drift) rather than accepting a new n=1 special case. This list should be updated
(items removed once explained, e.g. moved into a real formula) each time new data arrives.

## Session update (July 6 2026, newest — d224_torso reclassified via Ghidra RE: likely same "inherent template bonus" category as d110_focus/d111_torso; still NOT fixed, no formula)

Per user request, dug further into `d224_torso`'s two unexplained resistance numbers (Elemental
25→44%, phantom "20% Pierce Resistance" with no `defensivePierce` in the DB) using Ghidra, since
data-comparison approaches were exhausted in prior sessions.

**Ghidra findings:**
- `GAME::DefenseAttributeAbs_ElementalResistance` ctor (`0x1801eaed0`) — new detail not previously
  documented: this class has a **`defensiveElementalResistanceChance`** field (offset `this+0x10`)
  paired with `defensiveElementalResistance` (`this+0x48`), same shape as the already-known
  Reflex/CC `*Chance` fields (tag `"DefenseElementalResistance"`, index `0x3a`). No override of
  `AddJitter`/tooltip virtuals found on this class — it's a bare `DefenseAttributeAbs` subclass,
  same mechanism as every other resist field. **This rules out a special-cased formula living in
  this class** — the 44% value is not explained by anything unique to Elemental Resistance's own
  code.
- No `PierceResistance`-named class exists in the binary at all, and `"Pierce Resistance"` is not a
  static string (consistent with `StatManager.cs`'s `BodyDamageTypes` template-based tag naming) —
  ruled out a dedicated Pierce-resistance code path.
- Traced the Conversion mechanism (`ItemEquipment::GetConversionAttributes` @`0x180325e30` →
  `FUN_180169660` @`0x180169660`, the per-slot iterator for the item's own `conversionInType/
  OutType/Percentage`) — confirmed this only feeds a generic `ConversionAttributeAccumulator::
  AddAttribute(inType, outType, pct)` used for **damage conversion math**, nothing resistance- or
  tooltip-related. **Ruled out**: the Fire→Pierce conversion pair on this item does not mechanically
  synthesize a Pierce Resistance stat.
- Checked `ArmorProtective_Chest` (the item's own class) — only a constructor exists in the binary,
  no `Load`/stat-init override, meaning there's no per-class C++ code injecting extra chest-slot
  resistances. This matches the existing precedent for `d110_focus`/`d111_torso`: those "inherent
  template bonus" values were *also* never traceable to C++ code — they're presumed to come from
  the game's **data-driven DBR template files** (not present in this repo/DB; `userdata-test.db` has
  no template/baseline table, only `DatabaseItemStat_v2`'s rolled values), which apply a fixed
  bonus at display time outside the seed roll entirely.

**Conclusion (reclassification, not a fix):** `d224_torso`'s two numbers fit the exact same
signature as the already-accepted `d110_focus`/`d111_torso` cases — jittered value is **provably
outside the possible `cjit` range of the DB base** (44% vs max ~30% for base 25), and no Ghidra
class, set record, or conversion mechanism explains it. Reclassifying `d224_torso` from "genuinely
unexplained, unique" to **"same inherent-template-bonus class of issue as d110_focus/d111_torso,
just two bonuses on this item instead of one"** — this doesn't fix it (the exact bonus formula is
still unknown, same as the other two cases, which have been open for many sessions), but it means
there is no NEW bug class here, and further progress needs either the actual DBR template files
(external to this repo) or more Ghidra RE of wherever template DBRs get parsed/applied (not yet
located this session). **No code change** — same reasoning as d110_focus/d111_torso, a partial/
guessed formula isn't safe to commit.

`c009_shield`'s "823 Fire Retaliation" was not revisited this session (ran out of budget after the
d224_torso trace); still open per the prior session's notes (Aura of Censure skill-contribution
hypothesis, unconfirmed, n=1).

**Result:** no change to item_ok (1046/1051, 5 BAD) — investigation-only session, no code changes.
`dotnet test` unaffected.

## Session update (July 6 2026, newest — d105_blunt characterLife: FIXED via base-override quirk, using a new 128-seed sample)

The user supplied a new data file, `D:\Dev\iagrim\GrimDawnSeedStats\d105_blunt.txt` (TSV: baserecord,
Seed, DatabaseStats JSON, ReplicaRows JSON), containing **128 real seeds** of this exact base record
(`records/items/gearweapons/blunt1h/d105_blunt.dbr`) — a huge upgrade from the single seed
(438508348) all prior sessions had to work with. This resolved the long-standing n=1
underconstrained mystery.

**Reconciled the "373 vs 351" / "base 402" confusion from earlier docs first:** the DB's
`DatabaseItemStat_v2`/new TSV's `DatabaseStats` both agree the DBR's own `characterLife` value is
**402** (the "373 predicted" figure in earlier sessions was this session's/prior sessions'
*naive-model prediction* using base=402/pct=20%, not a different base value — no actual
discrepancy, just imprecise phrasing in the older note).

**Method:** imported `Rng`/`cjit` directly from `gdvalidate.py` (`sys.path.insert` + import, per
project rule — no reimplementation). This record only has 3 present CHAR fields: `characterLife`,
`characterDefensiveAbility`, `characterAttackSpeedModifier` (in that draw order; all other CHAR
fields are absent or FIXED, e.g. `characterBaseAttackSpeed`). First confirmed RNG sync/position:
replaying `characterDefensiveAbility` and `characterAttackSpeedModifier` (the two draws immediately
following `characterLife`) under the existing model matched their real tooltip lines on **128/128**
seeds exactly — proving the draw position and RNG state entering/leaving `characterLife`'s draw are
already correct; only the value at that position was wrong. Plain `cjit(402, pct=20)` matched
**0/128** real "+N Health" lines. Brute-forced `pct` alone (402 fixed, pct 0.1 to 200 step 0.1)
against all 128 seeds' raw RNG draws — best fit only 3/128 (not remotely close, ruling out "wrong
pct" entirely). Then did a full 2D grid search over `(base in 1..1000) x (pct in 5.0..40.0 step
0.5)` against all 128 seeds simultaneously: found a **single unique solution, base=302, pct=20%**
(the record's normal default pct), fitting **all 128/128 seeds exactly** — confirmed with the real
`cjit` function bit-for-bit (not just the brute-force's rounded predicate). Next-best candidate
anywhere in the grid only fit 7/128, so this is not a coincidental fit.

**302 is not explained by any sibling record**: the item's set (`itemset_d106.dbr`, "Stoneguard",
3-piece) has no `characterLife` field at all (confirmed via direct DB query of its stat rows), and
there is no upgraded variant of `d105_blunt` in the DB (`select distinct baserecord ... where
baserecord like '%d105_blunt%'` returns only the one path) — so unlike `c016_axe2h`'s override
(which coincided with its linked set record's own copy of the same field), this one has no
discovered structural cause. Treated the same way as `c016_axe2h`: a confirmed-by-data,
not-yet-Ghidra-confirmed per-record quirk.

**Fix:** added `('records/items/gearweapons/blunt1h/d105_blunt.dbr', 'characterLife'): 302.0` to
the existing `BASE_OVERRIDE_QUIRKS` table in `gdvalidate.py` (same table/pattern as the
`c016_axe2h` `retaliationTotalDamageModifier` entry). Generalized `quirk_base_override()`'s call
site: it was previously only invoked from the `retalmod` branch; moved the lookup into the shared
generic-field fallback (covers `char`/`dmg`/`def`/`skill`/`retalmod` alike) so CHAR-store fields can
also be overridden — harmless for all other records since the lookup table only has these two
specific `(baserecord, field)` keys.

**No C# port**: grepped `ItemCorpusTests`'/fixture files for `d105_blunt` — zero hits, confirmed
absent from the C# test corpus, same exception precedent as `c016_axe2h`.

**Result:** `gdvalidate.py` item_ok **1045→1046/1051**, BAD **6→5** (d105_blunt's single mismatch
resolved, zero regressions — every other previously-passing item still passes). `dotnet test`:
unchanged, **43 pass**, 1 skip (no C# changes, per the confirmed absent-from-corpus exception).

**Remaining 5 BAD**: `d224_torso` (n=1, TWO unexplained resistance numbers — Elemental AND phantom
Pierce), `c009_shield` (1 residual display-only "823 Fire Retaliation" formula unknown),
`d110_focus`/`d111_torso` (pre-existing accepted "inherent template bonus" cases, formula still not
pinned down, same as always — 2 seeds/1 seed respectively, same root class of issue).

## Session update (July 6 2026, newest — c009_shield "823" investigated (still open); d224_torso red-herring theory tested and REFUTED, still open)

Followed up on two specific user hints for the two remaining n=1 mysteries. No code changes this
session (investigation only); `dotnet test` unaffected (43 pass, 1 skip).

**`c009_shield` "823 Fire Retaliation" — investigated per user's `ProccessBodyRetaliation` lead,
still unresolved.** Read `StatTranslator/StatManager.cs` `ProccessBodyRetaliation` (~379-414) and
`StatTranslator/EnglishLanguage.cs` tag definitions: `customtag_013_retaliation` = `"{0}-{1} {3}
Retaliation"` (range, used when a matching `*Max` field exists), `customtag_03_retaliation` =
`"{0} {3} Retaliation"` (single value, used when no `*Max` exists) — these are IA's OWN
tooltip-reconstruction tags, not proof of the real game engine's behavior; the DB `ReplicaItemRow`
text is the actual game-rendered tooltip and is the ground truth either way.

Queried the DB broadly for **every** base-only item with both `retaliation{Type}Min` AND
`retaliation{Type}Max` present (10 damage types, ~15 samples across Physical/Fire/Cold/Lightning/
Poison). **Every single other sample shows a proper `"N-M Type Retaliation"` range line** (e.g.
`c022_shoulder` "229-317 Physical Damage Retaliation", `c017_gun1h` "11-43 Fire Retaliation",
`c016_axe2h`/`c023_head`/`c021_torso`/`c015_hands` all "N-M Lightning Retaliation") — `c009_shield`
seed 1475933991 is the **only** sample in the whole corpus that collapses to a single number
despite having both Min and Max in the DB. This rules out "single combined number when both exist"
as a general rule — `c009_shield` is a genuine outlier, not evidence of an unknown general code path.

Interestingly, several of those other samples (all `c016_axe2h`/`c023_head`/`c021_torso`/
`c015_hands`, all sharing the Mythical `itemset_c017b.dbr` "Ultos" set) show **two** Lightning
Retaliation lines: their own range (e.g. "35-318") AND a second, larger flat range ("267-822") from
the set's granted bonus — confirming **set items really can inject a second same-type Retaliation
tooltip line** distinct from the item's own roll. This raised the hypothesis that c009_shield's
"823" might likewise be a second (skill/aura-granted) line rather than the item's own roll — but
`c009_shield` (`Dawnshard Bulwark`) is **not** a set item (no `itemSetName` field, confirmed via its
full DB stat dump) and its DB augment-skill fields (`Temper` lvl2, `Aura of Censure` lvl2) are
granted-skill augments, not set bonuses. So the "set bonus line" mechanism that explains the Ultos
items does not directly apply here, though it raises a related possibility: **`Aura of Censure`**
(a real Grim Dawn Soldier skill, granted at level 2 by this item) is itself a damage-retaliation
aura and could plausibly contribute its own Fire Retaliation number to the "823" total, independent
of the item's own DB-rolled `retaliationFireMin/Max`. This can't be confirmed from the available DB
(no skill-effect-value table — `itemskill_v2`/`itemskill_mapping` only store descriptions, not
scaling values) and there's only 1 seed sample for this exact base record in the DB, so it remains
n=1 and unconfirmed. Re-verified the existing `SINGLE_SIDED_QUIRKS` fix is still correct RNG-wise:
replaying the full order WITHOUT forcing single-sided (treating Min+Max as a real min+spread pair)
produces `jmn=325, jmx=425` but then desyncs `retaliationTotalDamageModifier` (66 vs expected
55/56) and `defensiveElementalResistance` (34 vs expected 38) — confirming single-sided (1 draw) is
the right RNG-state model; the "823" mismatch is purely a display-value question, not a draw-order
one. Tried `jmn+raw_max` (730), `jmn*(1+mod)` (507), `(jmn+raw_max)*(1+mod)` (1139) — none land on
823. **Left open, exactly as before** (contained, non-desyncing, still the single known-mismatch
line on this item). Next step if resumed: find ANY other seed on this exact base record, or any
other item granting `Aura of Censure`/`Temper` with a retaliation stat, to test the skill-
contribution hypothesis with a second data point.

**`d224_torso` "phantom 20% Pierce Resistance" — user's red-herring/set-bonus-text theory TESTED
AND REFUTED; reclassified from "possible set-bonus noise" back to "genuinely unexplained, still
open."** Confirmed `d224_torso` ("Goredrinker's Armor") **is** part of a 4-piece set
(`itemset_d221.dbr`, "Goredrinker") — the user's premise was correct that far. However, checking
the full real tooltip (pulled via `ReplicaItemRow`) shows the "20% Pierce Resistance" and "44%
Elemental Resistance" lines sit in the **item's own type-19 stat block**, well before the "(2)/(3)/
(4) Set" bonus divider lines (type 23/28) — i.e. the game engine itself attributes these lines to
the base item, not to set-bonus text. To directly test whether this is set-wide behavior, queried
the DB for the item's own set-sibling `d224_shoulder` (same `itemset_d221.dbr`, same
`defensiveElementalResistance=25` base, same `conversionInType=Fire/OutType=Pierce/Percentage=30`,
same `offensivePierceModifier=48` base) — **its real tooltip shows a normal jittered "21% Elemental
Resistance" and NO Pierce Resistance line at all.** Also checked all OTHER base-only Fire→Pierce
conversion items in the corpus (`d220_necklace`, `d224_shoulder`) — neither shows a synthetic Pierce
Resistance line, ruling out "Fire→Pierce conversion always emits a Pierce Resistance line" as a
general rule too. **Conclusion: the phantom Pierce Resistance line is specific to `d224_torso`
alone, not explained by set membership, not explained by the conversion pairing, and not sourced
from set-bonus text** (the set's own bonus lines are a separate, clearly-delimited tooltip section
that doesn't overlap). The user's specific theory is **refuted** by direct DB comparison — this
stays genuinely open, unchanged from prior sessions (still needs either the item's exact Ghidra
class behavior for `defensiveElementalResistance`+conversion pairing, or a wider sample of similar
items, neither available this session).

**Result:** item_ok unchanged at **1045/1051**, BAD unchanged at **6** (no fixes landed — both
leads were investigated per user request and conclusively narrowed/ruled out, not blindly retried).
`dotnet test`: unchanged, 43 pass, 1 skip (no code changes this session).

## Session update (July 6 2026, newest — c016_axe2h retaliationTotalDamageModifier: FIXED via base-override quirk)

Picked up the priority list from the previous handoff (c016_axe2h, d105_blunt, c009_shield residual,
d224_torso). One clean fix found; the other three remain genuinely unresolved (confirmed, not just
re-tried) after deeper hand-replay.

**`c016_axe2h` (upgraded) `retaliationTotalDamageModifier` — FIXED.** The previous session only had
2 upgraded seeds; this session found **4** (`retaliationTotalDamageModifier` appears on many other
upgraded records too, listed via a DB scan of `DatabaseItemStat_v2`, but only `c016_axe2h` and
`c009_shield` had real PlayerItem/tooltip seeds available). Re-verified draw **position** is
correct on all 4 seeds (the preceding `retaliationLightningMin/Max` flat pair, drawn immediately
before it, matches every seed's tooltip exactly, confirming RNG state entering the draw is right).
The mismatch is specifically the item's own **"+N% to All Retaliation Damage"** tooltip line
(distinguished this session from the **separate** "(2) Set: +100% to All Retaliation Damage" line
also present — this is a Mythical set item, `itemset_c017b.dbr`, and earlier sessions' "184 vs
observed" comparisons may have conflated the two lines). Full grid brute-force of `(base in
1..400) x (i in 1..400)` against all 4 seeds' raw RNG draws at the confirmed position found a
**single unique solution: base=150, pct=20%** (the item's own DBR value is 200) — fits all 4 seeds
exactly, not a coincidence (4 independent constraints, 1 solution). Curiously, **150 is exactly
the linked item-set record's (`itemset_c017b.dbr`) own copy of `retaliationTotalDamageModifier`**
(same field name, different value than the equip item's 200) — though the causal mechanism isn't
Ghidra-confirmed (the set record's copy is presumably meant for its granted item-skill's damage,
not as an override of the equip stat; a broad DB scan found ~852 cases of set/member stat-name
overlaps across 209 sets, most looking like ordinary independent set-bonus values, not overrides,
so this is NOT assumed to be a general "set members borrow the set's base" rule). Implemented as a
new **`BASE_OVERRIDE_QUIRKS`** per-record lookup table in `gdvalidate.py` (same pattern/precedent
as `NODRAW_QUIRKS`/`SINGLE_SIDED_QUIRKS`), keyed by `(baserecord, field) -> override value`, applied
in the `retalmod` branch before the `cjit` call. **No C# port** — confirmed `c016_axe2h` is not in
`ItemCorpusTests`' fixture corpus (grepped, zero hits), same reasoning as prior quirk-table entries.

**Not fixed, re-investigated and confirmed still open:**
- **`d105_blunt` `characterLife`** (373 predicted vs 351 real, seed 438508348): also a set item
  (`itemset_d106.dbr`, "Stoneguard" 3-piece), but the set record has **no** `characterLife` field to
  test the same override theory against (set record's fields: `defensiveBonusProtection`,
  `defensiveProtectionModifier`, `defensiveAbsorptionModifier`, `retaliationPhysicalModifier` — no
  overlap). Brute-forced `(base,i)` against the single raw RNG draw: hundreds of solutions exist
  (n=1 is fundamentally underconstrained, unlike the 4-seed c016_axe2h case) — genuinely
  unresolvable without a second seed on this exact record. Left open.
- **`d224_torso` `defensiveElementalResistance`** (base 25 → real tooltip 44%, seed 1140888749):
  re-replayed the FULL draw chain by hand (characterLife 850→761, characterOffensiveAbility 60→55,
  offensivePierceModifier 48→75%(scaled), offensiveSlowBleedingModifier 60→78%,
  offensiveSlowBleedingDurationModifier 30→44% — **all 5 match the real tooltip exactly**), confirming
  the RNG state is correct going into Defense. `defensiveLife` (18 base) is still missing from the
  tooltip like before. But **44% is impossible under any cjit of base 25** (max jittered range is
  [20,30] even skipping defensiveLife's draw) — ruled out plain reorder/skip theories entirely. Also
  discovered the tooltip shows **"20% Pierce Resistance" despite `defensivePierce` not existing
  anywhere in this item's DB stats** — i.e. there are (at least) TWO unexplained resistance numbers
  on this item, not one, and neither is explained by the item's own linked set record
  (`itemset_d221.dbr`, checked, no overlapping resistance field). Genuinely unresolved; needs either
  more Ghidra RE (possibly `defensiveElementalResistance`'s Ghidra class does something with the
  item's `conversionInType=Fire/OutType=Pierce` pair to also emit a synthetic Pierce resistance
  line, which would be new/unknown behavior) or more DB samples of Fire→Pierce conversion items
  with defensiveElementalResistance to triangulate a formula. Left open, no quirk added (a
  single-field quirk wouldn't explain the second unexplained Pierce Resistance line anyway).
- **`c009_shield` "823 Fire Retaliation" residual** (seed 1475933991, a different seed than the one
  used to establish the `SINGLE_SIDED_QUIRKS` fix): re-confirmed single-sided draw (1 draw, Min
  only) is still correct — with it, `retaliationTotalDamageModifier` (56%) and
  `defensiveElementalResistance` (38%, happens to equal base by jitter coincidence) both match the
  tooltip exactly. Jittered Min=325. Tried `mn*(100+pct)/100` for `pct` 0..300 against the displayed
  823 — no integer pct reproduces it (823/325=2.53x, not a plausible modifier stack). Still an
  unexplained pure-display formula, contained (does not desync any other field) — left open exactly
  as before.

**Result:** item_ok 1041→**1045/1051**, BAD 10→**6** (c016_axe2h's 4 seeds all fixed). `dotnet
test`: still 43 pass, 1 skip (no C# changes — quirk-table-only fix).

**Remaining 6 BAD, triaged (all re-confirmed unresolved this session, not re-tried blindly):**
`d105_blunt` (n=1, characterLife value unexplained), `d224_torso` (n=1, TWO unexplained resistance
numbers — Elemental AND phantom Pierce), `c009_shield` (1 residual display-only line, "823"
formula unknown), `d110_focus`/`d111_torso` (pre-existing accepted "inherent template bonus" cases,
formula still not pinned down, same as always).

## Session update (July 6 2026, newest — 3 more per-record "phantom draw" quirks found & fixed)

Continued working the remaining BAD list after the Physical-Retaliation fix (below). Found and
fixed **3 more cascading-desync bugs**, all the same shape as the already-known `d008_necklace`
`characterOffensiveAbility` oddity: a specific base record has one field that the real game does
NOT draw/display at all (0 draws), while every other field of that same type draws normally on
every other sampled record — and because our model draws it anyway, every downstream field on
that item was silently desynced by one RNG step. Found via the same technique each time: hand-
replay the RNG draw chain with `Rng`/`cjit`/`scale` imported from `gdvalidate.py`, compare against
`DatabaseItemStat_v2` (base values) + `ReplicaItemRow` (real tooltip) for the exact seed, and test
whether skipping one specific draw makes every other field on the item match exactly.

- **`c019_necklace`**: `offensiveLifeMin/Max` (base 4-8) never appears in the tooltip on **2
  different seeds** of this record (289972429, 547153716) — confirmed NOT a general
  "jewelry doesn't roll Vitality flat" rule (other jewelry/rings/weapons with the same field, e.g.
  `d107_ring`, `c024_blunt`, `c020_axe`, all draw and display it normally). Skipping its 2 draws
  fixes `offensiveLifeModifier` on both seeds exactly (36, 39 — previously off by 1 and 5).
- **`c025_shield`**: `characterLife` (base 600) never appears in the tooltip (seed 1792341605).
  Skipping its 1 draw fixes **6** downstream fields exactly (characterDefensiveAbility,
  offensivePhysicalModifier, retaliationPhysical flat pair, retaliationTotalDamageModifier,
  defensivePhysical, defensiveFire) — this was the single worst cascade found this session.
- **`c009_shield`**: `retaliationFireMax` (405, alongside Min=320) is present in the DB but does
  NOT consume a 2nd draw — forcing it to single-sided (Min-only, 1 draw) fixes
  `retaliationTotalDamageModifier`, `defensiveElementalResistance`, and `conversionPercentage`.
  **Not fully resolved**: the real tooltip shows a single combined `"823 Fire Retaliation"` line
  (not a `"N-M Fire Retaliation"` range), and 823 is not explained by any simple function of the
  jittered Min (325) and the raw Max (405) tried so far — left as one remaining known-mismatch
  display line on this item (down from 3 mismatches to 1).

Implemented as two small lookup tables in `gdvalidate.py` gated by exact `baserecord` path —
**`NODRAW_QUIRKS`** (field never draws) and **`SINGLE_SIDED_QUIRKS`** (pair treated as Min-only) —
rather than any change to the general field-order/formula tables, since each is confirmed specific
to one exact record, not a type/class-wide rule. **No C# port needed**: same reasoning as the
Physical-Retaliation fix — `ItemStatEngine.cs` only computes rolled values from the field-order
tables, it has no per-record exception mechanism and doesn't need one since these 3 exact records
aren't in its 15-item test corpus.

**Result:** item_ok 1037→**1041/1051**, BAD 32→**10** (7 fixed cleanly, 1 partially on c009_shield
leaving 1 residual). `dotnet test` still 43 pass, 1 skip (unaffected).

**Remaining 10 BAD, triaged:**
- **`c016_axe2h`** (4 seeds) — the already-documented upgraded-record `retaliationTotalDamageModifier`
  issue (see the dedicated section below); still unresolved, needs more upgraded-record samples.
- **`d110_focus`** (2 seeds) `characterSpellCastSpeedModifier` and **`d111_torso`** (1 seed)
  `defensiveChaos` — these are the **pre-existing, intentionally-accepted "inherent template
  bonus" cases** flagged all the way back in this doc's original status section ("focus cast-speed
  ×2, torso chaos-resist... added at DISPLAY, not rolled"). Confirmed again this session: the
  jittered values are provably outside the possible `cjit` range of the base (e.g. d111_torso base
  18 → real tooltip 25%, but max possible jitter is ~21), consistent with a fixed bonus added on
  top at display time, not a roll error. Formula for the exact bonus amount is still not pinned
  down (not simply base×2 or a flat +N in the samples checked) — not fixed, same as originally
  documented.
- **`c009_shield`** (1 residual field) — see above, `"823 Fire Retaliation"` display formula
  unexplained.
- **`d105_blunt`** (1 seed) `characterLife`: NEW finding — unlike the other cases, the draw
  *position* is provably correct here (the next 2 CHAR draws, `characterDefensiveAbility` and
  `characterAttackSpeedModifier`, both match exactly using the assumed order), but the *value* at
  that position (373) doesn't match the real tooltip (351) for any reasonable alternate jitter
  `pct` (brute-forced i=1..401 against the raw RNG draw; only a nonsensical i=359/pct≈89% fits
  exactly) — genuinely unexplained, not a simple reorder or skip-draw fix. Left unresolved, n=1.
- **`d224_torso`** (1 seed) `defensiveLife`: NEW finding, only partially diagnosed. `defensiveLife`
  (base 18) is missing from the tooltip like the other quirks above, but skipping its draw does
  **not** fix the next field (`defensiveElementalResistance`, base 25 → real tooltip 44%, which is
  impossible under plain `cjit` regardless of whether `defensiveLife` draws or not — 44 is far
  outside any reasonable jitter of 25). This means d224_torso has a **second, distinct** desync
  source beyond the missing `defensiveLife` draw; not chased further this session (no quirk added,
  since the single-field skip doesn't fully resolve the item — would need to also explain the
  Elemental Resistance jump before it's safe to commit a partial fix).

## Session update (July 6 2026, newest — Physical-Retaliation display wording bug: found & fixed)

Picked up "remaining known BAD items" per user request. Found and fixed a real display-formatter
bug (not a roll bug): **the game's Retaliation tooltip lines only include the word "Damage" for the
`Physical` type** — e.g. `"229-317 Physical Damage Retaliation"` and `"+100% Physical Damage
Retaliation"` — while every other damage type omits it (`"325-425 Fire Retaliation"`, `"+250%
Lightning Retaliation"`, `"+50% Acid Retaliation"`, etc). Confirmed via a broad DB scan of all
`%Retaliation%` tooltip lines (`select distinct Text from ReplicaItemRow where Text like
'%Retaliation%' ...`) — the pattern was 100% consistent across dozens of samples, no exceptions in
either direction. `gdvalidate.py`'s `retal_line()` (flat pair display) and the `retaliation{Type}
Modifier` branch of `fmt()` both unconditionally added/omitted "Damage" incorrectly; fixed both to
gate on `dtype/m.group(1) == 'Physical'`. **No C# port needed** — `ItemStatEngine.cs` only computes
rolled values, it has no display/tooltip-formatting code at all (confirmed via grep), so this fix
is Python-validator-only. **Result:** item_ok 1032→**1037/1051**, BAD 37→**32**. `dotnet test`
still 43 pass, 1 skip (unaffected, no C# changes).

Looked at the newly-visible `c019_necklace` mismatch surfaced by this fix (`offensiveLifeModifier`
30→expected 37 vs actual tooltip 36, off-by-one; `offensiveLifeMin/Max` flat pair 4-8→6-9 entirely
**absent** from the real tooltip despite nonzero DB base) — this is the same **n=1 long-tail
oddity** category as the existing `d008_necklace` `characterOffensiveAbility`-missing case (a field
that should draw/display but doesn't, isolated to one item), not a new systemic bug. Not chased
further, per the "don't chase single-item coincidences without more samples" lesson from earlier
sessions.

**Remaining ~32 BAD** are the same known long-tail: `c016_axe2h` upgraded-record
`retaliationTotalDamageModifier` (2 seeds, see below), `d110_focus`/`d008_necklace`
`characterSpellCastSpeedModifier` mismatches, `d008_necklace` OA-missing (n=1), `c019_necklace`
flat-missing/off-by-one (n=1, new today), and `c009_shield`/`c022_shoulder` cases that look like
**another upgraded-record variant of the same c016_axe2h issue** (worth checking next: whether
`c009_shield`/`c022_shoulder` are upgraded records too, to test the "upgraded records use parent's
base value" theory with more samples).

## Session update (July 6 2026, newest — c016_axe2h retaliationTotalDamageModifier: narrowed, NOT fixed)

Investigated the `c016_axe2h` `retaliationTotalDamageModifier` mismatch flagged as "confirmed NOT a
position issue, isolated single-item" in the prior handoff. Replayed the full draw chain by hand
(rng.nxt() call-by-call) for both the **non-upgraded** (`records/items/gearweapons/melee2h/
c016_axe2h.dbr`, base=80) and **upgraded** (`records/items/upgraded/gearweapons/melee2h/
c016_axe2h.dbr`, base=200) variants, against 5 base-only seeds + 2 upgraded seeds.

- **Non-upgraded: exact match on all 5 sampled seeds** (83, 93, 64, 88, 86 — all reproduced exactly
  by `cjit(80, rng)` at the existing draw position, right after the `RETAL_FLAT` Lightning pair).
  Every other field on this item (`characterLifeModifier`, `characterAttackSpeedModifier`,
  `offensiveLightningModifier`, `offensiveSlowLightningModifier`, `retaliationLightningMin/Max`)
  also matches exactly — draw position and formula are fully correct for the base item.
- **Upgraded: both sampled seeds (1535743216→observed 133, 1184141686→observed 159) are OUTSIDE
  the possible `cjit(200,20%)` range of [160,240]** — i.e. not just "wrong", provably impossible
  under the current formula/base. All *other* fields on the upgraded item (incl. the
  `retaliationLightning` flat pair drawn immediately before it) still match exactly, so the draw
  POSITION is right and the RNG state entering this draw is confirmed correct — the bug is
  specific to what base value/formula is used for `retaliationTotalDamageModifier` on this
  particular upgraded record.
- Tried brute-forcing alternate (v, pct) pairs against the single known RNG output for each of the
  2 upgraded seeds independently — found a coincidental (v=160, pct=10 or 25) match for the first
  seed, but it did NOT reproduce the second seed's result, so that's noise, not a real formula.
  **Do not chase this further without more upgraded-record samples with mixed fields** (same trap
  as the earlier Bleeding/Poison reorder mistake — a single-seed match is not evidence).
- **New, narrower finding for next session:** this is very likely an **upgraded-record-specific**
  quirk (e.g. game might jitter off the non-upgraded/template's base=80 instead of the upgraded
  DBR's own override=200, or apply some upgrade-tier transform before jittering) rather than a
  general `RETAL_MOD` ordering bug — worth checking whether OTHER upgraded items with a
  `retaliationTotalDamageModifier` override also mismatch, vs. their non-upgraded counterparts
  matching fine, to confirm/refute the "upgraded records use parent's base value" theory.

## Session update (July 6 2026, latest — Bleeding/Poison "ordering" bug: real cause found & fixed)

Resolved the long-standing "Bleeding/Poison resist-ordering issue" flagged in earlier sessions as
needing a wider DB sample. **It was never actually a Bleeding/Poison field-order bug.** Gathered a
wide sample of base-only items with both `defensiveBleeding` and `defensivePoison` present, then
did a full positional brute-force search (every insertion point for Bleeding, and a 2D
Bleeding×Poison joint search) against the whole 1051-item corpus — the existing order
(`...defensivePoison...defensiveBleeding` near the end of `DEF`, i.e. Poison drawn before Bleeding)
was already the global optimum (1029/1051, 43 BAD) and could not be beaten by any reordering. That
proved the fields' relative order was already correct and the remaining single Bleeding/Poison
mismatch (`c021_torso` upgraded, two seeds sharing identical base values 38/33) had a different root
cause.

Root-caused by hand-replaying the full RNG draw chain for both seeds against their real tooltips:
**`retaliationDamageMultModifier` draws its cjit jitter AFTER the Defense store, not alongside the
rest of `RETAL_MOD` before Defense** (its sibling `retaliationTotalDamageModifier` and the
per-type `retaliation*Modifier` fields correctly stay before Defense — confirmed by testing that
move in isolation, which regressed the corpus). One seed (1047462139) had happened to validate
"by coincidence" under the old (wrong) order because the wrong RNG draw still landed in a range
that satisfied the tooltip substring check for a different, nearby field — masking the bug until a
second seed on the same base record (479889632, identical base Bleeding/Poison values) exposed it.

Fixed in `gdvalidate.py` (`retaliationDamageMultModifier` pulled out of `RETAL_MOD`'s `ORDER`
insertion point and re-appended after `DEF`) and ported into `ItemStatEngine.cs` (new
`RetalDamageMultPostDef` array, inserted into `BuildOrder()` right after the `Def` loop, using the
existing generic `Kind.RetalMod` case — no new switch arm needed). Added regression test
`RetaliationDamageMultModifier_DrawsAfterDefense` (c021_torso upgraded, seed 479889632, DB-
cross-validated against the real tooltip). **Result:** `gdvalidate.py` item_ok 1029→**1032/1051**,
BAD 43→**37**, zero Bleeding/Poison mismatches anywhere in the corpus. `dotnet test`: **43 pass**
(was 42), 1 skip.

**Affixes caveat (per user, before starting affix work):** this bug is exactly the kind of
draw-position error that would be invisible in base-record-only testing until a second real sample
exposed it — affixed items interleave prefix/suffix draws into the same per-field slots, so any
existing per-field position assumptions (including this fix) need re-validation once affix work
starts, not assumed to "just work" by extension.

**Remaining ~37 BAD are unrelated long-tail cases** (e.g. `c016_axe2h` retaliationTotalDamageModifier
mismatch — confirmed NOT a position issue, isolated single-item; the n=1 `characterOffensiveAbility`
oddity on `d008_necklace`; a couple of focus-item `characterSpellCastSpeedModifier` mismatches) —
no other known systemic bug.

## Environment note — python3 on PATH is a Windows Store shim

`python3`/`python` resolved from PATH (`C:\Users\Admin\AppData\Local\Microsoft\WindowsApps\`) are
**Windows app-execution-alias shims**, not real interpreters — they fail/prompt to install from
the Store instead of running. The real interpreter is at
`D:\Programs\Python\Python311\python.exe` (3.11.9). Use the full path (or ensure that directory
is earlier on PATH) when running `gdvalidate.py`:
`D:\Programs\Python\Python311\python.exe GrimDawnSeedStats/tools/gdvalidate.py`

## Session update (July 6 2026, latest session — offensive Slow*DurationModifier wired)

Picked up the "offensiveSlow*DurationMin/DurationModifier" item left out-of-scope by the previous
session (retaliation Dur/Reflex handoff). DB query showed only **Physical, Bleeding, Fire, Cold,
Life, Poison** slow types have a `DurationModifier` field; Lightning/Aether/Chaos plus non-damage
slows (AttackSpeed/RunSpeed/DamageMult/DefensiveAbility/DefensiveReduction/LifeLeach/ManaLeach/
OffensiveAbility/TotalSpeed) only have `DurationMin`.

- **`offensiveSlow{Type}DurationModifier`** (Physical/Bleeding/Fire/Cold/Life/Poison only): a real
  **scaled cjit draw**, positioned immediately **after** the matching `offensiveSlow{Type}Modifier`
  in the DMG store's field order — confirmed via c023_ring (seed 308368603, sp=20): base 30 →
  jittered/scaled to 44, DB tooltip "+36% Frostburn Damage **with +44% Increased Duration**". No
  new `Kind` needed — it's an ordinary scaled offensive %-modifier, just a different field name.
- **`offensiveSlow{Type}DurationMin`** (all types): **FIXED** (0 draws, echoed base) — same pattern
  as the already-solved retaliation Dur block's `DurationMin` companion.
- Display: the combined tooltip line is `"+X% {SlowName} Damage with +Y% Increased Duration"` (one
  line, not two) — but since the validator checks each modeled field as an independent tooltip
  *substring*, a bare `"+{v}% Increased Duration"` formatter is sufficient (it matches inside the
  `"...with +{v}% Increased Duration"` tail). No line-merging logic needed.

**Result:** `gdvalidate.py` modeled 1023→**1051**, item_ok 1002→**1029/1051** — only **+1 new BAD**
(unrelated to this field, contained). Ported into `ItemStatEngine.cs` (`Dmg` array gained the 6
`{Type}DurationModifier` entries right after their `{Type}Modifier` — falls through to the existing
scaled-cjit `default` case; `IsFixed` gained the `offensiveSlow*DurationMin` rule). Added regression
test `OffensiveSlowDurationModifier_DrawsAfterMatchingSlowModifier` (c023_ring, DB-cross-validated).
`dotnet test`: **42 pass** (was 41), 1 skip.

**Still NOT wired / out of scope:** the Bleeding/Poison resist-ordering issue (needs a wider DB
sample per the earlier "do not retry without more evidence" note), the n=1
`characterOffensiveAbility`-missing oddity on `d008_necklace`, and other small long-tail BAD cases
(~43 remaining stat-check mismatches, spread thin, no known systemic bug).

## Session update (July 6 2026, earlier session — retaliation Dur/DurMod/Reflex block wired)

Picked up the "Retaliation Dur/DurMod/Reflex-CC block" item from the previous handoff. Found
real (non-affixed) DB samples for retaliationStun/Freeze/Confusion and retaliationSlow{AttackSpeed,
Poison} and used permutation-search cross-validation (fixing known blocks' internal order, brute-
forcing only the unknown insertion point) against real seeds/tooltips — same technique as the
earlier shield/block ordering resolution. Confirmed and wired into both `gdvalidate.py` and
`ItemStatEngine.cs`:

- **RetalDur** (`RetaliationAttributeDur_*`: Slow/DoT block) — bare `DamageAttributeAbs` subclass,
  same 1-draw `cjit` as RetalFlat, **no scale**. Position: right after RetalFlat, **before**
  RetalMod (confirmed via `c125_head`/`c019_legs`, seeds 963649166/1860173906: `retaliationSlowPoisonMin`
  sits between `retaliationPoisonMin` and `retaliationTotalDamageModifier`). `{prefix}DurationMin`
  is **FIXED** (0 draws, echoed base — confirmed via `c003_necklace`/`c044_necklace` AttackSpeed
  samples, and the Poison samples). Display: AttackSpeed/RunSpeed show a bare `%` (no duration
  multiply, e.g. "30% Reduced Attack Speed Retaliation for 5 Seconds"); damage types (Fire/Cold/
  Poison/etc) show a DoT total = `jittered_per_tick * DurationMin` (e.g. base 1000, jittered 1155,
  duration 5 → "5775 Poison Retaliation over 5 Seconds") using the **SLOWNAME** map (raw type name,
  e.g. "Poison" — confirmed DB-distinct from RetalFlat's DMGNAME map which shows "Acid").
- **RetalReflex** (`RetaliationAttributeReflex_*`: Stun/Freeze/Confusion) — same base, 1-draw
  `cjit`, no scale. Position: **after** RetalMod, before Defense (confirmed via `c019_legs`:
  `retaliationConfusionMin` sits after `retaliationTotalDamageModifier`). `{prefix}Chance` is
  **FIXED** (0 draws, echoed base — confirmed on all 3: Stun `c015_necklace`, Freeze
  `d006_shoulder` (upgraded), Confusion `c019_legs`). Display: `"{chance}% Chance of {N} Second(s)
  of {Stun|Freeze|Confuse} Retaliation"` (note: Confusion displays as "Confuse", not "Confusion").

So full Retaliation store order is now: **Flat → Dur (Slow/DoT) → Mod (%-modifiers) → Reflex
(CC)**, then Defense.

**Result:** `gdvalidate.py` modeled 1017→**1023**, item_ok 996→**1002/1023** — all 6 newly-modeled
items validated exactly, **zero new BAD** (no regressions). Ported into `ItemStatEngine.cs`
(`Kind.RetalDur`/`Kind.RetalReflex`, new `RetalDur`/`RetalReflex` field-order arrays, wired into
`BuildOrder`/`BuildModeled`/the compute switch). Added 2 new regression tests using fully-modeled
(no-unmodeled-field) items so the C# assertions are byte-exact against real tooltips:
`RetaliationSlowDur_DrawsBeforeDefense_DurationIsFixed` (c044_necklace) and
`RetaliationReflex_DrawsAfterDmg_ChanceIsFixed` (d006_shoulder upgraded). `dotnet test`: **41
pass** (was 39), 1 skip.

**Still NOT wired / out of scope this session:** offensive-side `offensiveSlow*DurationMin`/
`offensiveSlow*DurationModifier` (the DoT-duration companion on the OFFENSIVE side, e.g.
`offensiveSlowPoisonDurationModifier`) — encountered while cross-validating (`c019_legs`,
`c125_head` both have it) and empirically found to be a **scaled cjit draw sitting right after
the matching `offensiveSlow{Type}Modifier` in the DMG block** (base 30 → tooltip 36 via
`scale(cjit(30),sp=30)`, confirmed twice), but this is a distinct offensive-side field, not part
of the requested retaliation block — left unmodeled/unwired pending a dedicated pass (items with
it are still safely skipped, not mis-rolled).

## Session update (July 6 2026, earlier session — armor physical-pair fix)

Dug into the remaining 32 BAD items from the flat-max fix. Found and fixed a real bug:
`offensivePhysicalMin/Max` was hardcoded FIXED (0 draws) everywhere, but that's only true for
actual **weapons** — for **armor/jewelry** the same field name is a real jittered+scaled flat pair
(same mechanism as `offensiveBonusPhysical`). Fix is gated on the item's own `Class` DBR field
(`TextValue` on the `Class` stat row): `Class.startswith("Weapon")` → FIXED; else → real FLAT
pair draw. Wired into `gdvalidate.py` (`FLAT` list + `is_weapon_class()` gate) and
`ItemStatEngine.cs` (`Flat` array + `Kind.Flat` case), with new regression test
`ArmorOffensivePhysicalPair_DrawsAndScales`. **Result: item_ok 985→996/1017, BAD 83→42.**
`dotnet test`: 39 pass (was 38), 1 skip.

Also investigated (and reverted) a `defensiveBleeding`/`defensivePoison` ordering theory from
`c021_torso` — two different reorderings each fixed that one item but regressed the total badly
(925 and 895/1017, worse than the 996 baseline). **Do not retry this reorder without a wider
sample of items mixing Bleeding+Poison+other resists** to find the real rule; a single matching
item isn't enough evidence (this session's mistake).

One more residual case examined but not fixed: `d008_necklace` has `characterOffensiveAbility`
fully absent from its real tooltip despite nonzero DB base (45) — confirmed via full tooltip pull,
not a truncation artifact. Skipping that one draw makes the rest of the item match exactly, but
other similar Legendary items have OA working normally, so this looks like an isolated data oddity
(n=1), not a general rule — not worth a blanket special-case yet.

Remaining 42 BAD per-stat checks are the Bleeding/Poison ordering issue (needs more samples) plus
the still-unwired Retaliation Dur/DurMod/Reflex-CC block, plus this n=1 OA oddity and similar
long-tail cases.

# Grim Dawn seed→stat — continuation handoff

Read this + `docs/seed-stat-algorithm.md` to resume. Goal: compute an item's real stats
from (records + seed) with a 1:1 match to the game, replacing the OnDemandSeedInfo round-trip.

## Current status (base-record-only path)

**The base-record roll algorithm is REVERSE-ENGINEERED, Ghidra-confirmed, and DB-validated at scale.**
Batch validation over the game DB: **638 base-only items fully modeled → 635/638 exact,
3604/3607 per-stat rolls exact (99.9%).** The only 3 non-matches are display-time **inherent
item-template bonuses** (focus cast-speed ×2, torso chaos-resist) added *outside* the seed roll —
NOT roll errors. So the roll math is trustworthy.

**CONVERSIONS DONE (July 6 2026).** conversionInType/OutType live in the `TextValue` column
(val1 is 0). Valid slot (non-empty inType, pct!=0) = one `convJitter` float draw, **after Defense,
before deferred Skill**; 2 slots (base set1, set2). Display type names use game map (Pierce→
**Piercing**, Poison→Acid, Life→Vitality). Also **corrected the scale transform** to
`(int)((float32)(jittered*(100+scale))/(float32)100)` (see §6) — the old `jittered*(1+scale/100)f32`
mis-truncated exact-integer cases (90*130/100=117 vs 90*1.3f→116). Validator now also skips items
with an empty ReplicaItemRow tooltip (nothing to validate against).

**FLAT ADDED DAMAGE — partially done (July 6 2026).** `offensiveXMin/Max` (Abs_ loaders 08-18)
modeled as min/max PAIRS in true damage-store loader order (before %-modifiers). Ghidra-confirmed:
- `DamageAttributeAbs::AddJitter` @0x1801790a0 iterates the value array in pairs, jittering
  element[0] (min) and element[1] (max) **identically** via `DamageAttributeAbs::Jitter`
  @0x180176120 — which is byte-for-byte the same integer jitter as `cjit` (Char). 2 draws, min then max.
- `DamageAttributeAbs::ScaleAttribute` @0x180179270 steps the array in pairs and scales
  **ONLY element[0] (min); element[1] (max) is left UNSCALED.** So: **min = scale(cjit(min)),
  max = cjit(max) (no scale).** This is Ghidra-verified and validated: min matches 24/24 on clean
  armor pairs. Applied to gdvalidate.py + C# ItemStatEngine (Math.Truncate on max, no ApplyScale).
- Weapon-base physical `offensivePhysicalMin/Max` = Create_BasePhysical preamble, 0 draws (FIXED).

Result: 638→851 modeled, 736→**751 items fully correct**, BAD 140→**121**.

**OPEN BUG (residual max misalignment) — SHELVED, confirmed contained.** Decision (July 6 2026):
live with the wrong max for now since it does NOT desync. Verified: of 191 base-only items with a
flat pair, 93 have the flat MAX value wrong, but only 2 have ANY other stat wrong (and those 2 are
the known display-time template bonuses, not desync). ⇒ the pair consumes exactly 2 draws; max is
mis-valued but downstream stays synced. Detail below.

For ~40% of flat pairs the MAX value is off by ~1 draw.
Proof it's a draw-alignment issue: two items with identical DB min/max/scale and identical computed
cjit produce different game tooltips (e.g. Row3 vs Row16: both DB 9-12 sp40 → tips 12-14 vs 12-15).
Min (draw1) is always right; max (draw2) is variably wrong → an **unmodeled draw sometimes sits
between min and max**. It is NOT a Chance/GlobalChance companion (failing items have none), NOT a
constant gap (no offset 0-3 beats 9/24), NOT reversed [max,min] layout (0/24).

**RE PROGRESS (July 6 2026, later session) — `LoadFromTable` decompiled, bug NOT yet found.**
`GAME::DamageAttributeAbs::LoadFromTable` @**0x180178b30** (full body pulled via Ghidra
`decompile_function_by_address`; per-item factory is `FUN_1801983d0` for Abs_Fire, which `new`s a
`0xe0`-byte object, sets vtable, then calls this). Its logic (`this+0xb0/0xb8/0xc0` = a
`std::vector`-shaped value-array: begin/end/cap pointers):
1. First virtual call (vtable **+0x100**) gets a tag, loads a float array via `(LoadTable+0x88)(tag,
   this+0x38)` — writes into a DIFFERENT member (`this+0x38`), not the value array. Unidentified
   purpose (possibly the raw/undamaged base value used elsewhere, not part of the jitter array).
2. Second virtual call (vtable **+0x150** — presume `GetLoadValueMinTag`-equivalent, though the
   *named* `GetLoadValueMinTag`/`GetLoadValueMaxTag` virtuals seen on the Reflex/Dur retaliation
   classes are a **different, unrelated** vtable slot naming — Abs_Fire's own tag getters weren't
   individually named-resolved this session) gets a tag, loads a float array into a **local**
   begin/end pointer pair (`local_58`/`pfStack_50`) — this is the array LoadFromTable actually
   iterates below.
3. Third virtual call (vtable **+0x158**, presumably the Max tag) loads via the same `(LoadTable+0x88)`
   call but the decompiler shows only 2 args (`param_2,uVar4`), dropping the output pointer — most
   likely (unconfirmed) it reuses the SAME `&local_58` output, i.e. **appends the Max value onto the
   same array**, giving a 2-element array `[min, max]` in the ordinary single-valued case.
4. It then loops over every element `i` of that array and pushes a PAIR `(value[i], max(0,
   value[0]-value[i]))` into the `this+0xb0` value-array vector. For the ordinary 2-element
   `[min,max]` case this pushes 2 pairs: `(min, 0)` and `(max, max(0,min-max))` — i.e. **the actual
   stored value-array ends up with 4 floats (2 pairs), not 2 floats**, if this reading is right.
   This second component of each pair (the "delta from first element", clamped at 0) is NOT
   currently understood — it may be an unrelated display/range field, OR it may explain the bug:
   if `AddJitter`/`ScaleAttribute` iterate the vector as flat elements-in-pairs (not
   pairs-of-pairs), a 4-float layout `[min,0,max,delta]` would explain an extra unmodeled slot
   sitting between min and max exactly as observed (the `0` element between min and max draws).
   **This is a live, unconfirmed hypothesis — not yet validated against DB samples.**
5. **`AddJitter` DECOMPILED (0x1801790a0) — confirms strict pair-stride processing.** Body: skips
   entirely if `param_1` (scale-percent-ish float) `<=0` or rng ptr null; else walks `this+0xb0..0xb8`
   in **strides of 2 floats** (`puVar1` then `puVar1+1`, advance by 2), calling the per-element
   jitter virtual (vtable+0x128) on BOTH elements of every pair unconditionally, writing results back
   in place. This means: if `LoadFromTable` really produces 4 floats (2 pairs: `(min,delta_min)`,
   `(max,delta_max)`) instead of 2, ALL 4 get run through the per-element jitter fn each pair-step —
   not just min/max. The per-element jitter fn is presumed to be the same `cjit`-style function
   (returns `v` unchanged with NO draw when `v==0`, matching the known cjit short-circuit) — so a
   **zero delta costs no draw, but a NONZERO delta silently consumes one**.
   - **Refined hypothesis:** recall step 4's delta formula was `max(0, value[0]-value[i])` where
     `value[0]` is whichever tag loads FIRST (Min or Max — direction unconfirmed this session). If
     Min loads first: pair1=(min,0) [delta=min-min=0, no draw], pair2=(max, max(0,min-max)) — for
     ordinary weapons max≥min so this delta is also 0 → NO extra draw predicted (contradicts the bug
     existing at all). If **Max loads first** instead: pair1=(max,0) [no draw on delta], pair2=
     (min, max(0,max-min)) — since max>min is the normal case, `max-min` is POSITIVE and NONZERO →
     **this delta draws**, consuming an extra unmodeled RNG step. This flips which tag is "first"
     from what was assumed (doc previously assumed order = min-then-max; this suggests the loader's
     internal tag-order may actually be **max-then-min**, with the visible min/max jitter draws
     still happening in some order, PLUS a 3rd hidden draw for the nonzero max-min delta).
   - **Problem with this hypothesis as stated:** it predicts the hidden draw fires on effectively
     EVERY flat pair with max>min (i.e. ~100% of them), but the observed bug rate is only ~40%
     (93/191). So either (a) the delta is not simply `max(0, value[0]-value[i])` for every element —
     maybe it only applies to a SUBSET of damage types/subclasses (some Abs subclasses might override
     `LoadFromTable` or the tag-getter differently), or (b) direction-of-load guess is still wrong, or
     (c) there's a further condition (e.g. delta only computed/pushed when the array has >1 element
     to begin with, and single-value Min-tag/Max-tag fields don't actually produce a 2-element local
     array the way step 3 assumed — the "third call reuses `&local_58`" reuse-of-output-pointer
     assumption in step 3 is UNCONFIRMED, not read from disassembly, just inferred from the missing
     arg in decompiled output). **This needs bytes-level disassembly (not decompiler pseudo-C) of
     the call at the "third call" site in `FUN_1801983d0`/`LoadFromTable`@0x180178b30 to see the true
     third argument**, since the decompiler is dropping it.
6. **ROOT CAUSE CONFIRMED + FIXED (July 6 2026, same session, via raw disassembly).** Pulled the
   actual disassembly of `LoadFromTable`@0x180178b30 (the decompiler's pseudo-C was dropping an
   argument and misleading). There are TWO SEPARATE local arrays — MinArray (from the Min tag,
   loaded to RBP-0x40) and MaxArray (from the Max tag, RBP-0x58) — not one combined array. The
   pair-construction loop iterates `i` over `range(len(MinArray))` and for each `i` pushes ONE PAIR:
   **`(MinArray[i], max(0, MaxArray[i] - MinArray[i]))`**. So the value array's second element is
   the **SPREAD** (max−min), NOT the raw max! `AddJitter` (0x1801790a0, disasm-confirmed) jitters
   BOTH elements via the same cjit-shaped fn in stride-2; `ScaleAttribute` scales only element[0]
   (already known). True model: `jittered_min = scale(cjit(min_raw))`, `jittered_spread =
   cjit(max(0,max_raw-min_raw))` (never scaled), **`displayed_max = jittered_min + jittered_spread`**
   (NOT an independent `cjit(max_raw)`!). Hand-verified against the DB 9-12@sp40 smoking-gun example:
   `spread_raw=3`, `cjit(3,20%)∈{2,3,4}`, `jittered_min=12` (already known to match in both items) →
   `displayed_max=12+{2,3,4}={14,15,16}` — exactly reproduces the observed 14-vs-15 split. **Wired
   into `gdvalidate.py`** (`flat`/`retalflat` kinds now compute `jspread=cjit(max(0,mx-mn),rng)` and
   `jmx=int(jmn+jspread)` instead of an independent `cjit(mx)`; unpaired single-sided fields keep the
   old fallback). **Result: item_ok 863→938/1017** just from this fix. Two more small, unrelated
   display-format bugs surfaced and were fixed in the same pass (surfaced because the flat-max noise
   had been masking them): (a) flat damage lines use the **conversion-style type name** (`Pierce`→
   `Piercing`), not the %-modifier name (`DMGNAME` keeps `Pierce`→`Pierce`, which IS correct for
   `"+X% Pierce Damage"` modifier lines) — `flat_line` now uses `CONVNAME` instead of `DMGNAME`
   (item_ok →964); (b) `offensiveBonusPhysical` flat lines do **NOT** get a `+` prefix (verified via
   DB tooltip `d009_shield`: "53 Physical Damage", not "+53") — removed the assumed `plus` prefix
   entirely from `flat_line` (item_ok →**985/1017**, BAD **218→83**). Applies identically to
   `RetalFlat` (same `DamageAttributeAbs` base, no scale at all for retaliation) — wired there too.
   **PORTED to `ItemStatEngine.cs`** (`Flat`/`RetalFlat` switch cases rewritten to the same
   min+spread model, unpaired single-sided fallback preserved). Added regression test
   `FlatDamagePair_MaxIsMinPlusJitteredSpread` (c027_gun1h, seed 1234651150, sp40) asserting
   `offensiveBonusPhysicalMin=32, Max=41` — hand-cross-validated against the Python oracle replay
   AND the real in-game tooltip "32-41 Physical Damage". `dotnet test`: **38 pass** (was 37), 1 skip.
   **The flat-max bug is FIXED and validated end-to-end (Python oracle + C# port + real DB
   tooltips).** This was the last item on the "shelved" list — base-record modeling is now at
   985/1017 items exact (96.9%), with remaining 32 BAD spread across small unrelated issues
   (a few more display-name edge cases, the still-unwired retaliation Dur/Reflex/CC fields, etc)
   rather than any known systemic bug.

**DEFENSE MAXRESIST CAPS — DONE (July 6 2026).** Added `defensive*MaxResist` (incl. `defensiveAllMaxResist`)
to `FIXED` in gdvalidate.py — base value has jitter pct=0 (const), so `cjit` returns it unchanged,
0 draws, matching the doc's existing prediction. No Ghidra needed, no formatter entry needed (not
verified against tooltip, same treatment as `defensiveProtection`). Result: skipped 536→**489**,
fully-modeled 851→**898**, fully-correct 751→**789** (the new BAD are the same shelved flat-max issue,
not new desync).

**RETALIATION — Abs + AbsMod blocks DONE (July 6 2026).** Wired into `gdvalidate.py`: inserted
right after `DMG`, before `DEF` (order = Char→Damage→**Retaliation**→Defense→Skill). Two blocks
modeled: `RETAL_FLAT` (Abs pairs: Physical/Pierce/Fire/Cold/Lightning/Poison/Life/Aether/Chaos/
Elemental — same min/max-pair `cjit` draw as offensive flat, but **no scale**, confirmed no
`ApplyScale` call in Retaliation's chain) and `RETAL_MOD` (`retaliationTotalDamageModifier`,
`retaliationDamageMultModifier`, per-type `retaliation*Modifier`). Formats added to `fmt()`/
`retal_line()`: flat pair → `"N-M Type Retaliation"` (DMGNAME map, Poison→Acid), single-sided →
`"N Type Retaliation"`; `retaliationTotalDamageModifier` → `"+N% to All Retaliation Damage"`;
per-type modifier → `"+N% Type Damage Retaliation"`.

Result: modeled 898→**957**, item_ok 789→**814**. Confirmed retaliation flat pairs hit the
**exact same shelved max-misalignment bug** as offensive flat pairs (min always right, max off
~40% of the time) — expected, since it's the literal same `AddJitter` function. Checked for
downstream desync risk: of 59 modeled items with retaliation fields present, only **4** also have
a non-retaliation stat wrong — same low contained-error rate as the offensive case (2/191) →
**confirmed CONTAINED, not a new desync**, safe to leave as-is alongside the existing shelved bug.

**NOT yet wired**: Retaliation's Dur/DurMod blocks (slow/DoT: `retaliationSlow*Min` +
`retaliationSlow*DurationMin`, e.g. observed in DB: `retaliationSlowFireMin`,
`retaliationSlowFireDurationMin`, `retaliationSlowPoisonMin`, `retaliationSlowPoisonDurationMin`)
and the Reflex/CC block (`retaliationStunMin/Chance`, `retaliationFreezeMin/Chance`,
`retaliationConfusionMin/Chance`) — items using them are still safely skipped via `concerning()`
rather than guessed at. **Partial RE progress (July 6 2026, later session), NOT yet wired:**
- Ghidra class hierarchy confirmed: Stun/Freeze/Confusion are `RetaliationAttributeReflex_Stun`/
  `_Freeze`/`_Confusion`, which extend `DamageAttributeReflex` → `DamageAttributeAbs` (no
  `AddJitter` override found in any ctor inspected — same base as the flat min/max pairs). Field
  names come from per-subclass virtual tag methods (`GetLoadValueMinTag` etc, e.g.
  `RetaliationAttributeReflex_Stun::GetLoadValueMinTag` @0x180413c60 returns `"retaliationStunMin"`).
  Slow/DoT fields (Fire/Poison/AttackSpeed/...) are `RetaliationAttributeDur_*` (value) +
  `RetaliationAttributeDurMod_*` (the %-modifier variant) — parallel naming to the already-modeled
  offensive Slow/DoT convention.
- **Empirical DB samples** (3 single-field items each, base-only, no other Def/Retal fields to
  cross-check position against): `retaliationFreezeMin` base 2 → tooltip 1 (jitters);
  `retaliationConfusionMin` base 2 → tooltip 1 (jitters); `retaliationStunMin` base 2 → tooltip 2
  (inconclusive — could be no-op jitter or FIXED). All 3 `*Chance` companions matched their base
  value EXACTLY (15%, 20%, 24%) → **Chance looks FIXED, not jittered** (tentative, only 3 samples).
  `retaliationSlowAttackSpeedMin`/`Modifier` clearly jitters (20→16, 32→27 across 2 samples);
  its `DurationMin` looked FIXED (unchanged in both).
- **Unresolved**: `retaliationSlowFireMin`/`retaliationSlowPoisonMin` samples did NOT cleanly
  match a simple cjit-of-base hypothesis (one sample's tooltip had no matching line at all for
  Slow Fire; Poison's DoT tooltip total didn't decompose against base+TotalDamageModifier in an
  obvious way) — may need a different formula or may combine with `retaliationTotalDamageModifier`
  in the display line. Also completely unconfirmed: draw ORDER/position of the Dur/DurMod/Reflex
  blocks relative to the already-wired RetalFlat/RetalMod blocks (no items sampled yet that mix
  a Dur/Reflex field with a RetalFlat/RetalMod field to test ordering, the way the shield/block
  ordering was resolved). **Do not wire this into gdvalidate.py/ItemStatEngine.cs until the
  Chance-FIXED hypothesis and the Slow Fire/Poison formula are confirmed on more samples** — a
  wrong guess here risks real desync, unlike the shield/block fix which was self-contained.

**SHIELD/BLOCK — DONE (July 6 2026).** Two pieces, Ghidra-confirmed:
- `defensiveBlock`, `defensiveBlockChance`, `blockAbsorption`, `blockRecoveryTime` are read
  **straight from the raw LoadTable** in `WeaponArmor_Shield::Load` (Ghidra 0x18056d180) directly
  into member fields — **no jitter/attribute-store call at all**, 0 draws, FIXED (echoed as base).
- `defensiveBlockModifier` / `defensiveBlockAmountModifier` (`DefenseAttributeMisc_BlockModifier`
  @0x1801e8a80 / `_BlockAmountModifier` @0x1801e8ae0) are bare `DefenseAttributeAbs` subclasses —
  identical single-value `cjit` draw, no scale, same as the resist fields (`DefenseAttributeAbs_Physical`
  etc have the exact same ctor shape). Their position in the Defense store's load order was
  **empirically resolved**: appending them after the resists desynced items carrying both a block
  modifier and a resist (item_ok 840/1017, BAD 272); moving them to the **front of the Defense
  list** (before Physical/Pierce/.../TotalSpeedResistance) fixed every such item (item_ok
  **863/1017**, BAD **218**). Wired into `gdvalidate.py` and ported into
  `GrimDawnSeedStats/src/GrimDawnSeedStats/ItemStatEngine.cs` (`Def` array + `BlockFixed` set).
  `dotnet test` still 37 pass/1 skip.

Next: the shelved flat-max draw alignment (value-array loader RE) — see below, left for last per
user request. The C# CLI (`src/GrimDawnSeedStats.Cli`) is the deliverable and already drives
ItemStatEngine end-to-end (reads items.json → record+seed → rolled stats).

## The confirmed algorithm

- **RNG**: Park–Miller MINSTD (Schrage), `next(s) = (s%127773)*16807 - (s/127773)*2836; if(<0)+=2147483647`.
- **Priming**: state = one `next()` of the item seed before any draw.
- **Base-record jitter draw ORDER** (validated): Char → (CharPenaltyReduction) → Damage →
  Retaliation → Defense **during load, in field order**, then the **Skill store LAST**
  (its `AddJitter` is deferred to the end of `InitializeItem`; `SkillAttributeStore::AddJitter`
  @0x1805027b0, skill-store vtable slot +0x28). This deferral was the crux — NOT store-load order.
- **Jitter fns** (integer uniform, per value-array element = 1 draw each):
  - `cjit` (CharAttribute::Jitter @0x1800b8c00) for Char/Damage/Retaliation/Defense: `if v==0||pct==0 return v (no draw); i=int(v*pct*0.01); if i==0 i=1; DRAW; r=(int)(s%(2i+1))-i+v; if abs(r)<1 return v else r`.
  - `sjit` (SkillAttribute::Jitter @0x180501460) for Skill: same but NO min-1 clamp; still draws when pct==0 (if v!=0).
- **Offensive scale** (offensive damage modifiers only): `scaled = (int)((float32)jittered * (float32)(1+scale/100))`.
  MUST be float32 (C++ float): e.g. 45*1.4f → 63.0 in float32, 62.9999→62 in float64 (wrong).
  `scale = attributeScalePercent` (base) [+ prefix/suffix lootRandomizerScale for affixed items].
  Non-scaling: `offensiveCritDamageModifier`, `offensiveTotalDamageModifier`.
- pct (jitter percent) = 20 for base-record stats (const DAT_18076ab28).

## Draw classification (base-record)

- **Draws (rollable)**: character attributes/modifiers (most), offensive damage %-modifiers
  (direct + slow/DoT), defensive resists + CC-durations, skill cooldown/manacost. See catalog
  in `GrimDawnSeedStats/tools/gdvalidate.py` (CHAR/DMG/DEF/SKILL lists = field order).
- **FIXED (0 draws)**: `defensiveProtection` (armor); `offensiveBase*Min/Max` (weapon base
  damage, AbsBase override is a no-op); `characterHealIncreasePercent` (discovered via d105
  desync — does NOT draw); `characterManaRegen`, `characterConstitution`, base attack/run/cast
  speed, ReqReductions, light radius, increased exp/gold, +skills/mastery augments, cosmetics.
- **Inherent template bonuses** (added at DISPLAY, not rolled): some item classes add a fixed
  bonus not present in DatabaseItemStat — e.g. focus off-hand cast-speed (+~2), torso chaos-res.
  These make the tooltip exceed the rolled value; the ROLL is still correct.

## NOT yet modeled (the ~370 skipped items) — next work

Ranked by item count (numbers stale from before shield/block wiring, kept for prioritization order):
1. **Conversions (427)** — `conversionInType/OutType/Percentage` (+ `...2` second slot). The
   conversion store (`0x180169ab0`) uses a FLOAT multiplicative jitter (`FUN_180167dc0`), NOT
   the integer cjit. Need: its position in the draw order (store loaded last; jitter likely
   during-load after Defense but before deferred Skill — MUST verify against DB), the exact
   float-jitter formula (see docs/seed-stat-algorithm.md §3c), and the 8-slot order
   (base set1, base set2, then affix sets). For base-only: base set1 then set2.
2. **Flat added damage** `offensiveXMin/Max` (NOT Base) — these are DamageAttributeAbs_* which
   jitter as **min/max PAIRS = 2 draws** (DamageAttribute::AddJitter @0x1801790a0 iterates the
   value array 2-at-a-time). Confirm pair order (min then max) + whether flat damage scales.
   `offensiveXChance`/`Global` companions may or may not draw — verify.
3. **Retaliation** (retaliation* flat + modifiers) — retaliation store, cjit, no scale.
4. **Shield**: `defensiveBlock`, `defensiveBlockChance`, `blockAbsorption`, `blockRecoveryTime`,
   `defensiveBlockModifier/AmountModifier` — block value/chance draw behavior TBD.
5. **Defense MaxResist caps** `defensive*MaxResist` — DefenseAttributeDefenseCap_*, base uses
   const 0 → NO base draw (affix-only draws); so for base-only they're FIXED. `defensiveAllMaxResist` too.
6. **Life leech** `offensiveLifeLeechMin`, `offensiveSlow*Leach*`, duration modifiers
   `offensiveSlow*DurationModifier/DurationMin`, reduction percents.

## How to validate (the oracle)

DB: `GrimDawnSeedStats/userdata-test.db` (307MB, freshly re-run on the CURRENT game version —
authoritative). Tables: `PlayerItem` (baserecord, PrefixRecord, SuffixRecord, ModifierRecord,
Seed, Id), `DatabaseItem_v2`/`DatabaseItemStat_v2` (base stats: join on baserecord; Stat, val1),
`ReplicaItem2`/`ReplicaItemRow` (game tooltip: playeritemid→Id; Type 18/19/28 = stat lines, Text).

Validator: `GrimDawnSeedStats/tools/gdvalidate.py` (python3 + sqlite3, no deps). It:
- catalogs rollable fields in draw order, skips any item with an UNMODELED rollable field
  (to avoid silent desync), computes rolls, and checks each computed value appears in the
  game tooltip text (substring, via a display formatter).
- Extend by adding fields to CATALOG/formatter + reducing the `concerning()` skip set, then
  re-run: target rising "fully-modeled" count with BAD staying ~0.
- Run: `python GrimDawnSeedStats/tools/gdvalidate.py`
- Display format reference: `StatTranslator/EnglishLanguage.cs` (but the DB tooltip is the true
  target; game tags can differ from IA's — match the DB text).
- Enumerate all base-only stat fields: `select distinct stat.Stat from DatabaseItemStat_v2 stat
  join DatabaseItem_v2 item on item.id_databaseitem=stat.id_databaseitem join PlayerItem pi on
  pi.baserecord=item.baserecord where pi.suffixrecord='' and pi.prefixrecord='' and pi.modifierrecord=''`.

## The C# library (deliverable)

`GrimDawnSeedStats/` (net8.0, no deps, for OSS split then merge into IAGD):
- `src/GrimDawnSeedStats/`: `MinstdRandom`, `Jitter` (Char/Skill/Conversion + ApplyScale float32),
  `ItemStatEngine` (THE real draw-ordered engine — base-record fields fully wired, matches
  gdvalidate.py's field-order tables; now also has optional `prefixStats`/`suffixStats` params
  for the confirmed single-affix path, see "Affixes" below). `StatCatalog`/`BaseItemCalculator`/
  `StatRoller` are earlier PoC scaffolding, superseded by `ItemStatEngine` — not the thing to
  extend further.
- `src/GrimDawnSeedStats.Cli/`: `stats` command — `--record`/`--seed` (+ optional `--prefix`/
  `--suffix`, looked up in the same `--data` items.json by dbr path; warns to stderr if both are
  supplied together, see Affixes below).
- `tests/`: `MinstdRandomTests`, `JitterTests`, `ItemStatEngineTests` (base-record pipeline,
  DB-cross-validated), `AffixEngineTests` (single-affix pipeline, DB-cross-validated),
  `ItemCorpusTests` + `Fixtures/items.json` + `affixes.json` (15-item corpus; the affixed items in
  this OLD fixture are still `[Fact(Skip=...)]` — superseded by the newer, more targeted
  `AffixEngineTests` rather than wired up, since many of the 14 affixed fixture items are
  two-affix and would hit the known-broken 56% path; not worth un-skipping until that's fixed).
- `dotnet test` → **47 pass / 1 skip** (July 6 2026).
- Base-record `Kind`/field-order tables in `ItemStatEngine.cs` are up to date with
  `gdvalidate.py`'s `ORDER` as of the retaliation Dur/DurMod/Reflex + flat-max-spread fixes.
  Per-record quirk tables (`BASE_OVERRIDE_QUIRKS`/`NODRAW_QUIRKS`/`SINGLE_SIDED_QUIRKS`,
  `gdvalidate.py`) are intentionally NOT ported — none of the affected base records are in the C#
  test corpus (grepped each time one was added), so there's nothing to regress yet.

## Affixes (July 6 2026 — single-affix DONE; two-affix MOSTLY FIXED (88%) in both Python and C#)

Status has moved past "not started" — see `docs/affix-draw-order-plan.md` for full detail
(Ghidra findings, all session updates, next steps). Summary:

- **Draw order Ghidra-confirmed, per STORE (not universal!):** Char/Skill/Def/RetalMod scalar
  fields are **Prefix → Suffix → Base (base draws LAST)**. **Damage-store scalar fields
  (`offensive{Type}Modifier` etc.) are the one exception: Base → Prefix → Suffix (base draws
  FIRST)** — Ghidra-confirmed via `FUN_18019f110` (`DamageAttributeStore_Equipment`'s per-field
  loader builds/jitters its up-to-4 objects in that literal order). Getting this backwards was the
  root cause of the two-affix Damage-store failures below — fixed July 6 2026, see session log.
  All three (whichever order applies) call the exact same jitter fn with their own value + own
  `lootRandomizerJitter` pct (not base's constant 20%). An affix missing `lootRandomizerJitter`
  entirely means its contribution is FIXED (0 draws) — confirmed via `aa010a_damod_01.dbr`.
- **Single-affix (prefix XOR suffix) is DONE and trustworthy: 753/799 (94%) on a real 2000+-item
  DB sample**, wired into both `gdvalidate.py` (Python oracle) and `ItemStatEngine.Compute`'s
  optional `prefixStats`/`suffixStats` params (C#, `AffixEngineTests.cs`), plus CLI
  `--prefix`/`--suffix` flags. Only scalar fields (Char/Dmg/Def/RetalMod/Skill) are modeled for
  affixes — pair-shaped fields (flat added damage, retaliation flat/Dur/Reflex) touched by an
  affix are reported via `UnmodeledFields`/a Python skip, not modeled. (Single-affix was NEVER
  affected by the draw-order bug below — with only one non-base contributing source, cjit no-ops
  on a zero value without consuming a draw, so source order is invisible when there's nothing to
  reorder around.)
- **Two-affix (prefix AND suffix together): FIXED from 135/240 (56%) → now 304/347 (88%)** on the
  current, larger corpus (July 6 2026). Root cause was the draw-order bug above, NOT a scale
  constant or the `MergeDamage`/`Global`/`XOR` mechanisms investigated and ruled out earlier the
  same session (see session log for the full elimination trail). Fixed in both
  `tools/gdvalidate.py` (the `kind=='dmg'` branch of the scalar-field loop) and
  `src/GrimDawnSeedStats/ItemStatEngine.cs` (`e.Kind == Kind.Dmg` branch, mirrors the Python fix).
  `dotnet test`: 47 pass / 1 skip, unaffected. CLI warning updated to reflect ~88% instead of ~56%.
  **Remaining ~12% (43/347) is a fresh, not-yet-characterized long tail** (mixed
  `conversionPercentage`/`offensiveSlowPoisonModifier`/one-off Damage+Defense fields per the sample
  — no single dominant pattern found yet) — next session should NOT assume it's the same root
  cause; treat as a new investigation.
- Residual single-affix failures (46/799, not yet fixed) split into two sub-patterns, not one:
  "class" mastery-affix prefixes (`b_classNNN_*`) with inconsistent per-stat DB-value drift, and
  shields granting an active item-skill where the equip modifier may actually feed the granted
  skill's own damage formula. Neither investigated to a fix yet.
- **Also fixed this round**: a real DB query bug — newer captures store absent
  Prefix/Suffix/Modifier/etc columns as SQL `NULL`, older ones as `''`; every query that checked
  `col=''` only was silently blind to 2000+ new samples until a `_empty(col)` helper was added
  everywhere in `gdvalidate.py`. Watch for this if writing any NEW query against this DB.

## Key memory files
- `gd-seedstats-poc.md` — the running project log (most detailed, read first).
- `gd-seed-stat-algorithm.md`, `gd-store-field-orders.md` — Ghidra RE details.
- `gd-playtest13-item-internals.md` — item/replica internals.
- `gd-affix-draw-order-plan.md` — affix (prefix/suffix) draw order + current status (single-affix
  done, two-affix open); full detail in `docs/affix-draw-order-plan.md`.
