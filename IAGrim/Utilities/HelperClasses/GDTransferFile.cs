using System;

namespace IAGrim.Utilities.HelperClasses {
    public class GDTransferFile : IComboBoxItemToggle, IEquatable<GDTransferFile> {
        public bool IsHardcore { get; set; }

        public string? Mod { get; set; }

        public virtual bool Enabled { get; set; }

        public override string ToString() {
            var text = string.IsNullOrEmpty(Mod) ? RuntimeSettings.Language!.GetTag("iatag_ui_vanilla") : Mod;

            if (IsHardcore) {
                return $"{text}{RuntimeSettings.Language!.GetTag("iatag_ui_hc")}";
            }

            return text;
        }

        public bool Equals(GDTransferFile? other) {
            if (ReferenceEquals(null, other)) {
                return false;
            }

            if (ReferenceEquals(this, other)) {
                return true;
            }

            return IsHardcore == other.IsHardcore
                   && string.Equals(Mod, other.Mod);
        }

        public override bool Equals(object? obj) {
            if (ReferenceEquals(null, obj)) {
                return false;
            }

            if (ReferenceEquals(this, obj)) {
                return true;
            }

            if (obj.GetType() != GetType()) {
                return false;
            }

            return Equals((GDTransferFile) obj);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = IsHardcore.GetHashCode();
                hashCode = (hashCode * 397) ^ (Mod != null ? Mod.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
