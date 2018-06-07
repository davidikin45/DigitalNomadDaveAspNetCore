using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.TestSetup
{
    public class DbSetupXUnitFixture : DbSetupFixture
    {
        public DbSetupXUnitFixture()
        {
            DatabaseSetup();
        }
    }
}
