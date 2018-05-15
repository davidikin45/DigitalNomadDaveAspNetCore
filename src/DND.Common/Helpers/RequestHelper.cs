using DND.Common.Infrastructure;

namespace DND.Common.Helpers
{
    public static class RequestHelper
    {
        public static string GetIPAddress()
        {
            string szRemoteAddr = HttpContext.Current.Connection.RemoteIpAddress.ToString();

            return szRemoteAddr;
        }
    }
}
