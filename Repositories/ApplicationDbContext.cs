using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrderManagement.Controllers;
using OrderManagement.Models;
using System.Collections.Generic;

namespace OrderManagement.Repositories
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Cardholder> CardholderData { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}
