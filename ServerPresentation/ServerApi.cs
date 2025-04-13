using System;

namespace ClientApi
{
    [Serializable]
    public abstract class ServerCommand
    {
        public string Header { get; set; }

        protected ServerCommand(string header) 
        {
            Header = header;
        }
    }

    [Serializable]
    public class  GetItemsCommand : ServerCommand
    {
        public static string HeaderStatic = "GetItems";

        public GetItemsCommand() : base(HeaderStatic)
        {

        }
    }

    [Serializable]
    public class SellItemCommand : ServerCommand
    {
        public static string HeaderStatic = "SellItem";
        public Guid TransactionId { get; set; }
        public Guid ItemId { get; set; }

        public SellItemCommand(Guid itemID) : base(HeaderStatic)
        {
            TransactionId = Guid.NewGuid();
            ItemId = itemID;
        }
    }

    [Serializable]
    public class ItemDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public float Price { get; set; }
        public bool IsSold { get; set; }

        public ItemDTO()
        {
            Id = Guid.Empty;
            Name = "Missing";
            Description = "Missing";
            Type = "Missing";
            Price = 0f;
            IsSold = false;
        }

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
        public Guid ItemId { get; set; }
        public float NewPrice { get; set; }

        public NewPriceDTO(Guid itemId, float newPrice)
        {
            ItemId = itemId;
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

        public UpdateAllResponse()
            : base(HeaderStatic)
        {
        }
    }

    [Serializable]
    public class ReputationChangedResponse : ServerResponse
    {
        public static readonly string HeaderStatic = "ReputationChanged";

        public float NewReputation;
        public NewPriceDTO[]? NewPrices;

        public ReputationChangedResponse()
            : base(HeaderStatic)
        {
        }

    }

    [Serializable]
    public class TransactionResponse : ServerResponse
    {
        public static readonly string HeaderStatic = "TransactionResponse";

        public Guid TransactionId;
        public bool Succeeded;

        public TransactionResponse()
            : base(HeaderStatic)
        {
        }
    }
}
