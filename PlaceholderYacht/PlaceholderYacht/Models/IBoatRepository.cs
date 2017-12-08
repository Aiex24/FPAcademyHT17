using PlaceholderYacht.Models.ViewModels;

namespace PlaceholderYacht.Models
{
    public interface IBoatRepository
    {
        AccountBoatItemVM[] GetUsersBoatsByUID(string UID);
        void InterpolateVpp(BoatPageVM boatVM);
        void SaveBoat(BoatPageVM model);
        BoatPageVM GetBoatPageVM(int BoatID);
    }
}