using System;
using System.Collections.Generic;
using System.Linq;
using CakeStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace CakeStore.Data
{
    public class CakeDBInitializer
    {
        public static void Initialize(ApplicationContext context, IServiceProvider service)
        {

            context.Database.EnsureCreated();
                var roleManager = service.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = service.GetRequiredService<UserManager<IdentityUser>>();

            if (context.Cakes.Any())
            {
                return;
            }
            CreateAdminRole(context, roleManager, userManager);
                SeedDatabase(context, roleManager, userManager);
            
        }
        
        public static void CreateAdminRole(ApplicationContext context,RoleManager<IdentityRole> _roleManager, UserManager<IdentityUser> _userManager)
        {
            //string adminEmail = "admin@default.com";
            //string password = "Admin1234567";
            //if (_roleManager.FindByNameAsync("admin")==null)
            //{
            //    _roleManager.CreateAsync(new IdentityRole("admin"));
            //}
            //if (_userManager.FindByEmailAsync(adminEmail)==null)
            //{
            //    IdentityUser admin = new IdentityUser { Email = adminEmail, UserName = adminEmail };
            //    IdentityResult result = _userManager.CreateAsync(admin, password);

            //}


            bool roleExists = _roleManager.RoleExistsAsync("Admin").Result;
            if (roleExists)
            {
                return;
            }

            var role = new IdentityRole()
            {
                Name = "Admin"
            };
            _roleManager.CreateAsync(role).Wait();

            var user = new IdentityUser()
            {
                UserName = "admin",
                Email = "admin@default.com"
            };

            string adminPassword = "Password123";
            var userResult =  _userManager.CreateAsync(user, adminPassword).Result;

            if (userResult.Succeeded)
            {
                _userManager.AddToRoleAsync(user, "Admin").Wait();
            }


        }
        public static void SeedDatabase(ApplicationContext _context, RoleManager<IdentityRole> _roleManager, UserManager<IdentityUser> _userManager)
        {
            var catig = new Categories { Name = "Chokolate", Description = "Standart lovely cakes" };
            var catig2 = new Categories { Name = "New", Description = "New cakes" };
            
            var catigs = new List<Categories>()
            {catig,catig2};
            var cake = new Cakes { Name = "Vanilla", Price = 70.00M, Category = catig2, Description = "Cake with Vanilla", ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRz1z8Mcov1gqzZ1Ff4eDOqRBUlmJ6dhH4_A&us1p=CAU", CakeOfTheWeek = true };
            var cake2 = new Cakes { Name = "Brawnie", Price = 80.00M, Category = catig, Description = "Cake with chocolate filling", ImageUrl = "https://cheese-cake.ru/DesertThump/brauni-rockslide-260x260.jpj", CakeOfTheWeek = true };
           
            var caks = new List<Cakes>()
           {
                cake,cake2
           };
            _context.Categories.AddRange(catigs);
            _context.Cakes.AddRange(caks);
            _context.SaveChanges();
        }
        
    }
       
}
