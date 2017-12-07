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
            new Boat{Id = 1, Uid = "c835a93e-eee9-4c37-bb76-3b05d49d44f2", Manufacturer = "Sigma", Modelname = "Nimbus2000", Boatname = "Testbåt2" }
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
    }
}
