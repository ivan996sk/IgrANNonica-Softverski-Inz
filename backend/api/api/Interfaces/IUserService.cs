using api.Models;
using Microsoft.AspNetCore.Mvc;

namespace api.Services
{
    public interface IUserService
    {
        List<User> Get();// daje sve korisnike
        User GetUserUsername(string username); //daje korisnika po korisnickom imenu
        User Create(User user); // kreira korisnika
        bool Update(string username, User user); //apdejtuje korisnika po idu
        void Delete(string username);//brise korisnika
        public User GetUserByUsername(string username);//Uzima jednog korisnika po username-u
        public User GetUserById(string id);//Uzima jednog korisnika po id-u
    }
}
