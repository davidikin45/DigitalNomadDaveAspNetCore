using DND.Common.Interfaces.Data;
using DND.Domain.Blog.Authors;
using DND.Domain.Blog.BlogPosts;
using DND.Domain.Blog.Categories;
using DND.Domain.Blog.Locations;
using DND.Domain.Blog.Tags;
using System.Data.Entity;

namespace DND.Interfaces.Blog.Data
{
    public interface IBlogDbContext : IBaseDbContext
    {
        IDbSet<BlogPost> BlogPosts { get; set; }
        IDbSet<BlogPostTag> BlogPostTags { get; set; }
        IDbSet<BlogPostLocation> BlogPostLocations { get; set; }
        IDbSet<Author> Authors { get; set; }
        IDbSet<Tag> Tags { get; set; }
        IDbSet<Category> Categories { get; set; }
        IDbSet<Location> Locations { get; set; }
    }
}
