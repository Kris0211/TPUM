using ServerData;

namespace ServerLogic
{
    internal class Store : IStore
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
            foreach (var item in depot.GetItems())
            {
                if (!item.IsSold)
                {
                    items.Add(new StoreItem(item));
                }
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
            foreach (var item in depot.GetItems())
            {
                items.Add(new StoreItem(item));
            }

            return items;
        }
    }
}
