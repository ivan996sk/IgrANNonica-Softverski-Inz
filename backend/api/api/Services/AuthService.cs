using System.Net.Http.Headers;
using api.Interfaces;
using api.Models;
using api.Models.Users;
using MongoDB.Driver;

namespace api.Services
{
    public class AuthService : IAuthService
    {
        private IJwtToken _jwt;
        private readonly IConfiguration _configuration;
        private readonly IMongoCollection<User> _users;
        public AuthService(IConfiguration configuration, IUserStoreDatabaseSettings settings, IMongoClient mongoClient,IJwtToken jwt)
        {
            _configuration = configuration;
            _jwt = jwt;
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _users = database.GetCollection<User>(settings.CollectionName);
        }
        public string Login(AuthRequest user)
        {
            User u = _users.Find(x => x.Username == user.UserName).FirstOrDefault();
            if (u == null)
                return "Username doesn't exist";
            if (!PasswordCrypt.checkPassword(user.Password, u.Password))
                return "Wrong password";
            return _jwt.GenToken(u);

        }
        public string Register(RegisterRequest user,string id)
        {
            User u = new User();
            u.Username = user.username;
            u.Email = user.email;
            u.Password = PasswordCrypt.hashPassword(user.password);
            u.FirstName = user.firstName;
            u.LastName = user.lastName;
            u.photoId = "1";
            u.isPermament = true;
            u._id = id;
            u.dateCreated= DateTime.Now.ToUniversalTime();
            if (_users.Find(user => user.Username == u.Username).FirstOrDefault() != null)
                return "Username Already Exists";
            if (_users.Find(user => user.Email == u.Email).FirstOrDefault() != null)
                return "Email Already Exists";

            _users.ReplaceOne(x=>x._id==u._id,u);
            return "User added";
        }

        public string RenewToken(string header)
        {
            if (AuthenticationHeaderValue.TryParse(header, out var headerValue))
            {

                var scheme = headerValue.Scheme;
                var parameter = headerValue.Parameter;
                return _jwt.RenewToken(parameter);
            }
            return null;
        }

        public string GuestToken()
        {
            User u = new User();
            u._id = "";
            u.dateCreated = DateTime.Now.ToUniversalTime();
            _users.InsertOne(u);
            return _jwt.GenGuestToken(u._id);
 
        }


    }
}
