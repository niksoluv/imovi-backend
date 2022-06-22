using imovi_backend.Core.IConfiguration;
using imovi_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace imovi_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public AccountController(ILogger<AccountController> logger,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("getUsers")]
        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _unitOfWork.Users.All(new Guid());
        }

        [HttpPost("/create")]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult("Smth is wrong") { StatusCode = 500 };
            }

            bool usernameExists = await _unitOfWork.Users.DoesUsernameExists(user.Username);
            if (usernameExists)
            {
                return Ok(new { errorMessage = "User with this username already exists." });
            }

            bool emailExists = await _unitOfWork.Users.DoesEmailExists(user.Email);
            if (emailExists)
            {
                return Ok(new { errorMessage = "User with this email already exists." });
            }

            await _unitOfWork.Users.Add(user);
            await _unitOfWork.CompleteAsync();

            return Ok(user);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var user = await _unitOfWork.Users.GetById(id);
            if (user == null)
            {
                return NotFound(); //404
            }
            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> GetByUsername()
        {
            var user = await _unitOfWork.Users.GetByUsername(User.Identity.Name);
            if (user == null)
            {
                return NotFound(new { errorMessage = "Invalid username or password." }); //404
            }
            return Ok(user);
        }

        [HttpPost("/token")]
        public async Task<IActionResult> Token([FromBody] User user)
        {
            var token = _unitOfWork.Users.GetToken(user);
            if (token == null) return NotFound();
            return Json(token);
        }
    }
}
