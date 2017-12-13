using PlaceholderYacht.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlaceholderYacht.Models.ViewModels;
using WindCatchersMathLibrary;
using System.Net.Http;
using Newtonsoft.Json;
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
            Boat boat = context.Boat.Include(b => b.VppuserInput).FirstOrDefault(b => b.Id == BoatID);

            var baten = new BoatPageVM
            {
                Boatname = boat.Boatname,
                Manufacturer = boat.Manufacturer,
                Modelname = boat.Modelname,
                BoatID = boat.Id,
                VppList = boat.VppuserInput.Select(v => new AngleTwsKnotVM
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

        public AccountBoatItemVM[] GetAllBoats()
        {
            return context.Boat
                .Select(b => new AccountBoatItemVM
                {
                    BoatID = b.Id,
                    BoatName = b.Boatname,
                    Manufacturer = b.Manufacturer,
                    ModelName = b.Modelname,
                    Owner = b.Uid
                }).ToArray();
        }

        public void InterpolateVpp(BoatPageVM viewModel)
        {
            //Plockar ut alla distinkta tws-värden.
            var twsEs = viewModel.VppList
                .Select(b => b.TWS)
                .Distinct();

            //Gör om VppList från array till lista för att enklare kunna lägga till värden.
            var VppListAsList = viewModel.VppList
                .Select(v => new AngleTwsKnotDBVM
                {
                    ID = v.ID,
                    TWS = v.TWS,
                    WindDegree = v.WindDegree,
                    Knot = v.Knot
                })
                .ToList();
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
                        VppListAsList.Add(new AngleTwsKnotDBVM
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

        public void UpdateBoat(BoatPageVM viewModel)
        {
            Boat boatToUpdate = context.Boat.Include(b => b.VppuserInput).Include(b => b.Vpp).FirstOrDefault(b => b.Id == viewModel.BoatID);
            boatToUpdate.Boatname = viewModel.Boatname;
            boatToUpdate.Manufacturer = viewModel.Manufacturer;
            boatToUpdate.Modelname = viewModel.Modelname;

            boatToUpdate.VppuserInput.Clear();
            foreach (var vpp in viewModel.VppList)
            {
                boatToUpdate.VppuserInput.Add(new VppuserInput
                {
                    Tws = vpp.TWS,
                    WindDegree = vpp.WindDegree,
                    Knot = vpp.Knot
                });
            }

            InterpolateVpp(viewModel);
            boatToUpdate.Vpp.Clear();
            foreach (var vpp in viewModel.VppDBList)
            {
                boatToUpdate.Vpp.Add(new Vpp
                {
                    Tws = vpp.TWS,
                    WindDegree = vpp.WindDegree,
                    Knot = vpp.Knot
                });
            }

            context.SaveChanges();
        }

        public BoatPageVM AddEmptyVPP(BoatPageVM boat)
        {
            boat.VppList = new AngleTwsKnotVM[] { new AngleTwsKnotVM { } };
            return boat;
        }

        public void SaveBoat(BoatPageVM model)
        {
            var boat = new Boat { Boatname = model.Boatname, Manufacturer = model.Manufacturer, Modelname = model.Modelname, Uid = model.Uid };
            foreach (var vpp in model.VppList)
            {
                boat.VppuserInput.Add(new VppuserInput
                {
                    Tws = vpp.TWS,
                    WindDegree = vpp.WindDegree,
                    Knot = vpp.Knot,
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

        public DistanceAndTime RouteCalculation(RouteCalculationJson routeCalculationJson)
        {
            int boatId = routeCalculationJson.BoatID;
            double startLatitude = routeCalculationJson.StartLatitude;
            double startLongitude = routeCalculationJson.StartLongitude;
            double endLatitude = routeCalculationJson.EndLatitude;
            double endLongitude = routeCalculationJson.EndLongitude;

            //Call function 
            double[] distance = CalcDistanceAndTime(startLatitude, startLongitude, endLatitude, endLongitude, boatId); //45 equals minimum angle TWS is defined for
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
            DistanceAndTime distanceAndTime = new DistanceAndTime
            {
                TripDistanceKm = distance[0],
                InitialBearing = distance[1],
                ArrivalTime = arrival,
                DepartureTime = departure,
                TripDurationDays = days,
                TripDurationHours = hours,
                TripDurationMinutes = minutes
            };
            return distanceAndTime;
        }
        public double[] CalcDistanceAndTime(double startLatitude, double startLongitude, double endLatitude, double endLongitude, int boatId)
        {
            double φ1 = Rad(startLatitude);  //latitude starting point in radian
            double λ1 = Rad(startLongitude); //longitude starting point in radian
            double φ2 = Rad(endLatitude);  //latitude final point in radian
            double λ2 = Rad(endLongitude); //longitude final point in radian
            double Δφ = φ2 - φ1;         //Difference in latitude
            double Δλ = λ2 - λ1;         //Difference in longitude
            string u = "km";             //Unit either [km] or [Nm]
            string m = "haversine";           //Haversine or tangential
            double Ra = 6378.1370;       //Equatorial radius
            double Rb = 6356.7523;       //Polar radius
            double L = 0;                  //Total length to destination in km
            double θ = 0;                //Initial bearing
            //int θmin = minAngle;

            // Calculates radius R(φ) [km] where R is a function of the center of the latitudes of latitude1 and latitude2
            double Bx = Math.Cos(φ2) * Math.Cos(λ2 - λ1);
            double By = Math.Cos(φ2) * Math.Sin(λ2 - λ1);
            double φm = Math.Atan2(Math.Sin(φ1) + Math.Sin(φ2),
                        Math.Sqrt((Math.Cos(φ1) + Bx) * (Math.Cos(φ1) + Bx) + By * By));

            double Rφ = Math.Sqrt((Math.Pow(Math.Pow(Ra, 2) * Math.Cos(φm), 2) +
                Math.Pow(Math.Pow(Rb, 2) * Math.Sin(φm), 2)) /
                (Math.Pow(Ra * Math.Cos(φm), 2) + Math.Pow(Rb * Math.Sin(φm), 2)));

            //Converts from kilometers to Nautical miles if unit = "Nm"
            if (u == "Nm") Rφ = Rφ / 1.852;

            // h = hav(L/R(φ)) where hav is the haversine formula => d = 2rsin^-1(√h)
            double h = Math.Pow(Math.Sin(Δφ / 2), 2) + Math.Cos(φ1) * Math.Cos(φ2) * Math.Pow(Math.Sin(Δλ / 2), 2);

            if (m == "haversine") L = Math.Sqrt(h) > 1 ? 2 * Rφ * Math.Asin(Math.Sqrt(1)) : 2 * Rφ * Math.Asin(Math.Sqrt(h));
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
            double resolution = 100; //Distance the program approximates the calculations without retrieve new data for wind from SMHI API and speed from the VPP database
            double Li = resolution;
            double Lrest = L % Li;
            double n = Math.Truncate(L / Li);
            double a, b, x, y, z, f, δ, φi, λi;
            int T = 0; // ΣTi , total time needed to reach the destination i = {1,2,3,...,n-1,n}
            double φ0 = φ1; //sets value to the initial starting point 
            double λ0 = λ1; //sets value to the initial starting point 
            θ = InitialBearing(φ1, λ1, φ2, λ2);
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
                T += GetTime(Degree(φ0), Degree(λ0), θ, Li, boatId);
                φ0 = φi;
                λ0 = λi;
            }
            T += GetTime(Degree(φ1), Degree(λ1), θ, Lrest, boatId);
            return new double[3] { L, θ, T };
        }

        //Calculates the initial bearing θ using tangential
        private double InitialBearing(double φ1, double λ1, double φ2, double λ2)
        {
            double Δφ = φ2 - φ1;
            double Δλ = λ2 - λ1;
            double y = Math.Sin(λ2 - λ1) * Math.Cos(φ2);
            double x = Math.Cos(φ1) * Math.Sin(φ2) - Math.Sin(φ1) * Math.Cos(φ2) * Math.Cos(Δλ);
            double θ = Degree(Math.Atan2(y, x));
            θ = (θ + 360) % 360;
            return θ;
        }

        public int GetTime(double latitude, double longitude, double bearing, double ΔL, int boatId)
        {
            SmhiApi smhi = new SmhiApi();

            // för att få async i .core än så länge
            Task.Run(async () =>
            {
                smhi = await CallWebApi(latitude, longitude);
            }).GetAwaiter().GetResult();


            //double θmin = (double)minAngle;
            double L = ΔL;
            double x0, x, x1, y0, y, y1, penalty;
            double θ = bearing;
            double θSMHI = smhi.timeSeries[0].parameters[3].values[0];
            double TwsAPI = smhi.timeSeries[0].parameters[4].values[0];


            double θrelative = Math.Abs(θSMHI - θ); //Difference in degrees between winddirection and bearing 

            θrelative = Math.Abs(θrelative - 2 * (θrelative % 180)); //Normalize the relative winddirection to fit the right side of the polardiagram
            θrelative = Math.Round(θrelative, 0);
            double v; // v [knot]
            double vms; //v [m/s]
            bool tacking = false;
            Boat boat = new Boat();
            int[] TWS = null;

            // för att få async i .core än så länge
            Task.Run(async () =>
            {
                boat = await GetTwsByBoatId(boatId);
            }).GetAwaiter().GetResult();

            // plocka ut tws:erna (int-array)
            TWS = boat.Vpp
                 .Select(t => t.Tws).Distinct()
                 .ToArray();

            var getMinAngle = boat.Vpp
                .Where(d => d.Knot != 0)
                .OrderBy(d => d.WindDegree)
                .Select(d => d.WindDegree).FirstOrDefault();


            //Penalty for tacking
            if (θrelative <= getMinAngle)
            {
                θrelative = getMinAngle;
                tacking = true;
            }

            //new int[] { 5, 6, 8, 10 }; //Hämta in för vilka definierade TWS databasen har VPP-diagram för (i detta fall 5,6,8 och 10 m/s)

            if (TWS.SingleOrDefault(t => t == TwsAPI) != 0) //Will be executed if there is a diagram that correlates to the exact windspeed retrieved from the API
            {

                var knot = boat.Vpp
                    .Where(t => t.Tws == TwsAPI && t.WindDegree == θrelative)
                    .Select(t => t.Knot).SingleOrDefault();
                //█████Get v from table where TWS = TwsAPI at position θrelative
                v = (double)knot;
            }
            else if (TwsAPI < TWS.Min()) //The actual windspeed is lower than the lowest defined VPP-diagram
            {
                x0 = TwsAPI;
                x = TWS[0];
                x1 = TWS[1];

                var knotY = boat.Vpp
                    .Where(t => t.Tws == x && t.WindDegree == θrelative)
                    .Select(t => t.Knot).SingleOrDefault();
                var knotY1 = boat.Vpp
                    .Where(t => t.Tws == x1 && t.WindDegree == θrelative)
                    .Select(t => t.Knot).SingleOrDefault();

                y = (double)knotY;
                //5.4; //█████ Replace 5.4 with value from database where ID = TWS[0] at position θrelative█████
                y1 = (double)knotY1;
                //6.2; //█████ Replace 6.2 with value from database where ID = TWS[1] at position θrelative█████
                v = (x0 * y + x * y1 - x0 * y1 - x1 * y) / (x - x1);
            }
            else if (TwsAPI > TWS.Max())//The actual windspeed is higher than the highest defined VPP-diagram
            {
                x0 = TWS[TWS.Length - 2];
                x = TWS[TWS.Length - 1];
                x1 = TwsAPI;

                var knotY0 = boat.VppuserInput
                    .Where(t => t.Tws == x0 && t.WindDegree == θrelative)
                    .Select(t => t.Knot).SingleOrDefault();
                var knotY1 = boat.VppuserInput
                    .Where(t => t.Tws == x && t.WindDegree == θrelative)
                    .Select(t => t.Knot).SingleOrDefault();

                y0 = (double)knotY0; //█████ Replace 8.4 with value from database where ID = TWS[TWS.Length - 2] at position θrelative█████
                y1 = (double)knotY1; //█████ Replace 9.4 with value from database where ID = TWS[TWS.Length - 1] at position θrelative█████
                v = (x * y0 - x * y1 + x0 * y1 - x1 * y0) / (x0 - x1);
            }
            else
            {
                x0 = TWS.TakeWhile(p => p < TwsAPI).Last();
                x = TwsAPI;
                x1 = TWS.SkipWhile(p => p <= TwsAPI).First();

                var knotY0 = boat.Vpp
                    .Where(t => t.Tws == x0 && t.WindDegree == θrelative)
                    .Select(t => t.Knot).SingleOrDefault();
                var knotY1 = boat.Vpp
                    .Where(t => t.Tws == x1 && t.WindDegree == θrelative)
                    .Select(t => t.Knot).SingleOrDefault();


                y0 = (double)knotY0; //█████ Replace 6.4 with value from database where ID = TWS[TWS.Length - 2] at position θrelative█████
                y1 = (double)knotY1; //█████ Replace 8.4 with value from database where ID = TWS[TWS.Length - 1] at position θrelative█████
                v = (x * y0 - x * y1 + x0 * y1 - x1 * y0) / (x0 - x1);
            }            
            if (tacking)
            {
                penalty = (L / (Math.Cos(Rad(θrelative))))/L;
                //(1 / Math.Cos(Rad(θrelative)));
            }
            else penalty = 1;
            tacking = false;
            vms = v * 1852 / 3600;
            var i = (int)Math.Round((L * penalty * 1000 / vms), 0); //[s]
            return i;
        }

        // måste vara async, databasen är hyfsat seg
        public async Task<Boat> GetTwsByBoatId(int boatId)
        {
            var boat = await context.Boat
                .Include(b => b.VppuserInput)
                .Include(b => b.Vpp)
                .FirstOrDefaultAsync(b => b.Id == boatId);

            //var tws = boat.Vpp
            //     .Select(v => v.Tws)
            //     .Distinct().ToArray();

            return boat;
        }

        private double Rad(double valueToCast)
        {
            return valueToCast * Math.PI / 180;
        }
        private double Degree(double valueToCast)
        {
            return valueToCast * 180 / Math.PI;
        }

        // en äkta skön api-metod
        private async Task<SmhiApi> CallWebApi(double longitude, double latitude)
        {
            var url = @"http://opendata-download-metanalys.smhi.se/api/category/mesan1g/version/1/geotype/point/lon/"+ Math.Round(latitude, 6)+"/lat/"+Math.Round(longitude, 6)+"/data.json";
            var client = new HttpClient();

            var json = await client.GetStringAsync(url);
            var smhi = JsonConvert.DeserializeObject<SmhiApi>(json);
            return smhi;
        }

        public void DeleteBoat(int boatID)
        {
            //Ta bort alla rader i tabellerna VPP, VPPuserinput och boats med boatID
            VppuserInput[] vppsUser = context.VppuserInput.Where(v => v.BoatId == boatID).ToArray();
            context.VppuserInput.RemoveRange(vppsUser);

            Vpp[] vpps = context.Vpp.Where(v => v.BoatId == boatID).ToArray();
            context.Vpp.RemoveRange(vpps);

            context.Boat.Remove(context.Boat.FirstOrDefault(b => b.Id == boatID));
            context.SaveChanges();
        }
    }
}
