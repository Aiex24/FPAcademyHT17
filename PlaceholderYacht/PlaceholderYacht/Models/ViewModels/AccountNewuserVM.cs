using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PlaceholderYacht.Models.ViewModels
{
    public class AccountNewuserVM
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        [EmailAddress]
        public string Email { get; set; }
    }
}
