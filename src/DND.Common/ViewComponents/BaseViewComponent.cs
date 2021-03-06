﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Common.ViewComponents
{
    public abstract class BaseViewComponent : ViewComponent
    {
        protected CancellationToken ClientDisconnectedToken()
        {
            return HttpContext.RequestAborted;
        }
    }
}
