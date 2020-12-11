using DataAccess;
using IAGrim.Database.Interfaces;
using IAGrim.Services.Dto;
using IAGrim.Utilities;
using StatTranslator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using IAGrim.Database.Model;

namespace IAGrim.Database {
    public class BuddyItemRecord {
        public virtual long Id { get; set; }
        public virtual long BuddyId { get; set; }
        public virtual string Record { get; set; }

        public override bool Equals(object obj) {
            if (obj is BuddyItemRecord that) {
                return this.BuddyId == that.BuddyId && this.Record == that.Record;
            }
            else {
                return base.Equals(obj);
            }
        }

        public override int GetHashCode() {
            return (BuddyId + Record).GetHashCode();
        }
    }
}

