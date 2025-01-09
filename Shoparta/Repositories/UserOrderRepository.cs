using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shoparta.Data;
using Shoparta.Models;
using System.Security.Claims;

namespace Shoparta.Repositories
{
    public class UserOrderRepository : IUserOrderRepository
    {

        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;

        public UserOrderRepository(ApplicationDbContext db,
            UserManager<IdentityUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        private async Task<IdentityUser> GetCurrentUser()
        {
            var principal = _httpContextAccessor.HttpContext.User;
            var userId = _userManager.GetUserId(principal);
            if (string.IsNullOrEmpty(userId))
                throw new Exception("User is not logged-in");
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<IEnumerable<Order>> UserOrders()
        {
            var user = await GetCurrentUser();

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                return await _db.Orders
                            .Include(x => x.OrderStatus)
                            .Include(x => x.OrderDetail)
                                .ThenInclude(x => x.Item)
                                .ThenInclude(x => x.Category)
                            .OrderByDescending(o => o.CreatedDate)
                            .ToListAsync();
            }
            else
            {
                return await _db.Orders
                            .Include(x => x.OrderStatus)
                            .Include(x => x.OrderDetail)
                                .ThenInclude(x => x.Item)
                                .ThenInclude(x => x.Category)
                            .Where(a => a.UserId == user.Id)
                            .OrderByDescending(o => o.CreatedDate)
                            .ToListAsync();
            }
        }

        public async Task<Order> GetOrderDetails(int orderId)
        {
            var user = await GetCurrentUser();

            var order = await _db.Orders
                .Include(o => o.OrderStatus)
                .Include(o => o.OrderDetail)
                    .ThenInclude(od => od.Item)
                .FirstOrDefaultAsync(o => o.Id == orderId && !o.IsDeleted);

            if (order == null)
            {
                return null;
            }

            if (order.UserId != user.Id && !await _userManager.IsInRoleAsync(user, "Admin"))
            {
                return null;
            }

            return order;
        }
    }
}
