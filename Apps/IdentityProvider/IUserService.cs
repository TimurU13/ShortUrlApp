using AuthService.Models;
namespace AuthService
{
    public interface IUserService
    {
        User ValidateUser(User login);
        void SaveUserToFile(User user);
        string GenerateJwtToken(User user);
    }
}
