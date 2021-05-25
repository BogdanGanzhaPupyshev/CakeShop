using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using CakeStore.Models;

namespace CakeStore.Data
{
    public class ApplicationContext: IdentityDbContext<IdentityUser>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options):base(options)
        {
          
        }
        public DbSet<CakeIngridients> CakeIngredients { get; set; }
        public DbSet<Cakes> Cakes { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<Ingredients> Ingredients { get; set; }
        public DbSet<ShopCart> ShopCart { get; set; }
        public DbSet<ShopCartItem> ShopCartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderInfo> OrderInfo { get; set; }
        public DbSet<Review> Review { get; set; }
    }
}
