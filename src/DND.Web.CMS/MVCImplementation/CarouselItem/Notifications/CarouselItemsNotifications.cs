using DND.Common.SignalRHubs;
using DND.Domain.CMS.CarouselItems.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace DND.Web.CMS.MVCImplementation.CarouselItem.Notifications
{
    public class CarouselItemsNotifications : ISignalRHubMap
    {
        public void MapHub(HubRouteBuilder routes, string signalRUrlPrefix)
        {
            routes.MapHub<ApiNotificationHub<CarouselItemDto>>(signalRUrlPrefix + "/cms/carousel-items/notifications");
        }
    }
}
