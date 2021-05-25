using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CakeStore.Models
{
    public class OrderInfo
    {
        public int OrderInfoId { get; set; }
        public int OrderId { get; set; }
        public int CakeId { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
        public virtual Cakes Cake { get; set; }
        public virtual Order Order { get; set; }

    }
}
