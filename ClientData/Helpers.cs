using System;
using System.Text;
using ClientApi;

namespace ClientData
{
    internal static class Helpers
    {
        public static ItemType ItemTypeFromString(string typeStr)
        {
            return (ItemType)Enum.Parse(typeof(ItemType), typeStr);
        }

        public static string ToString(this ItemType typeStr)
        {
            return Enum.GetName(typeof(ItemType), typeStr) ?? throw new InvalidOperationException();
        }

        public static IItem ToItem(this ItemDTO itemDTO)
        {
            return new Item(itemDTO.Id, itemDTO.Name, itemDTO.Description, ItemTypeFromString(itemDTO.Type), itemDTO.Price, itemDTO.IsSold);
        }
    }
}
