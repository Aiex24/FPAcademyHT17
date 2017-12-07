using PlaceholderYacht.Models.Entities;
using PlaceholderYacht.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WindCatchersMathLibrary;

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

        public void InterpolateVpp(AddBoatVM viewModel)
        {
            var twsEs = viewModel.VppList
                .Select(b => b.TWS)
                .Distinct();

            var VppListAsList = viewModel.VppList.ToList();
            foreach (var tws in twsEs)
            {
                var degreesAndKnots = viewModel.VppList
                    .Where(b => b.TWS == tws)
                    .Select(b => new
                    {
                        degrees = b.WindDirection,
                        knots = b.Knot
                    });
                var degreesOnly = degreesAndKnots
                    .Select(d => d.degrees);

                double[] interpolationInfo = new double[180 + 1];
                foreach (var degreeAndKnot in degreesAndKnots)
                {
                    interpolationInfo[degreeAndKnot.degrees] = degreeAndKnot.knots;
                }
                InterpolationLogic.VppInterpolation(interpolationInfo);
                //TODO: Fixa den här!
                for (int i = 0; i < interpolationInfo.Length; i++)
                {
                    if (!degreesOnly.Contains(i))
                    {
                        VppListAsList.Add(new AngleTwsKnotVM
                        {
                            TWS = tws,
                            WindDirection = i,
                            Knot = interpolationInfo[i]
                        });
                    }
                }
            }
            viewModel.VppList = VppListAsList.ToArray();
        }
    }
}
