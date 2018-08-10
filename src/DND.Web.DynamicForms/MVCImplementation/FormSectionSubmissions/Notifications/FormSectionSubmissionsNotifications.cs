using DND.Common.SignalRHubs;
using DND.Domain.DynamicForms.Forms.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace DND.Web.DynamicForms.MVCImplementation.FormSectionSubmissions.Notifications
{
    public class FormSectionSubmissionsNotifications : ISignalRHubMap
    {
        public void MapHub(HubRouteBuilder routes, string signalRUrlPrefix)
        {
            routes.MapHub<ApiNotificationHub<FormDto>>(signalRUrlPrefix + "/forms/form-section-submissions/notifications");
        }
    }
}
