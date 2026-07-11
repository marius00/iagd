namespace IAGrim.Database.DAO.Table {
    /// <summary>
    /// Lookup table mapping a database item record to its rolled-value-independent, translated
    /// stat text, used to power free-text stat search. Built from the game database + the active
    /// language pack; numeric (roll-dependent) placeholders are treated as wildcards (blanked),
    /// while text placeholders (damage types, skill/race names) are filled in.
    /// </summary>
    internal static class RecordStatTextTable {
        public const string Table = "RecordStatText";
        public const string Record = "record";
        public const string SearchText = "searchtext";
    }
}
