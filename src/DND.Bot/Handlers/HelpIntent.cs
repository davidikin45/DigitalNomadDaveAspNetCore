
using DND.Bot.Models.Common;

namespace DND.Bot.Handlers
{
    public class HelpIntent
    {
        internal static CommonModel Process(CommonModel commonModel)
        {
            commonModel.Response.Text = "Hola and welcome to Digital Nomad Dave Tacos. I can help you book a table to our restaurant, to do so, say \"book a table\"?";
            commonModel.Response.Prompt = "If you want to book a table, say, \"book a table\".";

            commonModel.Session.EndSession = false;

            return commonModel;
        }
    }
}
