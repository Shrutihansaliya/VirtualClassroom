using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace VirtualClassroom.Infrastructure
{
    public class ApplicationDbContextFactory
        : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            optionsBuilder.UseSqlServer(
                "Server=tcp:virtualclass-sql.database.windows.net,1433;Initial Catalog=VirtualClassDB;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Database=VirtualClassDB;User Id=hetviVamja;Password=hetu&22022005;TrustServerCertificate=True;"
            );

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}