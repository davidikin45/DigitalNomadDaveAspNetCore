using Solution.Base.Infrastructure;

namespace Solution.Base.Helpers
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
