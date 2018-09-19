using DND.Common.ApplicationServices.SignalR;
using DND.Common.SignalRHubs;
using DND.Domain.CMS.Testimonials.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace DND.Web.CMS.MVCImplementation.Testimonial.Notifications
{
    public class TestimonialsNotifications : ISignalRHubMap
    {
        public void MapHub(HubRouteBuilder routes, string signalRUrlPrefix)
        {
            routes.MapHub<ApiNotificationHub<TestimonialDto>>(signalRUrlPrefix + "/cms/testimonials/notifications");
        }
    }
}
