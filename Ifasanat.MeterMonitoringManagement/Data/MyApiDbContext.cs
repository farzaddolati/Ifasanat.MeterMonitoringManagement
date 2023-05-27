using Ifasanat.MeterMonitoringManagement.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ifasanat.MeterMonitoringManagement.Data
{
    public class MyApiDbContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<City> Cities { get; set; }
        public MyApiDbContext(DbContextOptions<MyApiDbContext> options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json")
                    .Build();

                optionsBuilder.UseSqlServer(configuration.GetConnectionString("MyDatabaseConnection"));
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>().HasKey(e => e.Id);
            modelBuilder.Entity<Customer>().Property(e => e.FirstName).IsRequired();
            modelBuilder.Entity<Customer>().Property(e => e.LastName).IsRequired();
            modelBuilder.Entity<Customer>().Property(e => e.Email).IsRequired();
        }
    }
}
