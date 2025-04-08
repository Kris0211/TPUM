using Logic;

namespace LogicTest
{
    [TestClass]
    public class LogicTest
    {
        private AbstractLogicApi logicApi = AbstractLogicApi.Create(new FakeDataApi());

        [TestMethod]
        public void SellItemTest()
        {
            IStoreItem sellItem = logicApi.GetStore().GetAvailableItems()[0];

            logicApi.GetStore().SellItem(sellItem.Id);

            Assert.IsTrue(logicApi.GetStore().GetAvailableItems().Count == 1);
        }

        [TestMethod]
        public void GetItemsTest()
        {
            IStore store = logicApi.GetStore();
            Assert.IsTrue(store.GetAvailableItems().Count == 2);
            Assert.IsTrue(store.GetItems().Count == 3);
            Assert.IsTrue(store.GetItemsByType(LogicItemType.Ammo).Count == 2);
        }
    }
}
