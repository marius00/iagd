namespace IAGrim.Database {
    /// <summary>
    /// Represents a deleted player item which has an online-presence.
    /// Used to delete items from the online backup.
    /// </summary>
    public class DeletedPlayerItem {
        public virtual string Id { get; set; }

        public override bool Equals(object obj) {
            if (obj is DeletedPlayerItem that) {
                return this.Id == that.Id;
            }
            else {
                return base.Equals(obj);
            }
        }

        public override int GetHashCode() {
            return Id.GetHashCode();
        }
    }
}
