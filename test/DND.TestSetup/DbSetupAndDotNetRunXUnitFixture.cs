using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DND.TestSetup
{
    public class DbSetupAndDotNetRunXUnitFixture : DbSetupXUnitFixture, IDisposable
    {
        private DotNetRunFixture _dotNetRun;
        public DbSetupAndDotNetRunXUnitFixture()
        {
            _dotNetRun = new DotNetRunFixture();
        }

        public new void Dispose()
        {
            //1. Kill Process
            _dotNetRun.Dispose();
            //2. Delete Db
            base.Dispose();
        }
    }
}
