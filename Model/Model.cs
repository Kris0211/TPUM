using System;
using Logic;

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

        public DepotPresentation depotPresentation {  get; private set; }

        public event EventHandler<ModelReputationChangedEventArgs>? ReputationChanged;

        public Model(AbstractLogicApi abstractLogicApi)
        {
            this.abstractLogicApi = abstractLogicApi == null ? AbstractLogicApi.Create() : abstractLogicApi;
            this.abstractLogicApi.GetStore().ReputationChanged += HandleReputationChanged;
            this.depotPresentation = new DepotPresentation(this.abstractLogicApi.GetStore());
        }

        public void SellItem(Guid itemId)
        {
            abstractLogicApi.GetStore().SellItem(itemId);
        }

        public void HandleReputationChanged(object sender, LogicReputationChangedEventArgs args)
        {
            ReputationChanged?.Invoke(this, new ModelReputationChangedEventArgs(args));
        }
    }
}
