using DND.Domain.Blog.Locations;
using System.Data.Entity.ModelConfiguration;

namespace DND.Data.Configurations.Blog.Locations
{
    public class LocationConfiguration
           : EntityTypeConfiguration<Location>
    {
        public LocationConfiguration()
        {
            HasKey(p => p.Id);

            Ignore(p => p.DateDeleted);
            Ignore(p => p.UserDeleted);

            Property(p => p.RowVersion).IsRowVersion();

            //NotMapped
            //Ignore(p => p.LocationType);

            Property(p => p.LocationTypeString)
                .HasColumnName("LocationType");
                 
        }
    }
}
