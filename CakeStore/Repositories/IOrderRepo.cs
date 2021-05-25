using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CakeStore.Models;

namespace CakeStore.Repositories
{
   public interface IOrderRepo
    {
        Task CreateOrderAsync(Order order);
    }
}
