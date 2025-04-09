using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClientData;

namespace ClientLogic
{
    class Store : IStore, IObserver<ReputationChangedEventArgs>
    {
        private readonly IDepot depot;

        public event EventHandler<LogicReputationChangedEventArgs>? ReputationChanged;
        public event Action? ItemsUpdated;
        public event Action<bool>? TransactionFinished;

        private IDisposable DepotSubscriptionHandle;

        public Store(IDepot depot)
        {
            this.depot = depot;

            DepotSubscriptionHandle = depot.Subscribe(this);

            depot.ItemsUpdated += OnItemsUpdated;
            depot.TransactionFinished += OnTransactionFinished;
        }

        public void RequestUpdate()
        {
            depot.RequestUpdate();
        }

        public async Task SellItem(Guid itemId)
        {
            List<IStoreItem> items = new List<IStoreItem>();
            foreach (var item in depot.GetAvailableItems())
            {
                items.Add(new StoreItem(item));
            }

            IStoreItem? foundItem = null;
            foreach (var item in items)
            {
                if (item.Id == itemId)
                {
                    foundItem = item;
                    break;
                }
            }

            if (foundItem != null && !foundItem.IsSold)
            {
                await depot.SellItem(foundItem.Id);
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }

        public List<IStoreItem> GetItems()
        {
            List<IStoreItem> result = new List<IStoreItem>();
            foreach (var item in depot.GetItems())
            {
                result.Add(new StoreItem(item));
            }

            return result;
        }

        public List<IStoreItem> GetAvailableItems()
        {
            List<IStoreItem> result = new List<IStoreItem>();
            foreach (var item in depot.GetAvailableItems())
            {
                result.Add(new StoreItem(item));
            }

            return result;
        }

        public List<IStoreItem> GetItemsByType(LogicItemType logicItemType)
        {
            List<IStoreItem> result = new List<IStoreItem>();
            foreach (var item in depot.GetItemsByType((ItemType)logicItemType))
            {
                result.Add(new StoreItem(item));
            }

            return result;
        }

        public void OnCompleted()
        {
            DepotSubscriptionHandle.Dispose();
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(ReputationChangedEventArgs value)
        {
            ReputationChanged?.Invoke(this, new LogicReputationChangedEventArgs(value));
        }

        private void OnItemsUpdated()
        {
            ItemsUpdated?.Invoke();
        }

        private void OnTransactionFinished(bool succeeded)
        { 
            TransactionFinished?.Invoke(succeeded);
        }
    }
}
