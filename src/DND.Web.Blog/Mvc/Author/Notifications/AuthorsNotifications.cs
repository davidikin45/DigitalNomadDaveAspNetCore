using DND.Common.ApplicationServices.SignalR;
using DND.Common.SignalRHubs;
using DND.Domain.Blog.Authors.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace DND.Web.Blog.Mvc.Author.Notifications
{
    public class AuthorsNotifications : ISignalRHubMap
    {
        public void MapHub(HubRouteBuilder routes, string signalRUrlPrefix)
        {
            routes.MapHub<ApiNotificationHub<AuthorDto>>(signalRUrlPrefix + "/blog/authors/notifications");
        }
    }
}
