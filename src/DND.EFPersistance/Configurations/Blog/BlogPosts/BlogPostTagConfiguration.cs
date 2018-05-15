using DND.Domain.Blog.BlogPosts;
using DND.Domain.Blog.Categories;
using System.Data.Entity.ModelConfiguration;

namespace DND.EFPersistance.Configurations.Blog.BlogPosts
{
    public class BlogPostTagConfiguration
           : EntityTypeConfiguration<BlogPostTag>
    {
        public BlogPostTagConfiguration()
        {
            HasKey(p => p.Id);

            HasRequired(p => p.BlogPost)
                .WithMany()
                .HasForeignKey(p => p.BlogPostId);

            HasRequired(p => p.Tag)
                .WithMany()
                .HasForeignKey(p => p.TagId);
        }
    }
}
