﻿using DND.Domain.Blog.Locations;
using DND.Domain.Blog.Locations.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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

            builder
            .Property(e => e.LocationType)
            .HasConversion(new EnumToStringConverter<LocationType>());

        }
    }
}
