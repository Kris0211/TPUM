using ClientApi;
using ClientData;
using System.Runtime.CompilerServices;

namespace ClientDataTest
{
    [TestClass]
    public class ClientDataTest
    {
        static FakeConnectionService connectionService = new FakeConnectionService();
        AbstractDataApi data = AbstractDataApi.Create(connectionService);

        public void PrepareData()
        {
            connectionService.FakeUpdateAll([
                new ItemDTO { Id = Guid.NewGuid(), Name = "Name 1", Description = "Description 1", Type = "Generator", Price = 2000.0f, IsSold = false},
                new FakeItemDTO { Id = Guid.NewGuid(), Name = "Name 2", Description = "Description 2", Type = "Spaceship", Price = 10000.0f, IsSold = false}
            ]);
        }

        [TestMethod]
        public void GetItemsTest()
        {
            PrepareData();

            Assert.AreNotEqual(data.GetDepot().GetItems().Count, 0);
        }

        [TestMethod]
        public void GetItemByIdTest()
        {
            PrepareData();

            ManualResetEvent transactionFinishedEvent = new ManualResetEvent(false);

            data.GetDepot().TransactionFinished += (succeeded) =>
            {
                transactionFinishedEvent.Set();
            };

            bool eventTriggered = transactionFinishedEvent.WaitOne(TimeSpan.FromSeconds(5));
            Assert.IsTrue(eventTriggered, "Transaction timed out.");

            List<IItem> items = data.GetDepot().GetItems();
            IItem testItem = items[0];
            IItem testItemById = data.GetDepot().GetItemByID(testItem.Id);

            Assert.AreEqual(testItem, testItemById);
        }

        [TestMethod]
        public void GetItemsByTypeTest()
        {
            PrepareData();

            List<IItem> foundItems = data.GetDepot().GetItemsByType(ItemType.Generator);

            foreach (IItem item in foundItems)
            {
                Assert.AreEqual(item.Type, ItemType.Generator);
            }
        }

        [TestMethod]
        public async Task SellItemTest()
        {
            Guid sellGuid = Guid.NewGuid();
            connectionService.FakeUpdateAll([
                new FakeItemDTO { Id = sellGuid, Name = "Name 1", Description = "Description 1", Type = "Generator", Price = 2000.0f, IsSold = false },
                new FakeItemDTO { Id = Guid.NewGuid(), Name = "Name 2", Description = "Description 2", Type = "Spaceship", Price = 10000.0f, IsSold = false }
                ]);

            await data.GetDepot().SellItem(sellGuid);         

            Assert.AreEqual(sellGuid, connectionService.lastSoldGuid);
        }

        [TestMethod]
        public void UpdateAllTest()
        {
            PrepareData();

            FakeItemDTO[] itemDTOs = [
                new FakeItemDTO{ Id = Guid.NewGuid(), Name = "Name 3", Description = "Description 3", Type = "Generator", Price = 4000.0f, IsSold = false },
                new FakeItemDTO{ Id = Guid.NewGuid(), Name = "Name 4", Description = "Description 4", Type = "Spaceship", Price = 20000.0f, IsSold = false }
            ];

            connectionService.FakeUpdateAll(itemDTOs);

            List<IItem> items = data.GetDepot().GetItems();

            for (int i = 0; i < itemDTOs.Length; i++)
            {
                Assert.AreEqual(itemDTOs[i].Id, items[i].Id);
                Assert.AreEqual(itemDTOs[i].Name, items[i].Name);
                Assert.AreEqual(itemDTOs[i].Description, items[i].Description);
                Assert.AreEqual(itemDTOs[i].Price, items[i].Price);
                Assert.AreEqual(itemDTOs[i].IsSold, items[i].IsSold);
            }
        }

        [TestMethod]
        public void ReputationChangeTest()
        {
            PrepareData();

            List<IItem> itemsBefore = data.GetDepot().GetItems();

            ManualResetEvent transactionFinishedEvent = new ManualResetEvent(false);

            data.GetDepot().TransactionFinished += (succeeded) =>
            {
                transactionFinishedEvent.Set();
            };

            float newReputation = 2.0f;
            connectionService.FakeReputationChanged(itemsBefore, newReputation);

            bool eventTriggered = transactionFinishedEvent.WaitOne(TimeSpan.FromSeconds(5));
            Assert.IsTrue(eventTriggered, "Transaction timed out.");

            List<IItem> itemsAfter = data.GetDepot().GetItems();
            for (int i = 0; i < itemsBefore.Count; i++)
            {
                float expected = itemsBefore[i].Price * newReputation;
                Assert.AreEqual(expected, itemsAfter[i].Price);
            }
        }
    }
}
