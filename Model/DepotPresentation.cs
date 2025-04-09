using System.Collections.Generic;
using ClientLogic;

namespace Model
{
    public class DepotPresentation
    {
        private IStore store { get; set; }

        public DepotPresentation(IStore store)
        {
            this.store = store;
        }

        public List<ItemPresentation> GetItems()
        {
            List<ItemPresentation> items = new List<ItemPresentation>();

            foreach (IStoreItem item in store.GetItems())
            {
                items.Add(new ItemPresentation(item));
            }

            return items;
        }

        public List<ItemPresentation> GetAvailableItems()
        {
            List<ItemPresentation> items = new List<ItemPresentation>();

            foreach (IStoreItem item in store.GetAvailableItems())
            {
                items.Add(new ItemPresentation(item));
            }

            return items;
        }

        public List<ItemPresentation> GetItemsByType(PresentationItemType itemType)
        {
            List<ItemPresentation> items = new List<ItemPresentation>();

            foreach (IStoreItem item in store.GetItemsByType((LogicItemType)itemType))
            {
                items.Add(new ItemPresentation(item));
            }

            return items;
        }
    }
}
