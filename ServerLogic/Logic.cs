using ServerData;

namespace ServerLogic
{

    internal class Logic : LogicAbstractApi
    {
        private readonly IStore store;

        public Logic(AbstractDataApi dataApi)
            : base(dataApi)
        {
            store = new Store(dataApi.GetDepot());
        }

        public override IStore GetStore()
        {
            return store;
        }
    }
}