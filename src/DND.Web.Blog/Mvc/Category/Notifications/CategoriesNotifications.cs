using DND.Common.ApplicationServices.SignalR;
using DND.Common.SignalRHubs;
using DND.Domain.Blog.Categories.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace DND.Web.Blog.Mvc.Category.Notifications
{
    public class CategoriesNotifications : ISignalRHubMap
    {
        public void MapHub(HubRouteBuilder routes, string signalRUrlPrefix)
        {
            routes.MapHub<ApiNotificationHub<CategoryDto>>(signalRUrlPrefix + "/blog/categories/notifications");
        }
    }
}
