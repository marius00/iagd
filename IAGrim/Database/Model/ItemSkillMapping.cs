namespace IAGrim.Database {
    public class ItemSkillMapping {
        public virtual int Id { get; set; }
        public virtual int DatabaseItemId { get; set; }

        public override bool Equals(object obj) {
            if (obj is ItemSkillMapping that) {
                return this.Id == that.Id && this.DatabaseItemId == that.DatabaseItemId;
            }
            else {
                return base.Equals(obj);
            }
        }

        public override int GetHashCode() {
            return (Id * DatabaseItemId).GetHashCode();
        }
    }
}