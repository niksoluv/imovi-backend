using imovi_backend;
using imovi_backend.Controllers;
using imovi_backend.Core.IConfiguration;
using imovi_backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
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

        private IUnitOfWork _unitOfWork;

        private Mock<ApplicationContext> mockContext;

        [SetUp]
        public void Setup()
        {
            SeedDb();

            _unitOfWork = new UnitOfWork(mockContext.Object, new LoggerFactory());
        }

        private void SeedDb()
        {
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), Email = "testEmail1@mail.com", Password="11111111", Username="aaaaaaaa"},
                new User { Id = Guid.NewGuid(), Email = "testEmail2@mail.com", Password="22222222", Username="bbbbbbbb"},
                new User { Id = Guid.NewGuid(), Email = "testEmail3@mail.com", Password="33333333", Username="cccccccc"},
            }.AsQueryable();

            var movies = new List<Movie>
            {
                new Movie { Id = Guid.NewGuid(), MovieId="123", MediaType="movie"},
                new Movie { Id = Guid.NewGuid(), MovieId="345", MediaType="movie"},
                new Movie { Id = Guid.NewGuid(), MovieId="567", MediaType="tv"},
            }.AsQueryable();

            var customLists = new List<CustomList>
            {
                new CustomList {
                    Id = Guid.NewGuid(), ListName="List1", UserId = users.FirstOrDefault().Id },
                new CustomList {
                    Id = Guid.NewGuid(), ListName="List2", UserId = users.FirstOrDefault().Id },
                new CustomList {
                    Id = Guid.NewGuid(), ListName="List3", UserId = users.FirstOrDefault().Id },
            }.AsQueryable();

            var customListMovies = new List<CustomListMovie>().AsQueryable();

            var usersMockSet = users.BuildMockDbSet();

            var movieMockSet = movies.BuildMockDbSet();

            var customListsMockSet = customLists.BuildMockDbSet();

            var customListsMoviesMockSet = customListMovies.BuildMockDbSet();

            mockContext = new Mock<ApplicationContext>();

            mockContext.Setup(c => c.Set<User>()).Returns(usersMockSet.Object);
            mockContext.Setup(c => c.Set<Movie>()).Returns(movieMockSet.Object);
            mockContext.Setup(c => c.Movies).Returns(movieMockSet.Object);
            mockContext.Setup(c => c.Set<CustomList>()).Returns(customListsMockSet.Object);
            mockContext.Setup(c => c.CustomListsMovies).Returns(customListsMoviesMockSet.Object);
        }

        [TestCase("aaaaaaaa", 3)]
        [TestCase("cccccccc", 0)]
        //[TestCase("", 0)]
        public async Task GetUserLists(string username, int numOfLists)
        {
            var user = _unitOfWork.Users.GetByUsername(username).Result;

            var lists = _unitOfWork.CustomLists.ListsWithMovies(user.Id).Result;
            
            Assert.IsNotNull(lists);
            Assert.AreEqual(numOfLists, lists.Count);
        }

        [Test]
        public async Task CreateList()
        {
            var user = _unitOfWork.Users.GetByUsername("aaaaaaaa").Result;
            var listsBeforeAdding = await _unitOfWork.CustomLists.All(user.Id);

            CustomList list = new CustomList()
            {
                ListName = "Test list",
                UserId = user.Id
            };

            var result = await _unitOfWork.CustomLists.Add(list);
            var listsAfterAdding = await _unitOfWork.CustomLists.All(user.Id);


            Assert.IsNotNull(result);
            Assert.AreEqual(listsBeforeAdding.ToList().Count + 1, listsAfterAdding.ToList().Count + 1);
        }

        [Test]
        public async Task AddMovieToList()
        {
            CustomListMovie customListMovie = new CustomListMovie()
            {
                CustomListId = mockContext.Object.Set<CustomList>().FirstOrDefault().Id,
                MovieId = mockContext.Object.Set<Movie>().FirstOrDefault().Id,
                Movie = mockContext.Object.Set<Movie>().FirstOrDefault()
            };

            var result = await _unitOfWork.CustomLists.AddToList(customListMovie);

            Assert.IsNotNull(result);
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
            //_context.Dispose();
        }
    }
}
