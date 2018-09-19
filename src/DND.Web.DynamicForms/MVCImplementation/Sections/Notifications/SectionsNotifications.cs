using DND.Common.ApplicationServices.SignalR;
using DND.Common.SignalRHubs;
using DND.Domain.DynamicForms.Sections.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace DND.Web.DynamicForms.MVCImplementation.Sections.Notifications
{
    public class SectionsNotifications : ISignalRHubMap
    {
        public void MapHub(HubRouteBuilder routes, string signalRUrlPrefix)
        {
            routes.MapHub<ApiNotificationHub<SectionDto>>(signalRUrlPrefix + "/forms/sections/notifications");
        }
    }
}
