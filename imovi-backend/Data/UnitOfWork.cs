using imovi_backend.Core.IConfiguration;
using imovi_backend.Core.IRepositories;
using imovi_backend.Core.Repositories;
using imovi_backend.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace imovi_backend
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ApplicationContext _context;
        private readonly ILogger _logger;

        public IUserRepository Users { get; private set; }
        public IMoviesRepository Movies { get; private set; }
        public IMovieHistoryRepository UserHistory { get; private set; }
        public ICommentsRepository Comments { get; private set; }

        public UnitOfWork(
            ApplicationContext context, 
            ILoggerFactory loggerFactory)
        {
            _context = context;
            _logger = loggerFactory.CreateLogger("logs");

            Users = new UserRepository(_context, _logger);
            Movies = new MoviesRepository(_context, _logger);
            UserHistory = new MovieHistoryRepository(_context, _logger);
            Comments = new CommentsRepository(_context, _logger);
        }

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        //public async Task Dispose()
        //{
        //    await _context.DisposeAsync();
        //}
    }
}
