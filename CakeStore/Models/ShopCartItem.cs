using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CakeStore.Models
{
    public class ShopCartItem
    {
        public int ShopCartItemId { get; set; }
        public Cakes Cake { get; set; }
        public int Amount { get; set; }
        public string ShopCartId { get; set; }
    }
}
