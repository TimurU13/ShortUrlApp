using AuthService;
using AuthService.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }
    [HttpPost("login")]
    public IActionResult Login([FromBody] User login)
    {
        var user = _userService.ValidateUser(login);
        if (user == null)
            return Unauthorized();

        _userService.SaveUserToFile(login);

        var token = _userService.GenerateJwtToken(user);
        return Ok(new { Token = token });
    }
}