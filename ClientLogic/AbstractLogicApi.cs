using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClientData;

namespace ClientLogic
{
    public class LogicReputationChangedEventArgs : EventArgs
    {
        public float NewReputation { get; }

        public LogicReputationChangedEventArgs(float newReputation)
        {
            this.NewReputation = newReputation;
        }

        internal LogicReputationChangedEventArgs(ReputationChangedEventArgs args)
        {
            this.NewReputation = args.NewReputation;
        }
    }

    public enum LogicItemType
    {
        Weapon = 0,
        Ammo = 1,
        Generator = 2,
        Spaceship = 3
    }

    public interface IStoreItem
    {
        Guid Id { get; }
        string Name { get; }
        string Description { get; }

        LogicItemType Type { get; }
        float Price { get; }

        bool IsSold { get; }
    }

    public interface IStore
    {
        public event EventHandler<LogicReputationChangedEventArgs> ReputationChanged;

        public event Action? ItemsUpdated;
        public event Action<bool>? TransactionFinished;

        public void RequestUpdate();

        public List<IStoreItem> GetItems();
        public List<IStoreItem> GetAvailableItems();
        public List<IStoreItem> GetItemsByType(LogicItemType logicItemType);

        public Task SellItem(Guid itemId);
    }

    public interface ILogicConnectionService
    {
        public event Action<string>? Logger;
        public event Action? OnConnectionStateChanged;

        public event Action<string>? OnMessage;
        public event Action? OnError;

        public Task Connect(Uri peerUri);
        public Task Disconnect();

        public bool IsConnected();
    }

    public abstract class AbstractLogicApi
    {
        public AbstractDataApi DataApi { get; private set; }

        public AbstractLogicApi(AbstractDataApi dataApi)
        {
            DataApi = dataApi;
        }

        public static AbstractLogicApi Create(AbstractDataApi? dataAbstractApi = null)
        {
            AbstractDataApi dataApi = dataAbstractApi ?? AbstractDataApi.Create(null);
            return new ClientLogic.Logic(dataApi);
        }

        public abstract IStore GetStore();
        public abstract ILogicConnectionService GetConnectionService();
    }
}
