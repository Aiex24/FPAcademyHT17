using PlaceholderYacht.Models.ViewModels;

namespace PlaceholderYacht.Models
{
    public interface IBoatRepository
    {
        AccountBoatItemVM[] GetUsersBoatsByUID(string UID);
        BoatPageVM GetBoatPageVM(int BoatID);
    }
}