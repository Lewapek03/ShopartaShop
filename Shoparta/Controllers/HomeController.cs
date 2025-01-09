using Microsoft.AspNetCore.Mvc;
using Shoparta.Models;
using Shoparta.Models.DTOs;
using Shoparta.Repositories;
using System.Diagnostics;

namespace Shoparta.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeRepository _homeRepository;

        public HomeController(ILogger<HomeController> logger, IHomeRepository homeRepository)
        {
            _homeRepository = homeRepository;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string searchItem = "", int categoryId = 0)
        {
            IEnumerable<Item> items = await _homeRepository.GetItems(searchItem, categoryId);
            IEnumerable<Category> categories = await _homeRepository.Categories();
            ItemDTO itemDTO = new ItemDTO
            {
                Items = items,
                Categories = categories,
                SearchItem = searchItem,
                CategoryId = categoryId
            };
            return View(itemDTO);
        }

        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
