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
    public class MoviesControllerTests
    {
        private IUnitOfWork _unitOfWork;

        private Mock<ApplicationContext> _context;
        private MoviesController _controller;
        private ILogger<MoviesController> _logger;

        [SetUp]
        public void Setup()
        {
            SeedDb();

            _unitOfWork = new UnitOfWork(_context.Object, new LoggerFactory());
            _controller = new MoviesController(_logger, _unitOfWork);
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

            var favMovies = new List<FavouriteMovie>() {
                new FavouriteMovie{
                    Movie = movies.FirstOrDefault(), UserId = users.FirstOrDefault().Id
                },
                new FavouriteMovie{
                    Movie = movies.LastOrDefault(), UserId = users.FirstOrDefault().Id
                },
                new FavouriteMovie{
                    Movie = movies.FirstOrDefault(), UserId = users.LastOrDefault().Id
                },
                new FavouriteMovie{
                    Movie = movies.LastOrDefault(), UserId = users.LastOrDefault().Id
                }
            };

            _context = new Mock<ApplicationContext>();

            _context.Setup(c => c.FavoriteMovies).ReturnsDbSet(favMovies);
            _context.Setup(c => c.Set<FavouriteMovie>()).ReturnsDbSet(favMovies);
            _context.Setup(c => c.Set<User>()).ReturnsDbSet(users);
            _context.Setup(c => c.Users).ReturnsDbSet(users);
            _context.Setup(c => c.Set<Movie>()).ReturnsDbSet(movies);
            _context.Setup(c => c.Movies).ReturnsDbSet(movies);
        }

        [Test]
        public async Task AddToFavourites()
        {
            //arrange
            var user = await _context.Object.Users.FirstOrDefaultAsync();
            var userClaim = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Username),

            }, "mock"));

            _controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = userClaim };
            var favMovieDTO = new FavMovieDTO() { MediaType = "movie", MovieId = "123456" };

            //act
            var result = await _controller.AddToFavourites(favMovieDTO);
            OkObjectResult okObjectResult = result as OkObjectResult;

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task IsMovieInFavourites()
        {
            //arrange
            var favMovie = _context.Object.FavoriteMovies.FirstOrDefault();
            var user = await _context.Object.Users.Where(u=>u.Id==favMovie.UserId).FirstOrDefaultAsync();
            var userClaim = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Username),

            }, "mock"));

            _controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = userClaim };
            //act
            var actionResult = await _controller.IsMovieInFavourites(favMovie.Movie.MovieId);
            OkObjectResult okObjectResult = actionResult as OkObjectResult;
            bool exists = (bool)okObjectResult.Value;

            //assert
            Assert.IsInstanceOf<OkObjectResult>(actionResult);
            Assert.IsFalse(exists);
        }

        [Test]
        public async Task IsNotFavoriteMovieInFavourites()
        {
            //arrange
            var user = await _context.Object.Users.FirstOrDefaultAsync();
            var userClaim = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Username),

            }, "mock"));

            _controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = userClaim };

            //act
            var actionResult = await _controller.IsMovieInFavourites("000000");
            OkObjectResult okObjectResult = actionResult as OkObjectResult;
            bool exists = (bool)okObjectResult.Value;

            //assert
            Assert.IsInstanceOf<OkObjectResult>(actionResult);
            Assert.IsFalse(exists);
        }

        [Test]
        public async Task GetFavourites()
        {
            //arrange
            var user = await _context.Object.Users.FirstOrDefaultAsync();
            var userClaim = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Username),

            }, "mock"));

            _controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = userClaim };

            //act
            var actionResult = await _controller.GetFavourites();
            var objectResult = actionResult as ObjectResult;
            var result = objectResult.Value as List<FavouriteMovie>;

            //assert
            Assert.AreEqual(objectResult.StatusCode, 200);
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
        }

        [Test]
        public async Task RemoveFromFavourites()
        {
            //arrange
            var user = await _context.Object.Users.FirstOrDefaultAsync();
            var userClaim = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Username),

            }, "mock"));

            _controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = userClaim };
            var movie = _context.Object.Movies.FirstOrDefault();
            var favMovieDTO = new FavMovieDTO() { MediaType = "movie", MovieId = movie.MovieId };

            //act
            var actionResult = await _controller.RemoveFromFavourites(favMovieDTO);
            var objectResult = actionResult as ObjectResult;
            var result = objectResult.Value as CustomListMovie;

            //assert
            Assert.AreEqual(objectResult.StatusCode, 200);
        }
    }
}