using DND.Common.SignalRHubs;
using DND.Domain.Blog.BlogPosts.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace DND.Web.Blog.MVCImplementation.Blog.Notifications
{
    public class BlogPostsNotifications : ISignalRHubMap
    {
        public void MapHub(HubRouteBuilder routes, string signalRUrlPrefix)
        {
            routes.MapHub<ApiNotificationHub<BlogPostDto>>(signalRUrlPrefix + "/blog/blog-posts/notifications");
        }
    }
}
