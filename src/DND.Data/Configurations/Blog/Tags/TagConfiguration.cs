using DND.Domain.Blog.Tags;
using System.Data.Entity.ModelConfiguration;

namespace DND.Data.Configurations.Blog.Tags
{
    public class TagConfiguration
           : EntityTypeConfiguration<Tag>
    {
        public TagConfiguration()
        {
            HasKey(p => p.Id);

            Ignore(p => p.DateDeleted);
            Ignore(p => p.UserDeleted);

            Property(p => p.RowVersion).IsRowVersion();

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
