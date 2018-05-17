﻿using DND.Domain.Blog.Tags;
using DND.Domain.CMS.CarouselItems;
using System.Data.Entity.ModelConfiguration;

namespace DND.EFPersistance.Configurations.CMS.CarouselItems
{
    public class CarouselItemConfiguration
           : EntityTypeConfiguration<CarouselItem>
    {
        public CarouselItemConfiguration()
        {
            HasKey(p => p.Id);

            Property(p => p.RowVersion).IsRowVersion();

            Property(p => p.CarouselText)
                .HasMaxLength(200);

            Property(p => p.OpenInNewWindow)
                .IsRequired();

            Property(p => p.Published)
              .IsRequired();
        }
    }
}