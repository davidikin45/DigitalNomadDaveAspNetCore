using DND.Domain.Blog.BlogPosts;
using DND.Domain.Blog.Categories;
using System.Data.Entity.ModelConfiguration;

namespace DND.EFPersistance.Configurations.Blog.BlogPosts
{
    public class BlogPostConfiguration
           : EntityTypeConfiguration<BlogPost>
    {
        public BlogPostConfiguration()
        {
            HasKey(p => p.Id);

            Property(p => p.Title)
                 .IsRequired()
                .HasMaxLength(500);

            Property(p => p.ShortDescription)
                .IsRequired()
               .HasMaxLength(5000);

            Property(p => p.Description)
              .IsRequired()
             .HasMaxLength(300000);

            Property(p => p.UrlSlug)
             .IsRequired()
            .HasMaxLength(200);

            Property(p => p.CarouselText)
            .HasMaxLength(200);

            Property(p => p.ShowInCarousel)
            .IsRequired();

            Property(p => p.Published)
            .IsRequired();

            Property(p => p.DateCreated)
           .IsRequired();

            HasRequired(p => p.Author)
                .WithMany()
                .HasForeignKey(p => p.AuthorId)
                 .WillCascadeOnDelete(false);

            HasRequired(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .WillCascadeOnDelete(false);

            HasMany(p => p.Tags)
           .WithRequired()
           .HasForeignKey(p => p.BlogPostId);

            HasMany(p => p.Locations)
            .WithRequired()
           .HasForeignKey(p => p.BlogPostId);

            Property(p => p.ShowLocationDetail)
            .IsRequired();

            Property(p => p.ShowLocationMap)
            .IsRequired();

            Property(p => p.MapHeight)
            .IsRequired();

            Property(p => p.MapZoom)
            .IsRequired();

            Property(p => p.Album)
            .IsRequired();

            Property(p => p.ShowPhotosInAlbum)
            .IsRequired();

        }
    }
}
