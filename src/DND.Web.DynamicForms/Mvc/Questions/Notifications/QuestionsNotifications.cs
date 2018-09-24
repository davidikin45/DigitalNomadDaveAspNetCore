using DND.Common.ApplicationServices.SignalR;
using DND.Common.SignalRHubs;
using DND.Domain.DynamicForms.Questions.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace DND.Web.DynamicForms.Mvc.Questions.Notifications
{
    public class QuestionsNotifications : ISignalRHubMap
    {
        public void MapHub(HubRouteBuilder routes, string signalRUrlPrefix)
        {
            routes.MapHub<ApiNotificationHub<QuestionDto>>(signalRUrlPrefix + "/forms/questions/notifications");
        }
    }
}
