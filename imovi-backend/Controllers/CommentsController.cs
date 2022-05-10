using imovi_backend.Core.IConfiguration;
using imovi_backend.Data;
using imovi_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace imovi_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : Controller
    {
        private readonly ILogger<CommentsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public CommentsController(ILogger<CommentsController> logger,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        [Authorize]
        [Route("add")]
        public async Task<IActionResult> AddToFavourites([FromBody] Comment comment)
        {
            var user = await _unitOfWork.Users.GetByUsername(User.Identity.Name);
            if (user == null)
                return NotFound();
            var result = await _unitOfWork.Comments.CreateComment(comment,user);
            if (result == null)
                return null;
            await _unitOfWork.CompleteAsync();
            return Ok(new { response = result });
        }

        [HttpGet("{movieId}")]
        public async Task<IActionResult> GetComments(string movieId)
        {
            var comments = await _unitOfWork.Comments.GetMovieComments(movieId);
            return Ok(new { response = comments });
        }

        [HttpPost]
        [Authorize]
        [Route("like")]
        public async Task<IActionResult> LikeComment([FromBody] CommentIdDTO commentId)
        {
            var user = await _unitOfWork.Users.GetByUsername(User.Identity.Name);
            if (user == null)
                return NotFound();

            var result = await _unitOfWork.Comments.LikeComment(commentId.CommentId, user.Id);
            if (result == null)
                return null;

            await _unitOfWork.CompleteAsync();
            return Ok(new { response = result });
        }

        [HttpPost]
        [Authorize]
        [Route("unlike")]
        public async Task<IActionResult> UnlikeComment([FromBody] CommentIdDTO commentId)
        {
            var user = await _unitOfWork.Users.GetByUsername(User.Identity.Name);
            if (user == null)
                return NotFound();

            var result = await _unitOfWork.Comments.UnlikeComment(commentId.CommentId, user.Id);
            if (result == null)
                return null;

            await _unitOfWork.CompleteAsync();
            return Ok(new { response = result });
        }
    }
}
