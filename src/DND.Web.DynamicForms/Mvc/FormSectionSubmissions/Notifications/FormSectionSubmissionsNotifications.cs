using DND.Common.ApplicationServices.SignalR;
using DND.Common.SignalRHubs;
using DND.Domain.DynamicForms.Forms.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace DND.Web.DynamicForms.Mvc.FormSectionSubmissions.Notifications
{
    public class FormSectionSubmissionsNotifications : ISignalRHubMap
    {
        public void MapHub(HubRouteBuilder routes, string signalRUrlPrefix)
        {
            routes.MapHub<ApiNotificationHub<FormDto>>(signalRUrlPrefix + "/forms/form-section-submissions/notifications");
        }
    }
}
