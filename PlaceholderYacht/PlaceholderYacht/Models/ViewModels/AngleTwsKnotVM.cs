using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PlaceholderYacht.Models.ViewModels
{
    [Bind(Prefix =nameof(BoatPageVM.VppList))]
    public class AngleTwsKnotVM
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Please enter the wind degree")]
        [Range(0, 180)]
        public int WindDegree { get; set; }
        [Required(ErrorMessage = "Please enter the true wind speed")]
        public int TWS { get; set; }
        [Required(ErrorMessage = "Please enter the speed in knots")]
        public decimal Knot { get; set; }
    }
}
