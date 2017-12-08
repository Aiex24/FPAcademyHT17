using PlaceholderYacht.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlaceholderYacht.Models.ViewModels;
using WindCatchersMathLibrary;

namespace PlaceholderYacht.Models
{
    public class BoatDbRepository : IBoatRepository
    {
        WindCatchersContext context;
        public BoatDbRepository(WindCatchersContext context)
        {
            this.context = context;
        }

        public BoatPageVM GetBoatPageVM(int BoatID)
        {
            throw new NotImplementedException();
        }

        public AccountBoatItemVM[] GetUsersBoatsByUID(string UID)
        {
            return context.Boat
                .Where(b => b.Uid == UID)
                .Select(b => new AccountBoatItemVM
                {
                    BoatID = b.Id,
                    BoatName = b.Boatname,
                    Manufacturer = b.Manufacturer,
                    ModelName = b.Modelname
                }).ToArray();
        }

        public void InterpolateVpp(AddBoatVM viewModel)
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

        public void SaveBoat(AddBoatVM model)
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
            context.Boat.Add(boat);
            context.SaveChanges();
        }
    }
}
