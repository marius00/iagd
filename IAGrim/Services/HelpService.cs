using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.Utilities;

namespace IAGrim.Services {
    public static class HelpService {
        public enum HelpType {
            BuddyItems,
            CloudSavesEnabled,
            CannotFindGrimdawn,
            ShowRecipesAsItems,
            ShowAugmentsAsItems,
            SecureTransfers,
            TransferToAnyMod,
            RestoreBackup,
            DuplicateItem,
            NoStacks,
            BackupAutodetectDisabled,
            NotLootingUnidentified
        }

        public static void ShowHelp(HelpType type) {
            Process.Start($"http://grimdawn.dreamcrash.org/ia/help.html?q={type.ToString()}&r={DateTime.UtcNow.Ticks}");
        }

        public static string GetUrl(HelpType type) {
            return $"http://grimdawn.dreamcrash.org/ia/help.html?q={type.ToString()}&r={DateTime.UtcNow.Ticks}";
        }
    }
}
