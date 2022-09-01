using FluentAssertions;
using imovi_backend;
using imovi_backend.Controllers;
using imovi_backend.Core.IConfiguration;
using imovi_backend.Models;
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
using System.Threading.Tasks;

namespace ImoviTests
{
    public class AccountControllerTests
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
            IList<User> users = new List<User>
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

            //var usersMockSet = users.BuildMockDbSet();

            var movieMockSet = movies.BuildMockDbSet();

            var customListsMockSet = customLists.BuildMockDbSet();

            var customListsMoviesMockSet = customListMovies.BuildMockDbSet();

            _context = new Mock<ApplicationContext>();

            _context.Setup(c => c.Users).ReturnsDbSet(users);
            _context.Setup(c => c.Set<User>()).ReturnsDbSet(users);
            //_context.Setup(c => c.Users.FindAsync(It.IsAny<object[]>())).ReturnsAsync(r=>users.Where(u=>u.Id==r.Id).FirstOrDefault()));
            //_context.Setup(c => c.Users).Returns(usersMockSet.Object);
            _context.Setup(c => c.Set<Movie>()).Returns(movieMockSet.Object);
            _context.Setup(c => c.Movies).Returns(movieMockSet.Object);
            _context.Setup(c => c.Set<CustomList>()).Returns(customListsMockSet.Object);
            _context.Setup(c => c.CustomListsMovies).Returns(customListsMoviesMockSet.Object);
            _context.Setup(c => c.Set<CustomListMovie>()).Returns(customListsMoviesMockSet.Object);
        }

        [Test]
        public async Task GetAllUsers()
        {
            //arrange
            var user = _context.Object.Set<User>().FirstOrDefault();

            //act
            var result = await _unitOfWork.Users.All(user.Id);

            //assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count() > 0);
            result.All(r => r.Username == null).Should().BeFalse();
            result.All(r => r.CustomLists == null).Should().BeTrue();
        }

        [Test]
        public async Task CreateUser()
        {
            //Arrange
            User userToCreate = new User
            {
                Username = "username",
                Email = "testmail@testmail",
                Password = "testpassword"
            };

            //Act
            var result = await _unitOfWork.Users.Add(userToCreate);

            //Assert
            Assert.NotNull(result);
            Assert.IsTrue(result);
        }

        [Test]
        public async Task GetUserById()
        {
            //Arrange
            User user = _context.Object.Users.FirstOrDefault();

            //Act
            var result = await _unitOfWork.Users.GetById(user.Id);

            //Assert
            Assert.NotNull(result);

            result.Should().BeEquivalentTo(user);
            result.Username.Should().Be(user.Username);
            result.Password.Should().Be(user.Password);
            result.Email.Should().Be(user.Email);

            //_context.Users.ToList().Count.Should().Be(4);           
        }
    }
}