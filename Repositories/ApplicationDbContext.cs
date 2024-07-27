using Microsoft.EntityFrameworkCore;
using OrderManagement.Models;

namespace OrderManagement.Repositories
{
    public class ApplicationDbContext : DbContext
    {

        public DbSet<User> Users { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }



        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }

    public class CardholderDbContext : DbContext
    {
        public CardholderDbContext(DbContextOptions<CardholderDbContext> options)
            : base(options)
        {
        }

        // Define DbSets for cardholder data
    }

}
