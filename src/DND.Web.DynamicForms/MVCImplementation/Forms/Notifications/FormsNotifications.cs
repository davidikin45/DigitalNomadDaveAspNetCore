using DND.Common.ApplicationServices.SignalR;
using DND.Common.SignalRHubs;
using DND.Domain.DynamicForms.Forms.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace DND.Web.DynamicForms.MVCImplementation.Tags.Notifications
{
    public class SectionsNotifications : ISignalRHubMap
    {
        public void MapHub(HubRouteBuilder routes, string signalRUrlPrefix)
        {
            routes.MapHub<ApiNotificationHub<FormDto>>(signalRUrlPrefix + "/forms/forms/notifications");
        }
    }
}
