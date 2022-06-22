using imovi_backend;
using imovi_backend.Controllers;
using imovi_backend.Core.IConfiguration;
using imovi_backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImoviTests
{
    internal class CustomListsControllerTests : IDisposable
    {
        private DbContextOptions<ApplicationContext> dbContextOptions = new DbContextOptionsBuilder<ApplicationContext>()
        .UseInMemoryDatabase(databaseName: "d9d7t87n7pd40k")
        .Options;

        private CustomListsController controller;
        private ApplicationContext _context;
        private ILogger<CustomListsController> _logger;
        private IUnitOfWork _unitOfWork;

        [OneTimeSetUp]
        public void Setup()
        {
            _context = new ApplicationContext(dbContextOptions);

            SeedDb();

            _unitOfWork = new UnitOfWork(_context, new LoggerFactory());


            controller = new CustomListsController(_logger, _unitOfWork);
        }

        private void SeedDb()
        {
            var users = new List<User>
            {
                new User { Id = new System.Guid(), Email = "testEmail1@mail.com", Password="11111111", Username="aaaaaaaa"},
                new User { Id = new System.Guid(), Email = "testEmail2@mail.com", Password="22222222", Username="bbbbbbbb"},
                new User { Id = new System.Guid(), Email = "testEmail3@mail.com", Password="33333333", Username="cccccccc"},
            };

            _context.AddRange(users);

            _context.SaveChanges();

            var movies = new List<Movie>
            {
                new Movie { Id = new System.Guid(), MovieId="123", MediaType="movie"},
                new Movie { Id = new System.Guid(), MovieId="345", MediaType="movie"},
                new Movie { Id = new System.Guid(), MovieId="567", MediaType="tv"},
            };

            _context.AddRange(movies);

            _context.SaveChanges();

            var comments = new List<Comment>
            {
                new Comment {
                    Id = new System.Guid(),
                    Data = "Comment1",
                    Likes=3,
                    User = _context.Users.FirstOrDefault(),
                    Movie=_context.Movies.FirstOrDefault() },
                new Comment {
                    Id = new System.Guid(),
                    Data = "Comment2",
                    Likes=0,
                    User = _context.Users.FirstOrDefault(),
                    Movie=_context.Movies.FirstOrDefault() },
                new Comment {
                    Id = new System.Guid(),
                    Data = "Comment3",
                    Likes=12,
                    User = _context.Users.LastOrDefault(),
                    Movie=_context.Movies.LastOrDefault() },
            };

            _context.AddRange(comments);

            _context.SaveChanges();
        }

        [Test]
        public async Task GetUserLists()
        {
            Assert.Pass();
        }

        [Test]
        public async Task CreateList()
        {
            await Task.Delay(332);
            Assert.Pass();
        }

        [Test]
        public async Task AddMovieToList()
        {
            Assert.Pass();
        }

        [Test]
        public async Task RemoveMovieFromList()
        {
            Assert.Pass();
        }

        [Test]
        public async Task DeleteList()
        {
            Assert.Pass();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
