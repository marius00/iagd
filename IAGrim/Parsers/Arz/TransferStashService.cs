using EvilsoftCommons;
using IAGrim.Database;
using IAGrim.StashFile;
using IAGrim.Utilities;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using IAGrim.Parser.Stash;
using IAGrim.Settings;
using IAGrim.Services;

namespace IAGrim.Parsers.Arz {
    internal class TransferStashService {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(TransferStashService));

        private readonly SettingsService _settings;


        public TransferStashService(SettingsService settings) {
            _settings = settings;
        }

        public int GetStashToLootFrom(Stash stash) {
            if (_settings.GetLocal().StashToLootFrom == 0) {
                return stash.Tabs.Count - 1;
            }

            return (int) _settings.GetLocal().StashToLootFrom - 1;
        }


        /// <summary>
        /// Attempt to get the name of the current mod
        /// Vanilla leaves this tag empty
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryGetModLabel(string filename, out string result) {
            if (File.Exists(filename)) {
                var pCrypto = new GDCryptoDataBuffer(DataBuffer.ReadBytesFromDisk(filename));
                var stash = new Stash();

                if (stash.Read(pCrypto)) {
                    result = stash.ModLabel;
                    return true;
                }
                else {
                    Logger.Warn($"Discarding transfer file \"{filename}\", could not read the file.");
                }
            }

            result = string.Empty;
            return false;
        }



        public static Stash GetStash(string filename) {
            var pCrypto = new GDCryptoDataBuffer(DataBuffer.ReadBytesFromDisk(filename));
            var stash = new Stash();

            return stash.Read(pCrypto) ? stash : null;
        }


        public void Deposit(IList<PlayerItem> playerItems) {
            foreach (var item in playerItems) {
                var csv = CsvParsingService.Serialize(item);
                try {
                    var path = Path.Combine(GlobalPaths.CsvLocationOutgoing, item.IsHardcore ? "hc" : "sc");
                    if (item.Mod != string.Empty) {
                        path = Path.Combine(path, item.Mod);
                    }
                    Directory.CreateDirectory(path);

                    File.WriteAllText(Path.Combine(path, Guid.NewGuid().ToString() + ".csv"), csv);
                    Logger.Info($"Wrote item to {path}");
                    item.StackCount = 0;
                } catch (IOException e) {
                    Logger.Warn(e.Message, e);
                }
            }
                    
        }


        public static PlayerItem Map(Item item, string mod, bool isHardcore) {
            return new PlayerItem {
                BaseRecord = item.BaseRecord,
                EnchantmentRecord = item.EnchantmentRecord,
                EnchantmentSeed = item.EnchantmentSeed,
                MateriaCombines = item.MateriaCombines,
                MateriaRecord = item.MateriaRecord,
                ModifierRecord = item.ModifierRecord,
                PrefixRecord = item.PrefixRecord,
                RelicCompletionBonusRecord = item.RelicCompletionBonusRecord,
                RelicSeed = item.RelicSeed,
                Seed = item.Seed,
                StackCount = Math.Max(1, item.StackCount),
                SuffixRecord = item.SuffixRecord,
                TransmuteRecord = item.TransmuteRecord,
                UNKNOWN = item.UNKNOWN,
                Mod = mod,
                IsHardcore = isHardcore,
                CreationDate = DateTime.UtcNow.ToTimestamp(),
                CloudId = Guid.NewGuid().ToString()
            };
        }

        public static Item Map(PlayerItem item) {
            return new Item {
                BaseRecord = item.BaseRecord,
                EnchantmentRecord = item.EnchantmentRecord,
                EnchantmentSeed = (uint) item.EnchantmentSeed,
                MateriaCombines = (uint) item.MateriaCombines,
                MateriaRecord = item.MateriaRecord,
                ModifierRecord = item.ModifierRecord,
                PrefixRecord = item.PrefixRecord,
                RelicCompletionBonusRecord = item.RelicCompletionBonusRecord,
                RelicSeed = (uint) item.RelicSeed,
                Seed = item.USeed,
                StackCount = Math.Max(1, (uint) item.StackCount),
                SuffixRecord = item.SuffixRecord,
                TransmuteRecord = item.TransmuteRecord,
                UNKNOWN = (uint) item.UNKNOWN
            };
        }
    }
}