using DND.Common.SignalRHubs;
using DND.Domain.Blog.Authors.Dtos;
using DND.Domain.Blog.BlogPosts.Dtos;
using DND.Domain.Blog.Categories.Dtos;
using DND.Domain.CMS.CarouselItems.Dtos;
using DND.Web.MVCImplementation.FlightSearch.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web.MVCImplementation.Category.Notifications
{
    public class FlightSearchNotifications : ISignalRHubMap
    {
        public void MapHub(HubRouteBuilder routes, string signalRUrlPrefix)
        {
            routes.MapHub<FlightSearchHub>(signalRUrlPrefix + "/signalr/flight-search");
        }
    }
}
