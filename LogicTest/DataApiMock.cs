using System;
using System.Collections.Generic;
using Data;

namespace LogicTest
{
    public class DataApiMock : AbstractDataApi
    {
        private readonly DepotMock depotMock = new DepotMock();

        public override IDepot GetDepot()
        {
            return depotMock;
        }

        public class ItemMock : IItem
        {
            public ItemMock(string name, string description, ItemType type, float price, bool isSold)
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

        public class DepotMock : IDepot
        {
            private readonly List<IItem> allItems;
            private readonly List<IItem> availableItems;

            public DepotMock()
            {
                availableItems = new List<IItem>
                {
                    new ItemMock("MockTestName1", "MockTestDescription1", ItemType.Ammo, 1f, false),
                    new ItemMock("MockTestName2", "MockTestDescription2", ItemType.Ammo, 1.5f, false)
                };

                allItems = new List<IItem>
                {
                    new ItemMock("MockSoldTestName", "MockSoldTestDescription", ItemType.Spaceship, 200f, true)
                };

                allItems.AddRange(availableItems);
            }

            public void SellItem(Guid itemId)
            {
                IItem? foundItem = null;

                foreach (IItem item in availableItems)
                {
                    if (item.Id == itemId)
                    {
                        foundItem = item;
                    }
                }

                if (foundItem != null)
                {
                    availableItems.Remove(foundItem);
                }
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

            public event EventHandler<ReputationChangedEventArgs>? ReputationChanged;

            public IItem CreateItem(string name, string description, ItemType type, float price)
            {
                throw new NotImplementedException();
            }

            public void AddItem(IItem addItem)
            {
                throw new NotImplementedException();
            }

            public void RemoveItem(Guid removeItemId)
            {
                throw new NotImplementedException();
            }

            public IItem GetItemByID(Guid id)
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
        }
    }
}
