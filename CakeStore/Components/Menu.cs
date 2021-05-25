using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CakeStore.Data;
using CakeStore.Models;

namespace CakeStore.Components
{
    public class Menu:ViewComponent
    {
        private readonly ApplicationContext _context;
        public Menu(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
            return View(categories);
        }
    }
}
