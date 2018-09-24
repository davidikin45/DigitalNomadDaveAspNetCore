using DND.Common.ApplicationServices.SignalR;
using DND.Common.SignalRHubs;
using DND.Domain.CMS.ContentTexts.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace DND.Web.CMS.Mvc.ContentText.Notifications
{
    public class ContentTextsNotifications : ISignalRHubMap
    {
        public void MapHub(HubRouteBuilder routes, string signalRUrlPrefix)
        {
            routes.MapHub<ApiNotificationHub<ContentTextDto>>(signalRUrlPrefix + "/cms/content-texts/notifications");
        }
    }
}
