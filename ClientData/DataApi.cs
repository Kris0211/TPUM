namespace ClientData
{
    internal class DataApi : AbstractDataApi
    {
        private readonly Depot depot;
        private readonly IConnectionService connectionService;

        public DataApi(IConnectionService? connectionService)
        {
            this.connectionService = connectionService ?? new ConnectionService();
            depot = new Depot(this.connectionService);
        }

        public override IDepot GetDepot()
        {
            return depot;
        }

        public override IConnectionService GetConnectionService()
        {
            return connectionService;
        }
    }
}
