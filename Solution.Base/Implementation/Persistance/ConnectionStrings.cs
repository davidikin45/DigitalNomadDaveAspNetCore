using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solution.Base.Implementation.Persistance
{
    public static class ConnectionStrings
    {
        private static Dictionary<string, string> _connectionStrings = new Dictionary<string, string>();

        public static void AddConnectionString(string name, string connectionString)
        {
            if (_connectionStrings.ContainsKey(name))
            {
                _connectionStrings[name] = connectionString;
            }
            else
            {
                _connectionStrings.Add(name, connectionString);
            }
        }

        public static void AddConnectionStringIfNotExists(string name, string connectionString)
        {
            if (!_connectionStrings.ContainsKey(name))
            {
                _connectionStrings[name] = connectionString;
            }
        }

        public static string GetConnectionString(string name)
        {
            return _connectionStrings[name];
        }
    }
}
