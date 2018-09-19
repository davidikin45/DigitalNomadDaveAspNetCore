using System.ComponentModel.DataAnnotations;

namespace DND.Common.Dtos.Authentication
{
    public class AuthenticateDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
