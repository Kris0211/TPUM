using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace ServerData
{
    internal class Depot : IDepot
    {
        private readonly Dictionary<Guid, IItem> items = [];
        private object itemsLock = new();

        private bool useReputation;
        private object reputationLock = new();

        public event EventHandler<ReputationChangedEventArgs>? ReputationChanged;

        public Depot() 
        {
            AddItem(CreateItem("Laser Cannon", "+10 Damage", ItemType.Weapon, 50));
            AddItem(CreateItem("Rocket Launcher", "+20 Damage", ItemType.Weapon, 150));
            AddItem(CreateItem("Laser Battery", "Ammo for Laser Cannon", ItemType.Ammo, 10));
            AddItem(CreateItem("Rocket", "Ammo for Rocket Launcher", ItemType.Ammo, 25));
            AddItem(CreateItem("Shield Generator", "+25 Health", ItemType.Generator, 200));
            AddItem(CreateItem("Stinger", "Space Fighter", ItemType.Spaceship, 500));

            useReputation = true;
            SimulateReputation();
        }

        ~Depot () 
        {
            useReputation = false;
            lock (reputationLock)
            {

            }
        }

        private async void SimulateReputation()
        {
            while (true)
            {
                Random random = new Random();
                float waitSeconds = (float)random.NextDouble() * 2f + 3f; // range 3 - 5 seconds
                await Task.Delay((int)Math.Truncate(waitSeconds * 1000f));

                float reputation = (float)random.NextDouble() + 0.5f; // range 0.5 - 1.5
                lock (itemsLock)
                {
                    foreach (IItem item in items.Values)
                    {
                        item.Price *= reputation;
                    }
                }

                ReputationChanged?.Invoke(this, new ReputationChangedEventArgs(reputation));
                lock (reputationLock)
                {
                    if (!useReputation)
                    {
                        break;
                    }
                }
            }
        }

        public IItem CreateItem(string name, string description, ItemType type, float price)
        {
            return new Item(name, description, type, price);
        }

        public void SellItem(Guid itemId)
        {
            lock (itemsLock)
            {
                if (items.ContainsKey(itemId))
                {
                    items[itemId].IsSold = true;
                }
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

        public IItem GetItemById(Guid guid)
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
    }
}
