using IAGrim.StashFile;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.Parsers.GameDataParsing.Model;
using SharpTestsEx;

namespace Tests_IAGrim.ParserTests {

    [TestClass]
    public class ProgressBarTests {

        [TestMethod]
        public void One_out_of_two_should_be_50_percent() {
            var tracker = new ProgressTracker {
                MaxValue = 2
            };
            tracker.Increment();
            tracker.Progress.Should().Be.EqualTo(50);
        }

        [TestMethod]
        public void Two_out_of_two_should_be_100_percent() {
            var tracker = new ProgressTracker {
                MaxValue = 2
            };
            tracker.Increment();
            tracker.Increment();
            tracker.Progress.Should().Be.EqualTo(100);
        }
    }
}
