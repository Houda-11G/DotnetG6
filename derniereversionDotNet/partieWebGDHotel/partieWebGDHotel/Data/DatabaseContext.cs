using Microsoft.EntityFrameworkCore;
using partieWebGDHotel.Models;

namespace partieWebGDHotel.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        public DbSet<Client> Client { get; set; }
        public DbSet<Chambre> Chambre { get; set; }
        public DbSet<Reservation> Reservation { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Reservation>()
                .HasKey(r => r.ID_Reservation);

            modelBuilder.Entity<Reservation>()
                .Property(r => r.RowVersion)
                .IsRowVersion();
        }
    }
}



