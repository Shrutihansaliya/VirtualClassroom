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
                "Server=tcp:vcdb-last123.database.windows.net,1433;Initial Catalog=vcdb;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Database=vcdb;User Id=jeli123@;Password=Jeli@123@;TrustServerCertificate=True;"
            );

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}


//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Design;

//namespace VirtualClassroom.Infrastructure
//{
//    public class ApplicationDbContextFactory
//        : IDesignTimeDbContextFactory<ApplicationDbContext>
//    {
//        public ApplicationDbContext CreateDbContext(string[] args)
//        {
//            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

//            optionsBuilder.UseSqlServer(
//                "Server=(localdb)\\MSSQLLocalDB;Database=VirtualClassroomDb;Trusted_Connection=True;"
//            );

//            return new ApplicationDbContext(optionsBuilder.Options);
//        }
//    }
//}