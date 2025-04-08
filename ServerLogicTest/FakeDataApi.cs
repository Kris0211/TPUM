using ServerData;

namespace ServerLogicTest
{
    public class FakeDataApi : AbstractDataApi
    {
        private readonly FakeDepot fakeDepot = new FakeDepot();

        public override IDepot GetDepot()
        {
            return fakeDepot;
        }
    }

    public class FakeItem : IItem
    {
        public FakeItem(string name, string description, ItemType type, float price, bool isSold)
        {
            Id = Guid.NewGuid();
            Name = name;
            Description = description;
            Type = type;
            Price = price;
            IsSold = isSold;
        }

        public Guid Id { get; }
        public string Name { get; }
        public string Description { get; }
        public ItemType Type { get; }
        public float Price { get; set; }
        public bool IsSold { get; set; }
        public object Clone()
        {
            throw new NotImplementedException();
        }
    }

    public class FakeDepot : IDepot
    {
        private readonly List<IItem> items;

        public FakeDepot()
        {
            items = new List<IItem>
            {
                new FakeItem("Test", "TestDesc", ItemType.Ammo, 10f, false),
                new FakeItem("Test2", "TestDesc2", ItemType.Ammo, 11f, false),
                new FakeItem("Sold", "SoldDesc2", ItemType.Generator, 11f, true)
            };
        }

        public void SellItem(Guid itemId)
        {
            IItem? item = items.Find((item) => item.Id == itemId);
            if (item != null)
            {
                item.IsSold = true;
            }
        }

        public List<IItem> GetItems()
        {
            return items;
        }

        public float GetCurrentReputation()
        {
            return 1f;
        }

        public event EventHandler<ReputationChangedEventArgs>? ReputationChanged;

        public IItem CreateItem(string name, string description, ItemType type, float price)
        {
            throw new NotImplementedException();
        }

        public void AddItem(IItem itemToAdd)
        {
            throw new NotImplementedException();
        }

        public void RemoveItem(Guid itemIdToRemove)
        {
            throw new NotImplementedException();
        }

        public IItem GetItemByID(Guid guid)
        {
            throw new NotImplementedException();
        }

        public List<IItem> GetAvailableItems()
        {
            throw new NotImplementedException();
        }

        public IItem GetItemById(Guid guid)
        {
            throw new NotImplementedException();
        }

        public List<IItem> GetItemsByType(ItemType type)
        {
            throw new NotImplementedException();
        }
    }
}
