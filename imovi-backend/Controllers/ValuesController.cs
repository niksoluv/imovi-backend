using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace imovi_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
      
        [HttpGet]
        [Route("getlogin")]
        public IActionResult GetLogin()
        {
            return Ok($"Ваш логин: {User.Identity.Name}");
        }
         
        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("getrole")]
        public IActionResult GetRole()
        {
            return Ok("Ваша роль: администратор");
        }
    }
}
