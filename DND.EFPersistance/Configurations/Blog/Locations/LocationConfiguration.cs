using DND.Domain.Blog.Locations;
using System.Data.Entity.ModelConfiguration;

namespace DND.EFPersistance.Configurations.Blog.Locations
{
    public class LocationConfiguration
           : EntityTypeConfiguration<Location>
    {
        public LocationConfiguration()
        {
            HasKey(p => p.Id);

            //Property(p => p.RowVersion).IsRowVersion();

            //NotMapped
            //Ignore(p => p.LocationType);

            Property(p => p.LocationTypeString)
                .HasColumnName("LocationType");
                 
        }
    }
}
