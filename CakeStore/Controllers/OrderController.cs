using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CakeStore.Repositories;
using CakeStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using CakeStore.Data;



namespace CakeStore.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderRepo _orderRepo;
        private readonly ShopCart _shopCart;
        private readonly ApplicationContext _context;
        private readonly UserManager<IdentityUser> _userManager;
       

        public OrderController(IOrderRepo orderRepo,
            ShopCart shopCart, ApplicationContext context, UserManager<IdentityUser> userManager)
        {
            _orderRepo = orderRepo;
            _shopCart = shopCart;
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        public IActionResult Checkout()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Checkout(Order order)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            order.UserId = userId;

            var items = await _shopCart.GetShopCartItemsAsync();
            _shopCart.ShopCartItems = items;

            if (_shopCart.ShopCartItems.Count == 0)
            {
                ModelState.AddModelError("", "Your cart is empty, add some cake");
            }

            if (ModelState.IsValid)
            {

                await _orderRepo.CreateOrderAsync(order);
                await _shopCart.ClearCartAsync();

                return RedirectToAction("CheckoutComplete");
            }

            return View(order);
        }

        [Authorize]
        public IActionResult CheckoutComplete()
        {
            ViewBag.CheckoutCompleteMessage = $"Thanks for your order.";
            return View();
        }

        // GET: Reviews
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            if (isAdmin)
            {
                var allOrders = await _context.Orders.Include(o => o.OrderInf).Include(o => o.User).ToListAsync();
                return View(allOrders);
            }
            else
            {
                var orders = await _context.Orders.Include(o => o.OrderInf).Include(o => o.User)
                    .Where(r => r.User == user).ToListAsync();
                return View(orders);
            }
        }

        public IActionResult SendEmail()
        {
            return RedirectToAction("Checkout");
        }
       
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var orders = await _context.Orders.Include(o => o.OrderInf).Include(o => o.User)
                .SingleOrDefaultAsync(m => m.OrderId == id);
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userRoles = await _userManager.GetRolesAsync(user);
            bool isAdmin = userRoles.Any(r => r == "Admin");

            if (orders == null)
            {
                return NotFound();
            }

            if (isAdmin == false)
            {
                var userId = _userManager.GetUserId(HttpContext.User);
                if (orders.UserId != userId)
                {
                    return BadRequest("You do not have permissions to view this order.");
                }
            }

            var orderDetailsList = _context.OrderInfo.Include(o => o.Cake).Include(o => o.Order)
                .Where(x => x.OrderId == orders.OrderId);

            ViewBag.OrderDetailsList = orderDetailsList;

            return View(orders);
        }

        
        public IActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(IFormCollection collection)
        {
            try
            {
                

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

       
        public IActionResult Edit(int id)
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.Include(o => o.User)
                .SingleOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

       
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.SingleOrDefaultAsync(m => m.OrderId == id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

    }
}
