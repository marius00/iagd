using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.Backup.Azure.Dto;
using IAGrim.Database;

namespace IAGrim.Backup.Azure.Util {
    static class ItemConverter {
        public static AzureUploadItem ToUpload(PlayerItem pi) {
            return new AzureUploadItem {
                BaseRecord = pi.BaseRecord,
                EnchantmentRecord = pi.EnchantmentRecord,
                EnchantmentSeed = pi.EnchantmentSeed,
                IsHardcore = pi.IsHardcore,
                LocalId = pi.Id,
                MateriaCombines = pi.MateriaCombines,
                MateriaRecord = pi.MateriaRecord,
                Mod = pi.Mod,
                ModifierRecord = pi.ModifierRecord,
                PrefixRecord = pi.PrefixRecord,
                RelicCompletionBonusRecord = pi.RelicCompletionBonusRecord,
                RelicSeed = pi.RelicSeed,
                Seed = pi.Seed,
                StackCount = pi.StackCount,
                SuffixRecord = pi.SuffixRecord,
                TransmuteRecord = pi.TransmuteRecord
            };
        }

        public static PlayerItem ToPlayerItem(AzureItem item) {
            return new PlayerItem {
                BaseRecord = item.BaseRecord,
                EnchantmentRecord = item.EnchantmentRecord,
                EnchantmentSeed = item.EnchantmentSeed,
                IsHardcore = item.IsHardcore,
                MateriaCombines = item.MateriaCombines,
                MateriaRecord = item.MateriaRecord,
                Mod = item.Mod,
                ModifierRecord = item.ModifierRecord,
                PrefixRecord = item.PrefixRecord,
                RelicCompletionBonusRecord = item.RelicCompletionBonusRecord,
                RelicSeed = item.RelicSeed,
                Seed = item.Seed,
                StackCount = item.StackCount,
                SuffixRecord = item.SuffixRecord,
                TransmuteRecord = item.TransmuteRecord,
                AzureUuid = item.Id,
                AzurePartition = item.Partition
            };
        }
    }
}
