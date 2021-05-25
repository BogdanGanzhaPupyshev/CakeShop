using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CakeStore.Models;

namespace CakeStore.Repositories
{
    public interface ICakeRepo
    {
        IEnumerable<Cakes> Cakes { get; }
        IEnumerable<Cakes> CakeOfTheWeek { get; }

        Cakes GetById(int? id);
        Task<Cakes> GetByIdAsync(int? id);

        Cakes GetByIdIncluded(int? id);
        Task<Cakes> GetByIdIncludedAsync(int? id);

        bool Exists(int id);

        IEnumerable<Cakes> GetAll();
        Task<IEnumerable<Cakes>> GetAllAsync();

        IEnumerable<Cakes> GetAllIncluded();
        Task<IEnumerable<Cakes>> GetAllIncludedAsync();

        void Add(Cakes cake);
        void Update(Cakes cake);
        void Remove(Cakes cake);

        void SaveChanges();
        Task SaveChangesAsync();
    }
}
