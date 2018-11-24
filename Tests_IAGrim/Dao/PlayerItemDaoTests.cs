using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpTestsEx;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IAGrim.Database;
using NHibernate;
using IAGrim.Services.Dto;
using IAGrim.Database.Interfaces;
using IAGrim.Database.Dto;

namespace Tests_IAGrim.Dao {
    [TestClass]
    public class PlayerItemDaoTests {
        private static readonly IAGrim.Tests.SessionFactory factory = new IAGrim.Tests.SessionFactory();
        private IPlayerItemDao dao = new IAGrim.Database.PlayerItemDaoImpl(factory, new DatabaseItemStatDaoImpl(factory));


        [ClassInitialize]
        public static void Setup(TestContext context) {
            using (var session = factory.OpenSession()) {

                using (ITransaction transaction = session.BeginTransaction()) {
                    session.Save(new PlayerItem {
                        BaseRecord = "br",
                        Mod = "some mod",
                        LevelRequirement = 0
                    });
                    session.Save(new PlayerItem {
                        BaseRecord = "br",
                        Mod = string.Empty,
                        Name = "some specific name here",
                        LevelRequirement = 0
                    });
                    session.Save(new PlayerItem {
                        BaseRecord = "br",
                        Mod = string.Empty,
                        IsHardcore = true,
                        Name = "hc item",
                        LevelRequirement = 0
                    });
                    session.Save(new PlayerItem {
                        BaseRecord = "socketed-item",
                        MateriaRecord = "the socket",
                        Mod = string.Empty,
                        IsHardcore = false,
                        Name = "socket item",
                        LevelRequirement = 0
                    });
                    session.Save(new PlayerItem {
                        BaseRecord = "for-testing-rarity",
                        Mod = string.Empty,
                        Rarity = "Green",
                        LevelRequirement = 0
                    });
                    session.Save(new PlayerItem {
                        BaseRecord = "for-testing-max-level",
                        Mod = string.Empty,
                        MinimumLevel = 200,
                        LevelRequirement = 200
                    });
                    session.Save(new PlayerItem {
                        BaseRecord = "for-testing-max-level",
                        Mod = string.Empty,
                        MinimumLevel = -200,
                        LevelRequirement = -200
                    });

                    const string dupeName = "The item with dupes";
                    session.Save(new PlayerItem {
                        BaseRecord = "The item with dupes",
                        Mod = string.Empty,
                        LevelRequirement = 0,
                        Name = dupeName,
                        StackCount = 1
                    });
                    session.Save(new PlayerItem {
                        BaseRecord = "The item with dupes",
                        Mod = string.Empty,
                        LevelRequirement = 0,
                        Name = dupeName,
                        StackCount = 1
                    });

                    
                    var statSearchItem = new DatabaseItem {
                        Record = "for-testing-stat-search"
                    };
                    session.Save(statSearchItem);

                    session.Save(new DatabaseItemStat {
                        Parent = statSearchItem,
                        Stat = "some-imaginary-stat",
                        Value = 200,
                    });

                    session.Save(new DatabaseItemStat {
                        Parent = statSearchItem,
                        Stat = "Class",
                        TextValue = "some-slot"
                    });

                    session.Save(new DatabaseItemStat {
                        Parent = statSearchItem,
                        Stat = "retaliation stat",
                        Value = 1
                    });

                    var statItem = new PlayerItem {
                        BaseRecord = "for-testing-stat-search",
                        Mod = string.Empty,
                        LevelRequirement = 0,
                        Count = 1,
                        IsHardcore = false
                    };
                    session.Save(statItem);


                    session.Save(new PlayerItemRecord {
                        PlayerItemId = statItem.Id,
                        Record = statItem.BaseRecord
                    });

                    transaction.Commit();
                }
            }
        }

        

        PlayerItem GetBasicItem(long n) {
            return new PlayerItem {
                BaseRecord = "baseRecord" + n,
                Count = 1,
                IsHardcore = false,
                MinimumLevel = 15,
                Name = "Some item" + n,
                Tags = new HashSet<DBSTatRow>()
            };
        }

