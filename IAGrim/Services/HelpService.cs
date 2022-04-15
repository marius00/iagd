﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IAGrim.Utilities;

namespace IAGrim.Services {
    public class HelpService : IHelpService {
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
            NotLootingUnidentified,
            DeleteDuplicates,
            MultiplePcs,
            RegularUpdates,
            ExperimentalUpdates,
            WhatIsBuddyId,
            WhatIsBuddyNickname,
            OnlineBackups,
            NotEnoughStashTabs,
            StashError,
            No32Bit,
            WindowsAntiRansomwareIssue,
        }

        public void ShowHelp(HelpType type) {
            Process.Start($"https://grimdawn.evilsoft.net/help/?q={type.ToString()}&r={DateTime.UtcNow.Ticks}");
        }

        public void ShowCharacterBackups() {
            MessageBox.Show("Error - Not implemented!");
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
    }
}
