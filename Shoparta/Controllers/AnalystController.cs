using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shoparta.Data;
using Shoparta.Models;
using System.Collections.Generic;
using System.Linq;

namespace Shoparta.Controllers
{
    [Authorize(Roles = "Analyst")]
    public class AnalystController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AnalystController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            
            var totalOrders = _context.Orders.Count();
            var totalSales = _context.OrderDetails.Sum(od => od.Quantity * od.UnitPrice);

            var topSellingItemsData = _context.OrderDetails
                .GroupBy(od => od.ItemId)
                .Select(group => new
                {
                    ItemId = group.Key,
                    QuantitySold = group.Sum(od => od.Quantity)
                })
                .OrderByDescending(g => g.QuantitySold)
                .Take(5)
                .ToList();

            var topSellingItems = topSellingItemsData
                .Select(g => new TopSellingItem
                {
                    Item = _context.Items
                        .Include(i => i.Category)
                        .FirstOrDefault(i => i.Id == g.ItemId),
                    QuantitySold = g.QuantitySold
                }).ToList();

            ViewBag.TotalOrders = totalOrders;
            ViewBag.TotalSales = totalSales;
            ViewBag.TopSellingItems = topSellingItems;

            return View();
        }

        // Klasa pomocnicza do przechowywania danych o najlepiej sprzedających się produktach
        public class TopSellingItem
        {
            public Item Item { get; set; }
            public int QuantitySold { get; set; }
        }
    }
}
