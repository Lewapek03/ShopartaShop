using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shoparta.Models;

namespace Shoparta.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
            public DbSet<Category> Categories {  get; set; }
            public DbSet<ShoppingCart> ShoppingCarts {  get; set; }
            public DbSet<CardDetail> CardDetails { get; set; }
            public DbSet<Item> Items {  get; set; }
            public DbSet<Order> Orders {  get; set; }
            public DbSet<OrderDetail> OrderDetails {  get; set; }
            public DbSet<OrderStatus> OrderStatuses {  get; set; }
            public DbSet<CardDetail> CardDetail {  get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);
                
                modelBuilder.Entity<OrderStatus>().HasData(
                    new OrderStatus { Id = 1, Name = "Pending" },
                    new OrderStatus { Id = 2, Name = "Processing" },
                    new OrderStatus { Id = 3, Name = "Shipped" },
                    new OrderStatus { Id = 4, Name = "Delivered" },
                    new OrderStatus { Id = 5, Name = "Cancelled" }
                );
        }
    }
}