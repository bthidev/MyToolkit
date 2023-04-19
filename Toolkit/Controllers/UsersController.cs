using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToolKit.Entities;
using ToolKit.Services;

namespace ToolKit.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateModel model)
        {
            User user = await _userService.Authenticate(model.username, model.password);

            if (user == null)
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }

            return Ok(user);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            // var users = _userService.GetAll();
            return Ok(null);
        }

        [AllowAnonymous]
        [HttpPost("createUser")]
        public IActionResult CreateUser([FromBody] AuthenticateModel username)
        {
            if (_userService.CreateUser(username.username, username.password))
            {
                return Ok();
            }

            return Problem("user already exist");
        }
    }
}