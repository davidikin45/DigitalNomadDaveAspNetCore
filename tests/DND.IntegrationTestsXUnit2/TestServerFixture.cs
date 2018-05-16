using DND.Common.Testing.TestServer;
using DND.Web;
using System;

namespace DND.IntegrationTestsXUnit
{
    public class TestServerFixture : BaseTestServerFixture<Startup>, IDisposable
    {
        public TestServerFixture()
            :base($@"..\..\..\..\src\DND.Web", "Development", "v4.7.2", Program.BuildWebHostConfiguration)
        {

        }

        public new void Dispose()
        {
            base.Dispose();
        }
    }
}
