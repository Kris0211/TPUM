using System;
using System.Collections.Generic;
using Data;

namespace Logic
{
    class Store : IStore
    {
        private readonly IDepot depot;

        public event EventHandler<LogicReputationChangedEventArgs>? ReputationChanged;

        public Store(IDepot depot)
        {
            this.depot = depot;

            depot.ReputationChanged += HandleOnReputationChanged;
        }

        private void HandleOnReputationChanged(object sender, ReputationChangedEventArgs args)
        {
            ReputationChanged?.Invoke(this, new LogicReputationChangedEventArgs(args));
        }

        public void SellItem(Guid itemId)
        {
            List<IStoreItem> items = new List<IStoreItem>();

            foreach (IItem item in depot.GetAvailableItems())
            {
                items.Add(new StoreItem(item));
            }

            IStoreItem? foundItem = null;
            foreach (IStoreItem storeItem in items)
            {
                if (storeItem.Id == itemId)
                {
                    foundItem = storeItem;
                }
            }

            if (foundItem != null && !foundItem.IsSold)
            {
                depot.SellItem(foundItem.Id);
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }

        public List<IStoreItem> GetItems()
        {
            List<IStoreItem> items = new List<IStoreItem>();

            foreach (IItem item in depot.GetItems())
            {
                items.Add(new StoreItem(item));
            }

            return items;
        }

        public List<IStoreItem> GetAvailableItems()
        {
            List<IStoreItem> items = new List<IStoreItem>();

            foreach (IItem item in depot.GetAvailableItems())
            {
                items.Add(new StoreItem(item));
            }

            return items;
        }

        public List<IStoreItem> GetItemsByType(LogicItemType logicItemType)
        {
            List<IStoreItem> items = new List<IStoreItem>();

            foreach (IItem item in depot.GetItemsByType((ItemType)logicItemType))
            {
                items.Add(new StoreItem(item));
            }

            return items;
        }
    }
}
