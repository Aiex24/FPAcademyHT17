using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlaceholderYacht.Models.ViewModels
{
    [Bind(Prefix = nameof(AccountUserpageVM.UserItem))]
    public class AccountUserItemVM
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string UserID { get; set; }
        public bool Admin { get; set; }
    }
}
