using api.Models.Users;

namespace api.Models
{
    public interface IJwtToken
    {
        string GenGuestToken(string id);
        string GenToken(User user);
        string RenewToken(string existingToken);
        string TokenToUsername(string token);
        public string TokenToId(string token);
    }
}