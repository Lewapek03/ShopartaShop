using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shoparta.Models;
using Shoparta.Models.DTOs;
using Shoparta.Repositories;

namespace Shoparta.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ItemController : Controller
    {
        private readonly IHomeRepository _homeRepository;

        public ItemController(IHomeRepository homeRepository)
        {
            _homeRepository = homeRepository;
        }

        [HttpGet]
        public async Task<IActionResult> CreateItem()
        {
            ViewBag.Categories = new SelectList(await _homeRepository.Categories(), "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem(CreateEditItemDTO createEditItemDto)
        {
            if (ModelState.IsValid)
            {
                var newItem = new Item
                {
                    Name = createEditItemDto.Name,
                    Image = createEditItemDto.Image,
                    Description = createEditItemDto.Description,
                    Price = createEditItemDto.Price,
                    CategoryId = createEditItemDto.CategoryId
                };

                await _homeRepository.CreateItem(newItem);
                return RedirectToAction("Index", "Home");
            }

            return View(createEditItemDto);
        }


        [ HttpGet]
        public async Task<IActionResult> EditItem(int id)
        {
         var item = await _homeRepository.GetItemById(id);
         if (item == null)
         {
             return NotFound();
         }
         var model = new CreateEditItemDTO
         {
             Id = item.Id,
             Name = item.Name,
             Image = item.Image,
             Description = item.Description,
             Price = item.Price,
             CategoryId = item.CategoryId,
         };
         ViewBag.Categories = await _homeRepository.Categories();
         return View(model); 
        }
        
        [HttpPost]
        public async Task<IActionResult> EditItem(CreateEditItemDTO model)
        {
            if (ModelState.IsValid)
            {
                var itemToUpdate = await _homeRepository.GetItemById(model.Id);
                if (itemToUpdate == null)
                {
                    return NotFound();
                }
                itemToUpdate.Name = model.Name;
                itemToUpdate.Image = model.Image;
                itemToUpdate.Description = model.Description;
                itemToUpdate.Price = model.Price;
                itemToUpdate.CategoryId = model.CategoryId;

                await _homeRepository.UpdateItem(itemToUpdate);
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Categories = new SelectList(await _homeRepository.Categories(), "Id", "Name", model.CategoryId);
            return View(model);
        }

       
        [HttpPost]
        public async Task<IActionResult> RemoveItem(int id)
        {
            try
            {
                bool result = await _homeRepository.RemoveItem(id);
                if (result)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("Nie można usunąć produktu. Być może jest powiązany z istniejącymi zamówieniami.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Błąd podczas usuwania produktu: {ex.Message}");
            }
        }
    }
}
