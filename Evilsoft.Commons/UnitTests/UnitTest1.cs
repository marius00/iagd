using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EvilsoftCommons.SQL;


namespace UnitTests {
    [TestClass]
    public class UnitTest1 {
        [TestMethod]
        public void TestPostgresImportWorks() {

            String tmp = Environment.GetEnvironmentVariable("TEMP");
            new PostgresIntegrator(tmp, (uint) new Random().Next(10000, 65500));
        }
    }
}
