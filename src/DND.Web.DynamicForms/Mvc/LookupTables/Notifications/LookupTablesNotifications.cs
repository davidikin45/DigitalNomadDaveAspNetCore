using DND.Common.ApplicationServices.SignalR;
using DND.Common.SignalRHubs;
using DND.Domain.DynamicForms.LookupTables.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace DND.Web.DynamicForms.Mvc.LookupTables.Notifications
{
    public class LookupTablesNotifications : ISignalRHubMap
    {
        public void MapHub(HubRouteBuilder routes, string signalRUrlPrefix)
        {
            routes.MapHub<ApiNotificationHub<LookupTableDto>>(signalRUrlPrefix + "/forms/lookup-tables/notifications");
        }
    }
}
