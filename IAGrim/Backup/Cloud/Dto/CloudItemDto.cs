namespace IAGrim.Backup.Cloud.Dto {
    public class CloudItemDto {
        public string Id { get; set; }
        public string Mod { get; set; }
        public virtual bool IsHardcore { get; set; }

        public string BaseRecord { get; set; } = "";
        public string PrefixRecord { get; set; } = "";
        public string SuffixRecord { get; set; } = "";
        public string ModifierRecord { get; set; } = "";
        public string TransmuteRecord { get; set; } = "";
        public string MateriaRecord { get; set; } = "";
        public string RelicCompletionBonusRecord { get; set; } = "";
        public string EnchantmentRecord { get; set; } = "";

        public long Seed { get; set; } = 0u;
        public long RelicSeed { get; set; } = 0u;
        public long EnchantmentSeed { get; set; } = 0u;
        public long MateriaCombines { get; set; } = 0u;
        public long StackCount { get; set; } = 1;
    }
}
