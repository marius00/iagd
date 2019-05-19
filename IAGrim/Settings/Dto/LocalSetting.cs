namespace IAGrim.Settings.Dto {
    public enum LocalSetting {
        GrimDawnLocation,
        BackupNumber, // TODO; Make two of these.. and phase out local/persisted
        LastNagTimestamp,
        EasterPrank,
        HasSuggestedLanguageChange,
        SecureTransfers,
        StashToDepositTo,
        StashToLootFrom,
        LastSelectedMod, // Odd one..
        LocalizationFile,

        WindowPositionSettings,

        // Backup
        BackupDropbox,
        BackupGoogle,
        BackupOnedrive,
        BackupCustom,
        BackupCustomLocation,
        OptOutOfBackups,
    }
}
