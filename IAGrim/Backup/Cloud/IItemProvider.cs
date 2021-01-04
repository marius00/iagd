using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.Backup.Cloud.Dto;

namespace IAGrim.Backup.Cloud {
    public interface IItemProvider {
        IList<ItemIdentifierDto> GetItemsMarkedForOnlineDeletion();
        void ClearItemsMarkedForOnlineDeletion();
        
        IList<CloudItemDto> GetUnsynchronizedItems();
        void MarkAsSynchronized(IList<CloudItemDto> items);
    }
}
