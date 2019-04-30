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
        private Packer _packer;
        private readonly IDatabaseItemStatDao _itemStatDao;
        private readonly Dictionary<string, Shape> _itemShapeCache = new Dictionary<string, Shape>();
        private readonly List<ItemInsertion> _queue = new List<ItemInsertion>();

        public DynamicPacker(IDatabaseItemStatDao itemStatDao) {
            this._itemStatDao = itemStatDao;
        }

        /// <summary>
        /// Load the item sizes for the given records
        /// </summary>
        /// <param name="baseRecords"></param>
        private void LoadItemSizes(IEnumerable<string> baseRecords) {
            var relevant = baseRecords.Where(m => !_itemShapeCache.Keys.Contains(m));
            var tmp = _itemStatDao.MapItemBitmaps(relevant.ToList());
            foreach (var key in tmp.Keys) {

                var bitmap = $"{Path.GetFileName(tmp[key].Replace(".dbr", ".tex"))}.png";
                int h, w;
                ItemSizeService.MapItemSize(bitmap, out h, out w);

                _itemShapeCache[key] = new Shape {
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
            if (_packer != null) {
                _queue.Clear();
                _packer = new Packer(_packer.Height, _packer.Width);
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
            List<string> records = new List<string>(_queue.Select(m => m.BaseRecord));
            records.Add(baseRecord);
            LoadItemSizes(records);

            // Insert any queued items
            foreach (var elem in _queue) {
                _packer.Insert(_itemShapeCache[elem.BaseRecord], elem.X, elem.Y);
            }
            _queue.Clear();


            // Calculate the position for this item
            return _packer.Insert(_itemShapeCache[baseRecord]);
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
            _queue.Add(
                new ItemInsertion {
                    BaseRecord = baseRecord,
                    Seed = seed,
                    X = x,
                    Y = y
                });
        }

        public void Initialize(uint width, uint height) {
            _packer = new RectanglePacker.Packer(height, width);
        }


        private class ItemInsertion {
            public string BaseRecord { get; set; }
            public uint Seed { get; set; }

            public uint X { get; set; }
            public uint Y { get; set; }
        }
    }
}
