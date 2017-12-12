using PlaceholderYacht.Models.Entities;
using PlaceholderYacht.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WindCatchersMathLibrary;

namespace PlaceholderYacht.Models
{
    public class BoatDevRepository /*: IBoatRepository*/
    {
        static List<Vpp> vppList = new List<Vpp> {
            new Vpp { Id = 1, Knot = 10, Tws = 2, WindDegree = 45 },
            new Vpp{ Id = 2, Knot = 8, Tws = 2, WindDegree=90 }
        };

        static List<Boat> boats = new List<Boat>
        {
            new Boat{Id = 1, Uid = "c835a93e-eee9-4c37-bb76-3b05d49d44f2", Modelname = "Nacra17", Manufacturer = "NACRA", Boatname = "Testbåt" },
            new Boat{Id = 2, Uid = "c835a93e-eee9-4c37-bb76-3b05d49d44f2", Manufacturer = "Sigma", Modelname = "Nimbus2000", Boatname = "Testbåt2"}
        };

        static BoatDevRepository()
        {
            vppList[0].Boat = boats[0];
            vppList[0].BoatId = boats[0].Id;
            vppList[1].Boat = boats[0];
            vppList[1].BoatId = boats[0].Id;

            boats[0].Vpp.Add(vppList[0]);
            boats[0].Vpp.Add(vppList[1]);
        }

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

        public void InterpolateVpp(BoatPageVM viewModel)
        {
            //Plockar ut alla distinkta tws-värden.
            var twsEs = viewModel.VppList
                .Select(b => b.TWS)
                .Distinct();

            //Gör om VppList från array till lista för att enklare kunna lägga till värden.
            var VppListAsList = viewModel.VppList.ToList();
            //Anropar interpolationslogik för varje unik tws som lagts till.
            foreach (var tws in twsEs)
            {
                //De här värdena används när vi interpolerar.
                var degreesAndKnots = viewModel.VppList
                    .Where(b => b.TWS == tws)
                    .Select(b => new
                    {
                        degrees = b.WindDegree,
                        knots = b.Knot
                    });
                //Den här används i sista for-loopen för att inte använda samma värden två gånger.
                var degreesOnly = degreesAndKnots
                    .Select(d => d.degrees);
                //Krävs för interpolationsmetoden, se klassbiblioteket för mer info.
                decimal[] interpolationInfo = new decimal[180 + 1];
                foreach (var degreeAndKnot in degreesAndKnots)
                {
                    interpolationInfo[degreeAndKnot.degrees] = degreeAndKnot.knots;
                }
                InterpolationLogic.VppInterpolation(interpolationInfo);
                //Lägger till de nya värden som fåtts ut i Vpp-listan.
                for (int i = 0; i < interpolationInfo.Length; i++)
                {
                    if (!degreesOnly.Contains(i))
                    {
                        VppListAsList.Add(new AngleTwsKnotVM
                        {
                            TWS = tws,
                            WindDegree = i,
                            Knot = interpolationInfo[i]
                        });
                    }
                }
            }
            //Ersätter den lista som skickades in med den nya som innehåller värden för alla grader vi behöver.
            viewModel.VppList = VppListAsList.ToArray();
        }

        public void SaveBoat(BoatPageVM model)
        {
            var boat = new Boat { Boatname = model.Boatname, Manufacturer = model.Manufacturer, Modelname = model.Modelname };
            foreach (var vpp in model.VppList)
            {
                boat.Vpp.Add(new Vpp
                {
                    Tws = vpp.TWS,
                    WindDegree = vpp.WindDegree,
                    Knot = vpp.Knot
                });
            }
        }

        public BoatPageVM GetBoatPageVM(int BoatID)
        {
            var boat = boats.FirstOrDefault(b => b.Id == BoatID);
            return new BoatPageVM
            {
                Boatname = boat.Boatname,
                Manufacturer = boat.Manufacturer,
                Modelname = boat.Modelname,
                BoatID = boat.Id,
                VppList = boat.Vpp.Select(v => new AngleTwsKnotVM
                {
                    Knot = v.Knot,
                    TWS = v.Tws,
                    WindDegree = v.WindDegree,
                    ID = v.Id

                }).ToArray()
            };
        }

        public void UpdateBoat(BoatPageVM viewModel)
        {
            Boat boatToUpdate = boats.FirstOrDefault(b => b.Id == viewModel.BoatID);
            boatToUpdate.Boatname = viewModel.Boatname;
            boatToUpdate.Manufacturer = viewModel.Manufacturer;
            boatToUpdate.Modelname = viewModel.Modelname;

            foreach (AngleTwsKnotVM item in viewModel.VppList)
            {
                //Uppdatera VPP'n om den redan finns.
                if (item.ID > 0)
                {
                    var boatVPP = boatToUpdate.Vpp.FirstOrDefault(v => v.Id == item.ID);
                    boatVPP.Knot = item.Knot;
                    boatVPP.Tws = item.TWS;
                    boatVPP.WindDegree = item.WindDegree;
                }
                //Om VPP'n inte finns så lägg till.
                else
                {
                    boatToUpdate.Vpp.Add(new Vpp
                    {
                        Boat = boatToUpdate,
                        BoatId = viewModel.BoatID,
                        Knot = item.Knot,
                        Tws = item.TWS,
                        WindDegree = item.WindDegree,
                        Id = vppList[vppList.Count - 1].Id
                    });
                }
            }
        }

        public BoatPageVM AddEmptyVPP(BoatPageVM boat)
        {
            boat.VppList = new AngleTwsKnotVM[] { new AngleTwsKnotVM { } };
            return boat;
        }

        public Task<Boat> GetTwsByBoatId(int v)
        {
            throw new NotImplementedException();
        }

        public int GetTime(double latitude, double longitude, double bearing, double ΔL, int minAngle)
        {
            throw new NotImplementedException();
        }

        public double[] CalcDistance(double latitude1, double longitude1, double latitude2, double longitude2, string unit, string method, int minAngle)
        {
            throw new NotImplementedException();
        }
    }
}
