using Microsoft.EntityFrameworkCore;
using partieWebGDHotel.Models;

namespace partieWebGDHotel.Data
{
    public class ApplicationDbContext : DbContext
    {
        // Constructeur de ApplicationDbContext
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Définir les DbSets pour vos entités (tables)
        public DbSet<Client> Clients { get; set; }

        // Ajoutez d'autres DbSet pour d'autres entités si nécessaire
        // public DbSet<AutreEntite> AutreEntites { get; set; }
    }
}
