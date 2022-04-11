using imovi_backend.Core.Repositories;
using imovi_backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace imovi_backend.Core.IRepositories
{
    public class MovieHistoryRepository : GenericRepository<UserMovieHistory>, IMovieHistoryRepository
    {
        public MovieHistoryRepository(
            ApplicationContext context,
            ILogger logger) : base(context, logger)
        {

        }
        public override async Task<IEnumerable<UserMovieHistory>> All(Guid userId)
        {
            var history = await dbSet.Where(fm => fm.UserId == userId).Include(fm => fm.Movie).ToListAsync();
            history.Reverse();
            return history;
        }

        public async Task<bool> AddToHistory(Guid userId, Movie movie)
        {
            try
            {
                var existingMovie = await _context.Movies.Where(m => m.MovieId == movie.MovieId).FirstOrDefaultAsync();
                if (existingMovie == null)
                    await _context.Movies.AddAsync(movie);

                var movieHistory = await dbSet.Where(mh => mh.UserId == userId && mh.Movie.MovieId == movie.MovieId).FirstOrDefaultAsync();
                if (movieHistory == null)
                {
                    movieHistory = new UserMovieHistory { Movie = movie, UserId = userId };
                    await dbSet.AddAsync(movieHistory);
                }
                else
                {
                    dbSet.Update(movieHistory);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} AddToHistory method error", typeof(MovieHistoryRepository));
                return false;
            }
        }
    }
}
