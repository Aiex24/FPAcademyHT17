using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlaceholderYacht.Models.ViewModels
{
    [Bind(Prefix = nameof(AccountUserpageVM.BoatItem))]
    public class AccountBoatItemVM
    {
        public int BoatID { get; set; }
        public string ModelName { get; set; }
        public string Manufacturer { get; set; }
        public string BoatName { get; set; }
    }
}
