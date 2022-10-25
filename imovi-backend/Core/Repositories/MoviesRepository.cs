using imovi_backend.Core.IRepositories;
using imovi_backend.Data;
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
            try
            {
                var movies = await dbSet.Where(fm => fm.UserId == userId)
                    .Include(fm => fm.Movie)
                    .ToListAsync();
                if (movies == null)
                    return null;
                movies.Reverse();
                return movies;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} All method error", typeof(MoviesRepository));
                return null;
            }
        }

        public async Task<FavouriteMovie> AddToFavourites(Guid userId, FavMovieDTO favouriteMovieDTO)
        {
            try
            {
                var movie = await _context.Movies.Where(m=>m.MovieId== favouriteMovieDTO.MovieId).FirstOrDefaultAsync();

                if(movie==null)
                    _context.Movies.Add(new Movie()
                    {
                        MovieId = favouriteMovieDTO.MovieId,
                        MediaType = favouriteMovieDTO.MediaType,
                    });

                FavouriteMovie newMovie = new FavouriteMovie()
                { UserId = userId, Movie = movie };
                await _context.FavoriteMovies.AddAsync(newMovie);
                return newMovie;
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
                var movie = await _context.Movies.Where(m => m.MovieId == movieId).FirstOrDefaultAsync();
                if (movie == null)
                {
                    return false;
                }
                FavouriteMovie favouriteMovie = await dbSet
                    .Where(fm => fm.UserId == userId && fm.MovieId == movie.Id).FirstOrDefaultAsync();
                
                dbSet.Remove(favouriteMovie);
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
                var movie = await _context.Movies.Where(m=>m.MovieId==movieId).FirstOrDefaultAsync();
                if (movie == null)
                {
                    return false;
                }

                var favouriteMovie = await dbSet.
                    Where(fm => fm.UserId == userId && fm.MovieId == movie.Id).
                    FirstOrDefaultAsync();
                await dbSet.AddAsync(favouriteMovie);
                if (favouriteMovie != null)
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
