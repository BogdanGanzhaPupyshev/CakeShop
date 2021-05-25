using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CakeStore.Data;
using CakeStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CakeStore.Components
{
    public class Carousel:ViewComponent
    {
        private readonly ApplicationContext _context;
        public Carousel(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cakes = await _context.Cakes.Where(x => x.CakeOfTheWeek).ToListAsync();
            return View(cakes);
        }
    }
}
