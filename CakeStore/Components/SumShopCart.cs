using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CakeStore.Models;
using CakeStore.Viewcs;
namespace CakeStore.Components
{
    public class SumShopCart : ViewComponent
    {
        private readonly ShopCart _shopCart;
        public SumShopCart(ShopCart shopCart)
        {
            _shopCart = shopCart;
        }

        public async Task<IViewComponentResult> InvokeAsync()
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
    }
}