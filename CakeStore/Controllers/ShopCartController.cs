using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CakeStore.Repositories;
using CakeStore.Models;
using CakeStore.Viewcs;
using CakeStore.Data;

namespace CakeStore.Controllers
{
    public class ShopCartController : Controller
    {
        private readonly ICakeRepo _cakeRepo;
        private readonly ApplicationContext _context;
        private readonly ShopCart _shopCart;

        public ShopCartController(ICakeRepo cakeRepo,
            ShopCart shopCart, ApplicationContext context)
        {
            _cakeRepo = cakeRepo;
            _shopCart = shopCart;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _shopCart.GetShopCartItemsAsync();
            _shopCart.ShopCartItems = items;

            var shopCartView = new ShopCartView
            {
                ShopCart = _shopCart,
                ShopCartTotal = _shopCart.GetShopCartTotal()
            };

            return View(shopCartView);
        }

        public async Task<IActionResult> AddToShopCart(int cakeId)
        {
            var selectedCake = await _cakeRepo.GetByIdAsync(cakeId);

            if (selectedCake != null)
            {
                await _shopCart.AddToCartAsync(selectedCake, 1);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> RemoveFromShopCart(int cakeId)
        {
            var selectedCake = await _cakeRepo.GetByIdAsync(cakeId);

            if (selectedCake != null)
            {
                await _shopCart.RemoveFromCartAsync(selectedCake);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ClearCart()
        {
            await _shopCart.ClearCartAsync();

            return RedirectToAction("Index");
        }

    }
}
