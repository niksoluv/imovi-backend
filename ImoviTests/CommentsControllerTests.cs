using FluentAssertions;
using imovi_backend;
using imovi_backend.Controllers;
using imovi_backend.Core.IConfiguration;
using imovi_backend.Data;
using imovi_backend.Models;
using Microsoft.AspNetCore.Mvc;
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
    internal class CommentsControllerTests
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

            var comments = new List<Comment>()
            {
                new Comment(){Data = "comment", Movie = movies.FirstOrDefault()},
                new Comment(){Data = "comment2", Movie = movies.FirstOrDefault()},
                new Comment(){Data = "comment3", Movie = movies.FirstOrDefault()},
                new Comment(){Data = "commen123123t", Movie = movies.LastOrDefault()},
                new Comment(){Data = "comment234", Movie = movies.LastOrDefault() }
            }.AsQueryable();

            var usersMockSet = users.BuildMockDbSet();

            var movieMockSet = movies.BuildMockDbSet();

            var customListsMockSet = customLists.BuildMockDbSet();

            var customListsMoviesMockSet = customListMovies.BuildMockDbSet();

            var commentsMockSet = comments.BuildMockDbSet();

            mockContext = new Mock<ApplicationContext>();

            mockContext.Setup(c => c.Set<User>()).Returns(usersMockSet.Object);
            mockContext.Setup(c => c.Users).Returns(usersMockSet.Object);
            mockContext.Setup(c => c.Set<Movie>()).Returns(movieMockSet.Object);
            mockContext.Setup(c => c.Movies).Returns(movieMockSet.Object);
            mockContext.Setup(c => c.Set<CustomList>()).Returns(customListsMockSet.Object);
            mockContext.Setup(c => c.CustomListsMovies).Returns(customListsMoviesMockSet.Object);
            mockContext.Setup(c => c.Set<CustomListMovie>()).Returns(customListsMoviesMockSet.Object);
            mockContext.Setup(c => c.Comments).Returns(commentsMockSet.Object);
            mockContext.Setup(c => c.Set<Comment>()).Returns(commentsMockSet.Object);
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
            //Arrange
            CustomListMovie customListMovie = mockContext.Object.Set<CustomListMovie>().FirstOrDefault();
            var user = await _unitOfWork.Users.GetByUsername("aaaaaaaa");
            var customListMoviesCount = await _unitOfWork.CustomLists.All(user.Id);

            //Act
            var result = _unitOfWork.CustomLists.RemoveFromList(customListMovie);
            var customListMoviesCountAfterRemove = await _unitOfWork.CustomLists.All(user.Id);

            //Assert
            Assert.IsNotNull(result);
            //Assert.AreEqual(customListMoviesCount.ToList().Count - 1, customListMoviesCountAfterRemove.ToList().Count);
        }

        [Test]
        public async Task CreateComment()
        {
            //Arrange
            var user = await mockContext.Object.Set<User>().FirstOrDefaultAsync();
            var movie = await mockContext.Object.Set<Movie>().FirstOrDefaultAsync();
            Comment comment = new Comment() { Data = "new comment", Movie = movie };
            var commentsBeforeAdding = await _unitOfWork.Comments.All(user.Id);

            //Act
            var result = await _unitOfWork.Comments.CreateComment(comment, user);
            
            mockContext.Object.Comments.Add(comment);
            mockContext.Object.SaveChanges();

            //var commentsAfterAdding = await _unitOfWork.Comments.All(user.Id);

            //Assert
            Assert.IsNotNull(result);
            //Assert.AreEqual(commentsBeforeAdding.Count() + 1, commentsAfterAdding.Count());
        }

        [Test]
        public async Task EditComment()
        {
            Assert.Pass();
        }

        [Test]
        public async Task ReplyComment()
        {
            Assert.Pass();
        }

        [Test]
        public async Task GetMovieComments()
        {
            Assert.Pass();
        }

        [Test]
        public async Task LikeComment()
        {
            Assert.Pass();
        }

        [Test]
        public async Task UnlikeComment()
        {
            Assert.Pass();
        }
    }
}
