using DND.Domain.Blog.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DND.Data.Configurations.Blog.Locations
{
    public class LocationConfiguration
           : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Ignore(p => p.DateDeleted);
            builder.Ignore(p => p.UserDeleted);

            builder.Property(p => p.RowVersion).IsRowVersion();

            //NotMapped
            //Ignore(p => p.LocationType);

            builder.Property(p => p.LocationTypeString)
                .HasColumnName("LocationType");
        }
    }
}
