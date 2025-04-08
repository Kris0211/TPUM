using ServerLogic;

namespace ServerLogicTest
{
    [TestClass]
    public class LogicTest
    {
        private LogicAbstractApi logicApi = LogicAbstractApi.Create(new FakeDataApi());

        [TestMethod]
        public void SellItem()
        {
            IStore store = logicApi.GetStore();

            List<IStoreItem> allItems = store.GetItems();
            List<IStoreItem> availableItems = new List<IStoreItem>();
            foreach (var item in allItems)
            {
                if (!item.IsSold)
                {
                    availableItems.Add(item);
                }
            }

            Assert.IsTrue(availableItems.Count == 2);

            IStoreItem itemToSell = availableItems[0];
            store.SellItem(itemToSell.Id);

            allItems = store.GetItems();
            availableItems.Clear();
            foreach (var item in allItems)
            {
                if (!item.IsSold)
                {
                    availableItems.Add(item);
                }
            }

            Assert.IsTrue(availableItems.Count == 1);
        }

        [TestMethod]
        public void GetItems()
        {
            Assert.IsTrue(logicApi.GetStore().GetItems().Count == 3);
        }
    }
}
