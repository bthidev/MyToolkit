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
        public async Task<IActionResult> AuthenticateAsync([FromBody] AuthenticateModel model)
        {
            var user = await _userService.AuthenticateAsync(model.Username, model.Password).ConfigureAwait(true);

            if (user == null)
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }

            return Ok(user);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(null);
        }

        [AllowAnonymous]
        [HttpPost("createUser")]
        public async Task<IActionResult> CreateUserAsync([FromBody] AuthenticateModel username)
        {
            if (await _userService.CreateUserAsync(username.Username, username.Password).ConfigureAwait(true))
            {
                return Ok();
            }

            return Problem("user already exist");
        }
    }
}
