using IAGrim.Backup.Cloud;
using IAGrim.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IAGrim.Services {
    public class HelpService : IHelpService {
        public enum HelpType {
            BuddyItems,
            CloudSavesEnabled,
            CannotFindGrimdawn,
            TransferToAnyMod,
            RestoreBackup,
            DuplicateItem,
            NoStacks,
            NotLootingUnidentified,
            MultiplePcs,
            DelayWhenSearching,
            RegularUpdates,
            ExperimentalUpdates,
            WhatIsBuddyId,
            WhatIsBuddyNickname,
            OnlineBackups,
            NotEnoughStashTabs,
            StashError,
            PathError,
            No32Bit,
            WindowsAntiRansomwareIssue,
        }

        public void ShowHelp(HelpType type) {
            Process.Start(new ProcessStartInfo { FileName = $"https://grimdawn.evilsoft.net/help/?q={type.ToString()}&r={DateTime.UtcNow.Ticks}", UseShellExecute = true });
        }

        public void ShowCharacterBackups() {
            MessageBox.Show("Error - Not implemented!");
        }

        public void SetIsFirstRun(bool v) {
            // shrug..
        }

        public void SetIsGrimParsed(bool enabled) {
            // shrug..
        }

        public static string GetUrl(HelpType type) {
            return $"https://grimdawn.evilsoft.net/help/?q={type.ToString()}&r={DateTime.UtcNow.Ticks}";
        }
    }

    public interface IHelpService {
        void ShowHelp(HelpService.HelpType type);
        void ShowCharacterBackups(); // TODO: Not strictly the right place for it..
        void SetIsGrimParsed(bool enabled); // TODO: Not strictly the right place for it..
        void SetIsFirstRun(bool value);
    }
}
