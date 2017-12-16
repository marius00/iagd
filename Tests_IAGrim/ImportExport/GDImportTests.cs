using IAGrim.Backup.FileWriter;
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
    public class GDImportTests {
        private static FileExporter importer;
        private const string ModName = "";
        private const bool IsExpansion1 = false;
        private const string SourceFile = @"ImportExport\gd_3_items.gds";

        [ClassInitialize]
        public static void Setup(TestContext context) {
            importer = new GDFileExporter(SourceFile, IsExpansion1, ModName);
        }

        [TestMethod]
        public void CanLoadFile() {
            Executing.This(() => {
                importer.Read();
            }).Should().NotThrow();
        }

        [TestMethod]
        public void ContainsFourValidItems() {
            var items = importer.Read();
            items.Count.Should().Be.EqualTo(4);

            foreach (var item in items) {
                item.BaseRecord.Should().StartWith("records/");
            }
        }



        [TestMethod]
        public void CanWriteItems() {
            var items = importer.Read();

            using (var file = new TempFile()) {
                var exporter = new GDFileExporter(file.filename, IsExpansion1, ModName);
                exporter.Write(items);

                // Byte wise compare
                byte[] written = File.ReadAllBytes(file.filename);
                byte[] read = File.ReadAllBytes(SourceFile);
                written.Length.Should().Be.EqualTo(read.Length);

                for (int i = 0; i < written.Length; i++) {
                    written[i].Should().Be.EqualTo(read[i]);
                }

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
