using api.Models.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;
using api.Models;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IAuthService _auth;
        private IJwtToken _jwtToken;
        public AuthController(IAuthService auth, IJwtToken Token)
        {
            _auth = auth;
            _jwtToken = Token;
        }

        [HttpPost("register")]
        public async Task<ActionResult<string>> Register(RegisterRequest user)
        {
            string id=getUserId();
            if (id == null)
                return BadRequest();
            return Ok(_auth.Register(user,id));
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(AuthRequest user)
        {
            
            return Ok(_auth.Login(user));
        }
        [HttpPost("guestToken")]
        public async Task<ActionResult<string>> guestToken()
        {

            return Ok(_auth.GuestToken());
        }

        [HttpGet("Auth")]
        [Authorize(Roles ="User")]
        public async Task<ActionResult<string>> TestAuth()
        {
            return Ok("works");
        }

        [HttpPost("renewJwt")]
        [Authorize(Roles = "User,Guest")]
        public async Task<ActionResult<string>> RenewJwt() {
            var authorization = Request.Headers[HeaderNames.Authorization];
            
            var newToken=_auth.RenewToken(authorization);
            if(newToken== null)
                return BadRequest();
            return Ok(newToken);


        }

        public string getUserId()
        {
            string uploaderId;
            var header = Request.Headers[HeaderNames.Authorization];
            if (AuthenticationHeaderValue.TryParse(header, out var headerValue))
            {
                var scheme = headerValue.Scheme;
                var parameter = headerValue.Parameter;
                uploaderId = _jwtToken.TokenToId(parameter);
                if (uploaderId == null)
                    return null;
            }
            else
                return null;

            return uploaderId;
        }



    }
}
