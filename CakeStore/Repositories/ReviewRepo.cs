using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CakeStore.Models;
using Microsoft.EntityFrameworkCore;
using CakeStore.Data;

namespace CakeStore.Repositories
{
    public class ReviewRepo: IReviewRepo
    {
        private readonly ApplicationContext _context;

        public ReviewRepo(ApplicationContext context)
        {
            _context = context;
        }

        public IEnumerable<Review> Review => _context.Review.Include(x => x.Cake); //include here

        public void Add(Review review)
        {
            _context.Review.Add(review);
        }

        public IEnumerable<Review> GetAll()
        {
            return _context.Review.ToList();
        }

        public async Task<IEnumerable<Review>> GetAllAsync()
        {
            return await _context.Review.ToListAsync();
        }

        public Review GetById(int? id)
        {
            return _context.Review.FirstOrDefault(p => p.Id == id);
        }

        public async Task<Review> GetByIdAsync(int? id)
        {
            return await _context.Review.FirstOrDefaultAsync(p => p.Id == id);
        }

        public bool Exists(int id)
        {
            return _context.Review.Any(p => p.Id == id);
        }

        public void Remove(Review review)
        {
            _context.Review.Remove(review);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Update(Review review)
        {
            _context.Review.Update(review);
        }
    }
}
