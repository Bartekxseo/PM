using Microsoft.EntityFrameworkCore;

namespace PM.DataAccess
{
    public class ParkingManagerDbContext : DbContext
    {
        public ParkingManagerDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ParkingManagerDbContext).Assembly);
        }
    }
}
