using System;

namespace IAGrim.Database.Interfaces {
    [Obsolete]
    public interface IDatabaseSettingDao : IBaseDao<DatabaseSetting> {
        void UpdateCurrentDatabase(string path);
        string GetCurrentDatabasePath();

        [Obsolete]
        string GetUuid();
        void Clean();
    }
}