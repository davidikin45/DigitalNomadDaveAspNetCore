using DND.Common;
using Microsoft.AspNetCore.Hosting;
using System;

namespace DND.Web
{
    public class Program : ProgramBase<Startup>
    {
        public static int Main(string[] args)
        {
            return RunApp(args);
        }

    }
}
