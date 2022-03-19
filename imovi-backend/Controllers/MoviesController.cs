using imovi_backend.Core.IConfiguration;
using imovi_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace imovi_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : Controller
    {
        private readonly ILogger<MoviesController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public MoviesController(ILogger<MoviesController> logger,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        [Authorize]
        [Route("addToFavourites")]
        public async Task<IActionResult> AddToFavourites([FromBody] FavouriteMovie movie)
        {
            var user = await _unitOfWork.Users.GetByUsername(User.Identity.Name);
            if (user == null)
                return NotFound();
            var result = await _unitOfWork.Movies.AddToFavourites(user.Id, movie);
            if (result == null)
                return null;
            await _unitOfWork.CompleteAsync();
            return Ok(new { response = result });
        }

        [HttpGet("{movieId}")]
        public async Task<IActionResult> IsMovieInFavourites(string movieId)
        {
            var user = await _unitOfWork.Users.GetByUsername(User.Identity.Name);
            if (user == null)
                return NotFound();
            var result = await _unitOfWork.Movies.IsMovieInFavourites(user.Id, movieId);
            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetFavourites()
        {
            var user = await _unitOfWork.Users.GetByUsername(User.Identity.Name);
            if (user == null)
                return NotFound();
            var result = await _unitOfWork.Movies.All(user.Id);
            return Ok(result);
        }

        [HttpDelete]
        [Authorize]
        [Route("removeFromFavourites")]
        public async Task<IActionResult> RemoveFromFavourites([FromBody] FavouriteMovie movie)
        {
            var user = await _unitOfWork.Users.GetByUsername(User.Identity.Name);
            if (user == null)
                return NotFound();
            var result = await _unitOfWork.Movies.RemoveFromFavourites(user.Id, movie.MovieId);
            await _unitOfWork.CompleteAsync();
            return Ok(new { response = result });
        }
    }
}
