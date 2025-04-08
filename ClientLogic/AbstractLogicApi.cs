using System;
using System.Collections.Generic;
using Data;

namespace Logic
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

        public void SellItem(Guid itemId);

        public List<IStoreItem> GetItems();
        public List<IStoreItem> GetAvailableItems();
        public List<IStoreItem> GetItemsByType(LogicItemType type);
    }

    public abstract class AbstractLogicApi
    {
        public AbstractDataApi DataApi { get; private set; }

        public AbstractLogicApi(AbstractDataApi dataApi)
        {
            DataApi = dataApi;
        }

        public static AbstractLogicApi Create(AbstractDataApi? abstractDataApi = null)
        {
            AbstractDataApi dataApi = abstractDataApi ?? AbstractDataApi.Create();
            return new Logic(dataApi);
        }

        public abstract IStore GetStore();
    }
}
