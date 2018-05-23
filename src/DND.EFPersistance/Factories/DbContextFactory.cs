﻿using DND.Domain.Interfaces.Persistance;
using DND.Common.Implementation.Persistance;
using DND.Common.Interfaces.Persistance;
using System;
using System.Data.SQLite;
using DND.Domain;
using Microsoft.Extensions.DependencyInjection;
using DND.Common.DomainEvents;

namespace DND.EFPersistance
{
    public class DbContextFactory : IDbContextFactory
    {
        private IDomainEvents _domainEvents;

        public DbContextFactory(IDomainEvents domainEvents = null)
        {
            _domainEvents = domainEvents;
            var connectionString = DNDConnectionStrings.GetConnectionString("DefaultConnectionString");
        }

        public IBaseDbContext CreateDefault()
        {
            if (bool.Parse(DNDConnectionStrings.GetConnectionString("UseSQLite")))
            {
                var con = new SQLiteConnection()
                {
                    ConnectionString = DNDConnectionStrings.GetConnectionString("SQLite")
                };
                return new ApplicationDbContext(con);
            }
            else
            {
                return new ApplicationDbContext(DNDConnectionStrings.GetConnectionString("DefaultConnectionString"), false, _domainEvents);
            }
        }

        public TIDbContext Create<TIDbContext>() where TIDbContext : IBaseDbContext
        {
            if (typeof(TIDbContext) == typeof(IApplicationDbContext) || typeof(TIDbContext) == typeof(ApplicationDbContext) || typeof(TIDbContext) == typeof(IBaseDbContext))
            {
                return (TIDbContext)CreateDefault();
            }
            else
            {
                throw new ApplicationException("DbContext Type Unknown");
            }
        }
    }
}
