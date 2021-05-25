using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CakeStore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CakeStore.Models
{
    public class ShopCart
    {
        private readonly ApplicationContext _applicationContext;

        private ShopCart(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public string ShopCartId { get; set; }

        public List<ShopCartItem> ShopCartItems { get; set; }


        public static ShopCart GetCart(IServiceProvider services)
        {
            ISession session = services.GetRequiredService<IHttpContextAccessor>()?
                .HttpContext.Session;

            var context = services.GetService<ApplicationContext>();
            string cartId = session.GetString("CartId") ?? Guid.NewGuid().ToString();

            session.SetString("CartId", cartId);

            return new ShopCart(context) { ShopCartId = cartId };
        }

        public async Task AddToCartAsync(Cakes cake, int amount)
        {
            var shopCartItem =
                    await _applicationContext.ShopCartItems.SingleOrDefaultAsync(
                        s => s.Cake.Id == cake.Id && s.ShopCartId == ShopCartId);

            if (shopCartItem == null)
            {
                shopCartItem = new ShopCartItem
                {
                    ShopCartId = ShopCartId,
                    Cake = cake,
                    Amount = 1
                };

                _applicationContext.ShopCartItems.Add(shopCartItem);
            }
            else
            {
                shopCartItem.Amount++;
            }

            await _applicationContext.SaveChangesAsync();
        }

        public async Task<int> RemoveFromCartAsync(Cakes cake)
        {
            var shopCartItem =
                    await _applicationContext.ShopCartItems.SingleOrDefaultAsync(
                        s => s.Cake.Id == cake.Id && s.ShopCartId == ShopCartId);

            var localAmount = 0;

            if (shopCartItem != null)
            {
                if (shopCartItem.Amount > 1)
                {
                    shopCartItem.Amount--;
                    localAmount = shopCartItem.Amount;
                }
                else
                {
                    _applicationContext.ShopCartItems.Remove(shopCartItem);
                }
            }

            await _applicationContext.SaveChangesAsync();

            return localAmount;
        }

        public async Task<List<ShopCartItem>> GetShopCartItemsAsync()
        {
            return ShopCartItems ??
                   (ShopCartItems = await
                       _applicationContext.ShopCartItems.Where(c => c.ShopCartId == ShopCartId)
                           .Include(s => s.Cake)
                           .ToListAsync());
        }

        public async Task ClearCartAsync()
        {
            var cartItems = _applicationContext
                .ShopCartItems
                .Where(cart => cart.ShopCartId == ShopCartId);

            _applicationContext.ShopCartItems.RemoveRange(cartItems);

            await _applicationContext.SaveChangesAsync();
        }

        public decimal GetShopCartTotal()
        {
            var total = _applicationContext.ShopCartItems.Where(c => c.ShopCartId == ShopCartId)
                .Select(c => c.Cake.Price * c.Amount).Sum();
            return total;
        }
    }
}
