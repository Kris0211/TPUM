using System;
using System.Text.Json.Serialization;

namespace ClientApi
{
    [Serializable]
    public abstract class ServerCommand
    {
        public string Header;

        protected ServerCommand(string header) 
        {
            Header = header;
        }
    }

    [Serializable]
    public class  GetItemsCommand : ServerCommand
    {
        public static string HeaderStatic = "GetItems";

        public GetItemsCommand() : base(HeaderStatic) { }
    }

    [Serializable]
    public class SellItemCommand : ServerCommand
    {
        public static string HeaderStatic = "SellItem";

        public Guid TransactionId;
        public Guid ItemId;

        public SellItemCommand(Guid id) : base(HeaderStatic)
        {
            TransactionId = Guid.NewGuid();
            ItemId = id;
        }
    }

    [Serializable]
    public struct ItemDTO
    {
        public Guid Id;
        public string Name;
        public string Description;
        public string Type;
        public float Price;
        public bool IsSold;

        public ItemDTO(Guid id, string name, string description, string type, float price, bool isSold)
        {
            Id = id;
            Name = name;
            Description = description;
            Type = type;
            Price = price;
            IsSold = isSold;
        }
    }

    [Serializable]
    public struct NewPriceDTO
    {
        public Guid ItemId;
        public float NewPrice;

        public NewPriceDTO(Guid id, float newPrice)
        {
            ItemId = id;
            NewPrice = newPrice;
        }
    }

    [Serializable]
    public abstract class ServerResponse
    {
        public string Header { get; private set; }

        protected ServerResponse(string header)
        {
            Header = header;
        }
    }

    [Serializable]
    public class UpdateAllResponse : ServerResponse
    {
        public static readonly string HeaderStatic = "UpdateAllItems";

        public ItemDTO[]? Items;

        public UpdateAllResponse() : base(HeaderStatic) { }
    }

    [Serializable]
    public class ReputationChangedResponse : ServerResponse
    {
        public static readonly string HeaderStatic = "ReputationChanged";

        public float NewReputation;
        public NewPriceDTO[]? NewPrices;

        public ReputationChangedResponse() : base(HeaderStatic) { }
    }

    [Serializable]
    public class TransactionResponse : ServerResponse
    {
        public static readonly string HeaderStatic = "TransactionResponse";

        public Guid TransactionId;
        public bool Succeeded;

        public TransactionResponse() : base(HeaderStatic) { }
}
