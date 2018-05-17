using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.SQLite;
using System.Data.SQLite.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Implementation.Persistance
{
    public class DbConfigurationSwitcher : DbConfiguration
    {
        public DbConfigurationSwitcher()
        {
            if (bool.Parse(ConnectionStrings.GetConnectionString("UseSQLite")))
            {
                SetProviderFactory("System.Data.SQLite", SQLiteFactory.Instance);
                SetProviderFactory("System.Data.SQLite.EF6", SQLiteProviderFactory.Instance);
                SetProviderServices("System.Data.SQLite", (DbProviderServices)SQLiteProviderFactory.Instance.GetService(typeof(DbProviderServices)));
            }
        }
    }
}
