using Microsoft.EntityFrameworkCore;
using Core.Entities.Concrete; // User, OperationClaim ve UserOperationClaim buradan gelecek
using Entities.Concrete;      // Car, Brand, Color vb. buradan gelecek

namespace DataAccess.Concrete.EntityFramework
{
    public class ReCapContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=ReCapDb;Trusted_Connection=true;TrustServerCertificate=true");
        }

        // Araçlar ve Kiralama Tabloları
        public DbSet<Car> Cars { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<CarImage> CarImages { get; set; }

        // Yetkilendirme (Auth) Tabloları - Core Katmanındaki User kullanılacak
        public DbSet<User> Users { get; set; }
        public DbSet<OperationClaim> OperationClaims { get; set; }
        public DbSet<UserOperationClaim> UserOperationClaims { get; set; }
    }
}