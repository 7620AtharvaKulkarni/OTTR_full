using Microsoft.EntityFrameworkCore;

namespace OTTR.Models
{
    //Inherit the DbContext Class
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<GESGroupProductMapping>().HasKey(table => new {
                table.GESGroupId,
                table.ProductId
            });

            builder.Entity<RegionCountryMapping>().HasKey(table => new {
                table.RegionID,
                table.CountryID
            });
            builder.Entity<OttrMonitoringModel>(entity =>
            {
                entity.HasNoKey();
            });
        }

         

        //Creating the Instances of Every Entity
        public DbSet<OttrMonitoring> OttrMonitorings { get; set; }
        public DbSet<Region> Regions { get; set; }
        public virtual DbSet<OttrMonitoringModel> OttrMonitoringModels { get; set; }

        public DbSet<Gesgroupdelivery> GesGroupDelivery { get; set; }
        public DbSet<GESGroup> GESGroups { get; set; }
        public DbSet<Pole> Poles { get; set; }
        public DbSet<Country> Countries { get; set; }

        public DbSet<RegionCountryMapping> RegionCountryMapping { get; set; }
        public DbSet<AttributeTo> AttributeTo { get; set; }
        public DbSet<InternalExternalAttributeTo> InternalExternalAttributeTo { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<GESGroupProductMapping> GESGroupProductMappings { get; set; }
        public DbSet<AuditOttrMonitoring> AuditOttrMonitorings { get; set; }
    }

}
