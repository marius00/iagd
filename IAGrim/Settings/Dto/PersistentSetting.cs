using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Settings.Dto {
    public enum PersistentSetting {
        // Buddy items
        BuddySyncUserIdV2,
        BuddySyncDescription,
        BuddySyncEnabled,

        // Settings
        SubscribeExperimentalUpdates,
        ShowRecipesAsItems,
        MinimizeToTray,
        UsingDualComputer,
        ShowAugmentsAsItems,
        MergeDuplicates,
        TransferAnyMod,
        AutoUpdateModSettings,
        DisplaySkills,

        // Azure Backups
        AzureAuthToken,

    }
}
