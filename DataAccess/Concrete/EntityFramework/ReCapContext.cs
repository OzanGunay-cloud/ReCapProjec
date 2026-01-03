using Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Concrete.EntityFramework
{
    public class ReCapContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            /* NOT: Eğer bilgisayar adınla bağlanamıyorsan "." (nokta) kullanmak 
               "yerel makine" anlamına gelir ve en garantisidir.
            */
            optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=ReCapDb;Trusted_Connection=true;TrustServerCertificate=true");
        }

        public DbSet<Car> Cars { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Color> Colors { get; set; }

        public DbSet<Rental> Rentals { get; set; }

        public DbSet<CarImage> CarImages { get; set; }
    }
}