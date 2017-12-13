using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlaceholderYacht.Models
{
    public class DistanceAndTime
    {
        public int TripDurationDays { get; set; }
        public int TripDurationHours { get; set; }
        public int TripDurationMinutes { get; set; }
        public DateTime ArrivalTime { get; set; }
        public DateTime DepartureTime { get; set; }
        public double TripDistanceKm { get; set; }
        public double InitialBearing { get; set; }
    }
}
