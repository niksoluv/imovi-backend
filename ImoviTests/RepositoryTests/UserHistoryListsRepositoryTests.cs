using imovi_backend;
using imovi_backend.Controllers;
using imovi_backend.Core.IConfiguration;
using imovi_backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImoviTests.RepositoryTests
{
    internal class UserHistoryListsRepositoryTests
    {
        private IUnitOfWork _unitOfWork;

        private Mock<ApplicationContext> _context;

        [SetUp]
        public void Setup()
        {
            SeedDb();

            _unitOfWork = new UnitOfWork(_context.Object, new LoggerFactory());
        }

        private void SeedDb()
        {
            var users = new List<User>
            {
                new User { Id = new Guid("614a4912-75ae-4fa6-8a54-4bd740af333f"),
                    Email = "testEmail1@mail.com", Password="11111111", Username="aaaaaaaa"},
                new User { Id = new Guid("fdfdffdf-75ae-4fa6-8a54-4bd740af333f"),
                    Email = "testEmail2@mail.com", Password="22222222", Username="bbbbbbbb"},
                new User { Id = new Guid("ffffffff-75ae-4fa6-8a54-4bd740af333f"),
                    Email = "testEmail3@mail.com", Password="33333333", Username="cccccccc"},
            };

            var movies = new List<Movie>
            {
                new Movie { Id = Guid.NewGuid(), MovieId="123", MediaType="movie"},
                new Movie { Id = Guid.NewGuid(), MovieId="345", MediaType="movie"},
                new Movie { Id = Guid.NewGuid(), MovieId="567", MediaType="tv"},
            };

            var usersHistory = new List<UserMovieHistory>();

            _context = new Mock<ApplicationContext>();

            _context.Setup(c => c.Set<User>()).ReturnsDbSet(users);
            _context.Setup(c => c.Users).ReturnsDbSet(users);
            _context.Setup(c => c.Set<Movie>()).ReturnsDbSet(movies);
            _context.Setup(c => c.Movies).ReturnsDbSet(movies);
            _context.Setup(c => c.UserMovieHistories).ReturnsDbSet(usersHistory);
            _context.Setup(c => c.Set<UserMovieHistory>()).ReturnsDbSet(usersHistory);
        }

        [Test]
        public async Task All()
        {
            //Arrange
            var user = await _context.Object.Set<User>().FirstOrDefaultAsync();

            //Act
            var result = await _unitOfWork.UserHistory.All(user.Id);

            //Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task AddToHistory()
        {
            //Arrange
            var user = await _context.Object.Set<User>().FirstOrDefaultAsync();
            var movie = await _context.Object.Set<Movie>().FirstOrDefaultAsync();

            //Act
            var result = await _unitOfWork.UserHistory.AddToHistory(user.Id, movie);

            //Assert
            Assert.IsTrue(result);
        }
    }
}
