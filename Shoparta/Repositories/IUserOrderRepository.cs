using Shoparta.Models;

namespace Shoparta.Repositories
{
    public interface IUserOrderRepository
    {
        Task<IEnumerable<Order>> UserOrders();
        Task<Order> GetOrderDetails(int orderId);
    }
}
