using System.Threading.Tasks;
using PlaceholderYacht.Models.ViewModels;
using PlaceholderYacht.Models.Entities;

namespace PlaceholderYacht.Models
{
    public interface IBoatRepository
    {
        AccountBoatItemVM[] GetUsersBoatsByUID(string UID);
        void InterpolateVpp(BoatPageVM boatVM);
        void SaveBoat(BoatPageVM model);
        BoatPageVM GetBoatPageVM(int BoatID);
        Task<Boat> GetTwsByBoatId(int v);
        int GetTime(double latitude, double longitude, double bearing, double ΔL, int boatId);
        BoatPageVM AddEmptyVPP(BoatPageVM boat);
        void UpdateBoat(BoatPageVM model);
        double[] CalcDistanceAndTime(double latitude1, double longitude1, double latitude2, double longitude2, int boatId);
        DistanceAndTime RouteCalculation(RouteCalculationJson jsonObject);
    }
}