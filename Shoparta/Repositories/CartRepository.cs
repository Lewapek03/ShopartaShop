using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shoparta.Data;
using Shoparta.Models;
using System.Security.Claims;

namespace Shoparta.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;

        public CartRepository(ApplicationDbContext db,
            UserManager<IdentityUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        private string GetUserId()
        {
            return _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public async Task<int> AddItem(int itemId, int quantity)
        {
            string userId = GetUserId();

            var cart = await _db.ShoppingCarts
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted);

            if (cart == null)
            {
                cart = new ShoppingCart
                {
                    UserId = userId,
                    IsDeleted = false
                };
                _db.ShoppingCarts.Add(cart);
                await _db.SaveChangesAsync();
            }

            var cartDetail = await _db.CardDetails
                .FirstOrDefaultAsync(cd => cd.ShoppingCartId == cart.Id && cd.ItemId == itemId);

            if (cartDetail != null)
            {
                cartDetail.Quantity += quantity;
            }
            else
            {
                var item = await _db.Items.FindAsync(itemId);
                if (item == null)
                {
                    throw new Exception("Item not found");
                }

                cartDetail = new CardDetail
                {
                    ShoppingCartId = cart.Id,
                    ItemId = itemId,
                    Quantity = quantity,
                    UnitPrice = item.Price
                };
                _db.CardDetails.Add(cartDetail);
            }

            await _db.SaveChangesAsync();

            return await GetCartItemCount();
        }

        public async Task<int> RemoveItem(int itemId)
        {
            string userId = GetUserId();

            var cart = await _db.ShoppingCarts
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted);

            if (cart == null)
                return 0;

            var cartDetail = await _db.CardDetails
                .FirstOrDefaultAsync(cd => cd.ShoppingCartId == cart.Id && cd.ItemId == itemId);

            if (cartDetail != null)
            {
                _db.CardDetails.Remove(cartDetail);
                await _db.SaveChangesAsync();
            }

            return await GetCartItemCount();
        }

        public async Task<int> GetCartItemCount()
        {
            string userId = GetUserId();

            var count = await _db.ShoppingCarts
                .Where(c => c.UserId == userId && !c.IsDeleted)
                .SelectMany(c => c.CartDetails)
                .SumAsync(cd => cd.Quantity);

            return count;
        }

        public async Task<ShoppingCart> GetUserCart()
        {
            string userId = GetUserId();

            var cart = await _db.ShoppingCarts
                .Include(c => c.CartDetails)
                    .ThenInclude(cd => cd.Item)
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted);

            return cart;
        }

        public async Task<bool> Checkout()
        {
            string userId = GetUserId();

            var cart = await _db.ShoppingCarts
                .Include(c => c.CartDetails)
                    .ThenInclude(cd => cd.Item)
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted);

            if (cart == null || !cart.CartDetails.Any())
                return false;

            var order = new Order
            {
                UserId = userId,
                CreatedDate = DateTime.UtcNow,
                OrderStatusId = 1, 
                IsDeleted = false,
                OrderDetail = new List<OrderDetail>()
            };

            foreach (var cartDetail in cart.CartDetails)
            {
                var orderDetail = new OrderDetail
                {
                    Order = order,
                    ItemId = cartDetail.ItemId,
                    Quantity = cartDetail.Quantity,
                    UnitPrice = cartDetail.UnitPrice
                };
                order.OrderDetail.Add(orderDetail);
            }

            _db.Orders.Add(order);

            _db.CardDetails.RemoveRange(cart.CartDetails);

            await _db.SaveChangesAsync();

            return true;
        }
    }
}
