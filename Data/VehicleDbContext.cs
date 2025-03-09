using Microsoft.EntityFrameworkCore;
using HjulinstallningAPI.Models;

namespace HjulinstallningAPI.Data
{
    public class VehicleDbContext : DbContext
    {
        public VehicleDbContext(DbContextOptions<VehicleDbContext> options) : base(options) { }

        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<WowCharacter> WowCharacters { get; set; }  // ✅ register new table inside DbSet()

    }
}
