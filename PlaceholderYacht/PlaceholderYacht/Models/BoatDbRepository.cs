using PlaceholderYacht.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlaceholderYacht.Models.ViewModels;
using WindCatchersMathLibrary;
using Microsoft.EntityFrameworkCore;

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
            Boat boat = context.Boat.Include(b => b.Vpp).FirstOrDefault(b => b.Id == BoatID);

            var baten = new BoatPageVM
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
            return baten;
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
            //Lägger in de interpolerade värdena i databaslistan som aldrig ses av användaren
            viewModel.VppDBList = VppListAsList.ToArray();
        }

        public void SaveBoat(BoatPageVM model)
        {
            var boat = new Boat { Boatname = model.Boatname, Manufacturer = model.Manufacturer, Modelname = model.Modelname };
            foreach (var vpp in model.VppList)
            {
                boat.VppuserInput.Add(new VppuserInput
                {
                    Tws = vpp.TWS,
                    WindDegree = vpp.WindDegree,
                    Knot = vpp.Knot
                });
            }

            foreach (var vpp in model.VppDBList)
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
        //Här börjar Calcmetoderna. Insatta från sandbox2 måndag 11/12

        public int RouteCalculation(int BoatID)
        {
            string unit = "km";
            string method = "haversine";
            //Start coordinate 
            double latitude1 = 59.39496;
            double longitude1 = 19.33388;
            //Goal coordinate
            double latitude2 = 57.67185;
            double longitude2 = 18.20489;
            int minAngle = 45;

            //Call function 
            double[] distance = CalcDistance(latitude1, longitude1, latitude2, longitude2, unit, method, minAngle); //45 equals minimum angle TWS is defined for
                                                                                                                    //Result
            double T = distance[2];
            DateTime departure = DateTime.Now;
            DateTime arrival = departure.AddSeconds(T);
            //Console.WriteLine("You will arrive at " + arrival);
            int days = (arrival - departure).Days;
            int hours = (arrival - departure).Hours;
            int minutes = (arrival - departure).Minutes;
            //Console.WriteLine($"This trip took {days}days, {hours}hours and {minutes}minutes");
            //    Console.WriteLine($"{method.ToUpper()} Distance: {Math.Round(distance[0], 3)} [{unit}] Bearing: {Math.Round(distance[1], 0)}°");
            return 1;
        }

        private static double[] CalcDistance(double latitude1, double longitude1, double latitude2, double longitude2, string unit, string method, int minAngle)
        {
            double φ1 = Rad(latitude1);  //latitude starting point in radian
            double λ1 = Rad(longitude1); //longitude starting point in radian
            double φ2 = Rad(latitude2);  //latitude final point in radian
            double λ2 = Rad(longitude2); //longitude final point in radian
            double Δφ = φ2 - φ1;         //Difference in latitude
            double Δλ = λ2 - λ1;         //Difference in longitude
            string u = unit;             //Unit either [km] or [Nm]
            string m = method;           //Haversine or tangential
            double φa = φ1 + (Δφ / 2);   //Average latitude
            double Ra = 6378.1370;       //Equatorial radius
            double Rb = 6356.7523;       //Polar radius
            double L = 0;                  //Total length to destination in km
            double θ = 0;                //Initial bearing
            int θmin = minAngle;
            // Calculates radius R(φ) [km] where R is a function of the linear average of the latitudes of latitude1 and latitude2
            double Rφ = Math.Sqrt((Math.Pow(Math.Pow(Ra, 2) * Math.Cos(φa), 2) + Math.Pow(Math.Pow(Rb, 2) * Math.Sin(φa), 2)) /
                (Math.Pow(Ra * Math.Cos(φa), 2) + Math.Pow(Rb * Math.Sin(φa), 2)));
            //Converts from kilometers to Nautical miles if unit = "Nm"
            if (u == "Nm") Rφ = Rφ / 1.852;

            // h = hav(L/R(φ)) where hav is the haversine formula => d = 2rsin^-1(√h)
            double h = Math.Pow(Math.Sin(Δφ / 2), 2) + Math.Cos(φ1) * Math.Cos(φ2) * Math.Pow(Math.Sin(Δλ / 2), 2);

            if (m == "haversine") L = 2 * Rφ * Math.Sqrt(h);
            else if (m == "tangential") { double c = 2 * Math.Atan2(Math.Sqrt(h), Math.Sqrt(1 - h)); L = Rφ * c; }
            //else Console.WriteLine("[Error] Wrong method requested");

            //Equations for Rhumb-lines (constant compass direction) which gives a longer distance compared to a non static bearing https://en.wikipedia.org/wiki/Rhumb_line
            double Δψ = Math.Log(Math.Tan(Math.PI / 4 + φ2 / 2) / Math.Tan(Math.PI / 4 + φ1 / 2));
            double q = Math.Abs(Δψ) > 10e-12 ? Δφ / Δψ : Math.Cos(φ1);
            // Chooses the shortest loxodrome over the 180th meridian
            if (Math.Abs(Δλ) > Math.PI) Δλ = Δλ > 0 ? -(2 * Math.PI - Δλ) : (2 * Math.PI + Δλ);
            double dist = Math.Sqrt(Δφ * Δφ + q * q * Δλ * Δλ) * Rφ;
            Δψ = Math.Log(Math.Tan(Math.PI / 4 + φ2 / 2) / Math.Tan(Math.PI / 4 + φ1 / 2));
            var brng = Degree(Math.Atan2(Δλ, Δψ));
            brng = (brng + 360) % 360;

            //If this value is set to 1, the resolution will be 1 km
            double resolution = 1; //Distance the program approximates the calculations without retrieve new data for wind from SMHI API and speed from the VPP database
            double Li = resolution;
            double Lrest = L % Li;
            double n = Math.Truncate(L / Li);
            double a, b, x, y, z, f, δ, φi, λi;
            int T = 0; // ΣTi , total time needed to reach the destination i = {1,2,3,...,n-1,n}
            double φ0 = φ1; //sets value to the initial starting point 
            double λ0 = λ1; //sets value to the initial starting point 
            for (int i = 1; i < n + 1; i++)
            {
                f = Li * i / L; //fraction of total distance
                δ = L / Rφ; //Angular distance
                a = Math.Sin((1 - (f)) * δ) / (Math.Sin(δ));
                b = Math.Sin(f * δ) / Math.Sin(δ);
                x = a * Math.Cos(φ1) * Math.Cos(λ1) + b * Math.Cos(φ2) * Math.Cos(λ2);
                y = a * Math.Cos(φ1) * Math.Sin(λ1) + b * Math.Cos(φ2) * Math.Sin(λ2);
                z = a * Math.Sin(φ1) + b * Math.Sin(φ2);
                φi = Math.Atan2(z, Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2)));
                λi = Math.Atan2(y, x);
                θ = InitialBearing(φ0, λ0, φi, λi);
                T += GetTime(Degree(φ0), Degree(λ0), θ, Li, θmin);
                φ0 = φi;
                λ0 = λi;
            }
            T += GetTime(Degree(φ1), Degree(λ1), θ, Lrest, minAngle);
            return new double[3] { L, θ, T };
        }

        //Calculates the initial bearing θ using tangential
        private static double InitialBearing(double φ1, double λ1, double φ2, double λ2)
        {
            double Δφ = φ2 - φ1;
            double Δλ = λ2 - λ1;
            double y = Math.Sin(λ2 - λ1) * Math.Cos(φ2);
            double x = Math.Cos(φ1) * Math.Sin(φ2) - Math.Sin(φ1) * Math.Cos(φ2) * Math.Cos(Δλ);
            double θ = Degree(Math.Atan2(y, x));
            θ = (θ + 360) % 360;
            return θ;
        }

        private static int GetTime(double latitude, double longitude, double bearing, double ΔL, int minAngle)
        {
            double θmin = (double)minAngle;
            double L = ΔL;
            double x0, x, x1, y0, y, y1, penalty;
            double θ = 0;
            double θSMHI = 140; //█████vindriktning 140 [grader] ska ersättas med värde från API i (latitude,longitude) █████
            double TwsAPI = 5; //█████vindstyrka  5.1 [m/s] ska ersättas med värde från API i (latitude,longitude) █████
            double θrelative = Math.Abs(θSMHI - θ); //Difference in degrees between winddirection and bearing 

            θrelative = Math.Abs(θrelative - 2 * (θrelative % 180)); //Normalize the relative winddirection to fit the right side of the polardiagram
            double v; // v [knot]
            double vms; //v [m/s]
            bool tacking = false;

            //Penalty for tacking
            if (θrelative < 0)
            {
                θrelative = θmin;
                tacking = true;
            }

            int[] TWS = new int[] { 5, 6, 8, 10 }; //Hämta in för vilka definierade TWS databasen har VPP-diagram för (i detta fall 5,6,8 och 10 m/s)

            if (TWS.SingleOrDefault(t => t == TwsAPI) != 0) //Will be executed if there is a diagram that correlates to the exact windspeed retrieved from the API
            {
                //█████Get v from table where TWS = TwsAPI at position θrelative
                v = 2;
            }
            else if (TwsAPI < TWS.Min()) //The actual windspeed is lower than the lowest defined VPP-diagram
            {
                x0 = TwsAPI;
                x = TWS[0];
                x1 = TWS[1];
                y = 5.4; //█████ Replace 5.4 with value from database where ID = TWS[0] at position θrelative█████
                y1 = 6.2; //█████ Replace 6.2 with value from database where ID = TWS[1] at position θrelative█████
                v = (x0 * y + x * y1 - x0 * y1 - x1 * y) / (x - x1);
            }
            else if (TwsAPI > TWS.Max())//The actual windspeed is higher than the highest defined VPP-diagram
            {

                x0 = TWS[TWS.Length - 2];
                x = TWS[TWS.Length - 1];
                x1 = TwsAPI;
                y0 = 6.2; //█████ Replace 8.4 with value from database where ID = TWS[TWS.Length - 2] at position θrelative█████
                y1 = 8.8; //█████ Replace 9.4 with value from database where ID = TWS[TWS.Length - 1] at position θrelative█████
                v = (x * y0 - x * y1 + x0 * y1 - x1 * y0) / (x0 - x1);
            }
            else
            {
                x0 = TWS.TakeWhile(p => p < TwsAPI).Last();
                x = TwsAPI;
                x1 = TWS.SkipWhile(p => p <= TwsAPI).First();
                y0 = 6.4; //█████ Replace 6.4 with value from database where ID = TWS[TWS.Length - 2] at position θrelative█████
                y1 = 8.4; //█████ Replace 8.4 with value from database where ID = TWS[TWS.Length - 1] at position θrelative█████
                v = (x * y0 - x * y1 + x0 * y1 - x1 * y0) / (x0 - x1);

            }
            if (tacking) penalty = (1 / Math.Cos(Math.PI / 4));
            else penalty = 1;
            tacking = false;
            vms = v * 1852 / 3600;
            return (int)Math.Round((L * penalty * 1000 / vms), 0); //[s]

        }

        private static double Rad(double valueToCast)
        {
            return valueToCast * Math.PI / 180;
        }

        private static double Degree(double valueToCast)
        {
            return valueToCast * 180 / Math.PI;
        }

        public void UpdateBoat(BoatPageVM viewModel)
        {
            Boat boatToUpdate = context.Boat.Include(b => b.Vpp).FirstOrDefault(b => b.Id == viewModel.BoatID);
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
                        WindDegree = item.WindDegree
                    });
                }
                context.SaveChanges();
            }
        }

        public BoatPageVM AddEmptyVPP(BoatPageVM boat)
        {
            boat.VppList = new AngleTwsKnotVM[] { new AngleTwsKnotVM { } };
            return boat;
        }
    }
}
