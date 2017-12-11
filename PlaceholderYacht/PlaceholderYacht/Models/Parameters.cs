using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlaceholderYacht.Models
{
    public class Parameters
    {
        public int level { get; set; }
        public string levelType { get; set; }
        public string name { get; set; }
        public string unit { get; set; }
        public double[] values { get; set; }
    }
}
