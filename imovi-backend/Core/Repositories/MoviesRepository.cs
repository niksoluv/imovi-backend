using imovi_backend.Core.IRepositories;
using imovi_backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace imovi_backend.Core.Repositories
{
    public class MoviesRepository : GenericRepository<FavouriteMovie>, IMoviesRepository
    {
        public MoviesRepository(
            ApplicationContext context,
            ILogger logger) : base(context, logger)
        {

        }

        public override async Task<IEnumerable<FavouriteMovie>> All(Guid userId)
        {
            var movies = await dbSet.Where(fm => fm.UserId == userId).ToListAsync();
            movies.Reverse();
            return movies;
        }

        public async Task<FavouriteMovie> AddToFavourites(Guid userId, FavouriteMovie favouriteMovie)
        {
            try
            {
                FavouriteMovie existingMovie = await dbSet.
                    Where(fm => fm.UserId == userId && fm.MovieId == favouriteMovie.MovieId).FirstOrDefaultAsync();
                if (existingMovie != null)
                {
                    return existingMovie;
                }
                FavouriteMovie movie = new FavouriteMovie()
                { UserId = userId, MovieId = favouriteMovie.MovieId, MediaType = favouriteMovie.MediaType };
                await _context.FavoriteMovies.AddAsync(movie);
                return movie;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} AddToFavourites method error", typeof(MoviesRepository));
                return null;
            }
        }

        public async Task<bool> RemoveFromFavourites(Guid userId, string movieId)
        {
            try
            {
                FavouriteMovie movie = await dbSet.Where(fm => fm.UserId == userId && fm.MovieId == movieId).FirstOrDefaultAsync();
                dbSet.Remove(movie);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} RemoveFromFavourites method error", typeof(MoviesRepository));
                return false;
            }
        }

        public async Task<bool> IsMovieInFavourites(Guid userId, string movieId)
        {
            try
            {
                var movie = await dbSet.
                    Where(fm => fm.UserId == userId && fm.MovieId == movieId).
                    FirstOrDefaultAsync();
                await dbSet.AddAsync(movie);
                if (movie != null)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} IsMovieInFavourites method error", typeof(MoviesRepository));
                return false;
            }
        }
    }
}
