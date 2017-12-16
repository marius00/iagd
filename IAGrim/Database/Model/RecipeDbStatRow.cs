using IAGrim.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Database.Model {
    public class RecipeDbStatRow : DBSTatRow {
        public DatabaseItemDto Parent { get; set; }
    }
}
