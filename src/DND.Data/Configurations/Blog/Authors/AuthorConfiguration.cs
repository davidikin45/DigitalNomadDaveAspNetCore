using DND.Domain.Blog.Authors;
using System.Data.Entity.ModelConfiguration;

namespace DND.Data.Configurations.Blog.Authors
{
    public class AuthorConfiguration
           : EntityTypeConfiguration<Author>
    {
        public AuthorConfiguration()
        {
            HasKey(p => p.Id);

            Ignore(p => p.DateDeleted);
            Ignore(p => p.UserDeleted);

            Property(p => p.RowVersion).IsRowVersion();

            Property(p => p.Name)
                .IsRequired();

            Property(p => p.UrlSlug)
               .HasMaxLength(50);
          
        }
    }
}
