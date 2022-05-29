using api.Models;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Diagnostics;
using System.Net.Http.Headers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
//dovrsi kontroler
namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        private IJwtToken jwtToken;

        public UserController(IUserService userService, IConfiguration configuration,IJwtToken token)
        {
            this.userService = userService;
            jwtToken = token;
        }
        public string getUsername()
        {
            string username;
            var header = Request.Headers[HeaderNames.Authorization];
            if (AuthenticationHeaderValue.TryParse(header, out var headerValue))
            {
                var scheme = headerValue.Scheme;
                var parameter = headerValue.Parameter;
                username = jwtToken.TokenToUsername(parameter);
                if (username == null)
                    return null;
            }
            else
                return null;

            return username;
        }

        // GET: api/<UserController>
        [HttpGet]
        public ActionResult<List<User>> Get()
        {
            return userService.Get();
        }
        
        // GET api/<UserController>/5
        //potrebno za profile page
        [HttpGet("myprofile")]
        [Authorize(Roles = "User")]
        public ActionResult<User> MyProfilePage()
        {
            string username = getUsername();

            if (username == null)
                return BadRequest();

            var user = userService.GetUserUsername(username);

            if (user == null)
                return NotFound($"User with Id = {username} not found");

            return user;
        }
        
        // POST api/<UserController>
        [HttpPost]
        public ActionResult<User> Post([FromBody] User user)
        {
            var existingUser = userService.GetUserUsername(user.Username);

            if (existingUser != null)
                return NotFound($"User with username = {user.Username} exisits");
            else
            {
                userService.Create(user);

                //Debug.WriteLine("\nTest.\n");

                return CreatedAtAction(nameof(Get), new { id = user._id }, user);
            }
        }

        // PUT api/<UserController>/changepass 
        [HttpPut("changepass")]
        [Authorize(Roles = "User")]
        public ActionResult PutPass([FromBody] string[] Password)
        {
            string username = getUsername();

            if (username == null)
                return BadRequest();

            User user = new User();

            user = userService.GetUserUsername(username);
            
            if(PasswordCrypt.checkPassword(Password[0], user.Password))
            {
                if(PasswordCrypt.checkPassword(Password[1], user.Password))
                {
                    return BadRequest($"Identical password!");
                }

                user.Password = PasswordCrypt.hashPassword(Password[1]);
                userService.Update(username, user);
                return Ok($"Succeful password change!");
            }
            else
                return BadRequest($"Wrong old password!");


            return NoContent();
        }

        // PUT api/<UserController>/5
        [HttpPut("changeinfo")]
        public ActionResult Put([FromBody] User user)
        {
            string username = getUsername();

            if (username == null)
                return BadRequest();

            if (user.Username != username)
            {
                User user2 = userService.GetUserByUsername(user.Username);
                if (user2 == null)
                    return Ok(userService.Update(username, user));
                else
                    return BadRequest("Username already exists!");
            }
            else
                return Ok(userService.Update(username, user));
        }

        // DELETE api/<UserController>/5
        [HttpDelete("deleteprofile")]
        [Authorize(Roles = "User")]
        public ActionResult Delete()
        {
            string username = getUsername();

            if (username == null)
                return BadRequest();

            var user = userService.GetUserUsername(username);

            userService.Delete(user._id);
            return Ok($"Profile with username = {username} deleted!");
        }
    }
}