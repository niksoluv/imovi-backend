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
    public class CommentsControllerTests
    {
        private IUnitOfWork _unitOfWork;

        private Mock<ApplicationContext> _context;
        private CommentsController _controller;
        private ILogger<CommentsController> _logger;

        [SetUp]
        public void Setup()
        {
            SeedDb();

            _unitOfWork = new UnitOfWork(_context.Object, new LoggerFactory());
            _controller = new CommentsController(_logger, _unitOfWork);
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

            _context.Setup(c => c.Users).ReturnsDbSet(users);
            _context.Setup(c => c.Set<User>()).ReturnsDbSet(users);
            _context.Setup(c => c.Set<Movie>()).ReturnsDbSet(movies);
            _context.Setup(c => c.Movies).ReturnsDbSet(movies);
            _context.Setup(c => c.Comments).ReturnsDbSet(comments);
            _context.Setup(c => c.Set<Comment>()).ReturnsDbSet(comments);
            _context.Setup(c => c.CommentReplies).ReturnsDbSet(commentReplies);
            _context.Setup(c => c.LikedComments).ReturnsDbSet(commentLikes);
        }

        [Test]
        public async Task AddComment()
        {
            //arrange
            var user = await _context.Object.Users.FirstOrDefaultAsync();
            var userClaim = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Username),

            }, "mock"));

            _controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = userClaim };
            var comment = new Comment()
            {
                Data = "test comment",
                Movie = _context.Object.Movies.FirstOrDefault(),
                User = user,
                CommentReplies = new List<CommentReply>()
            };

            //act
            var result = await _controller.AddComment(comment);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task EditComment()
        {
            //arrange
            var user = await _context.Object.Users.FirstOrDefaultAsync();
            var userClaim = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Username),

            }, "mock"));

            _controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = userClaim };
            var comment = await _context.Object.Comments.FirstOrDefaultAsync();
            string newCommentData = "new comment data 123";
            comment.Data = newCommentData;

            //act
            var actionResult = await _controller.EditComment(comment);
            var okObjectResult = actionResult as OkObjectResult;
            var result = okObjectResult.Value as Comment;

            //assert
            Assert.IsInstanceOf<OkObjectResult>(actionResult);
        }

        [Test]
        public async Task ReplyComment()
        {
            //arrange
            var user = await _context.Object.Users.FirstOrDefaultAsync();
            var userClaim = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Username),

            }, "mock"));

            _controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = userClaim };
            var comment = await _context.Object.Comments.FirstOrDefaultAsync();
            var commentReplyDTO = new CommentReplyDTO() { Data = "odfjvosdf", CommentId = comment.Id };

            //act
            var actionResult = await _controller.ReplyComment(commentReplyDTO);
            var objectResult = actionResult as ObjectResult;
            var result = objectResult.Value as CommentReply;
            //assert
            Assert.AreEqual(objectResult.StatusCode, 200);
        }

        [Test]
        public async Task GetComments()
        {
            //arrange
            var movie = await _context.Object.Movies.FirstOrDefaultAsync();

            //act
            var actionResult = await _controller.GetComments(movie.MovieId);

            //assert
            Assert.IsInstanceOf<OkObjectResult>(actionResult);
        }

        [Test]
        public async Task LikeComment()
        {
            //arrange
            var user = await _context.Object.Users.FirstOrDefaultAsync();
            var comment = await _context.Object.Comments.FirstOrDefaultAsync();
            var commentIdDTO = new CommentIdDTO() { CommentId = comment.Id };
            var userClaim = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Username),

            }, "mock"));

            _controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = userClaim };

            //act
            var actionResult = await _controller.LikeComment(commentIdDTO);
            var okObjectResult = actionResult as OkObjectResult;
            var result = okObjectResult.Value;

            //assert
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task UnlikeComment()
        {
            //arrange
            var user = await _context.Object.Users.FirstOrDefaultAsync();
            var comment = await _context.Object.Comments.FirstOrDefaultAsync();
            var commentIdDTO = new CommentIdDTO() { CommentId = comment.Id };
            var userClaim = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Username),

            }, "mock"));

            _controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = userClaim };

            //act
            var actionResult = await _controller.UnlikeComment(commentIdDTO);
            var okObjectResult = actionResult as OkObjectResult;
            var result = okObjectResult.Value;

            //assert
            Assert.IsNotNull(result);
        }
    }
}