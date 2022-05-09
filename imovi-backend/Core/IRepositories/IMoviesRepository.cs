using imovi_backend.Data;
using imovi_backend.Models;
using System;
using System.Threading.Tasks;

namespace imovi_backend.Core.IRepositories
{
    public interface IMoviesRepository : IGenericRepository<FavouriteMovie>
    {
        Task<FavouriteMovie> AddToFavourites(Guid userId, FavMovieDTO movie);
        Task<bool> RemoveFromFavourites(Guid userId, string movieId);
        Task<bool> IsMovieInFavourites(Guid userId, string movieId);
    }
}
