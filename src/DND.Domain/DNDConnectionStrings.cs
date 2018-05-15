using DND.Common.Implementation.Persistance;
using DND.Common.Interfaces.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Domain
{
    public static class DNDConnectionStrings 
    {
        static DNDConnectionStrings()
        {
            ConnectionStrings.AddConnectionStringIfNotExists("DefaultConnectionString", "Data Source=Dave; Initial Catalog=DNDAspNetCore; Integrated Security=true; MultipleActiveResultSets=True");
        }

        public static void AddConnectionString(string name, string connectionString)
        {
            ConnectionStrings.AddConnectionString(name, connectionString);
        }

        public static string GetConnectionString(string name)
        {
            return ConnectionStrings.GetConnectionString(name);
        }
    }
}
