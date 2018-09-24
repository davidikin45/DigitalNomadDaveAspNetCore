using DND.Common.ApplicationServices.SignalR;
using DND.Common.SignalRHubs;
using DND.Domain.CMS.ContentHtmls.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace DND.Web.CMS.Mvc.ContentHtml.Notifications
{
    public class ContentHtmlsNotifications : ISignalRHubMap
    {
        public void MapHub(HubRouteBuilder routes, string signalRUrlPrefix)
        {
            routes.MapHub<ApiNotificationHub<ContentHtmlDto>>(signalRUrlPrefix + "/cms/content-htmls/notifications");
        }
    }
}
