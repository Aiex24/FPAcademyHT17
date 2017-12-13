using System.Threading.Tasks;
using PlaceholderYacht.Models.ViewModels;
using PlaceholderYacht.Models.Entities;

namespace PlaceholderYacht.Models
{
    public interface IBoatRepository
    {
        AccountBoatItemVM[] GetUsersBoatsByUID(string UID);
        AccountBoatItemVM[] GetAllBoats();
        void InterpolateVpp(BoatPageVM boatVM);
        void SaveBoat(BoatPageVM model);
        BoatPageVM GetBoatPageVM(int BoatID);
        Task<Boat> GetTwsByBoatId(int v);
        int GetTime(double latitude, double longitude, double bearing, double ΔL, int minAngle);
        BoatPageVM AddEmptyVPP(BoatPageVM boat);
        void UpdateBoat(BoatPageVM model);
        void DeleteBoat(int id);
        double[] CalcDistance(double latitude1, double longitude1, double latitude2, double longitude2, string unit, string method, int minAngle);
    }
}