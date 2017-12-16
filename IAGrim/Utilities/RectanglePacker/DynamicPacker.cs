using IAGrim.Database;
using IAGrim.Database.Interfaces;
using IAGrim.Parsers;
using IAGrim.Parsers.Arz;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Utilities.RectanglePacker {
    class DynamicPacker {
        private Packer packer;
        private readonly IDatabaseItemStatDao itemStatDao;
        private readonly Dictionary<string, Shape> itemShapeCache = new Dictionary<string, Shape>();
        private readonly List<ItemInsertion> Queue = new List<ItemInsertion>();

        public DynamicPacker(IDatabaseItemStatDao itemStatDao) {
            this.itemStatDao = itemStatDao;
        }

        /// <summary>
        /// Load the item sizes for the given records
        /// </summary>
        /// <param name="baseRecords"></param>
        private void LoadItemSizes(IEnumerable<string> baseRecords) {
            var relevant = baseRecords.Where(m => !itemShapeCache.Keys.Contains(m));
            var tmp = itemStatDao.MapItemBitmaps(relevant.ToList());
            foreach (var key in tmp.Keys) {

                var bitmap = string.Format("{0}.png", Path.GetFileName(tmp[key].Replace(".dbr", ".tex")));
                int h, w;
                ItemSizeService.MapItemSize(bitmap, out h, out w);

                itemShapeCache[key] = new Shape {
                    Width = w,
                    Height = h
                };
            }
            
        }

        /// <summary>
        /// Clear the InventorySack
        /// Should be called every time the stash is closed
        /// </summary>
        public void Clear() {
            if (packer != null) {
                Queue.Clear();
                packer = new Packer(packer.Height, packer.Width);
            }
        }


        /// <summary>
        /// Gets and reserves a position for the given item
        /// Returns null if there is no room.
        /// </summary>
        /// <param name="baseRecord"></param>
        /// <param name="seed"></param>
        /// <returns>A valid position of null if stash is full</returns>
        public Packer.Position Insert(string baseRecord, uint seed) {
            // Load item sizes
            List<string> records = new List<string>(Queue.Select(m => m.BaseRecord));
            records.Add(baseRecord);
            LoadItemSizes(records);

            // Insert any queued items
            foreach (var elem in Queue) {
                packer.Insert(itemShapeCache[elem.BaseRecord], elem.X, elem.Y);
            }
            Queue.Clear();


            // Calculate the position for this item
            return packer.Insert(itemShapeCache[baseRecord]);
        }

        /// <summary>
        /// Enqueue an item for insertion
        /// This should be called when getting messages from GD
        /// </summary>
        /// <param name="baseRecord"></param>
        /// <param name="seed"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Insert(string baseRecord, uint seed, uint x, uint y) {
            Queue.Add(
                new ItemInsertion {
                    BaseRecord = baseRecord,
                    Seed = seed,
                    X = x,
                    Y = y
                });
        }

        public void Initialize(uint width, uint height) {
            packer = new RectanglePacker.Packer(height, width);
        }


        private class ItemInsertion {
            public string BaseRecord { get; set; }
            public uint Seed { get; set; }

            public uint X { get; set; }
            public uint Y { get; set; }
        }
    }
}
