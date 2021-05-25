using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CakeStore.Data;
using CakeStore.Models;


namespace CakeStore.Repositories
{
    public class CakeRepo:ICakeRepo
    {
        private readonly ApplicationContext _context;

        public CakeRepo(ApplicationContext context)
        {
            _context = context;
        }

        public IEnumerable<Cakes> Cakes => _context.Cakes.Include(p => p.Category).Include(p => p.Review).Include(p => p.CakeIngridients); //include here

        public IEnumerable<Cakes> CakeOfTheWeek => _context.Cakes.Where(p => p.CakeOfTheWeek).Include(p => p.Category);

        public void Add(Cakes cake)
        {
            _context.Add(cake);
        }

        public IEnumerable<Cakes> GetAll()
        {
            return _context.Cakes.ToList();
        }

        public async Task<IEnumerable<Cakes>> GetAllAsync()
        {
            return await _context.Cakes.ToListAsync();
        }

        public async Task<IEnumerable<Cakes>> GetAllIncludedAsync()
        {
            return await _context.Cakes.Include(p => p.Category).Include(p => p.Review).Include(p => p.CakeIngridients).ToListAsync();
        }

        public IEnumerable<Cakes> GetAllIncluded()
        {
            return _context.Cakes.Include(p => p.Category).Include(p => p.Review).Include(p => p.CakeIngridients).ToList();
        }

        public Cakes GetById(int? id)
        {
            return _context.Cakes.FirstOrDefault(p => p.Id == id);
        }

        public async Task<Cakes> GetByIdAsync(int? id)
        {
            return await _context.Cakes.FirstOrDefaultAsync(p => p.Id == id);
        }

        public Cakes GetByIdIncluded(int? id)
        {
            return _context.Cakes.Include(p => p.Category).Include(p => p.Review).Include(p => p.CakeIngridients).FirstOrDefault(p => p.Id == id);
        }

        public async Task<Cakes> GetByIdIncludedAsync(int? id)
        {
            return await _context.Cakes.Include(p => p.Category).Include(p => p.Review).Include(p => p.CakeIngridients).FirstOrDefaultAsync(p => p.Id == id);
        }

        public bool Exists(int id)
        {
            return _context.Cakes.Any(p => p.Id == id);
        }

        public void Remove(Cakes cake)
        {
            _context.Remove(cake);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public void Update(Cakes cake)
        {
            _context.Update(cake);
        }

    }
}
