using Microsoft.AspNetCore.Mvc;
using Solution.Base.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Web.ViewComponents
{
    public abstract class BaseViewComponent : ViewComponent
    {
        protected CancellationToken ClientDisconnectedToken()
        {
            return HttpContext.RequestAborted;
        }
    }
}
