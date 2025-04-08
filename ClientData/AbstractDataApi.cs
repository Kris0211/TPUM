using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientData
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

    public interface IDepot : IObservable<ReputationChangedEventArgs>
    {
        public event Action? ItemsUpdated;
        public event Action<bool>? TransactionFinish;

        public void RequestUpdate();
        
        List<IItem> GetItems();
        List<IItem> GetAvailableItems();

        IItem GetItemByID(Guid guid);
        List<IItem> GetItemsByType(ItemType type);

        void SellItem(Guid itemId);
    }

    public interface IConnectionService
    {
        public event Action<string>? Logger;
        public event Action? OnConnectionStateChanged;

        public event Action<string>? OnMessage;
        public event Action? OnError;
        public event Action? OnDisconnect;

        public Task Connect(Uri uriPeer);
        public Task Disconnect();

        public bool IsConnected();

        public Task SendAsync(string message);
    }

    public abstract class AbstractDataApi
    {
        public static AbstractDataApi Create(IConnectionService? connectionService)
        {
            return new DataApi(connectionService);
        }

        public abstract IDepot GetDepot();
        public abstract IConnectionService GetConnectionService();
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
