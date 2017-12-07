using PlaceholderYacht.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlaceholderYacht.Models.ViewModels;

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
            throw new NotImplementedException();
        }
    }
}
