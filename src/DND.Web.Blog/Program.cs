//https://www.ryansouthgate.com/2017/07/21/asp-net-core-mvc-common-components-view-across-applications/
using DND.Common;

namespace DND.Web.Blog
{
    public class Program : ProgramBase<Startup>
    {
        public static int Main(string[] args)
        {
            return RunApp(args);
        }
    }
}
