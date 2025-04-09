using System;
using System.Threading.Tasks;
using ClientLogic;

namespace Model
{
    public enum PresentationItemType
    {
        Weapon = 0,
        Ammo = 1,
        Generator = 2,
        Spaceship = 3
    }

    public class ModelReputationChangedEventArgs : EventArgs
    {
        public float NewReputation { get; }

        public ModelReputationChangedEventArgs(float newReputation)
        {
            this.NewReputation = newReputation;
        }

        internal ModelReputationChangedEventArgs(LogicReputationChangedEventArgs args)
        {
            this.NewReputation = args.NewReputation;
        }
    }

    public class Model
    {
        private AbstractLogicApi abstractLogicApi;
        public DepotPresentation DepotPresentation { get; private set; }
        public ModelConnectionService ModelConnectionService { get; private set; }

        public event Action? ItemsUpdated;

        public Model(AbstractLogicApi? abstractLogicApi)
        {
            this.abstractLogicApi = abstractLogicApi ?? AbstractLogicApi.Create();

            DepotPresentation = new DepotPresentation(this.abstractLogicApi.GetStore());
            ModelConnectionService = new ModelConnectionService(this.abstractLogicApi.GetConnectionService());
        }

        public async Task SellItem(Guid itemId)
        {
            await abstractLogicApi.GetStore().SellItem(itemId);
        }
    }
}
