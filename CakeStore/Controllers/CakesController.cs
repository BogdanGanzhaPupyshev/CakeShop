using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CakeStore.Models;
using CakeStore.Repositories;
using Microsoft.AspNetCore.Authorization;
using CakeStore.Viewcs;
using Newtonsoft.Json;
using CakeStore.Data;

namespace CakeStore.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class CakesController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly ICakeRepo _cakeRepo;
        private readonly ICategoryRepo _categoryRepo;

        public CakesController(ApplicationContext context, ICakeRepo cakeRepo, ICategoryRepo categoryRepo)
        {
            _context = context;
            _cakeRepo = cakeRepo;
            _categoryRepo = categoryRepo;
        }

        // GET: Cakes
        public async Task<IActionResult> Index()
        {
            return View(await _cakeRepo.GetAllIncludedAsync());
        }

        // GET: Cakes
        [AllowAnonymous]
        public async Task<IActionResult> ListAll()
        {
            var model = new SearchCake()
            {
                CakeList = await _cakeRepo.GetAllIncludedAsync(),
                SearchText = null
            };

            return View(model);
        }

        private async Task<List<Cakes>> GetCakesSearchList(string userInput)
        {
            List<Cakes> buff = new List<Cakes>();
            var ck = await _cakeRepo.GetAllAsync();

            foreach (var item in ck)
            {
                buff.Add(item);
            }

            if (!string.IsNullOrEmpty(userInput))
            {
                userInput = userInput.ToLower().Trim();

                var result = _context.Cakes.Include(p => p.Category)
                    .Where(p => p
                        .Name.ToLower().Contains(userInput))
                        .Select(p => p).OrderBy(p => p.Name);
                if (result == null) { return buff; }
                else
                {
                    return await result.ToListAsync();
                }
            }
            else
            {
                return buff;
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> SearchResult(string searchString)
        {
            var cakeList = await GetCakesSearchList(searchString);

            return PartialView(cakeList);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ListAll([Bind("SearchText")] SearchCake model)
        {
            var Cakes = await _cakeRepo.GetAllIncludedAsync();
            if (model.SearchText == null || model.SearchText == string.Empty)
            {
                model.CakeList = Cakes;
                return View(model);
            }

            var input = model.SearchText.Trim();
            if (input == string.Empty || input == null)
            {
                model.CakeList = Cakes;
                return View(model);
            }
            var searchString = input.ToLower();

            if (string.IsNullOrEmpty(searchString))
            {
                model.CakeList = Cakes;
            }
            else
            {
                var cakeList = await _context.Cakes.Include(x => x.Category).Include(x => x.Review).Include(x => x.CakeIngridients).OrderBy(x => x.Name)
                     .Where(p =>
                     p.Name.ToLower().Contains(searchString)
                  || p.Price.ToString("c").ToLower().Contains(searchString)
                  || p.Category.Name.ToLower().Contains(searchString)
                  || p.CakeIngridients.Select(x => x.Ingredient.Name.ToLower()).Contains(searchString))
                    .ToListAsync();

                if (cakeList.Any())
                {
                    model.CakeList = cakeList;
                }
                else
                {
                    model.CakeList = new List<Cakes>();
                }

            }
            return View(model);
        }

        // GET: Cakes
        [AllowAnonymous]
        public async Task<IActionResult> ListCategory(string categoryName)
        {
            bool categoryExtist = _context.Categories.Any(c => c.Name == categoryName);
            if (!categoryExtist)
            {
                return NotFound();
            }

            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == categoryName);

            if (category == null)
            {
                return NotFound();
            }

            bool anyCakes = await _context.Cakes.AnyAsync(x => x.Category == category);
            if (!anyCakes)
            {
                return NotFound($"No Cakes were found in the category: {categoryName}");
            }

            var Cakes = _context.Cakes.Where(x => x.Category == category)
                .Include(x => x.Category).Include(x => x.Review);

            ViewBag.CurrentCategory = category.Name;
            return View(Cakes);
        }

        // GET: Cakes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Cakes = await _cakeRepo.GetByIdIncludedAsync(id);

            if (Cakes == null)
            {
                return NotFound();
            }

            return View(Cakes);
        }

        // GET: Cakes/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> DisplayDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Cakes = await _cakeRepo.GetByIdIncludedAsync(id);

            var listOfIngredients = await _context.CakeIngredients.Where(x => x.CakeId == id).Select(x => x.Ingredient.Name).ToListAsync();
            ViewBag.CakeING = listOfIngredients;

        
            double score;
            if (_context.Review.Any(x => x.CakeId == id))
            {
                var review = _context.Review.Where(x => x.CakeId == id);
                score = review.Average(x => x.Grade);
                score = Math.Round(score, 2);
            }
            else
            {
                score = 0;
            }
            ViewBag.AverageReviewScore = score;

            if (Cakes == null)
            {
                return NotFound();
            }

            return View(Cakes);
        }

        // GET: Cakes
        [AllowAnonymous]
        public async Task<IActionResult> SearchCakes()
        {
            var model = new SearchCake()
            {
                CakeList = await _cakeRepo.GetAllIncludedAsync(),
                SearchText = null
            };

            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchCakes([Bind("SearchText")] SearchCake model)
        {
            var Cakes = await _cakeRepo.GetAllIncludedAsync();
            var search = model.SearchText.ToLower();

            if (string.IsNullOrEmpty(search))
            {
                model.CakeList = Cakes;
            }
            else
            {
                var cakeList = await _context.Cakes.Include(x => x.Category).Include(x => x.Review).Include(x => x.CakeIngridients).OrderBy(x => x.Name)
                    .Where(p =>
                     p.Name.ToLower().Contains(search)
                  || p.Price.ToString("c").ToLower().Contains(search)
                  || p.Category.Name.ToLower().Contains(search)
                  || p.CakeIngridients.Select(x => x.Ingredient.Name.ToLower()).Contains(search)).ToListAsync();

                if (cakeList.Any())
                {
                    model.CakeList = cakeList;
                }
                else
                {
                    model.CakeList = new List<Cakes>();
                }

            }
            return View(model);
        }

        // GET: Cakes/Create
        public IActionResult Create()
        {
            ViewData["CategoriesId"] = new SelectList(_categoryRepo.GetAll(), "Id", "Name");
            return View();
        }

        // POST: Cakes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Description,ImageUrl,CakeOfTheWeek,CategoriesId")] Cakes Cakes)
        {
            if (ModelState.IsValid)
            {
                _cakeRepo.Add(Cakes);
                await _cakeRepo.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["CategoriesId"] = new SelectList(_categoryRepo.GetAll(), "Id", "Name", Cakes.CategoriesId);
            return View(Cakes);
        }

        // GET: Cakes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Cakes = await _cakeRepo.GetByIdAsync(id);

            if (Cakes == null)
            {
                return NotFound();
            }
            ViewData["CategoriesId"] = new SelectList(_categoryRepo.GetAll(), "Id", "Name", Cakes.CategoriesId);
            return View(Cakes);
        }

        // POST: Cakes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Description,ImageUrl,CakeOfTheWeek,CategoriesId")] Cakes Cakes)
        {
            if (id != Cakes.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _cakeRepo.Update(Cakes);
                    await _cakeRepo.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CakesExists(Cakes.Id))
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
            ViewData["CategoriesId"] = new SelectList(_categoryRepo.GetAll(), "Id", "Name", Cakes.CategoriesId);
            return View(Cakes);
        }

        // GET: Cakes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Cakes = await _cakeRepo.GetByIdIncludedAsync(id);

            if (Cakes == null)
            {
                return NotFound();
            }

            return View(Cakes);
        }

        // POST: Cakes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var Cakes = await _cakeRepo.GetByIdAsync(id);
            _cakeRepo.Remove(Cakes);
            await _cakeRepo.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool CakesExists(int id)
        {
            return _cakeRepo.Exists(id);
        }
    }
}
