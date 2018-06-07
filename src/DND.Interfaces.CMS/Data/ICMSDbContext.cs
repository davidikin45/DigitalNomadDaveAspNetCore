using DND.Common.Interfaces.Data;
using DND.Domain.CMS.CarouselItems;
using DND.Domain.CMS.ContentHtmls;
using DND.Domain.CMS.ContentTexts;
using DND.Domain.CMS.Faqs;
using DND.Domain.CMS.MailingLists;
using DND.Domain.CMS.Projects;
using DND.Domain.CMS.Testimonials;
using System.Data.Entity;

namespace DND.Interfaces.CMS.Data
{
    public interface ICMSDbContext : IBaseDbContext
    {
        IDbSet<Faq> Faqs { get; set; }
        IDbSet<ContentText> ContentText { get; set; }
        IDbSet<ContentHtml> ContentHtml { get; set; }
        IDbSet<MailingList> MailingList { get; set; }
        IDbSet<CarouselItem> CarouselItems { get; set; }
        IDbSet<Project> Projects { get; set; }
        IDbSet<Testimonial> Testimonials { get; set; }
    }
}
