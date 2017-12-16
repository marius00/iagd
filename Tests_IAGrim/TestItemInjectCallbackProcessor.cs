using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpTestsEx;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IAGrim.Database;
using IAGrim.Services.MessageProcessor;

namespace Tests_IAGrim {
    [TestClass]
    public class TestItemInjectCallbackProcessor {
        [TestMethod]
        public void TestCanSerialize() {
            Random r = new Random();
            PlayerItem pi = new PlayerItem {
                Seed = (uint)r.Next(),
                RelicSeed = (uint)r.Next(),
                UNKNOWN = (uint)r.Next(),
                EnchantmentSeed = (uint)r.Next(),
                MateriaCombines = (uint)r.Next(),
                StackCount = (uint)r.Next(1, 100),
                BaseRecord = "base" + RandomString(r, r.Next(10, 150)),
                PrefixRecord = "prefix" + RandomString(r, r.Next(10, 150)),
                SuffixRecord = "suffix" + RandomString(r, r.Next(10, 150)),
                ModifierRecord = "modifier" + RandomString(r, r.Next(10, 150)),
                MateriaRecord = "materia" + RandomString(r, r.Next(10, 150)),
                RelicCompletionBonusRecord = "relic" + RandomString(r, r.Next(10, 150)),
                EnchantmentRecord = "enchant" + RandomString(r, r.Next(10, 150)),
                TransmuteRecord = "transmute" + RandomString(r, r.Next(10, 150)),
            };

            List<byte> serialized = ItemInjectCallbackProcessor.Serialize(pi);
            PlayerItem deserialized = ItemInjectCallbackProcessor.Deserialize(serialized.ToArray());

            pi.Seed.Should().Be.EqualTo(deserialized.Seed);
            pi.RelicSeed.Should().Be.EqualTo(deserialized.RelicSeed);
            pi.UNKNOWN.Should().Be.EqualTo(deserialized.UNKNOWN);
            pi.EnchantmentSeed.Should().Be.EqualTo(deserialized.EnchantmentSeed);
            pi.MateriaCombines.Should().Be.EqualTo(deserialized.MateriaCombines);
            pi.StackCount.Should().Be.EqualTo(deserialized.StackCount);

            pi.BaseRecord.Should().Be.EqualTo(deserialized.BaseRecord);
            pi.PrefixRecord.Should().Be.EqualTo(deserialized.PrefixRecord);
            pi.SuffixRecord.Should().Be.EqualTo(deserialized.SuffixRecord);
            pi.ModifierRecord.Should().Be.EqualTo(deserialized.ModifierRecord);
            pi.MateriaRecord.Should().Be.EqualTo(deserialized.MateriaRecord);
            pi.RelicCompletionBonusRecord.Should().Be.EqualTo(deserialized.RelicCompletionBonusRecord);
            pi.EnchantmentRecord.Should().Be.EqualTo(deserialized.EnchantmentRecord);
            pi.TransmuteRecord.Should().Be.EqualTo(deserialized.TransmuteRecord);


        }
        [TestMethod]
        public void TestCanSerializeEmptyStrings() {
            Random r = new Random();
            PlayerItem pi = new PlayerItem {
                Seed = (uint)r.Next(),
                RelicSeed = (uint)r.Next(),
                UNKNOWN = (uint)r.Next(),
                EnchantmentSeed = (uint)r.Next(),
                MateriaCombines = (uint)r.Next(),
                StackCount = (uint)r.Next(1, 100),
                BaseRecord = string.Empty,
                PrefixRecord = string.Empty,
                SuffixRecord = string.Empty,
                ModifierRecord = string.Empty,
                MateriaRecord = string.Empty,
                RelicCompletionBonusRecord = string.Empty,
                EnchantmentRecord = string.Empty,
                TransmuteRecord = string.Empty,
            };

            List<byte> serialized = ItemInjectCallbackProcessor.Serialize(pi);
            PlayerItem deserialized = ItemInjectCallbackProcessor.Deserialize(serialized.ToArray());

            pi.Seed.Should().Be.EqualTo(deserialized.Seed);
            pi.RelicSeed.Should().Be.EqualTo(deserialized.RelicSeed);
            pi.UNKNOWN.Should().Be.EqualTo(deserialized.UNKNOWN);
            pi.EnchantmentSeed.Should().Be.EqualTo(deserialized.EnchantmentSeed);
            pi.MateriaCombines.Should().Be.EqualTo(deserialized.MateriaCombines);
            pi.StackCount.Should().Be.EqualTo(deserialized.StackCount);

            pi.BaseRecord.Should().Be.EqualTo(deserialized.BaseRecord);
            pi.PrefixRecord.Should().Be.EqualTo(deserialized.PrefixRecord);
            pi.SuffixRecord.Should().Be.EqualTo(deserialized.SuffixRecord);
            pi.ModifierRecord.Should().Be.EqualTo(deserialized.ModifierRecord);
            pi.MateriaRecord.Should().Be.EqualTo(deserialized.MateriaRecord);
            pi.RelicCompletionBonusRecord.Should().Be.EqualTo(deserialized.RelicCompletionBonusRecord);
            pi.EnchantmentRecord.Should().Be.EqualTo(deserialized.EnchantmentRecord);
            pi.TransmuteRecord.Should().Be.EqualTo(deserialized.TransmuteRecord);


        }


        public static string RandomString(Random random, int length) {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
