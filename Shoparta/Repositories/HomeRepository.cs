﻿using Microsoft.EntityFrameworkCore;
using Shoparta.Data;
using Shoparta.Models;
using Shoparta.Models.DTOs;

namespace Shoparta.Repositories
{
    public class HomeRepository : IHomeRepository
    {
        private readonly ApplicationDbContext _db;
        public HomeRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IEnumerable<Category>> Categories()
        {
            return await _db.Categories.ToListAsync();
        }

        public async Task CreateCategory(Category category)
        {
            _db.Categories.Add(category);
            await _db.SaveChangesAsync();
        }
        public async Task CreateItem(Item item)
        {
            _db.Items.Add(item);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> RemoveItem(int id)
        {
            try
            {
                
                var item = await _db.Items.FirstOrDefaultAsync(x => x.Id == id);
                if (item == null)
                {
                    throw new Exception("Produkt nie został znaleziony.");
                }
                if (item != null)
                {
                    _db.Items.Remove(item);
                    await _db.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (DbUpdateException ex)
            {
                
                if (ex.InnerException != null && ex.InnerException.Message.Contains("FOREIGN KEY"))
                {
                    
                    throw new Exception("Nie można usunąć produktu, ponieważ jest powiązany z istniejącymi zamówieniami. " + id);
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }

        public async Task<IEnumerable<Item>> GetItems(string seachItem = "", int categoryId = 0)
        {
            seachItem = seachItem.ToLower();
            IEnumerable<Item> items = await (from item in _db.Items
                                             join category in _db.Categories
                                             on item.CategoryId equals category.Id
                                             where string.IsNullOrWhiteSpace(seachItem) || item != null && item.Name.ToLower().Contains(seachItem)
                                             select new Item
                                             {
                                                 Id = item.Id,
                                                 Image = item.Image,
                                                 Name = item.Name,
                                                 Description = item.Description,
                                                 Price = item.Price,
                                                 CategoryId = item.CategoryId,
                                                 CategoryName = category.Name
                                             }).ToListAsync();
            if (categoryId > 0)
            {

                items = items.Where(a => a.CategoryId == categoryId).ToList();
            }
            return items;


        }
       
        public async Task<Item> GetItemById(int id)
        {
            return await _db.Items.FindAsync(id);
        }

        public async Task UpdateItem(Item item)
        {
            _db.Items.Update(item);
            await _db.SaveChangesAsync();
        }



    }
}