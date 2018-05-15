using DND.Bot.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DND.Bot.Handlers
{
    public class YesIntent
    {
        internal static CommonModel Process(CommonModel commonModel)
        {
            commonModel.Response.Event = "RESERVATIONS";

            return commonModel;
        }
    }
}