using DND.Domain.Blog.BlogPosts;
using DND.Domain.Blog.Categories;
using System.Data.Entity.ModelConfiguration;

namespace DND.EFPersistance.Configurations.Blog.BlogPosts
{
    public class BlogPostLocationConfiguration
           : EntityTypeConfiguration<BlogPostLocation>
    {
        public BlogPostLocationConfiguration()
        {
            HasKey(p => p.Id);

            HasRequired(p => p.BlogPost)
                .WithMany()
                .HasForeignKey(p => p.BlogPostId);

            HasRequired(p => p.Location)
                .WithMany()
                .HasForeignKey(p => p.LocationId);
        }
    }
}
