using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CakeStore.Data;
using CakeStore.Models;

namespace CakeStore.Repositories
{
    public class OrderRepo : IOrderRepo
    {
        private readonly ApplicationContext _context;
        private readonly ShopCart _shopCart;


        public OrderRepo(ApplicationContext context, ShopCart shopCart)
        {
            _context = context;
            _shopCart = shopCart;
        }

        public async Task CreateOrderAsync(Order order)
        {
            order.OrderPlaced = DateTime.Now;
            decimal totalPrice = 0M;

            var shopCartItems = _shopCart.ShopCartItems;

            foreach (var shopCartItem in shopCartItems)
            {
                var orderDetail = new OrderInfo()
                {
                    Amount = shopCartItem.Amount,
                    CakeId = shopCartItem.Cake.Id,
                    Order = order,
                    Price = shopCartItem.Cake.Price,

                };
                totalPrice += orderDetail.Price * orderDetail.Amount;
                _context.OrderInfo.Add(orderDetail);
            }

            order.OrderTotal = totalPrice;
            _context.Orders.Add(order);

            await _context.SaveChangesAsync();
        }
    }
}
