using FluentAssertions;
using imovi_backend;
using imovi_backend.Controllers;
using imovi_backend.Core.IConfiguration;
using imovi_backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImoviTests
{
    public class AccountControllerTests : IDisposable
    {
        private DbContextOptions<ApplicationContext> dbContextOptions = new DbContextOptionsBuilder<ApplicationContext>()
        .UseInMemoryDatabase(databaseName: "d9d7t87n7pd40k")
        .Options;

        private AccountController controller;
        private ApplicationContext _context;
        private ILogger<AccountController> _logger;
        private IUnitOfWork _unitOfWork;

        [OneTimeSetUp]
        public void Setup()
        {
            _context = new ApplicationContext(dbContextOptions);

            SeedDb();

            _unitOfWork = new UnitOfWork(_context, new LoggerFactory());


            controller = new AccountController(_logger, _unitOfWork);
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
        }

        [Test]
        public async Task GetAllUsers()
        {
            var usersCount = _context.Users.ToList().Count;
            var users = (await controller.GetAllUsers()).ToList();

            users.Count.Should().Be(usersCount);
            users.All(r => r.Username == null).Should().BeFalse();
            users.All(r => r.CustomLists == null).Should().BeTrue();
        }

        [Test]
        public async Task CreateUser()
        {
            //Arrange
            var usersCount = _context.Users.ToList().Count;
            User userToCreate = new User
            {
                Username = "username",
                Email = "testmail@testmail",
                Password = "testpassword"
            };

            //Act
            var actionResult = await controller.CreateUser(userToCreate);

            //Assert
            var okObjectResult = actionResult as OkObjectResult;
            Assert.NotNull(okObjectResult);

            var user = okObjectResult.Value as User;
            Assert.NotNull(user);

            user.Username.Should().Be("username");
            user.Password.Should().Be("testpassword");
            user.Email.Should().Be("testmail@testmail");

            _context.Users.ToList().Count.Should().Be(usersCount + 1);
        }

        [Test]
        public async Task GetUserBuId()
        {
            //Arrange
            User user = _context.Users.FirstOrDefault();

            //Act
            var actionResult = await controller.GetUser(user.Id);

            //Assert
            var okObjectResult = actionResult as OkObjectResult;
            Assert.NotNull(okObjectResult);

            var userResult = okObjectResult.Value as User;
            Assert.NotNull(userResult);

            userResult.Should().BeEquivalentTo(user);
            userResult.Username.Should().Be(user.Username);
            userResult.Password.Should().Be(user.Password);
            userResult.Email.Should().Be(user.Email);

            //_context.Users.ToList().Count.Should().Be(4);           
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}