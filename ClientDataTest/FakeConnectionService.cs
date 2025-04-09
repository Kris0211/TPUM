using ClientApi;
using ClientData;

namespace ClientDataTest
{
    internal class FakeConnectionService : IConnectionService
    {
        public event Action<string>? Logger;
        public event Action? OnConnectionStateChanged;
        public event Action<string>? OnMessage;
        public event Action? OnError;
        public event Action? OnDisconnect;

        public Task Connect(Uri peerUri)
        {
            throw new NotImplementedException();
        }

        public Task Disconnect()
        {
            throw new NotImplementedException();
        }

        public bool IsConnected()
        {
            return true;
        }

        public async Task SendAsync(string message)
        {
            if (serializer.GetResponseHeader(message) == GetItemsCommand.StaticHeader)
            {
                UpdateAllResponse response = new UpdateAllResponse();
                response.Items = [new ItemDTO(new Guid("testId"), "Test", "Random Description", "Generator", 2000.0f, false)];
                OnMessage?.Invoke(serializer.Serialize(response));
            }
            else if (serializer.GetResponseHeader(message) == SellItemCommand.StaticHeader)
            {
                SellItemCommand sellItemCommand = serializer.Deserialize<SellItemCommand>(message);
                lastSoldGuid = sellItemCommand.ItemID;

                TransactionResponse response = new TransactionResponse();
                response.Succeeded = true;
                OnMessage?.Invoke(serializer.Serialize(response));
            }

            await Task.Delay(0);
        }

        // Fields and methods for test purposes

        private Serializer serializer = Serializer.Create();
        public Guid lastSoldGuid;

        public void FakeReputationChanged(List<IItem> items, float newReputation)
        {
            ReputationChangedResponse response = new ReputationChangedResponse();
            response.NewReputation = newReputation;

            NewPriceDTO[] newPriceDTOs = new NewPriceDTO[items.Count];
            int i = 0;
            foreach (IItem item in items)
            {
                newPriceDTOs[i].ItemID = item.Id;
                newPriceDTOs[i].NewPrice = item.Price * newReputation;
                i++;
            }
            response.NewPrices = newPriceDTOs;

            OnMessage?.Invoke(serializer.Serialize(response));
        }

        public void FakeUpdateAll(ItemDTO[] items)
        {
            UpdateAllResponse response = new UpdateAllResponse();
            response.Items = items;
            OnMessage?.Invoke(serializer.Serialize(response));
        }
    }
}
