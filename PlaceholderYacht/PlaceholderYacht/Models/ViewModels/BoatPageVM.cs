using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PlaceholderYacht.Models.ViewModels
{
    public class BoatPageVM
    {
        public int BoatID { get; set; }
        public string Uid { get; set; }
        [Required(ErrorMessage = "Please enter a Model Name")]
        public string Modelname { get; set; }
        [Required(ErrorMessage = "Please enter a Manufacturer")]
        public string Manufacturer { get; set; }
        [Required(ErrorMessage = "Please enter the name of the boat")]
        public string Boatname { get; set; }
        public int MinAngle { get; set; }
        public AngleTwsKnotVM[] VppList { get; set; }
        public AngleTwsKnotDBVM[] VppDBList { get; set; }
    }
}
