using FluentAssertions;
using imovi_backend;
using imovi_backend.Core.IConfiguration;
using imovi_backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ImoviTests.RepositoryTests
{
    public class AccountControllerTests
    {
        private IUnitOfWork _unitOfWork;

        private Mock<ApplicationContext> _context;
        private Mock<DbSet<User>> usersDbSetMock;

        [SetUp]
        public void Setup()
        {
            SeedDb();

            _unitOfWork = new UnitOfWork(_context.Object, new LoggerFactory());
        }

        private void SeedDb()
        {
            //var users = new List<User>
            //{
            //    new User { Id = new Guid("614a4912-75ae-4fa6-8a54-4bd740af333f"), Email = "testEmail1@mail.com", Role = "user", Password="11111111", Username="aaaaaaaa"},
            //    new User { Id = Guid.NewGuid(), Email = "testEmail2@mail.com", Password="22222222", Username="bbbbbbbb", Role = "user"},
            //    new User { Id = Guid.NewGuid(), Email = "testEmail3@mail.com", Password="33333333", Username="cccccccc", Role = "user"},
            //}.AsQueryable();


            var users = new List<User>()
            {
                new User { Id = new Guid("614a4912-75ae-4fa6-8a54-4bd740af333f"), Email = "testEmail1@mail.com", Role = "user", Password="11111111", Username="aaaaaaaa"},
                new User { Id = new Guid("77777777-75ae-4fa6-8a54-4bd740af333f"), Email = "testEmail2@mail.com", Password="22222222", Username="bbbbbbbb", Role = "user"},
                new User { Id = Guid.NewGuid(), Email = "testEmail3@mail.com", Password="33333333", Username="cccccccc", Role = "user"},
            };
            var mock = users.AsQueryable().BuildMockDbSet();

            mock.Setup(x => x.FindAsync(It.IsAny<object[]>())).ReturnsAsync((object[] ids) =>
            {
                var id = (Guid)ids[0];
                var result = users.FirstOrDefault(x => x.Id == (Guid)ids[0]);
                return result;
            });

            mock.Setup(x => x.Add(It.IsAny<User>())).Callback<User>((s) =>
            {
                users.Add(s);
            });

            usersDbSetMock = mock;

            _context = new Mock<ApplicationContext>();
            _context.Setup(m => m.Users).Returns(mock.Object);
            _context.Setup(m => m.Set<User>()).Returns(mock.Object);
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
            _unitOfWork.Users.Add(userToCreate);

            //Assert
            usersDbSetMock.Verify(m => m.Add(It.IsAny<User>()), Times.Once());
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
        }

        [TestCase("614a4912-75ae-4fa6-8a54-4bd740af333f", "aaaaaaaa")]
        [TestCase("00000000-75ae-4fa6-8a54-4bd740af333f", "")]
        [TestCase("77777777-75ae-4fa6-8a54-4bd740af333f", "bbbbbbbb")]
        public async Task GetUserUsername(string id, string expectedResult)
        {
            //Arrange
            Guid userId = new Guid(id);

            //Act
            var result = await _unitOfWork.Users.GetUserUsername(userId);

            //Assert
            Assert.NotNull(result);

            result.Should().BeEquivalentTo(expectedResult);
            usersDbSetMock.Verify(m => m.FindAsync(It.IsAny<object[]>()), Times.Once());
        }

        [Test]
        public async Task UpsertUser()
        {
            //Arrange
            var user = new User
            {
                Id = new Guid("614a4912-75ae-4fa6-8a54-4bd740af333f"),
                Email = "testEmail1@mail.com",
                Password = "asdfasdfasfasdfasdf",
                Username = "New Username"
            };

            //Act
            var result = await _unitOfWork.Users.Upsert(user);

            //Assert
            Assert.NotNull(result);
            Assert.IsTrue(result);
        }

        [TestCase("614a4912-75ae-4fa6-8a54-4bd740af333f", true)]
        [TestCase("00000000-75ae-4fa6-8a54-4bd740af333f", false)]
        public async Task DeleteUser(string id, bool expectedResult)
        {
            //Arrange
            Guid userId = new Guid(id);

            //Act
            var result = await _unitOfWork.Users.Delete(userId);

            //Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestCase("bbbbbbbb", true)]
        [TestCase("shafuhsduf", false)]
        public async Task DoesUsernameExists(string username, bool expectedResult)
        {
            //Arrange

            //Act
            var result = await _unitOfWork.Users.DoesUsernameExists(username);

            //Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestCase("testEmail3@mail.com", true)]
        [TestCase("nonExistingEmail@gmail.com", false)]
        public async Task DoesEmailExists(string email, bool expectedResult)
        {
            //Arrange

            //Act
            var result = await _unitOfWork.Users.DoesEmailExists(email);

            //Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public async Task GetToken()
        {
            //Arrange
            User user = await _context.Object.Users.FirstOrDefaultAsync();

            //Act
            var result = _unitOfWork.Users.GetToken(user);

            //Assert
            Assert.NotNull(result);
        }
    }
}