using imovi_backend.Models;
using System;
using System.Threading.Tasks;

namespace imovi_backend.Core.IRepositories
{
    public interface IMovieHistoryRepository : IGenericRepository<UserMovieHistory>
    {
        Task<bool> AddToHistory(Guid userId, Movie movie);
    }
}
