using ServerData;
using System.Collections.Generic;

namespace ServerDataTest
{
    [TestClass]
    public class ServerDataTest
    {
        private AbstractDataApi PrepareData()
        {
            AbstractDataApi data = AbstractDataApi.Create();
            return data;
        }

        [TestMethod]
        public void CreateItemTest()
        {
            AbstractDataApi data = PrepareData();

            string name = "Nuke";
            string desc = "Goes Boom";
            ItemType type = ItemType.Ammo;
            float price = 127.50f;

            IDepot depot = data.GetDepot();
            IItem item = depot.CreateItem(name, desc, type, price);

            Assert.AreEqual(name, item.Name);
            Assert.AreEqual(desc, item.Description);
            Assert.AreEqual(type, item.Type);
            Assert.AreEqual(price, item.Price);
            Assert.IsFalse(item.IsSold);
        }

        [TestMethod]
        public void GetItemsTest()
        {
            AbstractDataApi data = PrepareData();
            IDepot depot = data.GetDepot();
            Assert.AreNotEqual(0, depot.GetItems().Count);
        }

        [TestMethod]
        public void GetItemByIdTest()
        {
            AbstractDataApi data = PrepareData();
            IDepot depot = data.GetDepot();
            List<IItem> items = depot.GetItems();
            IItem testItem = items[0];

            Assert.AreEqual(testItem, depot.GetItemById(testItem.Id));
        }

        [TestMethod]
        public void ItemsSoldTest()
        {
            AbstractDataApi data = PrepareData();
            IDepot depot = data.GetDepot();
            List<IItem> items = depot.GetItems();

            depot.SellItem(items[0].Id);
            Assert.AreEqual(items.Count - 1, depot.GetAvailableItems().Count);
        }

        [TestMethod]
        public void AddRemoveItemsTest()
        {
            AbstractDataApi data = PrepareData();
            IDepot depot = data.GetDepot();
            IItem item = depot.CreateItem("Item", "Desc", ItemType.Weapon, 10f);

            depot.AddItem(item);
            Assert.AreEqual(item, depot.GetItemById(item.Id));

            depot.RemoveItem(item.Id);
            Assert.ThrowsException<KeyNotFoundException>(() => depot.GetItemById(item.Id));
        }

        [TestMethod]
        public void GetItemsByTypeTest()
        {
            AbstractDataApi data = PrepareData();
            IDepot depot = data.GetDepot();
            List<IItem> foundItems = depot.GetItemsByType(ItemType.Ammo);

            foreach (IItem item in foundItems)
            {
                Assert.AreEqual(ItemType.Ammo, item.Type);
            }
        }

        [TestMethod]
        public void CloneTest()
        {
            AbstractDataApi data = PrepareData();
            IDepot depot = data.GetDepot();
            IItem item = depot.GetItems().First();
            IItem clone = (IItem)item.Clone();

            Assert.AreNotSame(item, clone);
            Assert.AreNotSame(item.Name, clone.Name);
            Assert.AreNotSame(item.Description, clone.Description);

            Assert.AreEqual(item.Id, clone.Id);
            Assert.AreEqual(item.Name, clone.Name);
            Assert.AreEqual(item.Description, clone.Description);
            Assert.AreEqual(item.IsSold, clone.IsSold);
            Assert.AreEqual(item.Type, clone.Type);
        }
    }
}
