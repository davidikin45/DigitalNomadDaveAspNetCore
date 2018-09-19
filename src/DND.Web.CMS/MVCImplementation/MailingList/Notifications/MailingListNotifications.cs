using DND.Common.ApplicationServices.SignalR;
using DND.Common.SignalRHubs;
using DND.Domain.CMS.MailingLists.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace DND.Web.CMS.MVCImplementation.MailingList.Notifications
{
    public class MailingListNotifications : ISignalRHubMap
    {
        public void MapHub(HubRouteBuilder routes, string signalRUrlPrefix)
        {
            routes.MapHub<ApiNotificationHub<MailingListDto>>(signalRUrlPrefix + "/cms/mailing-list/notifications");
        }
    }
}
