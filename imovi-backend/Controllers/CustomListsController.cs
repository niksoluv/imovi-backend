using imovi_backend.Core.IConfiguration;
using imovi_backend.Data;
using imovi_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace imovi_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomListsController : ControllerBase
    {
        private readonly ILogger<CustomListsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public CustomListsController(ILogger<CustomListsController> logger,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("lists")]
        [Authorize]
        public async Task<IActionResult> Lists()
        {
            var user = await _unitOfWork.Users.GetByUsername(User.Identity.Name);
            if (user == null)
            {
                return NotFound(new { errorMessage = "Invalid username or password." }); //404
            }
            var lists = await _unitOfWork.CustomLists.ListsWithMovies(user.Id);
            return Ok(lists);
        }

        [HttpPost]
        [Route("create")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CustomListDTO customList)
        {
            var user = await _unitOfWork.Users.GetByUsername(User.Identity.Name);
            if (user == null)
            {
                return NotFound(new { errorMessage = "Invalid username or password." }); //404
            }
            CustomList cl = new CustomList()
            {
                UserId = user.Id,
                ListName = customList.ListName,
                IsVisible = false
            };
            await _unitOfWork.CustomLists.Add(cl);
            await _unitOfWork.CompleteAsync();
            return Ok();
        }

        [HttpPost]
        [Route("add")]
        [Authorize]
        public async Task<IActionResult> AddToList([FromBody] CustomListMovie customListMovie)
        {
            var user = await _unitOfWork.Users.GetByUsername(User.Identity.Name);
            if (user == null)
            {
                return NotFound(new { errorMessage = "Invalid username or password." }); //404
            }


            var res = await _unitOfWork.CustomLists.AddToList(customListMovie);
            await _unitOfWork.CompleteAsync();
            return Ok(res);
        }

        [HttpPost]
        [Route("remove")]
        [Authorize]
        public async Task<IActionResult> RemoveDromList([FromBody] CustomListMovie customListMovie)
        {
            var user = await _unitOfWork.Users.GetByUsername(User.Identity.Name);
            if (user == null)
            {
                return NotFound(new { errorMessage = "Invalid username or password." }); //404
            }


            var res = await _unitOfWork.CustomLists.RemoveFromList(customListMovie);
            await _unitOfWork.CompleteAsync();
            return Ok(res);
        }
    }
}
