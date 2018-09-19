using DND.Domain.Blog.BlogPosts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DND.Data.Configurations.Blog.BlogPosts
{
    public class BlogPostLocationConfiguration
           : IEntityTypeConfiguration<BlogPostLocation>
    {
        public void Configure(EntityTypeBuilder<BlogPostLocation> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Ignore(p => p.DateDeleted);
            builder.Ignore(p => p.UserDeleted);

            builder.HasOne(p => p.Location)
                .WithMany()
                .HasForeignKey(p => p.LocationId).IsRequired();
        }
    }
}
