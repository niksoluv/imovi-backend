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
using Moq.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImoviTests.RepositoryTests
{
    internal class CommentsRepositoryTests
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

            var comments = new List<Comment>()
            {
                new Comment(){Data = "comment", Movie = movies.FirstOrDefault(), User = users.FirstOrDefault(),
                CommentReplies = new List<CommentReply>(), Id = new Guid("614a4912-75ae-4fa6-8a54-4bd740af333f")},
                new Comment(){Data = "comment2", Movie = movies.FirstOrDefault(), User = users.FirstOrDefault(),
                CommentReplies = new List<CommentReply>()},
                new Comment(){Data = "comment3", Movie = movies.FirstOrDefault(), User = users.FirstOrDefault(),
                CommentReplies = new List<CommentReply>()},
                new Comment(){Data = "commen123123t", Movie = movies.LastOrDefault(), User = users.LastOrDefault(),
                CommentReplies = new List<CommentReply>()},
                new Comment(){Data = "comment234", Movie = movies.LastOrDefault(), User = users.LastOrDefault(),
                CommentReplies = new List<CommentReply>()}
            };

            var commentLikes = new List<LikedComment>();

            var commentReplies = new List<CommentReply>();

            _context = new Mock<ApplicationContext>();

            _context.Setup(c => c.Set<User>()).ReturnsDbSet(users);
            _context.Setup(c => c.Users).ReturnsDbSet(users);
            _context.Setup(c => c.Set<Movie>()).ReturnsDbSet(movies);
            _context.Setup(c => c.Movies).ReturnsDbSet(movies);
            _context.Setup(c => c.Comments).ReturnsDbSet(comments);
            _context.Setup(c => c.Set<Comment>()).ReturnsDbSet(comments);
            _context.Setup(c => c.CommentReplies).ReturnsDbSet(commentReplies);
            _context.Setup(c => c.LikedComments).ReturnsDbSet(commentLikes);
        }

        [Test]
        public async Task CreateComment()
        {
            //Arrange
            var user = await _context.Object.Set<User>().FirstOrDefaultAsync();
            var movie = await _context.Object.Set<Movie>().FirstOrDefaultAsync();
            Comment comment = new Comment() { Data = "new comment", Movie = movie };

            //Act
            var result = await _unitOfWork.Comments.CreateComment(comment, user);

            //Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task EditComment()
        {
            //Arrange
            var user = await _context.Object.Set<User>().FirstOrDefaultAsync();
            Comment comment = _context.Object.Comments.Where(x => x.User.Id == user.Id).FirstOrDefault();

            //Act
            var result = await _unitOfWork.Comments.EditComment(comment, user);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(comment, result);
        }

        [Test]
        public async Task ReplyComment()
        {
            //Arrange
            var user = await _context.Object.Set<User>().FirstOrDefaultAsync();
            var comment = await _context.Object.Set<Comment>().FirstOrDefaultAsync();
            CommentReplyDTO reply = new CommentReplyDTO() { CommentId = comment.Id, Data = "comment reply" };

            //Act
            var result = await _unitOfWork.Comments.ReplyComment(reply, user);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(comment.Id, result.Id);
        }

        [TestCase("123", false)]
        [TestCase("345", true)]
        public async Task GetMovieComments(string movieId, bool isEmpty)
        {
            //Arrange

            //Act
            var result = await _unitOfWork.Comments.GetMovieComments(movieId);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(isEmpty, result.Count == 0);
        }

        [Test]
        public async Task LikeComment()
        {
            //Arrange
            var comment = _context.Object.Comments.FirstOrDefault();
            var user = _context.Object.Users.FirstOrDefault();

            //Act
            var result = await _unitOfWork.Comments.LikeComment(comment.Id, user.Id);

            //Assert
            Assert.NotNull(result);
        }

        [Test]
        public async Task LikeCommentNegative()
        {
            //Arrange
            var commentGuidId = new Guid("614a4912-75ae-4fa6-8a54-000000000000");
            var user = _context.Object.Users.FirstOrDefault();

            //Act
            var result = await _unitOfWork.Comments.LikeComment(commentGuidId, user.Id);

            //Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task UnlikeComment()
        {
            //Arrange
            var comment = _context.Object.Comments.FirstOrDefault();
            var user = _context.Object.Users.FirstOrDefault();

            //Act
            var result = await _unitOfWork.Comments.UnlikeComment(comment.Id, user.Id);

            //Assert
            Assert.NotNull(result);
        }

        [Test]
        public async Task UnlikeCommentNegative()
        {
            //Arrange
            var commentGuidId = new Guid("614a4912-75ae-4fa6-8a54-000000000000");
            var user = _context.Object.Users.FirstOrDefault();

            //Act
            var result = await _unitOfWork.Comments.UnlikeComment(commentGuidId, user.Id);

            //Assert
            Assert.IsNull(result);
        }
    }
}
