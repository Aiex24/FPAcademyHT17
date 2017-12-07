using PlaceholderYacht.Models.Entities;
using PlaceholderYacht.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlaceholderYacht.Models
{
    public class BoatDevRepository : IBoatRepository
    {
        static List<Boat> boats = new List<Boat>
        {
            new Boat{Id = 1, Uid = "c835a93e-eee9-4c37-bb76-3b05d49d44f2", Modelname = "Nacra17", Manufacturer = "NACRA", Boatname = "Testbåt" },
            new Boat{Id = 2, Uid = "c835a93e-eee9-4c37-bb76-3b05d49d44f2", Manufacturer = "Sigma", Modelname = "Nimbus2000", Boatname = "Testbåt2" }
        };

        static List<Vpp> vpp = new List<Vpp>
        {
            new Vpp{ Id = 1, Boat = boats[0], BoatId = 1, Knot = 10, Tws = 2, WindDegree=45 },
            new Vpp{ Id = 1, Boat = boats[0], BoatId = 1, Knot = 8, Tws = 2, WindDegree=90 }
        };

        public AccountBoatItemVM[] GetUsersBoatsByUID(string UID)
        {
            return boats.Where(b => b.Uid == UID).Select(bo => new AccountBoatItemVM
            {
                BoatID = bo.Id,
                ModelName = bo.Modelname,
                Manufacturer = bo.Manufacturer,
                BoatName = bo.Boatname
            }).ToArray();
        }

        public BoatPageVM GetBoatPageVM(int BoatID)
        {
            var boat = boats.FirstOrDefault(b => b.Id == BoatID);
            return new BoatPageVM
            {
                Boatname = boat.Boatname,
                Manufacturer = boat.Manufacturer,
                Modelname = boat.Modelname,
                VppList = vpp.Select(v => new AngleTwsKnotVM
                {
                    Knot = (double)v.Knot,
                    TWS = (double)v.Knot,
                    WindDirection = v.WindDegree
                }).ToArray(),
                VppCount = vpp.Where(v => v.BoatId == BoatID).Count()
            };
        }
    }
}
