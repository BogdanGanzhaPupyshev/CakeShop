using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CakeStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using CakeStore.Data;

namespace CakeStore.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ReviewController(ApplicationContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminIndex()
        {
            var review = await _context.Review.Include(r => r.Cake).Include(r => r.User).ToListAsync();
            return View(review);
        }

       
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            if (isAdmin)
            {
                var allReview = _context.Review.Include(r => r.Cake).Include(r => r.User).ToList();
                return View(allReview);
            }
            else
            {
                var review = _context.Review.Include(r => r.Cake).Include(r => r.User)
                    .Where(r => r.User == user).ToList();
                return View(review);
            }
        }

       
        [AllowAnonymous]
        public async Task<IActionResult> ListAll()
        {
            var review = await _context.Review.Include(r => r.Cake).Include(r => r.User).ToListAsync();
            return View(review);
        }

        private async Task<List<Review>> SortReview(string sortBy, bool isDescending)
        {
            var reviewsList = _context.Review.Include(r => r.Cake).Include(r => r.User);
            IQueryable<Review> result;

            if (sortBy == null || sortBy == "")
            {
                result = reviewsList;
            }

            if (isDescending == false)
            {
                switch (sortBy.ToLower())
                {
                    case "date":
                        result = reviewsList.OrderBy(x => x.Date);
                        break;
                    case "grade":
                        result = reviewsList.OrderBy(x => x.Grade);
                        break;
                    case "title":
                        result = reviewsList.OrderBy(x => x.Title);
                        break;
                    case "cake name":
                        result = reviewsList.OrderBy(x => x.Cake.Name);
                        break;
                    default:
                        result = reviewsList.OrderBy(x => x.Cake.Id);
                        break;
                }
            }
            else
            {
                switch (sortBy.ToLower())
                {
                    case "date":
                        result = reviewsList.OrderByDescending(x => x.Date);
                        break;
                    case "grade":
                        result = reviewsList.OrderByDescending(x => x.Grade);
                        break;
                    case "title":
                        result = reviewsList.OrderByDescending(x => x.Title);
                        break;
                    case "cake name":
                        result = reviewsList.OrderByDescending(x => x.Cake.Name);
                        break;
                    default:
                        result = reviewsList.OrderByDescending(x => x.Cake.Id);
                        break;
                }
            }

            
            return await result.ToListAsync();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> AjaxListReview(string sortBy, bool isDescending)
        {
            var listOfReviews = await SortReview(sortBy, isDescending);

            return PartialView(listOfReviews);
        }

        
        [AllowAnonymous]
        public async Task<IActionResult> CakeReview(int? cakeId)
        {
            if (cakeId == null)
            {
                return NotFound();
            }
            var cake = _context.Cakes.FirstOrDefault(x => x.Id == cakeId);
            if (cake == null)
            {
                return NotFound();
            }
            var review = await _context.Review.Include(r => r.Cake).Include(r => r.User).Where(x => x.Cake.Id == cake.Id).ToListAsync();
            if (review == null)
            {
                return NotFound();
            }
            ViewBag.CakeName = cake.Name;
            ViewBag.CakeId = cake.Id;

            return View(review);
        }

        
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Review
                .Include(r => r.Cake)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        
        public IActionResult CreateWithCake(int? cakeId)
        {
            var review = new Review();

            if (cakeId == null)
            {
                return NotFound();
            }

            var cake = _context.Cakes.FirstOrDefault(c => c.Id == cakeId);

            if (cake == null)
            {
                return NotFound();
            }

            review.Cake = cake;
            review.CakeId = cake.Id;
            ViewData["CakeId"] = new SelectList(_context.Cakes.Where(c => c.Id == cakeId), "Id", "Name");
            var listOfNumbers = new List<int>() { 1, 2, 3, 4, 5 };
            var listOfGrades = listOfNumbers.Select(x => new { Id = x, Value = x.ToString() });
            ViewData["Grade"] = new SelectList(listOfGrades, "Id", "Value");

            return View(review);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateWithCake(int cakeId, Review review)
        {
            if (cakeId != review.CakeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(HttpContext.User);
                review.UserId = userId;
                review.Date = DateTime.Now;

                _context.Add(review);
                await _context.SaveChangesAsync();
                return Redirect($"XCakeReview?cakeId={cakeId}");
            }
            var listOfNumbers = new List<int>() { 1, 2, 3, 4, 5 };
            var listOfGrades = listOfNumbers.Select(x => new { Id = x, Value = x.ToString() });
            ViewData["Grade"] = new SelectList(listOfGrades, "Id", "Value", review.Grade);
            ViewData["CakeId"] = new SelectList(_context.Cakes.Where(p => p.Id == cakeId), "Id", "Name", review.CakeId);
            return View(review);
        }

        
        public IActionResult Create()
        {
            var listOfNumbers = new List<int>() { 1, 2, 3, 4, 5 };
            var listOfGrades = listOfNumbers.Select(x => new { Id = x, Value = x.ToString() });
            ViewData["Grade"] = new SelectList(listOfGrades, "Id", "Value");
            ViewData["CakeId"] = new SelectList(_context.Cakes, "Id", "Name");
            return View();
        }

        
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Grade,CakeId")] Review review)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(HttpContext.User);
                review.UserId = userId;

                review.Date = DateTime.Now;
                _context.Add(review);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            var listOfNumbers = new List<int>() { 1, 2, 3, 4, 5 };
            var listOfGrades = listOfNumbers.Select(x => new { Id = x, Value = x.ToString() });
            ViewData["Grade"] = new SelectList(listOfGrades, "Id", "Value", review.Grade);
            ViewData["CakeId"] = new SelectList(_context.Cakes, "Id", "Name", review.CakeId);

            return View(review);
        }

        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reviews = await _context.Review.SingleOrDefaultAsync(m => m.Id == id);
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userRoles = await _userManager.GetRolesAsync(user);
            bool isAdmin = userRoles.Any(r => r == "Admin");

            if (reviews == null)
            {
                return NotFound();
            }

            if (isAdmin == false)
            {
                var userId = _userManager.GetUserId(HttpContext.User);
                if (reviews.UserId != userId)
                {
                    return BadRequest("You do not have permissions to edit this review.");
                }
            }
            var listOfNumbers = new List<int>() { 1, 2, 3, 4, 5 };
            var listOfGrades = listOfNumbers.Select(x => new { Id = x, Value = x.ToString() });
            ViewData["Grade"] = new SelectList(listOfGrades, "Id", "Value", reviews.Grade);
            ViewData["CakeId"] = new SelectList(_context.Cakes, "Id", "Name", reviews.CakeId);
            return View(reviews);
        }

       
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Grade,Date,CakeId")] Review review)
        {
            if (id != review.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(HttpContext.User);
                try
                {
                    if (review.Date == null)
                    {
                        review.Date = DateTime.Now;
                    }
                    review.UserId = userId;

                    _context.Update(review);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewsExists(review.Id))
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
            var listOfNumbers = new List<int>() { 1, 2, 3, 4, 5 };
            var listOfGrades = listOfNumbers.Select(x => new { Id = x, Value = x.ToString() });
            ViewData["Grade"] = new SelectList(listOfGrades, "Id", "Value", review.Grade);
            ViewData["CakeId"] = new SelectList(_context.Cakes, "Id", "Name", review.CakeId);
            return View(review);
        }

        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Review
                .Include(r => r.Cake)
                .SingleOrDefaultAsync(m => m.Id == id);
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userRoles = await _userManager.GetRolesAsync(user);
            bool isAdmin = userRoles.Any(r => r == "Admin");

            if (review == null)
            {
                return NotFound();
            }

            if (isAdmin == false)
            {
                var userId = _userManager.GetUserId(HttpContext.User);
                if (review.UserId != userId)
                {
                    return BadRequest("You do not have permissions to edit this review.");
                }
            }

            return View(review);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reviews = await _context.Review.SingleOrDefaultAsync(m => m.Id == id);
            _context.Review.Remove(reviews);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool ReviewsExists(int id)
        {
            return _context.Review.Any(e => e.Id == id);
        }

    }
}
