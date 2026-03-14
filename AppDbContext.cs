using CozyComfort.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CozyComfort.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Blanket> Blankets { get; set; }
        public DbSet<Distributor> Distributors { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<AppUser> Users { get; set; }
    }
}