using Microsoft.Extensions.Configuration;
using DND.Common.Implementation.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Extensions
{
    public static class ConfigurationExtensions
    {
        public static void PopulateStaticConnectionStrings(this IConfiguration configuration)
        {
            foreach (var connectionString in configuration.GetSection("ConnectionStrings").GetChildren().AsEnumerable())
            {
                ConnectionStrings.AddConnectionString(connectionString.Key, connectionString.Value);
            }
        }
    }
}
