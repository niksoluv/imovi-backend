using imovi_backend.Core.IRepositories;
using imovi_backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace imovi_backend.Core.Repositories
{
    public class CustomListsRepository : GenericRepository<CustomList>, ICustomListsRepository
    {
        public CustomListsRepository(
            ApplicationContext context,
            ILogger logger) : base(context, logger)
        {

        }

        public override Task<IEnumerable<CustomList>> All(Guid userId)
        {
            try
            {
                return base.All(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} All method error", typeof(CustomListsRepository));
                return null;
            }
        }

        public async Task<object> ListsWithMovies(Guid userId)
        {
            try
            {
                var lists = dbSet.Where(cl => cl.UserId == userId)
                    .Include(el=>el.RelatedMovies)
                    .ThenInclude(e=>e.Movie);
                List<CustomListMovie> listMovies = new List<CustomListMovie>();

                //for (int i = 0; i < lists.Count; ++i)
                //{
                //    var movies = await _context.CustomListsMovies.Where(clm => clm.CustomListId == lists[i].Id).ToListAsync();
                //    listMovies.AddRange(movies);
                //}

                var movies = await _context.CustomListsMovies
                    .Include(el=>el.Movie)
                    .ToListAsync();

                //var customLists = await dbSet.Join(_context.CustomListsMovies,
                //    cl => cl.Id,
                //    clm => clm.CustomListId,
                //    (cl, clm) =>
                //    new
                //    {
                //        ListName = cl.ListName,
                //        Movie = clm.Movie,
                //    }).ToListAsync();

                return lists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} All method error", typeof(CustomListsRepository));
                return null;
            }
        }

        public async Task<CustomListMovie> AddToList(CustomListMovie customListMovie)
        {
            try
            {
                var existingMovie = await _context.Movies.Where(movie => movie.MovieId == customListMovie.Movie.MovieId)
                    .FirstOrDefaultAsync();

                if (existingMovie == null)
                    await _context.Movies.AddAsync(customListMovie.Movie);

                await _context.CustomListsMovies.AddAsync(customListMovie);

                return customListMovie;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} AddToList method error", typeof(CustomListsRepository));
                return null;
            }
        }

        public async Task<CustomListMovie> RemoveFromList(CustomListMovie customListMovie)
        {
            try
            {
                _context.CustomListsMovies.Remove(customListMovie);

                return customListMovie;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} RemoveFromList method error", typeof(CustomListsRepository));
                return null;
            }
        }
    }
}
