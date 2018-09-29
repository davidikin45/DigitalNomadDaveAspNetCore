using DND.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
