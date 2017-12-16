using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.Services.Dto;

namespace IAGrim.Database.Model {
    public class DatabaseItemDto {
        public string Name { get; set; }

        public string Record { get; set; }
        public ISet<RecipeDbStatRow> Stats { get; } = new HashSet<RecipeDbStatRow>();
    }
}
