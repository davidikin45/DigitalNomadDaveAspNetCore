using DND.Common.Testing.DotNetRun;
using System;
using System.Configuration;

namespace DND.TestSetup
{
    public class DotNetRunFixture : DotNetRunFixtureBase, IDisposable
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
