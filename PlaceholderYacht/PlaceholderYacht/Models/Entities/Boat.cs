using System;
using System.Collections.Generic;

namespace PlaceholderYacht.Models.Entities
{
    public partial class Boat
    {
        public Boat()
        {
            Vpp = new HashSet<Vpp>();
            VppuserInput = new HashSet<VppuserInput>();
        }

        public int Id { get; set; }
        public string Uid { get; set; }
        public string Modelname { get; set; }
        public string Manufacturer { get; set; }
        public string Boatname { get; set; }

        public ICollection<Vpp> Vpp { get; set; }
        public ICollection<VppuserInput> VppuserInput { get; set; }
    }
}
