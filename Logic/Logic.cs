using System;
using Data;

namespace Logic
{
    public class Logic : AbstractLogicApi
    {
        private readonly IStore store;

        public Logic(AbstractDataApi dataApi) : base(dataApi)
        {
            store = new Store(dataApi.GetDepot());
        }

        public override IStore GetStore()
        {
            return store;
        }
    }
}
