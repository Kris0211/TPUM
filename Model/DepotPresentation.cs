using System;
using System.Collections.Generic;
using ClientLogic;

namespace Model
{
    public class DepotPresentation
    {
        private IStore Store { get; set; }

        public event EventHandler<ModelReputationChangedEventArgs>? ReputationChanged;
        public Action? OnItemsUpdated;
        public event Action<bool>? TransactionFinished;

        public DepotPresentation(IStore store)
        {
            this.Store = store;
            store.ItemsUpdated += UpdateItems;
            store.ReputationChanged += OnStoreReputationChanged;
            store.TransactionFinished += FinishTransaction;
        }

        public void RequestUpdate()
        {
            Store.RequestUpdate();
        }

        public List<ItemPresentation> GetItems()
        {
            List<ItemPresentation> result = new List<ItemPresentation>();
            foreach (IStoreItem item in Store.GetItems())
            {
                result.Add(new ItemPresentation(item));
            }

            return result;
        }

        public List<ItemPresentation> GetAvailableItems()
        {
            List<ItemPresentation> result = new List<ItemPresentation>();
            foreach (IStoreItem item in Store.GetAvailableItems())
            {
                result.Add(new ItemPresentation(item));
            }

            return result;
        }

        public List<ItemPresentation> GetItemsByType(PresentationItemType itemType)
        {
            List<ItemPresentation> result = new List<ItemPresentation>();
            foreach (IStoreItem item in Store.GetItemsByType((LogicItemType)itemType))
            {
                result.Add(new ItemPresentation(item));
            }

            return result;
        }

        private void UpdateItems() 
        {
            OnItemsUpdated?.Invoke();
        }

        private void OnStoreReputationChanged(object? sender, LogicReputationChangedEventArgs args)
        {
            ReputationChanged?.Invoke(this, new ModelReputationChangedEventArgs(args));
        }


        private void FinishTransaction(bool succeeded)
        {
            TransactionFinished?.Invoke(succeeded);
        }

    }
}
