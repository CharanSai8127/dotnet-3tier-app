using Microsoft.EntityFrameworkCore;
using DotNetSqlCRUDApp.Models;

namespace DotNetSqlCRUDApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Product> Products { get; set; }
    }
}
