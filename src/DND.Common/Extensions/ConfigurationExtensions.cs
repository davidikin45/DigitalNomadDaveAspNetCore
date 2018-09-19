﻿using DND.Common.Infrastructure;
using Microsoft.Extensions.Configuration;
using System.Linq;

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
