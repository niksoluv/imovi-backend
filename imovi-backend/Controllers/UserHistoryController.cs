using imovi_backend.Core.IConfiguration;
using imovi_backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace imovi_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserHistoryController:Controller
    {
        private readonly ILogger<UserHistoryController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public UserHistoryController(ILogger<UserHistoryController> logger,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var user = await _unitOfWork.Users.GetByUsername(User.Identity.Name);
            if (user == null)
                return NotFound();
            var res = await _unitOfWork.UserHistory.All(user.Id);
            return Ok(res);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToHistory([FromBody] Movie movie)
        {
            var user = await _unitOfWork.Users.GetByUsername(User.Identity.Name);
            if (user == null)
                return NotFound();
            var res = await _unitOfWork.UserHistory.AddToHistory(user.Id, movie);
            await _unitOfWork.CompleteAsync();
            return Ok(res);
        }
    }
}
