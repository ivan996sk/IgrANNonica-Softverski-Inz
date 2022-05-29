using System.ComponentModel.DataAnnotations;

namespace api.Models.Users
{
    public class RegisterRequest
    {
        [Required]
        public string firstName { get; set; }

        [Required] 
        public string lastName { get; set; }

        [Required] 
        public string username { get; set; }

        [Required]
        public string email { get; set; }

        [Required]
        public string password { get; set; }

    }
}
