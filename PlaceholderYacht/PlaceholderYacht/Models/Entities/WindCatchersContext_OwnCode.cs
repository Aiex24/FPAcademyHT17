using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PlaceholderYacht.Models.Entities
{
    public partial class WindCatchersContext : DbContext
    {
        public WindCatchersContext(DbContextOptions<WindCatchersContext> options) : base(options)
        {

        }
    }
}
