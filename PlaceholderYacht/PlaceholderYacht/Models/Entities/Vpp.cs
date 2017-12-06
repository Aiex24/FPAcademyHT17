using System;
using System.Collections.Generic;

namespace PlaceholderYacht.Models.Entities
{
    public partial class Vpp
    {
        public int Id { get; set; }
        public int? BoatId { get; set; }
        public int Tws { get; set; }
        public int WindDegree { get; set; }
        public decimal Knot { get; set; }

        public Boat Boat { get; set; }
    }
}
