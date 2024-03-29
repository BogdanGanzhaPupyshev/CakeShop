﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CakeStore.Models;

namespace CakeStore.Repositories
{
    public interface IIngredientsRepo
    {
        IEnumerable<Ingredients> Ingredients { get; }

        Ingredients GetById(int? id);
        Task<Ingredients> GetByIdAsync(int? id);

        bool Exists(int id);

        IEnumerable<Ingredients> GetAll();
        Task<IEnumerable<Ingredients>> GetAllAsync();

        void Add(Ingredients ingredient);
        void Update(Ingredients ingredient);
        void Remove(Ingredients ingredient);

        void SaveChanges();
        Task SaveChangesAsync();
    }
}
