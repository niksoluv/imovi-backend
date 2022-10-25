using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace imovi_backend.Controllers
{
    [ExcludeFromCodeCoverage]
    [ApiController]
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
      
        [HttpGet]
        [Authorize]
        [Route("getlogin")]
        public IActionResult GetLogin()
        {
            return Ok($"Your login: {User.Identity.Name}");
        }
         
        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("getrole")]
        public IActionResult GetRole()
        {
            return Ok($"Your role: admin");
        }
    }
}
