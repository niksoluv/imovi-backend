using FluentAssertions;
using imovi_backend;
using imovi_backend.Controllers;
using imovi_backend.Core.IConfiguration;
using imovi_backend.Data;
using imovi_backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using Moq.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace ImoviTests.ControllersTests
{
    public class UserHistoryControllerTests
    {
        private IUnitOfWork _unitOfWork;

        private Mock<ApplicationContext> _context;
        private UserHistoryController _controller;
        private ILogger<UserHistoryController> _logger;

        [SetUp]
        public void Setup()
        {
            SeedDb();

            _unitOfWork = new UnitOfWork(_context.Object, new LoggerFactory());
            _controller = new UserHistoryController(_logger, _unitOfWork);
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

            var usersHistory = new List<UserMovieHistory> {
                new UserMovieHistory() { UserId = users.FirstOrDefault().Id,
                Movie = movies.FirstOrDefault()}
            };

            _context = new Mock<ApplicationContext>();

            _context.Setup(c => c.Set<User>()).ReturnsDbSet(users);
            _context.Setup(c => c.Users).ReturnsDbSet(users);
            _context.Setup(c => c.Set<Movie>()).ReturnsDbSet(movies);
            _context.Setup(c => c.Movies).ReturnsDbSet(movies);
            _context.Setup(c => c.UserMovieHistories).ReturnsDbSet(usersHistory);
            _context.Setup(c => c.Set<UserMovieHistory>()).ReturnsDbSet(usersHistory);
        }

        [Test]
        public async Task Get()
        {
            //arrange
            var user = await _context.Object.Users.FirstOrDefaultAsync();
            var userClaim = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Username),

            }, "mock"));

            _controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = userClaim };

            //act
            var result = await _controller.Get();
            OkObjectResult okObjectResult = result as OkObjectResult;
            var history = okObjectResult.Value as List<UserMovieHistory>;

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.IsNotNull(history);
            Assert.IsNotEmpty(history);
        }

        [Test]
        public async Task AddToHistory()
        {
            //arrange
            var favMovie = _context.Object.UserMovieHistories.FirstOrDefault();
            var user = await _context.Object.Users.Where(u => u.Id == favMovie.UserId).FirstOrDefaultAsync();
            var userClaim = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Username),

            }, "mock"));

            _controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = userClaim };
            var movie = _context.Object.Movies.FirstOrDefault();
            //act
            var actionResult = await _controller.AddToHistory(movie);
            OkObjectResult okObjectResult = actionResult as OkObjectResult;
            var movieResult = (bool)okObjectResult.Value;

            //assert
            Assert.IsInstanceOf<OkObjectResult>(actionResult);
            Assert.IsTrue(movieResult);
        }
    }
}