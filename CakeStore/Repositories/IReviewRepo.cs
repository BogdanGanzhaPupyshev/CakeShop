using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CakeStore.Models;

namespace CakeStore.Repositories
{
    public interface IReviewRepo
    {
        IEnumerable<Review> Review { get; }

        Review GetById(int? id);
        Task<Review> GetByIdAsync(int? id);

        bool Exists(int id);

        IEnumerable<Review> GetAll();
        Task<IEnumerable<Review>> GetAllAsync();

        void Add(Review review);
        void Update(Review review);
        void Remove(Review review);

        void SaveChanges();
        Task SaveChangesAsync();
    }
}
