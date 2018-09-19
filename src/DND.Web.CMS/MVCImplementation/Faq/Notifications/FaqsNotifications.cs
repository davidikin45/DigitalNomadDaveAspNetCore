using DND.Common.ApplicationServices.SignalR;
using DND.Common.SignalRHubs;
using DND.Domain.CMS.Faqs.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace DND.Web.CMS.MVCImplementation.Faq.Notifications
{
    public class FaqsNotifications : ISignalRHubMap
    {
        public void MapHub(HubRouteBuilder routes, string signalRUrlPrefix)
        {
            routes.MapHub<ApiNotificationHub<FaqDto>>(signalRUrlPrefix + "/cms/faqs/notifications");
        }
    }
}
