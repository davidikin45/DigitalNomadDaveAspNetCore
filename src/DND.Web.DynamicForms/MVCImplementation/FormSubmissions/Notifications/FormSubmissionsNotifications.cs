using DND.Common.ApplicationServices.SignalR;
using DND.Common.SignalRHubs;
using DND.Domain.DynamicForms.FormSubmissions.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace DND.Web.DynamicForms.MVCImplementation.FormSubmissions.Notifications
{
    public class FormSubmissionsNotifications : ISignalRHubMap
    {
        public void MapHub(HubRouteBuilder routes, string signalRUrlPrefix)
        {
            routes.MapHub<ApiNotificationHub<FormSubmissionDto>>(signalRUrlPrefix + "/forms/form-submissions/notifications");
        }
    }
}
