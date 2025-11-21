using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using RadarHub.Infraestrutura;

namespace RadarHub.Repositorio
{
    public class RadarHubDbContextFactory : IDesignTimeDbContextFactory<RadarHubDbContext>
    {
        public RadarHubDbContext CreateDbContext(string[] args)
        {
            var connectionString = "Server=localhost;Database=db_radarhub;Uid=radarhub;Pwd=senha123;";

            var optionsBuilder = new DbContextOptionsBuilder<RadarHubDbContext>();
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            return new RadarHubDbContext(optionsBuilder.Options);
        }
    }
}
