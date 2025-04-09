using ClientApi;
using ServerLogic;

namespace ServerPresentation
{
    internal static class Utils
    {
        public static ItemDTO ToDTO(this IStoreItem item)
        {
            return new ItemDTO(
                item.Id,
                item.Name,
                item.Description,
                item.Type.ToString(),
                item.Price,
                item.IsSold
            );
        }
    }
}
