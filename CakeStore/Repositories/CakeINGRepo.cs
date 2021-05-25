using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CakeStore.Models;
using CakeStore.Data;
using Microsoft.EntityFrameworkCore;
namespace CakeStore.Repositories
{
    public class CakeINGRepo :ICakeINGRepo
    {
        private readonly ApplicationContext _context;

        public CakeINGRepo(ApplicationContext context)
        {
            _context = context;
        }

        public IEnumerable<CakeIngridients> CakeIngridients => _context.CakeIngredients.Include(x => x.Cake).Include(x => x.Ingredient); //include here

        public void Add(CakeIngridients cakeIngredient)
        {
            _context.CakeIngredients.Add(cakeIngredient);
        }

        public IEnumerable<CakeIngridients> GetAll()
        {
            return _context.CakeIngredients.ToList();
        }

        public async Task<IEnumerable<CakeIngridients>> GetAllAsync()
        {
            return await _context.CakeIngredients.ToListAsync();
        }

        public CakeIngridients GetById(int? id)
        {
            return _context.CakeIngredients.FirstOrDefault(p => p.Id == id);
        }

        public async Task<CakeIngridients> GetByIdAsync(int? id)
        {
            return await _context.CakeIngredients.FirstOrDefaultAsync(p => p.Id == id);
        }

        public bool Exists(int id)
        {
            return _context.CakeIngredients.Any(p => p.Id == id);
        }

        public void Remove(CakeIngridients cakeIngredient)
        {
            _context.CakeIngredients.Remove(cakeIngredient);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Update(CakeIngridients cakeIngredient)
        {
            _context.CakeIngredients.Update(cakeIngredient);
        }
    }
}
