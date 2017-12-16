using IAGrim.StashFile;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpTestsEx;

namespace Tests_IAGrim.ParserTests {

    [TestClass]
    public class StastTabTests {
        private Item item01 = new Item {
            BaseRecord = "baserecord",
            Seed = 12345
        };



        [TestMethod]
        public void TestAddItemWhichIsNotStackable() {
            StashTab tab = new StashTab();

            tab.AddItem(new Item {
                BaseRecord = "not-stackable",
                Seed = 1,
                RelicSeed = 0,
                EnchantmentSeed = 0,
                StackCount = 2
            }, "something");
            tab.Items.Should().Have.Count.EqualTo(1);


            tab.AddItem(new Item {
                BaseRecord = "not-stackable",
                Seed = 1,
                RelicSeed = 0,
                EnchantmentSeed = 0,
                StackCount = 2
            }, "something");
            tab.Items.Should().Have.Count.EqualTo(2);


            tab.AddItem(new Item {
                BaseRecord = "not-stackable",
                Seed = 1,
                RelicSeed = 0,
                EnchantmentSeed = 0,
                StackCount = 2
            }, "something");
            tab.Items.Should().Have.Count.EqualTo(3);
        }


        [TestMethod]
        public void TestAddItemCannotExeed100() {
            StashTab tab = new StashTab();

            tab.AddItem(new Item {
                BaseRecord = "stackable",
                Seed = 1,
                RelicSeed = 0,
                EnchantmentSeed = 0,
                StackCount = 99
            }, "ItemRelic");
            tab.Items.Should().Have.Count.EqualTo(1);

            tab.AddItem(new Item {
                BaseRecord = "stackable",
                Seed = 1,
                RelicSeed = 0,
                EnchantmentSeed = 0,
                StackCount = 1
            }, "ItemRelic");
            tab.Items.Should().Have.Count.EqualTo(1);

            tab.AddItem(new Item {
                BaseRecord = "stackable",
                Seed = 1,
                RelicSeed = 0,
                EnchantmentSeed = 0,
                StackCount = 1
            }, "ItemRelic");
            tab.Items.Should().Have.Count.EqualTo(2);

            tab.Items.Sum(item => item.StackCount).Should().Be.EqualTo(101);

        }

        [TestMethod]
        public void TestAddItem() {
            StashTab tab = new StashTab();
            tab.AddItem(item01, "ItemRelic");
            tab.Items.Should().Have.Count.EqualTo(1);


            tab.AddItem(new Item {
                BaseRecord = "stackable",
                Seed = 1,
                RelicSeed = 0,
                EnchantmentSeed = 0
            }, "ItemRelic");
            tab.Items.Should().Have.Count.EqualTo(2);

            tab.AddItem(new Item {
                BaseRecord = "stackable",
                Seed = 1,
                RelicSeed = 0,
                EnchantmentSeed = 0
            }, "ItemRelic");
            tab.Items.Should().Have.Count.EqualTo(2);

            tab.AddItem(new Item {
                BaseRecord = "stackable",
                Seed = 1,
                RelicSeed = 0,
                EnchantmentSeed = 0
            }, "ItemRelic");
            tab.Items.Should().Have.Count.EqualTo(2);

            tab.Items.Where(item => item.Seed == 1).FirstOrDefault().StackCount.Should().Be.EqualTo(3);
        }
    }
}
