using System.ComponentModel.DataAnnotations;

namespace api.Models.Users
{
    public class AuthRequest
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }

    }
}