        [TestMethod]
        public void TestDeleteItem() {
            int n = new Random().Next();
            var item = GetBasicItem(n);
            dao.Save(item);
            item.Id.Should().Be.GreaterThan(0);
            Executing.This(() => { dao.Remove(item); }).Should().NotThrow();
        }
        

        [TestMethod]
        public void CanSaveWithHighSeedValue() {
            var item = GetBasicItem(uint.MaxValue);
            Executing.This(() => { dao.Save(item); }).Should().NotThrow();
        }


        [TestMethod]
        public void CanFindItemWithSpecificName() {
            ItemSearchRequest query = new ItemSearchRequest {
                IsHardcore = false,
                Mod = string.Empty,
                Wildcard = "specific name",
                Classes = new List<string>(),
                Filters = new List<string[]>()
            };

            List<PlayerItem> result = DoSearch(query);
            result.Count.Should().Be.EqualTo(1);
        }




        [TestMethod]
        public void CanFindItemBasedOnSlot() {
            ItemSearchRequest query = new ItemSearchRequest {
                IsHardcore = false,
                Mod = string.Empty,
                Filters = new List<string[]>(),
                Classes = new List<string>(),
                Slot = new string[] { "some-slot" }
            };


            List<PlayerItem> result = DoSearch(query);
            result.Count.Should().Be.EqualTo(1);
        }
        [TestMethod]
        public void CanFindItemBasedOnStat() {
            ItemSearchRequest query = new ItemSearchRequest {
                IsHardcore = false,
                Mod = string.Empty,
                Filters = new List<string[]>(),
                Classes = new List<string>()
            };

            query.Filters.Add(new string[] { "some-imaginary-stat" });

            List<PlayerItem> result = DoSearch(query);
            result.Count.Should().Be.EqualTo(1);
        }

        [TestMethod]
        public void CanFindItemBasedOnSocketed() {
            ItemSearchRequest query = new ItemSearchRequest {
                IsHardcore = false,
                Mod = string.Empty,
                Filters = new List<string[]>(),
                Classes = new List<string>(),
                SocketedOnly = true
            };

            List<PlayerItem> result = DoSearch(query);
            result.Count.Should().Be.EqualTo(1);
        }

        [TestMethod]
        public void CanFindRetliationItems() {
            ItemSearchRequest query = new ItemSearchRequest {
                IsHardcore = false,
                Mod = string.Empty,
                Filters = new List<string[]>(),
                Classes = new List<string>(),
                IsRetaliation = true
            };

            List<PlayerItem> result = DoSearch(query);
            result.Count.Should().Be.EqualTo(1);
        }


        [TestMethod]
        public void CanFindItemWithMinLevel() {
            ItemSearchRequest query = new ItemSearchRequest {
                IsHardcore = false,
                Mod = string.Empty,
                Filters = new List<string[]>(),
                Classes = new List<string>(),
                MinimumLevel = 200
            };

            List<PlayerItem> result = DoSearch(query);
            result.Count.Should().Be.EqualTo(1);
        }

        [TestMethod]
        public void CanFindItemWithMaxLevel() {
            /* This cannot be tested */
        }


        [TestMethod]
        public void CanFindItemWithSpecificRarity() {
            ItemSearchRequest query = new ItemSearchRequest {
                IsHardcore = false,
                Mod = string.Empty,
                Rarity = "Green",
                Classes = new List<string>(),
                Filters = new List<string[]>()
            };

            var result = DoSearch(query);
            result.Count.Should().Be.EqualTo(1);
        }


        [TestMethod]
        public void CanFindItemWhichIsHardcore() {
            ItemSearchRequest query = new ItemSearchRequest {
                IsHardcore = true,
                Mod = string.Empty,
                Classes = new List<string>(),
                Filters = new List<string[]>()
            };

            var result = DoSearch(query);
            result.Count.Should().Be.EqualTo(1);
        }

        [TestMethod]
        public void CanFindItemWithSpecificMod() {
            ItemSearchRequest query = new ItemSearchRequest {
                IsHardcore = false,
                Mod = "some mod",
                Classes = new List<string>(),
                Filters = new List<string[]>()
            };

            var result = DoSearch(query);
            result.Count.Should().Be.EqualTo(1);
        }

        private List<PlayerItem> DoSearch(ItemSearchRequest query) {
            return dao.SearchForItems(query);
        }
    }
}
