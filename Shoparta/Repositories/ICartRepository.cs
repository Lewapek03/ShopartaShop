using Shoparta.Models;

namespace Shoparta.Repositories
{
    public interface ICartRepository
    {
        Task<int> AddItem(int itemId, int quantity);
        Task<int> RemoveItem(int itemId);
        Task<int> GetCartItemCount();
        Task<ShoppingCart> GetUserCart();
        Task<bool> Checkout();
    }
}
