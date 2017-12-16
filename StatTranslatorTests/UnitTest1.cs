using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatTranslator;
using SharpTestsEx;

namespace StatTranslatorTests {
    [TestClass]
    public class UnitTest1 {
        [TestMethod]
        public void TestTriggerTranslations() {
            string[] triggers = {
                "records/controllers/itemskills/cast_@allyonattack_15",
                "records/controllers/itemskills/cast_@allyonlowhealth_15",
                "records/controllers/itemskills/cast_@enemylocationonkill_15",
                "records/controllers/itemskills/cast_@enemyonanyhit_15",
                "records/controllers/itemskills/cast_@enemyonattack_15",
                "records/controllers/itemskills/cast_@enemyonattackcrit_15",
                "records/controllers/itemskills/cast_@enemyonblock_15",
                "records/controllers/itemskills/cast_@enemyonhitcritical_15",
                "records/controllers/itemskills/cast_@enemyonkill_15",
                "records/controllers/itemskills/cast_@enemyonmeleehit_15",
                "records/controllers/itemskills/cast_@enemyonprojectilehit_15",
                "records/controllers/itemskills/cast_@selfonanyhit_15",
                "records/controllers/itemskills/cast_@selfonattack_15",
                "records/controllers/itemskills/cast_@selfonattackcrit_15",
                "records/controllers/itemskills/cast_@selfonblock_15",
                "records/controllers/itemskills/cast_@selfonhitcritica_15",
                "records/controllers/itemskills/cast_@selfonkill_15",
                "records/controllers/itemskills/cast_@selfonlowhealth_15",
                "records/controllers/itemskills/cast_@selfonmeleehit_15",
                "records/controllers/itemskills/cast_@selfonprojectilehit_15",
                "records/controllers/itemskills/cast_@selfat15_abc_22"
            };
            foreach (var trigger in triggers) {
                StatManager sm = new StatManager(new EnglishLanguage());
                var result = sm.TranslateSkillAutoController(trigger);
                result.Should().Not.Be.Null();
                result.Text.Should().Not.Be.Null();
            }
        }
    }
}
