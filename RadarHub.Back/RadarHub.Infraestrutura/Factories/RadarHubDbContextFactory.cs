using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using RadarHub.Infraestrutura;

namespace RadarHub.Repositorio
{
    public class RadarHubDbContextFactory : IDesignTimeDbContextFactory<RadarHubDbContext>
    {
        public RadarHubDbContext CreateDbContext(string[] args)
        {
            var connectionString = "server=localhost;user id=root;password=mysql;database=db_radarhub;";

            var optionsBuilder = new DbContextOptionsBuilder<RadarHubDbContext>();
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            return new RadarHubDbContext(optionsBuilder.Options);
        }
    }
}
