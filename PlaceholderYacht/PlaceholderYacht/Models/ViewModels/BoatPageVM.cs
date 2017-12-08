using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlaceholderYacht.Models.ViewModels
{
    public class BoatPageVM
    {
        public string Uid { get; set; }
        public string Modelname { get; set; }
        public string Manufacturer { get; set; }
        public string Boatname { get; set; }
        public int MinAngle { get; set; }
        public AngleTwsKnotVM[] VppList { get; set; }
    }
}
