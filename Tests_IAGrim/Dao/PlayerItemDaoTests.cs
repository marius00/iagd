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
        public void TestRemoveHalfStack() {
            const long oid = 456499611;
            long id;
            {
                var item = new PlayerItem {
                    BaseRecord = "for-testing-item-halfstack-removal",
                    Mod = string.Empty,
                    MinimumLevel = 200,
                    LevelRequirement = 200,
                    StackCount = 30,
                    OnlineId = oid
                };
                dao.Save(item);


                item.Id.Should().Be.GreaterThan(0);
                id = item.Id;
            }


            // Reduce to 15
            {
                var reloaded = dao.GetByOnlineId(oid);
                reloaded.Id.Should().Be.EqualTo(id);
                reloaded.StackCount = 15;
                dao.Update(new List<PlayerItem> { reloaded }, true);
            }


            // Ensure the online ID has been cleared
            {
                var reloaded = dao.GetByOnlineId(oid);
                reloaded.Should().Be.Null();
            }

            // Ensure the item still exists
            {
                var reloaded = dao.GetById(id);
                reloaded.Should().Not.Be.Null();
                reloaded.Count.Should().Be.EqualTo(15);
            }

            // Ensure the old OID is marked for removal
            var deletionEntry = dao.GetItemsMarkedForOnlineDeletion().FirstOrDefault(m => m.OID == oid);
            deletionEntry.Should().Not.Be.Null();
        }

        [TestMethod]
        public void TestRemoveTransferredItem() {
            long id;
            {
                var item = new PlayerItem {
                    BaseRecord = "for-testing-item-transfer",
                    Mod = string.Empty,
                    MinimumLevel = 200,
                    LevelRequirement = 200,
                    StackCount = 3,
                    OnlineId = 123321
                };
                dao.Save(item);


                //session.Save(item);
                item.Id.Should().Be.GreaterThan(0);
                id = item.Id;
            }

            // Reduce by one
            {
                var reloaded = dao.GetByOnlineId(123321);
                reloaded.Id.Should().Be.EqualTo(id);
                reloaded.StackCount = 2;
                dao.Update(reloaded);
            }
            {
                var reloaded = dao.GetByOnlineId(123321);
                reloaded.Id.Should().Be.EqualTo(id);
                reloaded.Count.Should().Be.EqualTo(2);
            }

            // Reduce to zero
            {
                var reloaded = dao.GetByOnlineId(123321);
                reloaded.Id.Should().Be.EqualTo(id);
                reloaded.StackCount = 0;
                dao.Update(reloaded);
            }
            {
                var reloaded = dao.GetByOnlineId(123321);
                reloaded.Should().Be.EqualTo(null);
            }
            {
                var reloaded = dao.GetById(id);
                reloaded.Should().Be.EqualTo(null);
            }

        }

        [TestMethod]
        public void CanSaveWithHighSeedValue() {
            var item = GetBasicItem(uint.MaxValue);
            Executing.This(() => { dao.Save(item); }).Should().NotThrow();
        }


        [TestMethod]
        public void CanFindItemWithSpecificName() {
            Search query = new Search {
                IsHardcore = false,
                Mod = string.Empty,
                Wildcard = "specific name",
                Classes = new List<string>(),
                filters = new List<string[]>()
            };

            List<PlayerItem> result = DoSearch(query);
            result.Count.Should().Be.EqualTo(1);
        }




        [TestMethod]
        public void CanFindItemBasedOnSlot() {
            Search query = new Search {
                IsHardcore = false,
                Mod = string.Empty,
                filters = new List<string[]>(),
                Classes = new List<string>(),
                Slot = new string[] { "some-slot" }
            };


            List<PlayerItem> result = DoSearch(query);
            result.Count.Should().Be.EqualTo(1);
        }
        [TestMethod]
        public void CanFindItemBasedOnStat() {
            Search query = new Search {
                IsHardcore = false,
                Mod = string.Empty,
                filters = new List<string[]>(),
                Classes = new List<string>()
            };

            query.filters.Add(new string[] { "some-imaginary-stat" });

            List<PlayerItem> result = DoSearch(query);
            result.Count.Should().Be.EqualTo(1);
        }

        [TestMethod]
        public void CanFindItemBasedOnSocketed() {
            Search query = new Search {
                IsHardcore = false,
                Mod = string.Empty,
                filters = new List<string[]>(),
                Classes = new List<string>(),
                SocketedOnly = true
            };

            List<PlayerItem> result = DoSearch(query);
            result.Count.Should().Be.EqualTo(1);
        }

        [TestMethod]
        public void CanFindRetliationItems() {
            Search query = new Search {
                IsHardcore = false,
                Mod = string.Empty,
                filters = new List<string[]>(),
                Classes = new List<string>(),
                IsRetaliation = true
            };

            List<PlayerItem> result = DoSearch(query);
            result.Count.Should().Be.EqualTo(1);
        }


        [TestMethod]
        public void CanFindItemWithMinLevel() {
            Search query = new Search {
                IsHardcore = false,
                Mod = string.Empty,
                filters = new List<string[]>(),
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
            Search query = new Search {
                IsHardcore = false,
                Mod = string.Empty,
                Rarity = "Green",
                Classes = new List<string>(),
                filters = new List<string[]>()
            };

            var result = DoSearch(query);
            result.Count.Should().Be.EqualTo(1);
        }


        [TestMethod]
        public void CanFindItemWhichIsHardcore() {
            Search query = new Search {
                IsHardcore = true,
                Mod = string.Empty,
                Classes = new List<string>(),
                filters = new List<string[]>()
            };

            var result = DoSearch(query);
            result.Count.Should().Be.EqualTo(1);
        }

        [TestMethod]
        public void CanFindItemWithSpecificMod() {
            Search query = new Search {
                IsHardcore = false,
                Mod = "some mod",
                Classes = new List<string>(),
                filters = new List<string[]>()
            };

            var result = DoSearch(query);
            result.Count.Should().Be.EqualTo(1);
        }

        private List<PlayerItem> DoSearch(Search query) {
            return dao.SearchForItems(query);
        }
    }
}
