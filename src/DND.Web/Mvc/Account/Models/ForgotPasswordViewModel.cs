using System.ComponentModel.DataAnnotations;

namespace DND.Web.Mvc.Account.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
