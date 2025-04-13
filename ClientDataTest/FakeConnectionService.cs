using ClientApi;
using ClientData;
using Newtonsoft.Json;
using System.CodeDom.Compiler;

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
            if (serializer.GetResponseHeader(message) == FakeServerStatics.GetItemsCommandHeader)
            {
                FakeUpdateAllResponse response = new FakeUpdateAllResponse();
                response.Header = FakeServerStatics.UpdateAllResponseHeader;
                response.Items = [new FakeItemDTO { Id = new Guid("testId"), Name = "Test", Description = "Random Description", 
                    Type = "Generator", Price = 2000.0f, IsSold = false }];
                OnMessage?.Invoke(serializer.Serialize(response));
            }
            else if (serializer.GetResponseHeader(message) == FakeServerStatics.SellItemCommandHeader)
            {
                FakeSellItemCommand sellItemCommand = serializer.Deserialize<FakeSellItemCommand>(message);
                lastSoldGuid = sellItemCommand.ItemId;

                FakeTransactionResponse response = new FakeTransactionResponse();
                response.Header = FakeServerStatics.TransactionResponseHeader;
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
            FakeReputationChangedResponse response = new FakeReputationChangedResponse();
            response.Header = FakeServerStatics.ReputationChangedResponseHeader;
            response.NewReputation = newReputation;

            FakeNewPriceDTO[] newPriceDTOs = new FakeNewPriceDTO[items.Count];
            for (int i = 0; i < items.Count; i++)
            {
                newPriceDTOs[i] = new FakeNewPriceDTO
                {
                    ItemId = items[i].Id,
                    NewPrice = items[i].Price * newReputation
                };
            }
            response.NewPrices = newPriceDTOs;

            OnMessage?.Invoke(serializer.Serialize(response));
        }

        public void FakeUpdateAll(FakeItemDTO[] items)
        {
            FakeUpdateAllResponse response = new FakeUpdateAllResponse();
            response.Header = FakeServerStatics.UpdateAllResponseHeader;
            response.Items = items;
            OnMessage?.Invoke(serializer.Serialize(response));
        }
    }

    [GeneratedCode("NJsonSchema", "11.0.0.0 (Newtonsoft.Json v13.0.0.0)")]
    internal class FakeItemDTO
    {
        [JsonProperty("Id", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Guid Id { get; set; }

        [JsonProperty("Name", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("Description", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty("Type", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("Price", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public float Price { get; set; }

        [JsonProperty("IsSold", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool IsSold { get; set; }
    }

    [GeneratedCode("NJsonSchema", "11.0.0.0 (Newtonsoft.Json v13.0.0.0)")]
    internal abstract class FakeServerResponse
    {
        [JsonProperty("Header", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Header { get; set; }
    }

    [GeneratedCode("NJsonSchema", "11.0.0.0 (Newtonsoft.Json v13.0.0.0)")]
    internal class FakeUpdateAllResponse : FakeServerResponse
    {
        [JsonProperty("Items", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<FakeItemDTO> Items { get; set; }
    }

    [GeneratedCode("NJsonSchema", "11.0.0.0 (Newtonsoft.Json v13.0.0.0)")]
    internal class FakeReputationChangedResponse : FakeServerResponse
    {
        [JsonProperty("NewReputation", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public float NewReputation { get; set; }

        [JsonProperty("NewPrices", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<FakeNewPriceDTO> NewPrices { get; set; }
    }

    [GeneratedCode("NJsonSchema", "11.0.0.0 (Newtonsoft.Json v13.0.0.0)")]
    internal class FakeTransactionResponse : FakeServerResponse
    {
        [JsonProperty("TransactionId", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Guid TransactionId { get; set; }

        [JsonProperty("Succeeded", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool Succeeded { get; set; }
    }

    [GeneratedCode("NJsonSchema", "11.0.0.0 (Newtonsoft.Json v13.0.0.0)")]
    internal abstract class FakeServerCommand
    {
        [JsonProperty("Header", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Header { get; set; }
    }

    [GeneratedCode("NJsonSchema", "11.0.0.0 (Newtonsoft.Json v13.0.0.0)")]
    internal class FakeSellItemCommand : FakeServerCommand
    {
        [JsonProperty("TransactionId", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Guid TransactionId { get; set; }

        [JsonProperty("ItemId", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Guid ItemId { get; set; }
    }

    [GeneratedCode("NJsonSchema", "11.0.0.0 (Newtonsoft.Json v13.0.0.0)")]
    internal class FakeNewPriceDTO
    {
        [JsonProperty("ItemId", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Guid ItemId { get; set; }

        [JsonProperty("NewPrice", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public float NewPrice { get; set; }
    }

    internal static class FakeServerStatics
    {
        public static readonly string GetItemsCommandHeader = "GetItems";
        public static readonly string SellItemCommandHeader = "SellItem";

        public static readonly string UpdateAllResponseHeader = "UpdateAllItems";
        public static readonly string ReputationChangedResponseHeader = "ReputationChanged";
        public static readonly string TransactionResponseHeader = "TransactionResponse";
    }
}
