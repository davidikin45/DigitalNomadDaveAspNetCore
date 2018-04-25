using DND.Domain.Blog.Tags;
using System.Data.Entity.ModelConfiguration;

namespace DND.EFPersistance.Configurations.Blog.Tags
{
    public class TagConfiguration
           : EntityTypeConfiguration<Tag>
    {
        public TagConfiguration()
        {
            HasKey(p => p.Id);

            //Property(p => p.RowVersion).IsRowVersion();

            Property(p => p.Name)
                 .IsRequired()
                .HasMaxLength(50);

            Property(p => p.UrlSlug)
                .IsRequired()
               .HasMaxLength(50);

            Property(p => p.Description)
              .IsRequired()
             .HasMaxLength(200);
        }
    }
}
