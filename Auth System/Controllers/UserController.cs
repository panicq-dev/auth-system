using Auth_System.Data.Dto;
using Auth_System.Services;
using Microsoft.AspNetCore.Mvc;

namespace Auth_System.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private AuthService _authService;
    public UserController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
    {
        await _authService.Register(dto);
        return Ok("User created");
    }
    
    [HttpPost("login")]
    public IActionResult Login([FromBody] UserLoginDto dto)
    {
        var token = _authService.Login(dto);
        return Ok(token);
    }
}