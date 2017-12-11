using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlaceholderYacht.Models
{
    public class SmhiApi
    {
        public string approvedTime { get; set; }
        public Geometry geometry { get; set; }
        public string referenceTime { get; set; }
        public TimeSeries[] timeSeries { get; set; }
    }
}
