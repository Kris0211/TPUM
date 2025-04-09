using System;
using ClientData;

namespace ClientLogic
{
    internal class Logic : AbstractLogicApi
    {
        private readonly IStore store;
        private readonly ILogicConnectionService connectionService;

        public Logic(AbstractDataApi dataApi)
            : base(dataApi)
        {
            store = new Store(dataApi.GetDepot());
            connectionService = new LogicConnectionService(dataApi.GetConnectionService());
        }

        public override IStore GetStore()
        {
            return store;
        }

        public override ILogicConnectionService GetConnectionService()
        {
            return connectionService;
        }
    }
}

