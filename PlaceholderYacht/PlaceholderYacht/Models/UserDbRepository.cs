using PlaceholderYacht.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlaceholderYacht.Models
{
    public class UserDbRepository
    {
        WindCatchersContext context;
        public UserDbRepository(WindCatchersContext context)
        {
            this.context = context;
        }
    }
}
