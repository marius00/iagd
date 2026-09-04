using System;

namespace IAGrim.Settings.Dto {
    /// <summary>
    /// What the cloud is known to hold for one character.
    /// </summary>
    public class CharacterBackupState {
        /// <summary>
        /// Digest of the save files as they were when the server accepted them.
        /// </summary>
        public string Hash { get; set; } = string.Empty;

        /// <summary>
        /// When the server last accepted a backup for this character. The server stores
        /// at most one backup per character per day, so there is no point offering it
        /// another one before then.
        /// </summary>
        public DateTime UploadedUtc { get; set; }
    }
}
