using System;
using System.ComponentModel.DataAnnotations;
using EvilsoftCommons;
using IAGrim.Backup.Cloud.Dto;
using IAGrim.Database;

namespace IAGrim.Backup.Cloud.Util {
    static class ItemConverter {
        public static CloudItemDto ToUpload(PlayerItem pi) {
            return new CloudItemDto {
                BaseRecord = pi.BaseRecord,
                EnchantmentRecord = pi.EnchantmentRecord,
                EnchantmentSeed = pi.EnchantmentSeed,
                IsHardcore = pi.IsHardcore,
                MateriaCombines = pi.MateriaCombines,
                MateriaRecord = pi.MateriaRecord,
                Mod = pi.Mod,
                ModifierRecord = pi.ModifierRecord,
                PrefixRecord = pi.PrefixRecord,
                RelicCompletionBonusRecord = pi.RelicCompletionBonusRecord,
                RelicSeed = pi.RelicSeed,
                Seed = pi.Seed,
                StackCount = Math.Max(pi.StackCount, 1),
                SuffixRecord = pi.SuffixRecord,
                TransmuteRecord = pi.TransmuteRecord,
                Id = pi.CloudId,
                CreatedAt = pi.CreationDate ?? DateTime.UtcNow.ToTimestamp(),
                LevelRequirement = (int)pi.MinimumLevel,
                Name = pi.Name,
                NameLowercase = pi.NameLowercase,
                Rarity = pi.Rarity,
                PrefixRarity = pi.PrefixRarity,
            };
        }

        public static PlayerItem ToPlayerItem(CloudItemDto itemDto) {
            return new PlayerItem {
                BaseRecord = itemDto.BaseRecord,
                EnchantmentRecord = itemDto.EnchantmentRecord,
                EnchantmentSeed = itemDto.EnchantmentSeed,
                IsHardcore = itemDto.IsHardcore,
                MateriaCombines = itemDto.MateriaCombines,
                MateriaRecord = itemDto.MateriaRecord,
                Mod = itemDto.Mod,
                ModifierRecord = itemDto.ModifierRecord,
                PrefixRecord = itemDto.PrefixRecord,
                RelicCompletionBonusRecord = itemDto.RelicCompletionBonusRecord,
                RelicSeed = itemDto.RelicSeed,
                Seed = itemDto.Seed,
                StackCount = itemDto.StackCount,
                SuffixRecord = itemDto.SuffixRecord,
                TransmuteRecord = itemDto.TransmuteRecord,
                CloudId = itemDto.Id,
                IsCloudSynchronized = true,
                CreationDate = itemDto.CreatedAt,
                LevelRequirement = itemDto.LevelRequirement,
                Name = itemDto.Name,
                NameLowercase = itemDto.NameLowercase,
                Rarity = itemDto.Rarity
                
            };
        }
    }
}
