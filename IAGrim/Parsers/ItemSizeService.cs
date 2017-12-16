using IAGrim.Database.Interfaces;
using IAGrim.StashFile;
using IAGrim.Utilities;
using log4net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Parsers {
    class ItemSizeService {
        private static Dictionary<String, Point> itemSizeCache = new Dictionary<string, Point>();
        private static ILog logger = LogManager.GetLogger(typeof(ItemSizeService));
        private readonly IDatabaseItemStatDao dao;

        public static bool Is1x1(String filename) {
            int h;
            int w;
            MapItemSize(filename, out h, out w);
            return h <= 1 && w <= 1;
        }

        public ItemSizeService(IDatabaseItemStatDao databaseItemStatDao) {
            this.dao = databaseItemStatDao;
        }

        public void MapItemSizes(List<Item> items) {
            Dictionary<string, string> iconMap = dao.MapItemBitmaps(items.Select(m => m.BaseRecord).ToList());

            for (int i = 0; i < items.Count; i++) {
                var bitmap = string.Format("{0}.png", Path.GetFileName(iconMap[items[i].BaseRecord].Replace(".dbr", ".tex")));
                MapItemSize(bitmap, out items[i].Height, out items[i].Width);
            }
        }

        public void MapItemSize(Item item) {
            Dictionary<string, string> iconMap = dao.MapItemBitmaps(new List<string>() { item.BaseRecord });

            var bitmap = string.Format("{0}.png", Path.GetFileName(iconMap[item.BaseRecord].Replace(".dbr", ".tex")));
            MapItemSize(bitmap, out item.Height, out item.Width);
        }

        public void CacheItemSizes(IEnumerable<Database.PlayerItem> items) {
            Dictionary<string, string> iconMap = dao.MapItemBitmaps(items.Select(m => m.BaseRecord).ToList());

            foreach (var item in items) {
                var bitmap = string.Format("{0}.png", Path.GetFileName(iconMap[item.BaseRecord].Replace(".dbr", ".tex")));
                int w, h;
                MapItemSize(bitmap, out h, out w);
            }
        }


        public static void MapItemSize(string filename, out int height, out int width) {            
            int w = 2;
            int h = 4;
            if (itemSizeCache.ContainsKey(filename)) {
                height = itemSizeCache[filename].X;
                width = itemSizeCache[filename].Y;
                return;
            }

            string fullpath = Path.Combine(GlobalPaths.StorageFolder, filename);
            try {
                if (File.Exists(fullpath)) {
                    using (FileStream fs = new FileStream(fullpath, FileMode.Open)) {
                        using (Image img = Image.FromStream(fs, false, false)) {
                            w = img.Width / 32;
                            h = img.Height / 32;
                            itemSizeCache[filename] = new Point(h, w);
                        }
                    }
                }
            }
            catch (IOException) {
                logger.InfoFormat("Got an IOException reading file {0}, just assuming size {1}x{2}", fullpath, w, h);
            }

            height = h;
            width = w;
        }
    }
}
