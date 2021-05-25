using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CakeStore.Models;
using Microsoft.AspNetCore.Authorization;
using CakeStore.Data;

namespace CakeStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CakeINGController : Controller
    {
        private readonly ApplicationContext _context;
        
        public CakeINGController(ApplicationContext context)
        {
            _context = context;
        }

        
        public async Task<IActionResult> Index()
        {
            var ApplicationContext = _context.CakeIngredients.Include(c => c.Ingredient).Include(c => c.Cake);
            return View(await ApplicationContext.ToListAsync());
        }

        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cakeIngredients = await _context.CakeIngredients
                .Include(c => c.Ingredient)
                .Include(c => c.Cake)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (cakeIngredients == null)
            {
                return NotFound();
            }

            return View(cakeIngredients);
        }

        
        public IActionResult Create()
        {
            ViewData["IngredientId"] = new SelectList(_context.Ingredients, "Id", "Name");
            ViewData["CakeId"] = new SelectList(_context.Cakes, "Id", "Name");
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CakeId,IngredientId")] CakeIngridients cakeIngredients)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cakeIngredients);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["IngredientId"] = new SelectList(_context.Ingredients, "Id", "Name", cakeIngredients.IngredientId);
            ViewData["CakeId"] = new SelectList(_context.Cakes, "Id", "Name", cakeIngredients.CakeId);
            return View(cakeIngredients);
        }

        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cakeIngredients = await _context.CakeIngredients.SingleOrDefaultAsync(m => m.Id == id);
            if (cakeIngredients == null)
            {
                return NotFound();
            }
            ViewData["IngredientId"] = new SelectList(_context.Ingredients, "Id", "Name", cakeIngredients.IngredientId);
            ViewData["CakeId"] = new SelectList(_context.Cakes, "Id", "Name", cakeIngredients.CakeId);
            return View(cakeIngredients);
        }

       
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CakeId,IngredientId")] CakeIngridients cakeIngredients)
        {
            if (id != cakeIngredients.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cakeIngredients);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CakeIngredientsExists(cakeIngredients.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            ViewData["IngredientId"] = new SelectList(_context.Ingredients, "Id", "Name", cakeIngredients.IngredientId);
            ViewData["CakeId"] = new SelectList(_context.Cakes, "Id", "Name", cakeIngredients.CakeId);
            return View(cakeIngredients);
        }

        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cakeIngredients = await _context.CakeIngredients
                .Include(p => p.Ingredient)
                .Include(p => p.Cake)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (cakeIngredients == null)
            {
                return NotFound();
            }

            return View(cakeIngredients);
        }

       
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cakeIngredients = await _context.CakeIngredients.SingleOrDefaultAsync(m => m.Id == id);
            _context.CakeIngredients.Remove(cakeIngredients);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool CakeIngredientsExists(int id)
        {
            return _context.CakeIngredients.Any(e => e.Id == id);
        }
    }
}
