using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shoparta.Repositories;

namespace Shoparta.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartRepository _cartRepository;

        public CartController(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<IActionResult> AddItem(int itemId, int quantity = 1, int redirect = 0)
        {
            var cartCount = await _cartRepository.AddItem(itemId, quantity);
            if (redirect == 0)
            {
                return Ok(cartCount);
            }
            return RedirectToAction("UserCart");
        }

        public async Task<IActionResult> RemoveItem(int itemId)
        {
            var cartCount = await _cartRepository.RemoveItem(itemId);
            return RedirectToAction("UserCart");
        }

        public async Task<IActionResult> GetCartItemCount()
        {
            int cartItemCount = await _cartRepository.GetCartItemCount();
            return Ok(cartItemCount);
        }

        public async Task<IActionResult> UserCart()
        {
            var cart = await _cartRepository.GetUserCart();
            return View(cart);
        }

        public async Task<IActionResult> Checkout()
        {
            bool isCheckedOut = await _cartRepository.Checkout();
            if (!isCheckedOut)
            {
                return View("Error");
            }
            return RedirectToAction("UserOrders", "UserOrder");
        }
    }
}
