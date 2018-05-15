using DND.Bot.Models.Common;

namespace DND.Bot.Handlers
{
    public class CancelIntent
    {
        internal static CommonModel Process(CommonModel commonModel)
        {
            commonModel.Response.Text = "OK, all cancelled, have a wonderful day!";

            commonModel.Session.EndSession = true;

            return commonModel;
        }
    }
}