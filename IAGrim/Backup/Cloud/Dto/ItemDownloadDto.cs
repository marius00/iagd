using System.Collections.Generic;

namespace IAGrim.Backup.Cloud.Dto {
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ItemDownloadDto {
        public List<CloudItemDto>? Items { get; set; }
        public List<DeleteItemDto>? Removed { get; set; }
        public long Timestamp { get; set; }

        /// <summary>
        /// Partial item batch, may be more items.
        /// </summary>
        public bool IsPartial { get; set; }
    }
}
