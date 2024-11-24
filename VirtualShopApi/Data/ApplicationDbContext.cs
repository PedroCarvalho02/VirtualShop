using Microsoft.EntityFrameworkCore;
using LojaVirtualAPI.Models;

namespace LojaVirtualAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User>? Users { get; set; }
        public DbSet<Product>? Products { get; set; }
        public DbSet<RevokedToken>? RevokedTokens { get; set; }
    }
}