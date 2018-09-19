using Microsoft.AspNetCore.SignalR;

namespace DND.Common.SignalRHubs
{
    public interface ISignalRHubMap
    {
        void MapHub(HubRouteBuilder routes, string hubPathPrefix);
    }
}
