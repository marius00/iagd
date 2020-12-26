using System.Collections.Generic;

namespace IAGrim.Backup.Cloud.Dto {
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ItemDownloadDto {
        public List<CloudItemDto> Items { get; set; }
        public List<DeleteItemDto> Removed { get; set; }
        public long Timestamp { get; set; }
    }
}
