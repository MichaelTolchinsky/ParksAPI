using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models;
using ParkyAPI.Repository.IRepository;

namespace ParkyAPI.Controllers
{
  [Authorize]
  [Route("api/v{version:apiVersion}/users")]
  [ApiController]
  [ProducesResponseType(StatusCodes.Status418ImATeapot)]
  public class UsersController : ControllerBase
  {
    private readonly IUserRepository _userRepo;
    public UsersController(IUserRepository userRepo)
    {
      _userRepo = userRepo;
    }

    /// <summary>
    /// Login
    /// </summary>
    /// <param name="model">credentials entered by the user</param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("authenticate")]
    public IActionResult Authenticate([FromBody] AuthenticationModel model)
    {
      var user = _userRepo.Authenticate(model.Username, model.Password);
      if (user == null)
        return BadRequest(new { message = "Username or Password is incorrect" });
      return Ok(user);
    }

    /// <summary>
    /// Register
    /// </summary>
    /// <param name="mode4l">credentials entered by the user</param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("register")]
    public IActionResult Register([FromBody] AuthenticationModel model)
    {
      bool ifUsernameUnique = _userRepo.IsUniqueUser(model.Username);

      if (!ifUsernameUnique)
        return BadRequest(new { message = "Username already exists" });

      var user = _userRepo.Register(model.Username, model.Password);
      if (user == null)
        return BadRequest(new { message = "Error while registering" });
      return Ok();
    }
  }
}