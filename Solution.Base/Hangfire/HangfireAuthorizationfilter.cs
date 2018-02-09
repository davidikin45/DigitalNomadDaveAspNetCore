﻿using Hangfire.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solution.Base.Hangfire
{
    public class HangfireAuthorizationfilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var admin = context.GetHttpContext().User.IsInRole("admin");
            return context.Request.LocalIpAddress == "127.0.0.1" || context.Request.LocalIpAddress == "::1" || admin;
        }

    }
}