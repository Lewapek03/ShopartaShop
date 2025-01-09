using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shoparta.Repositories;

namespace Shoparta.Controllers
{
    [Authorize]
    public class UserOrderController : Controller
    {
        private readonly IUserOrderRepository _userOrderRepository;

        public UserOrderController(IUserOrderRepository userOrderRepository)
        {
            _userOrderRepository = userOrderRepository;
        }

        public async Task<IActionResult> UserOrders()
        {
            var orders = await _userOrderRepository.UserOrders();
            return View(orders);
        }

        public async Task<IActionResult> OrderDetails(int orderId)
        {
            var order = await _userOrderRepository.GetOrderDetails(orderId);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }
    }
}
