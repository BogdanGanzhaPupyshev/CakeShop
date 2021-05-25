using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CakeStore.Models;

namespace CakeStore.Repositories
{
    public interface ICakeINGRepo
    {
        IEnumerable<CakeIngridients> CakeIngridients { get; }

        CakeIngridients GetById(int? id);
        Task<CakeIngridients> GetByIdAsync(int? id);

        bool Exists(int id);

        IEnumerable<CakeIngridients> GetAll();
        Task<IEnumerable<CakeIngridients>> GetAllAsync();

        void Add(CakeIngridients cakeIngridient);
        void Update(CakeIngridients cakeIngridient);
        void Remove(CakeIngridients cakeIngridient);

        void SaveChanges();
        Task SaveChangesAsync();
    }
}
