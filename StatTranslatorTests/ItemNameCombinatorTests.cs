using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatTranslator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpTestsEx;

namespace StatTranslatorTests {
    [TestClass]
    public class ItemNameCombinatorTests {
        [TestMethod]
        public void TestItemNames() {

            {
                var ptbr = "{%_s3}{%_3a1}{%_3a2}{%_3a0}{%_s4}";
                string prefix = "[ms]Robusto[fs]Robusta[mp]Robustos[fp]Robustas";
                string quality = "";
                string style = "";
                string name = "[fs]Navalha";
                string suffix = "";
                var c = new ItemNameCombinator(ptbr);
                c.TranslateName(prefix, quality, style, name, suffix).Should().Be.EqualTo("Navalha Robusta");
            }
            {
                var ptbr = "{%_s3}{%_3a1}{%_3a2}{%_3a0}{%_s4}";
                string prefix = "[ms]Robusto[fs]Robusta[mp]Robustos[fp]Robustas";
                string quality = "";
                string style = "";
                string name = "[ms]Navalha";
                string suffix = "";
                var c = new ItemNameCombinator(ptbr);
                c.TranslateName(prefix, quality, style, name, suffix).Should().Be.EqualTo("Navalha Robusto");
            }
            {
                var polish = "{%_3a0}{%_3a1}{%_3a2}{%_s3}{%_3a4}";
                string prefix = "[ms]energetyczny[fs]energetyczna[ns]energetyczne[mp]energetyczni[fp]energetyczne[np]energetyczne";
                string quality = "";
                string style = "";
                string name = "[ms]Nożyk";
                string suffix = "$Aetheru";
                var c = new ItemNameCombinator(polish);
                c.TranslateName(prefix, quality, style, name, suffix).Should().Be.EqualTo("energetyczny Nożyk Aetheru");
            }
            {
                var english = "{%_s0}{%_s1}{%_s2}{%_s3}{%_s4}";
                string prefix = "Aetherfire";
                string quality = "";
                string style = "";
                string name = "Adept's Dagger";
                string suffix = "of Aether Storms";
                var c = new ItemNameCombinator(english);
                c.TranslateName(prefix, quality, style, name, suffix).Should().Be.EqualTo("Aetherfire Adept's Dagger of Aether Storms");
            }
        }


        [TestMethod]
        public void TestGenderDetection() {
            {
                var gender = ItemNameCombinator.DetermineGender("[fs]Navalha");
                ItemNameCombinator.GetGendered("[ms]Robusto[fs]Robusta[mp]Robustos[fp]Robustas", gender).Should().Be.EqualTo("Robusta");
            }
            {
                var gender = ItemNameCombinator.DetermineGender("[fp]Navalha");
                ItemNameCombinator.GetGendered("[ms]Robusto[fs]Robusta[mp]Robustos[fp]Robustas", gender).Should().Be.EqualTo("Robustas");
            }
            {
                var gender = ItemNameCombinator.DetermineGender("[ms]Navalha");
                ItemNameCombinator.GetGendered("[ms]Robusto[fs]Robusta[mp]Robustos[fp]Robustas", gender).Should().Be.EqualTo("Robusto");
            }
        }
    }
}
