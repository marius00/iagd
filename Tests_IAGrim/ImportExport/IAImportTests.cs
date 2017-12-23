using IAGrim.Backup.FileWriter;
using IAGrim.Database;
using IAGrim.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpTestsEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests_IAGrim.ImportExport {
    [TestClass]
    public class IAImportTests {
        private static FileExporter importer;
        private const string SourceFile = @"ImportExport\test123.ias";

        [ClassInitialize]
        public static void Setup(TestContext context) {
            importer = new IAFileExporter(SourceFile);
        }

        [TestMethod]
        public void CanLoadFile() {
            Executing.This(() => {
                importer.Read();
            }).Should().NotThrow();
        }

        [TestMethod]
        public void CanReadItems() {
            var items = importer.Read();
            items.Count.Should().Be.EqualTo(1562);
        }


        [TestMethod]
        public void WriteItemsForSillyPeople() {
            Random r = new Random();
            var items = new List<PlayerItem>();

            for (int i = 0; i < 8; i++) {
                var item = new PlayerItem() {
                    Count = 1,
                    //PrefixRecord = "records/items/lootaffixes/prefix/b_class021_shaman19_je_c.dbr",
                    BaseRecord = "records/items/gearhead/c034_head.dbr",
                    //SuffixRecord = "records/items/lootaffixes/suffix/a031d_off_dmg%lightning_07_we2h.dbr",
                    Seed = r.Next()
                };
                items.Add(item);
            }

            var path = @"f:\temp\export.ias";
            var exporter = new IAFileExporter(path);
            exporter.Write(items);


            var verifiy = exporter.Read();
            verifiy.Count.Should().Be.EqualTo(items.Count);
        }

        [TestMethod]
        public void CanWriteItems() {
            var items = new List<PlayerItem>();
            for (int i = 0; i < 4; i++) {
                items.Add(new PlayerItem {
                    BaseRecord = "records/br1"
                });
            }

            using (var file = new TempFile()) {
                var exporter = new IAFileExporter(file.filename);
                exporter.Write(items);
                /*
                // Byte wise compare
                byte[] written = File.ReadAllBytes(file.filename);
                byte[] read = File.ReadAllBytes(file.filename);
                written.Length.Should().Be.EqualTo(read.Length);

                for (int i = 0; i < written.Length; i++) {
                    written[i].Should().Be.EqualTo(read[i]);
                }*/

                // TOOD: Is this proper? its read related testing..
                var verifiy = exporter.Read();
                verifiy.Count.Should().Be.EqualTo(items.Count);

                foreach (var item in verifiy) {
                    item.BaseRecord.Should().StartWith("records/");
                }
            }
        }
    }
}
