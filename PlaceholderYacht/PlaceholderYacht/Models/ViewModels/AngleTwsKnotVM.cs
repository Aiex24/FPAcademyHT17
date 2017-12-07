using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlaceholderYacht.Models.ViewModels
{
    [Bind(Prefix =nameof(BoatPageVM.VppList))]
    public class AngleTwsKnotVM
    {
        public int WindDirection { get; set; }
        public double TWS { get; set; }
        public double Knot { get; set; }
    }
}
