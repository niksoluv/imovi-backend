﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace imovi_backend.Core.IRepositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> All(Guid userId);
        Task<T> GetById(Guid id);
        bool Add(T entity);
        Task<bool> Upsert(T entity);
        Task<bool> Delete(Guid id);
    }
}
