using DND.Common.SignalRHubs;
using DND.Web.MVCImplementation.Author.Notifications;
using DND.Web.MVCImplementation.Blog.Notifications;
using DND.Web.MVCImplementation.CarouselItem.Notifications;
using DND.Web.MVCImplementation.Category.Notifications;
using DND.Web.MVCImplementation.ContentHtml.Notifications;
using DND.Web.MVCImplementation.ContentText.Notifications;
using DND.Web.MVCImplementation.Faq.Notifications;
using DND.Web.MVCImplementation.FlightSearch.Hubs;
using DND.Web.MVCImplementation.Locations.Notifications;
using DND.Web.MVCImplementation.MailingList.Notifications;
using DND.Web.MVCImplementation.Project.Notifications;
using DND.Web.MVCImplementation.Tags.Notifications;
using DND.Web.MVCImplementation.Testimonial.Notifications;
using Microsoft.AspNetCore.SignalR;

namespace DND.Web
{
    public static class NoticationsConfig
    {
        public static void MapHubs(HubRouteBuilder routes)
        {
            routes.MapHub<NotificationHub>("/api/signalR/notifications");
            routes.MapHub<FlightSearchHub>("/api/signalR/flight-search");

            AuthorsNotifications.MapHub(routes);
            BlogPostsNotifications.MapHub(routes);
            CarouselItemsNotifications.MapHub(routes);
            CategoriesNotifications.MapHub(routes);
            ContentHtmlsNotifications.MapHub(routes);
            ContentTextsNotifications.MapHub(routes);
            FaqsNotifications.MapHub(routes);
            LocationsNotifications.MapHub(routes);
            MailingListNotifications.MapHub(routes);
            ProjectsNotifications.MapHub(routes);
            TagsNotifications.MapHub(routes);
            TestimonialsNotifications.MapHub(routes);
        }
    }
}
