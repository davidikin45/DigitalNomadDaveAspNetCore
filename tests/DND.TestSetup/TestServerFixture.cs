using DND.Common.Testing.TestServer;
using DND.Web;
using System;

namespace DND.TestSetup
{
    public class TestServerFixture : TestServerFixtureBase<Startup>, IDisposable
    {
        public TestServerFixture()
            :base($@"..\..\..\..\src\DND.Web", "Integration", "v4.7.2", Program.BuildWebHostConfiguration)
        {
            Launch();
        }

        public new void Dispose()
        {
            base.Dispose();
        }
    }
}
