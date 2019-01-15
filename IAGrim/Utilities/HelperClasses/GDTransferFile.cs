using IAGrim.UI;
using Newtonsoft.Json;
using System;

namespace IAGrim.Utilities.HelperClasses
{
    public class GDTransferFile : ComboBoxItemToggle, IEquatable<GDTransferFile> {
        public string Filename { get; set; }

        public bool IsHardcore { get; set; }

        public string Mod { get; set; }

        public virtual bool Enabled { get; set; }

        public bool IsExpansion1 { get; set; }

        [JsonIgnore]
        public virtual DateTime LastAccess { get; set; }
        
        public override string ToString() {
            string text = string.IsNullOrEmpty(Mod) ? GlobalSettings.Language.GetTag("iatag_ui_vanilla") : Mod;

            if (IsHardcore)
            {
                return $"{text}{GlobalSettings.Language.GetTag("iatag_ui_hc")}";
            }
               
            return text;
        }

        public bool Equals(GDTransferFile other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(Filename, other.Filename) 
                   && IsHardcore == other.IsHardcore 
                   && string.Equals(Mod, other.Mod) 
                   && IsExpansion1 == other.IsExpansion1;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((GDTransferFile) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Filename != null ? Filename.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ IsHardcore.GetHashCode();
                hashCode = (hashCode * 397) ^ (Mod != null ? Mod.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ IsExpansion1.GetHashCode();
                return hashCode;
            }
        }
    }
}
