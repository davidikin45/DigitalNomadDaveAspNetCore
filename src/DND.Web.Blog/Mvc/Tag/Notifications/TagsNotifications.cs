using DND.Common.ApplicationServices.SignalR;
using DND.Common.SignalRHubs;
using DND.Domain.Blog.Tags.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace DND.Web.Blog.Mvc.Tags.Notifications
{
    public class FormsNotifications : ISignalRHubMap
    {
        public void MapHub(HubRouteBuilder routes, string signalRUrlPrefix)
        {
            routes.MapHub<ApiNotificationHub<TagDto>>(signalRUrlPrefix + "/blog/tags/notifications");
        }
    }
}
