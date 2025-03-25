using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ve_xem_phim.ViewModel
{
    public class RegisterVM
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        [PasswordPropertyText]
        public string Password { get; set; }
        [Required]
        [Compare("Password")]
        public string ComfirmPassword { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

    }
}
