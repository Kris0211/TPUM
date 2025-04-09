using ClientData;

namespace LogicTest
{
    public class FakeDataApi : AbstractDataApi
    {
        private readonly FakeDepot fakeDepot = new FakeDepot();
        private readonly FakeConnectionService fakeConnectionService = new FakeConnectionService();

        public override IConnectionService GetConnectionService()
        {
            return fakeConnectionService;
        }

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
        private readonly List<IItem> allItems;
        private readonly List<IItem> availableItems;

        public FakeDepot()
        {
            availableItems = new List<IItem>
            {
                new FakeItem("TestItem1", "TestDescription1", ItemType.Ammo, 1f, false),
                new FakeItem("TestItem2", "TestDescription2", ItemType.Ammo, 1.5f, false)
            };

            allItems = new List<IItem>
            {
                new FakeItem("SoldTestItem", "SoldTestDescription", ItemType.Spaceship, 200f, true)
            };

            allItems.AddRange(availableItems);
        }

        public List<IItem> GetItems()
        {
            return allItems;
        }

        public List<IItem> GetAvailableItems()
        {
            return availableItems;
        }

        public float GetCurrentReputation()
        {
            return 1f;
        }

        public event EventHandler<ReputationChangedEventArgs>? InflationChanged;
        public event Action? ItemsUpdated;
        public event Action<bool>? TransactionFinished;

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

        public List<IItem> GetItemsByType(ItemType type)
        {
            if (type == ItemType.Ammo)
            {
                return availableItems;
            }

            return new List<IItem>();
        }

        public void RequestUpdate()
        {
            throw new NotImplementedException();
        }

        public async Task SellItem(Guid itemId)
        {
            IItem? item = availableItems.Find((item) => item.Id == itemId);
            if (item != null)
            {
                availableItems.Remove(item);
            }

            // Return dummy task
            await new Task(() => {});
        }

        private IObserver<ReputationChangedEventArgs> observer;

        public IDisposable Subscribe(IObserver<ReputationChangedEventArgs> observer)
        {
            this.observer = observer;
            return new FakeDisposable();
        }

        private class FakeDisposable : IDisposable
        {
            public FakeDisposable() { }

            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }
    }

    public class FakeConnectionService : IConnectionService
    {
        public event Action<string>? Logger;
        public event Action? OnConnectionStateChanged;
        public event Action<string>? OnMessage;
        public event Action? OnError;
        public event Action? OnDisconnect;

        public Task Connect(Uri peerUri)
        {
            throw new NotImplementedException();
        }

        public Task Disconnect()
        {
            throw new NotImplementedException();
        }

        public bool IsConnected()
        {
            throw new NotImplementedException();
        }

        public Task SendAsync(string message)
        {
            throw new NotImplementedException();
        }
    }
}
