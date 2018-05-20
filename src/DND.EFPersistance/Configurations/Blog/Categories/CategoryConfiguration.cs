using DND.Domain.Blog.Categories;
using System.Data.Entity.ModelConfiguration;

namespace DND.EFPersistance.Configurations.Blog.Categories
{
    public class CategoryConfiguration
           : EntityTypeConfiguration<Category>
    {
        public CategoryConfiguration()
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
