using PlaceholderYacht.Models.ViewModels;

namespace PlaceholderYacht.Models
{
    public interface IBoatRepository
    {
        AccountBoatItemVM[] GetUsersBoatsByUID(string UID);
        void InterpolateVpp(AddBoatVM boatVM);
        void SaveBoat(AddBoatVM model);
        BoatPageVM GetBoatPageVM(int BoatID);
    }
}