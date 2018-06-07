using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DND.TestSetup
{
    public class DbSetupAndTestServerXUnitFixture : DbSetupXUnitFixture, IDisposable
    {
        private TestServerFixture _testServer;
        public DbSetupAndTestServerXUnitFixture()
        {
            _testServer = new TestServerFixture();
        }

        public TestServerFixture TestServer
        {
            get
            {

                return _testServer;
            }
        }

        public new void Dispose()
        {
            //1. Kill Test Server
            _testServer.Dispose();
            //2. Delete Db
            base.Dispose();
        }
    }
}
