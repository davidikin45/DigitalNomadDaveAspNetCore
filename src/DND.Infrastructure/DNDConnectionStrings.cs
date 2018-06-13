﻿using DND.Common.Implementation.Data;

namespace DND.Infrastructure
{
    public static class DNDConnectionStrings 
    {
        //Used for Integration Tests
        static DNDConnectionStrings()
        {
            ConnectionStrings.AddConnectionStringIfNotExists("DefaultConnectionString", "Server=(localdb)\\mssqllocaldb;Database=DNDIntegrationTests;Trusted_Connection=True;MultipleActiveResultSets=true");
            ConnectionStrings.AddConnectionStringIfNotExists("UseSQLite", "false");
            ConnectionStrings.AddConnectionStringIfNotExists("SQLite", "Data Source=DND.db;foreign keys=true;");
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