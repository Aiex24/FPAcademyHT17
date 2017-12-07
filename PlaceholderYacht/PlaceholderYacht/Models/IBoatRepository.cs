using PlaceholderYacht.Models.ViewModels;

namespace PlaceholderYacht.Models
{
    public interface IBoatRepository
    {
        AccountBoatItemVM[] GetUsersBoatsByUID(string UID);
        void InterpolateVpp(AddBoatVM boatVM);
    }
}