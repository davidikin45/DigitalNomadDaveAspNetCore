using DND.Domain.Models;
using DND.Common.Interfaces.Persistance;
using System.Data.Entity;

namespace DND.Domain.Interfaces.Persistance
{
    public interface IApplicationDbContext : IBaseDbContext
    {
        IDbSet<Faq> Faqs { get; set; }
        IDbSet<ContentText> ContentText { get; set; }
        IDbSet<ContentHtml> ContentHtml { get; set; }
        IDbSet<MailingList> MailingList { get; set; }

        IDbSet<BlogPost> BlogPosts { get; set; }
        IDbSet<BlogPostTag> BlogPostTags { get; set; }
        IDbSet<BlogPostLocation> BlogPostLocations { get; set; }

        IDbSet<Author> Authors { get; set; }
        IDbSet<Tag> Tags { get; set; }
        IDbSet<Category> Categories { get; set; }

        IDbSet<Location> Locations { get; set; }

        IDbSet<CarouselItem> CarouselItems { get; set; }

        IDbSet<Project> Projects { get; set; }
        IDbSet<Testimonial> Testimonials { get; set; }
    }
}
