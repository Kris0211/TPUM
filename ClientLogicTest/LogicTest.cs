using ClientLogic;
using LogicTest;

namespace ClientLogicTest;

[TestClass]
public class ClientLogicTest
{
    private AbstractLogicApi logicApi = AbstractLogicApi.Create(new FakeDataApi());

    [TestMethod]
    public void SellItem()
    {
        IStore store = logicApi.GetStore();
        IStoreItem itemToSell = store.GetAvailableItems()[0];
        store.SellItem(itemToSell.Id);

        Assert.IsTrue(store.GetAvailableItems().Count == 1);
    }

    [TestMethod]
    public void GetItems()
    {
        IStore store = logicApi.GetStore();

        Assert.IsTrue(store.GetAvailableItems().Count == 2);
        Assert.IsTrue(store.GetItems().Count == 3);
        Assert.IsTrue(store.GetItemsByType(LogicItemType.Ammo).Count == 2);
    }
}
