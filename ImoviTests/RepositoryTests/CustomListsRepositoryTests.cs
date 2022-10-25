using imovi_backend;
using imovi_backend.Controllers;
using imovi_backend.Core.IConfiguration;
using imovi_backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
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
    internal class CustomListsRepositoryTests
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
                new User { Id = Guid.NewGuid(), Email = "testEmail1@mail.com", Password="11111111", Username="aaaaaaaa"},
                new User { Id = Guid.NewGuid(), Email = "testEmail2@mail.com", Password="22222222", Username="bbbbbbbb"},
                new User { Id = Guid.NewGuid(), Email = "testEmail3@mail.com", Password="33333333", Username="cccccccc"},
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
            };

            _context = new Mock<ApplicationContext>();

            _context.Setup(c => c.Set<User>()).ReturnsDbSet(users);
            _context.Setup(c => c.Set<Movie>()).ReturnsDbSet(movies);
            _context.Setup(c => c.Movies).ReturnsDbSet(movies);
            _context.Setup(c => c.Set<CustomList>()).ReturnsDbSet(customLists);
            _context.Setup(c => c.CustomListsMovies).ReturnsDbSet(customListMovies);
            _context.Setup(c => c.Set<CustomListMovie>()).ReturnsDbSet(customListMovies);
        }

        [Test]
        public async Task All()
        {
            //Arrange
            var user = await _context.Object.Set<CustomList>().FirstOrDefaultAsync();

            //Act
            var result = await _unitOfWork.CustomLists.All(user.Id);

            //Assert
            Assert.IsNotNull(result);
        }

        [TestCase("aaaaaaaa", 3)]
        [TestCase("cccccccc", 0)]
        //[TestCase("", 0)]
        public async Task GetUserLists(string username, int numOfLists)
        {
            var user = await _unitOfWork.Users.GetByUsername(username);

            var lists = await _unitOfWork.CustomLists.ListsWithMovies(user.Id);

            Assert.IsNotNull(lists);
            Assert.AreEqual(numOfLists, lists.Count);
        }

        [Test]
        public async Task CreateList()
        {
            var user = _unitOfWork.Users.GetByUsername("aaaaaaaa").Result;

            CustomList list = new()
            {
                ListName = "Test list",
                UserId = user.Id
            };

            var result = _unitOfWork.CustomLists.Add(list);

            Assert.IsNotNull(result);
        }

        [Test]
        public async Task AddMovieToList()
        {
            CustomListMovie customListMovie = new()
            {
                CustomListId = _context.Object.Set<CustomList>().FirstOrDefault().Id,
                MovieId = _context.Object.Set<Movie>().FirstOrDefault().Id,
                Movie = _context.Object.Set<Movie>().FirstOrDefault()
            };

            var result = await _unitOfWork.CustomLists.AddToList(customListMovie);

            Assert.IsNotNull(result);
        }

        [Test]
        public async Task RemoveMovieFromList()
        {
            //Arrange
            CustomListMovie customListMovie = await _context.Object.Set<CustomListMovie>().FirstOrDefaultAsync();

            //Act
            var result = _unitOfWork.CustomLists.RemoveFromList(customListMovie);

            //Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task DeleteList()
        {
            //Arrange
            CustomList customList = _context.Object.Set<CustomList>().FirstOrDefault();

            //Act
            var result = await _unitOfWork.CustomLists.Delete(customList.Id);

            //Assert
            Assert.IsTrue(result);
        }
    }
}
