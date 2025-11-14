// AuthService.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<string> Register(User user, string password)
    {
        if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            return "User already exists.";

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return GenerateToken(user);
    }

    public async Task<UserDto?> Login(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;

        var token = GenerateToken(user);

        return new UserDto
        {
            Id = user.Id,
            Username = user.Name,
            Email = user.Email,
            Role = user.Role,
            Token = token
        };
    }

    public string GenerateToken(User user)
    {
        var secretKey = _config["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret Key is missing in configuration.");
        var issuer = _config["Jwt:Issuer"];
        var audience = _config["Jwt:Audience"];

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, user.Name ?? "Unknown"),
            new Claim(ClaimTypes.Role, user.Role ?? "User")
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials
        );

        string generatedToken = new JwtSecurityTokenHandler().WriteToken(token);
        Console.WriteLine($"[DEBUG] Generated JWT Token: {generatedToken}");

        return generatedToken;
    }
}
