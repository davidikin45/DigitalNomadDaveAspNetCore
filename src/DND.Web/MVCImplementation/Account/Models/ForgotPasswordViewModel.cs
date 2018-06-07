using System.ComponentModel.DataAnnotations;

namespace DND.Web.MVCImplementation.Account.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
