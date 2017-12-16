namespace IAGrim.Database.Interfaces {
    public interface IDatabaseSettingDao : IBaseDao<DatabaseSetting> {
        long GetLastDatabaseUpdate();
        void UpdateCurrentDatabase(string path);
        string GetCurrentDatabasePath();
        void UpdateRecipeTimestamp(long ts);
        void UpdateDatabaseTimestamp(long ts);

        string GetUuid();
        void SetUuid(string uuid);
    }
}