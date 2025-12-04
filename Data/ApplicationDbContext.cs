using Microsoft.EntityFrameworkCore;
using DotNetMongoCRUDApp.Models;

namespace DotNetMongoCRUDApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Product> Products { get; set; }
    }
}
