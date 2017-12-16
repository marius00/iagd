using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpTestsEx;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IAGrim.Database;
using IAGrim.Database.DAO;
using NHibernate;
using IAGrim.Services.Dto;
using IAGrim.Database.Interfaces;
using IAGrim.Database.Dto;
using IAGrim.Parsers;

namespace Tests_IAGrim.Dao {
    [TestClass]
    public class RecipeItemDaoTests {
        private static readonly IAGrim.Tests.SessionFactory factory = new IAGrim.Tests.SessionFactory();
        private IRecipeItemDao dao = new RecipeItemDaoImpl(factory);



        [TestMethod]
        public void TestUpdateRecipes() {
            var parser = new RecipeParser(this.dao);

            var dbItemDao = new DatabaseItemDaoImpl(factory);
            dbItemDao.Save(new DatabaseItem {
                Record = "records/items/crafting/blueprints/armor/craft_armor_decoratedpauldrons.dbr",
                Name = "Whatever",
                Id = 123,
                Stats = new List<DatabaseItemStat> {
                    new DatabaseItemStat {
                        TextValue = "Whatever",
                        Stat = "artifactName"
                    }
                }
            });

            dbItemDao.Save(new DatabaseItem {
                Record = "records/items/crafting/blueprints/armor/craft_armorc02_unholyvisageofthecovenant.dbr",
                Name = "Whatever 2",
                Id = 1234,
                Stats = new List<DatabaseItemStat> {
                    new DatabaseItemStat {
                        TextValue = "Whatever 2",
                        Stat = "artifactName"
                    }
                }
            });

            using (ISession session = factory.OpenSession()) {
                var tmp = dbItemDao.ListAll();
                var sss = tmp[0].Stats;
                dbItemDao.ListAll().Count.Should().Be.EqualTo(2);
                // insert stat with name artifactName
            }

            parser.UpdateFormulas(Path.Combine("Dao", "TestData", "formulas.gst"), false);
            dao.ListAll().Count.Should().Be.GreaterThan(0);
            var numSoftcore = dao.ListAll().Count;

            parser.UpdateFormulas(Path.Combine("Dao", "TestData", "formulas.gsh"), true);
            dao.ListAll().Count.Should().Be.GreaterThan(numSoftcore);
        }

    }
}
