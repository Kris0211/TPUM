using ClientApi;
using ClientData;
using System.Runtime.InteropServices;

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
                new ItemDTO(Guid.NewGuid(), "Name 1", "Description 1", "Generator", 2000.0f, false),
                new ItemDTO(Guid.NewGuid(), "Name 2", "Description 2", "Spaceship", 10000.0f, false)
            ]);
        }

        [TestMethod]
        public void UpdateAllTest()
        {
            ItemDTO[] itemDTOs = [
                new ItemDTO(Guid.NewGuid(), "Name 1", "Description 1", "Generator", 2000.0f, false),
                new ItemDTO(Guid.NewGuid(), "Name 2", "Description 2", "Spaceship", 10000.0f, false)
                ];

            connectionService.FakeUpdateAll(itemDTOs);
            List<IItem> items = data.GetDepot().GetItems();
            Assert.IsTrue(items.Count > 0);

            for (int i = 0; i < itemDTOs.Length; i++)
            {
                Assert.IsTrue(itemDTOs[i].Id == items[i].Id &&
                    itemDTOs[i].Name == items[i].Name &&
                    itemDTOs[i].Description == items[i].Description &&
                    itemDTOs[i].Price == items[i].Price &&
                    itemDTOs[i].IsSold == items[i].IsSold);
            }
        }

        [TestMethod]
        public void ReputationChangeTest()
        {
            PrepareData();

            List<IItem> itemsBefore = data.GetDepot().GetItems();
            float newReputation = 5.0f;
            connectionService.FakeReputationChanged(itemsBefore, 5.0f);
            List<IItem> itemsAfter = data.GetDepot().GetItems();

            for (int i = 0; i < itemsBefore.Count; i++)
            {
                Assert.AreEqual(itemsBefore[i].Price * newReputation, itemsAfter[i].Price);
            }
        }

        [TestMethod]
        public async Task SellItemTest()
        {
            Guid sellGuid = Guid.NewGuid();
            connectionService.FakeUpdateAll([
                new ItemDTO(sellGuid, "Name 1", "Description 1", "Generator", 2000.0f, false),
                new ItemDTO(Guid.NewGuid(), "Name 2", "Description 2", "Spaceship", 10000.0f, false)
                ]);

            await data.GetDepot().SellItem(sellGuid);

            Assert.AreEqual(sellGuid, connectionService.lastSoldGuid);
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

            List<IItem> items = data.GetDepot().GetItems();
            IItem testItem = items[0];

            Assert.AreEqual(testItem, data.GetDepot().GetItemByID(testItem.Id));
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
    }
}
