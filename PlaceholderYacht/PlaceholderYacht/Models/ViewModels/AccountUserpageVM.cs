using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PlaceholderYacht.Models.ViewModels
{
    public class AccountUserpageVM
    {
        [Required]
        public string UserName { get; set; }
        [EmailAddress]
        public string Email { get; set; }

        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public AccountBoatItemVM[] BoatItem { get; set; }
    }
}
