using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlaceholderYacht.Models.ViewModels
{
    [Bind(Prefix =nameof(AddBoatVM.VppList))]
    public class AngleTwsKnotVM
    {
        public int WindDegree { get; set; }
        public int TWS { get; set; }
        public decimal Knot { get; set; }
    }
}
