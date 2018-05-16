using DND.Common.Testing.xUnit;
using DND.Web;
using System;

namespace DND.IntegrationTests2
{
    public class TestServerFixture : BaseTestServerFixture<Startup>, IDisposable
    {
        public TestServerFixture()
            :base($@"..\..\..\..\src\DND.Web", "Development", "v4.7.2", Program.BuildWebHostConfiguration)
        {

        }

        public void Dispose()
        {
            base.Dispose();
        }
    }
}
