using System.ComponentModel.DataAnnotations;

namespace DND.Common.Dtos.Authentication
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
