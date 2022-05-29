using api.Models.Users;

namespace api.Services
{
    public interface IAuthService
    {
        string Login(AuthRequest user);
        string Register(RegisterRequest user, string id);
        string RenewToken(string token);
        public string GuestToken();
    }
}