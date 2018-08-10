using DND.Domain.CMS.ContentHtmls;
using System;
using System.Linq;

namespace DND.Data
{
    public class DbSeed
    {
        public static void Seed(ApplicationDbContext context)
        {
            AddContentHtml(context);
        }

        private static void AddContentText(ApplicationDbContext context)
        {
           
        }

        private static void AddContentHtml(ApplicationDbContext context)
        {
            AddContentHTML(context, DND.Infrastructure.Constants.CMS.ContentHtml.About, "<p>About Me</p>");
            AddContentHTML(context, DND.Infrastructure.Constants.CMS.ContentHtml.SideBarAbout, "<p>About Me</p>");
            AddContentHTML(context, DND.Infrastructure.Constants.CMS.ContentHtml.WorkWithMe, "<p>Work With Me</p>");
            AddContentHTML(context, DND.Infrastructure.Constants.CMS.ContentHtml.MyWebsite, "<p>My Website</p>");
            AddContentHTML(context, DND.Infrastructure.Constants.CMS.ContentHtml.Affiliates, "<p>Affiliates</p>");
            AddContentHTML(context, DND.Infrastructure.Constants.CMS.ContentHtml.Resume, "<p>Resume</p>");
            AddContentHTML(context, DND.Common.Constants.CMS.ContentHtml.Contact, "<p>Contact</p>");
            AddContentHTML(context, DND.Common.Constants.CMS.ContentHtml.Head, "");
            AddContentHTML(context, DND.Common.Constants.CMS.ContentHtml.Main, "");
            AddContentHTML(context, DND.Common.Constants.CMS.ContentHtml.PrivacyPolicy, "");
        }

        private static void AddContentHTML(ApplicationDbContext context, string id, string content)
        {
            if (!context.ContentHtml.Any(c => c.Id == id))
            {
                context.ContentHtml.Add(new ContentHtml() { Id = id, HTML = content, PreventDelete = true, DateCreated = DateTime.Now, UserCreated = "SYSTEM" });
            }
        }
    }
}
