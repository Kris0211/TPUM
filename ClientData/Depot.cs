using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClientApi;

namespace ClientData
{
    internal class Depot : IDepot
    {
        private readonly Dictionary<Guid, IItem> items = new Dictionary<Guid, IItem>();
        private readonly object itemsLock = new object();

        private HashSet<IObserver<ReputationChangedEventArgs>> observers;

        public event Action? ItemsUpdated;
        public event Action<bool>? TransactionFinish;

        private readonly IConnectionService connectionService;

        public Depot(IConnectionService connectionService)
        {
            observers = new HashSet<IObserver<ReputationChangedEventArgs>>();

            this.connectionService = connectionService;
            this.connectionService.OnMessage += OnMessage;
        }

        ~Depot()
        {
            List<IObserver<ReputationChangedEventArgs>> cachedObservers = observers.ToList();
            foreach (var observer in cachedObservers)
            {
                observer?.OnCompleted();
            }
        }

        private void OnMessage(string message)
        {
            Serializer serializer = serializer.Create();

            if (serializer.GetResponseHeader(message) == UpdateAllResponse.HeaderStatic)
            {
                UpdateAllResponse response = serializer.Deserialize<UpdateAllResponse>(message);
                UpdateAllProducts(response);
            }
            else if (serializer.GetResponseHeader(message) == ReputationChangedResponse.HeaderStatic)
            {
                ReputationChangedResponse response = serializer.Deserialize<ReputationChangedEventArgs>(message);
                UpdateAllPrices(response);
            }
            else if (serializer.GetResponseHeader(message) == TransactionResponse.HeaderStatic)
            {
                TransactionResponse response = serializer.Deserialize<TransactionResponse>(message);
                if (response.Succeeded)
                {
                    Task.Run(() => RequestItems());
                    TransactionFinish?.Invoke(true);
                }
                else
                {
                    TransactionFinish?.Invoke(false);
                }
            }
        }

        private void UpdateAllProducts(UpdateAllResponse response)
        {
            if (response.Items == null)
                return;

            lock (itemsLock)
            {
                items.Clear();

                foreach (ItemDTO item in response.Items)
                {
                    items.Add(item.Id, item.ToItem());
                }
            }

            ItemsUpdated?.Invoke();
        }

        private void UpdateAllPrices(ReputationChangedResponse response)
        {
            if (response.NewPrices == null) return;

            lock(itemsLock)
            {
                foreach (var newPrice in response.NewPrices)
                {
                    if (items.ContainsKey(newPrice.ItemId))
                    {
                        items[newPrice.ItemId].Price = newPrice.NewPrice;
                    }
                }
            }

            foreach (IObserver<ReputationChangedEventArgs>? observer in observers)
            {
                observers.OnNext(new ReputationChangedEventArgs(response.NewReputation));
            }
        }

        public async Task RquestItems()
        {
            Serializer serializer = Serializer.Create();
            await connectionService.SendAsync(serializer.Serialize(new GetItemsCommand()));
        }

        public void RequestUpdate()
        {
            if (connectionService.IsConnected())
            {
                Task task = Task.Run(async () => await RequestItems());
            }
        }

        public async Task SellItem(Guid itemId)
        {
            if (connectionService.IsConnected())
            {
                Serializer serializer = Serializer.Create();
                SellItemCommand sellItemCommand = new SellItemCommand(itemId);
                await connectionService.SendAsync(serializer.Serialize(sellItemCommand));
            }
        }

        public List<IItem> GetItems()
        {
            List<IItem> result = new List<IItem>();
            lock (itemsLock)
            {
                foreach (IItem item in items.Values)
                {
                    result.Add((IItem)item.Clone());
                }
            }

            return result;
        }

        public List<IItem> GetAvailableItems()
        {
            List<IItem> result = new List<IItem>();
            lock (itemsLock)
            {
                foreach (IItem item in items.Values)
                {
                    if (!item.IsSold)
                    {
                        result.Add((IItem)item.Clone());
                    }
                }
            }

            return result;
        }

        public void AddItem(IItem itemToAdd)
        {
            lock (itemsLock)
            {
                items.Add(itemToAdd.Id, itemToAdd);
            }
        }

        public void RemoveItem(Guid itemIdToRemove)
        {
            lock (itemsLock)
            {
                items.Remove(itemIdToRemove);
            }
        }

        public IItem GetItemByID(Guid guid)
        {
            IItem result;
            lock (itemsLock)
            {
                if (!items.ContainsKey(guid))
                {
                    throw new KeyNotFoundException();
                }
                
                result = items[guid];
            }

            return result;
        }

        public List<IItem> GetItemsByType(ItemType type)
        {
            List<IItem> result = new List<IItem>();
            lock (itemsLock)
            {
                foreach (IItem item in items.Values)
                {
                    if (item.Type == type)
                    {
                        result.Add((IItem)item.Clone());
                    }
                }
            }

            return result;
        }

        public IDisposable Subscribe(IObserver<ReputationChangedEventArgs> observer)
        {
            observers.Add(observer);
            return new DepotDisposable(this, observer);
        }

        private void UnSubscribe(IObserver<ReputationChangedEventArgs> observer)
        {
            observers.Remove(observer);
        }

        private class DepotDisposable : IDisposable
        {
            private readonly Depot depot;
            private readonly IObserver<ReputationChangedEventArgs> observer;

            public DepotDisposable(Depot depot, IObserver<ReputationChangedEventArgs> observer)
            {
                this.depot = depot;
                this.observer = observer;
            }

            public void Dispose()
            {
                depot.UnSubscribe(observer);
            }
        }
    }
}
