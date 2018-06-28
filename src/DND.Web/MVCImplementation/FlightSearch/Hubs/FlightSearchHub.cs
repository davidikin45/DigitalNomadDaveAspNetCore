using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Web.MVCImplementation.FlightSearch.Hubs
{
    public class FlightSearchHub : Hub
    {
        //Client submits via API and receives an Accepted 202 response
        //Client then call this method and receives results
        public async Task GetResults(int id)
        {
            CheckResult result;
            do
            {
                result = new CheckResult();
                await Task.Delay(1000);
                if (result.New)
                    await Clients.Caller.SendAsync("ReceiveResult", result.Status);
            } while (!result.Finished);
            await Clients.Caller.SendAsync("Finished");
        }
    }

    public class CheckResult
    {
        public bool New { get; set; }
        public string Status { get; set; }
        public bool Finished { get; set; }
    }
}
