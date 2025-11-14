using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            Role = dto.Role
        };

        var token = await _authService.Register(user, dto.Password);
        
        // Return full user info along with token
        var response = new
        {
            token,
            id = user.Id,
            username = user.Name,
            email = user.Email,
            role = user.Role
        };

        return Ok(response);
    }

    [HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginDto dto)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

    var userDto = await _authService.Login(dto.Email, dto.Password);
    if (userDto == null)
    {
        return Unauthorized(new { message = "Invalid email or password" });
    }

    return Ok(userDto); // ✅ Now returning full user details with token
}
}
