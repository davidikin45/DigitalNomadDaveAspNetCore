using DND.Common.Testing.DotNetRun;
using DND.Common.Testing.Selenium;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DND.TestSetup
{
    public class DotNetRunFixture : BaseDotNetRunFixture, IDisposable
    {
        public DotNetRunFixture()
            :base($@"..\..\..\..\src\DND.Web", ConfigurationManager.AppSettings["Environment"], ConfigurationManager.AppSettings["SeleniumUrl"], bool.Parse(ConfigurationManager.AppSettings["HideBrowser"]))
        {
            Launch();
        }

        public new void Dispose()
        {
            base.Dispose();
        }
    }
}
