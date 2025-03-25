using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ve_xem_phim.Models
{
    public class User: IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Cart Cart { get; set; }
    }
}
