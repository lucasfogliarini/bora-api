using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Bora.Database
{
    public class BoraDbContext : DbContext
    {
        public BoraDbContext(DbContextOptions<BoraDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            var thisAssembly = Assembly.GetExecutingAssembly();
            builder.ApplyConfigurationsFromAssembly(thisAssembly);
        }
    }
}
