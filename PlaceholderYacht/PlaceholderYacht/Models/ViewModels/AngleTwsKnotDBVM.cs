using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlaceholderYacht.Models.ViewModels
{
    [Bind(Prefix = nameof(BoatPageVM.VppDBList))]
    public class AngleTwsKnotDBVM
    {
        public int ID { get; set; }
        public int WindDegree { get; set; }
        public int TWS { get; set; }
        public decimal Knot { get; set; }
    }
}
