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
    public class CustomListsControllerTests
    {
        private IUnitOfWork _unitOfWork;

        private Mock<ApplicationContext> _context;
        private CustomListsController _controller;
        private ILogger<CustomListsController> _logger;

        [SetUp]
        public void Setup()
        {
            SeedDb();

            _unitOfWork = new UnitOfWork(_context.Object, new LoggerFactory());
            _controller = new CustomListsController(_logger, _unitOfWork);
        }

        private void SeedDb()
        {
            IList<User> users = new List<User>
            {
                new User { Id = new Guid("614a4912-75ae-4fa6-8a54-4bd740af333f"), Email = "testEmail1@mail.com", Password="11111111", Username="aaaaaaaa", Role = "user"},
                new User { Id = Guid.NewGuid(), Email = "testEmail2@mail.com", Password="22222222", Username="bbbbbbbb", Role = "user"},
                new User { Id = Guid.NewGuid(), Email = "testEmail3@mail.com", Password="33333333", Username="cccccccc", Role = "user"},
            };

            var movies = new List<Movie>
            {
                new Movie { Id = Guid.NewGuid(), MovieId="123", MediaType="movie"},
                new Movie { Id = Guid.NewGuid(), MovieId="345", MediaType="movie"},
                new Movie { Id = Guid.NewGuid(), MovieId="567", MediaType="tv"},
            };

            var customLists = new List<CustomList>
            {
                new CustomList {
                    Id = Guid.NewGuid(), ListName="List1", UserId = users.FirstOrDefault().Id },
                new CustomList {
                    Id = Guid.NewGuid(), ListName="List2", UserId = users.FirstOrDefault().Id },
                new CustomList {
                    Id = Guid.NewGuid(), ListName="List3", UserId = users.FirstOrDefault().Id },
            };

            var customListMovies = new List<CustomListMovie>()
            {
                new CustomListMovie()
                {
                    CustomListId = customLists.FirstOrDefault().Id,
                    MovieId = movies.FirstOrDefault().Id,
                    Movie = movies.FirstOrDefault()
                },new CustomListMovie()
                {
                    CustomListId = customLists.FirstOrDefault().Id,
                    MovieId = movies.LastOrDefault().Id,
                    Movie = movies.FirstOrDefault()
                },new CustomListMovie()
                {
                    CustomListId = customLists.FirstOrDefault().Id,
                    MovieId = movies.FirstOrDefault().Id,
                    Movie = movies.LastOrDefault()
                },new CustomListMovie()
                {
                    CustomListId = customLists.FirstOrDefault().Id,
                    MovieId = movies.FirstOrDefault().Id,
                    Movie = movies.LastOrDefault()
                }
            }.AsQueryable();

            _context = new Mock<ApplicationContext>();

            _context.Setup(c => c.Set<User>()).ReturnsDbSet(users);
            _context.Setup(c => c.Users).ReturnsDbSet(users);
            _context.Setup(c => c.Set<Movie>()).ReturnsDbSet(movies);
            _context.Setup(c => c.Movies).ReturnsDbSet(movies);
            _context.Setup(c => c.Set<CustomList>()).ReturnsDbSet(customLists);
            _context.Setup(c => c.CustomLists).ReturnsDbSet(customLists);
            _context.Setup(c => c.CustomListsMovies).ReturnsDbSet(customListMovies);
            _context.Setup(c => c.Set<CustomListMovie>()).ReturnsDbSet(customListMovies);
        }

        [Test]
        public async Task Lists()
        {
            //arrange
            var user = await _context.Object.Users.FirstOrDefaultAsync();
            var userClaim = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Username),

            }, "mock"));

            _controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = userClaim };

            //act
            var result = await _controller.Lists();
            OkObjectResult okObjectResult = result as OkObjectResult;
            List<CustomList> lists = okObjectResult.Value as List<CustomList>;

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.IsNotEmpty(lists);
        }

        [Test]
        public async Task Create()
        {
            //arrange
            var user = await _context.Object.Users.FirstOrDefaultAsync();
            var userClaim = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Username),

            }, "mock"));

            _controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = userClaim };

            var customListDTO = new CustomListDTO() { ListName = "test list" };

            //act
            var actionResult = await _controller.Create(customListDTO);

            //assert
            Assert.IsInstanceOf<OkResult>(actionResult);
        }

        [Test]
        public async Task AddToList()
        {
            //arrange
            var user = await _context.Object.Users.FirstOrDefaultAsync();
            var userClaim = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Username),

            }, "mock"));

            _controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = userClaim };

            var customList = _context.Object.CustomLists.FirstOrDefault();
            var movie = _context.Object.Movies.FirstOrDefault();
            
            var customListMovie = new CustomListMovie()
            {
                CustomListId = customList.Id,
                MovieId = movie.Id,
                Movie = movie
            };

            //act
            var actionResult = await _controller.AddToList(customListMovie);
            var objectResult = actionResult as ObjectResult;
            var result = objectResult.Value as CustomListMovie;

            //assert
            Assert.AreEqual(objectResult.StatusCode, 200);
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task RemoveFromList()
        {
            //arrange
            var user = await _context.Object.Users.FirstOrDefaultAsync();
            var userClaim = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Username),

            }, "mock"));

            _controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = userClaim };
            var customListMovie = await _context.Object.CustomListsMovies.FirstOrDefaultAsync();

            //act
            var actionResult = await _controller.RemoveFromList(customListMovie);
            var objectResult = actionResult as ObjectResult;
            var result = objectResult.Value as CustomListMovie;

            //assert
            Assert.AreEqual(objectResult.StatusCode, 200);
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task DeleteList()
        {
            //arrange
            var user = await _context.Object.Users.FirstOrDefaultAsync();
            var userClaim = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Username),

            }, "mock"));

            _controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = userClaim };

            var customList = _context.Object.CustomLists.FirstOrDefault();

            //act
            var actionResult = await _controller.DeleteList(customList);
            var okObjectResult = actionResult as OkObjectResult;
            var result = okObjectResult.Value;

            //assert
            Assert.AreEqual(okObjectResult.StatusCode, 200);
            Assert.IsNotNull(result);
        }
    }
}