namespace Data
{
    internal class DataApi : AbstractDataApi
    {
        private readonly Depot depot;

        public DataApi()
        {
            depot = new Depot();
        }

        public override IDepot GetDepot()
        {
            return depot;
        }

    }
}
