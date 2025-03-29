using System;
using System.Collections.Generic;

namespace Data
{
    public enum ItemType
    {
        Weapon = 0,
        Ammo = 1,
        Generator = 2,
        Spaceship = 3
    }

    public interface IItem : ICloneable
    {
        Guid Id { get; }
        string Name { get; }
        string Description { get; }

        ItemType Type { get; }
        float Price { get; set; }

        bool IsSold { get; set; }
    }

    public interface IDepot
    {
        event EventHandler<ReputationChangedEventArgs> ReputationChanged;

        IItem CreateItem(string name, string description, ItemType type, float price);
        
        List<IItem> GetItems();
        List<IItem> GetAvailableItems();

        IItem GetItemByID(Guid guid);
        List<IItem> GetItemsByType(ItemType type);

        void AddItem(IItem itemToAdd);
        void RemoveItem(Guid itemIdToRemove);
        void SellItem(Guid itemId);
    }

    public abstract class AbstractDataApi
    {
        public static AbstractDataApi Create()
        {
            return new DataApi();
        }

        public abstract IDepot GetDepot();
    }

    public class ReputationChangedEventArgs : EventArgs
    {
        public float NewReputation { get; }

        public ReputationChangedEventArgs(float newReputation)
        {
            NewReputation = newReputation;
        }
    }
}
