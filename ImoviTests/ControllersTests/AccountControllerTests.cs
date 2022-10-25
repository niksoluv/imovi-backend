using FluentAssertions;
using imovi_backend;
using imovi_backend.Controllers;
using imovi_backend.Core.IConfiguration;
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
    public class AccountControllerTests
    {
        private IUnitOfWork _unitOfWork;

        private Mock<ApplicationContext> _context;
        private AccountController _controller;
        private ILogger<AccountController> _logger;

        [SetUp]
        public void Setup()
        {
            SeedDb();

            _unitOfWork = new UnitOfWork(_context.Object, new LoggerFactory());
            _controller = new AccountController(_logger, _unitOfWork);
        }

        private void SeedDb()
        {
            IList<User> users = new List<User>
            {
                new User { Id = new Guid("614a4912-75ae-4fa6-8a54-4bd740af333f"), Email = "testEmail1@mail.com", Password="11111111", Username="aaaaaaaa", Role = "user"},
                new User { Id = Guid.NewGuid(), Email = "testEmail2@mail.com", Password="22222222", Username="bbbbbbbb", Role = "user"},
                new User { Id = Guid.NewGuid(), Email = "testEmail3@mail.com", Password="33333333", Username="cccccccc", Role = "user"},
            };

            _context = new Mock<ApplicationContext>();

            _context.Setup(c => c.Users).ReturnsDbSet(users);
            _context.Setup(c => c.Set<User>()).ReturnsDbSet(users);
        }

        [Test]
        public async Task GetAllUsers()
        {
            //arrange

            //act
            var result = await _controller.GetAllUsers();

            //assert
            Assert.IsNotNull(result);
        }

        [TestCase("username", "testEmail123@mail.com", false)]
        [TestCase("test", "testEmail1@mail.com", true)]
        [TestCase("cccccccc", "testEmail123@mail.com", true)]
        public async Task CreateUser(string username, string email, bool containsErrorMessage)
        {
            //arrange
            var user = new User()
            {
                Email = email,
                Password = "11111111",
                Username = username
            };

            //act
            var actionResult = await _controller.CreateUser(user);
            var okObjectResult = actionResult as OkObjectResult;
            var result = okObjectResult.Value;

            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.ToString().Contains("errorMessage"), containsErrorMessage);
        }

        [TestCase("614a4912-75ae-4fa6-8a54-4bd740af333f")]
        public async Task GetUser(string guid)
        {
            //arrange
            var userId = new Guid(guid);

            //act
            var actionResult = await _controller.GetUser(userId);
            var objectResult = actionResult as ObjectResult;
            //assert
            Assert.AreEqual(objectResult.StatusCode, 200);
        }

        [TestCase("00000000-75ae-4fa6-8a54-4bd740af333f")]
        public async Task GetUserNegative(string guid)
        {
            //arrange
            var userId = new Guid(guid);

            //act
            var actionResult = await _controller.GetUser(userId);

            //assert
            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }

        [Test]
        public async Task GetByUsername()
        {
            //arrange
            var user = await _context.Object.Users.FirstOrDefaultAsync();
            var userClaim = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Username),

            }, "mock"));

            _controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = userClaim };

            //act
            var actionResult = await _controller.GetByUsername();
            var okObjectResult = actionResult as OkObjectResult;
            var result = okObjectResult.Value;

            //assert
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task GetByUsernameNegative()
        {
            //arrange
            var userClaim = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, ""),

            }, "mock"));

            _controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = userClaim };

            //act
            var actionResult = await _controller.GetByUsername();

            //assert
            Assert.IsInstanceOf<NotFoundObjectResult>(actionResult);
        }

        [Test]
        public async Task Token()
        {
            //arrange
            var user = await _context.Object.Users.FirstOrDefaultAsync();

            //act
            var actionResult = await _controller.Token(user);
            var objectResult = actionResult as JsonResult;

            //assert
            Assert.IsNotNull(objectResult.Value);
        }

        [Test]
        public async Task TokenNegative()
        {
            //arrange
            var user = await _context.Object.Users.FirstOrDefaultAsync();
            user.Role = null;

            //act
            var actionResult = await _controller.Token(user);

            //assert
            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }
    }
}