using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PlaceholderYacht.Models.ViewModels
{
    [Bind(Prefix =nameof(AddBoatVM.VppList))]
    public class AngleTwsKnotVM
    {
        [Required(ErrorMessage = "Please enter the wind degree")]
        public int WindDegree { get; set; }
        [Required(ErrorMessage = "Please enter the true wind speed")]
        public int TWS { get; set; }
        [Required(ErrorMessage = "Please enter the speed in knots")]
        public decimal Knot { get; set; }
    }
}
