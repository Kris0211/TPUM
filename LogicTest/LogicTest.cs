using Data;
using Logic;

namespace LogicTest
{
    [TestClass]
    public class LogicTest
    {
        private AbstractLogicApi logicApi = AbstractLogicApi.Create(new DataApiMock());

        [TestMethod]
        public void SellItem()
        {
            IStoreItem sellItem = logicApi.GetStore().GetAvailableItems()[0];

            logicApi.GetStore().SellItem(sellItem.Id);

            Assert.IsTrue(logicApi.GetStore().GetAvailableItems().Count == 1);
        }

        [TestMethod]
        public void GetItems()
        {
            Assert.IsTrue(logicApi.GetStore().GetAvailableItems().Count == 2);
            Assert.IsTrue(logicApi.GetStore().GetItems().Count == 3);
            Assert.IsTrue(logicApi.GetStore().GetItemsByType(LogicItemType.Ammo).Count == 2);
        }
    }
}
