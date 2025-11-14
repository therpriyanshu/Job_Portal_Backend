public interface IAuthService
{
    Task<string> Register(User user, string password);
    Task<UserDto> Login(string email, string password); // ✅ Updated return type
}
