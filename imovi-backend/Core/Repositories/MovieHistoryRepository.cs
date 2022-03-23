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
    public class MovieHistoryRepository: GenericRepository<UserMovieHistory>, IMovieHistoryRepository
    {
        public MovieHistoryRepository(
            ApplicationContext context,
            ILogger logger) : base(context, logger)
        {

        }
        public override async Task<IEnumerable<UserMovieHistory>> All(Guid userId)
        {
            var history = await dbSet.Where(fm => fm.UserId == userId).ToListAsync();
            return history;
        }

        public async Task<bool> AddToHistory(Guid userId, Movie movie)
        {
            try
            {
                await _context.Movies.AddAsync(movie);
                var movieHistory = new UserMovieHistory { MovieId = movie.Id, UserId = userId };
                await dbSet.AddAsync(movieHistory);
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
