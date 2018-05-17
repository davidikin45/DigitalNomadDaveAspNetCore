﻿using DND.Common.Infrastructure;
using DND.Domain.Models;
using DND.EFPersistance;
using DND.EFPersistance.Identity;
using DND.EFPersistance.Initializers;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Linq;


namespace DND.IntegrationTestsNUnit
{
    [SetUpFixture]
    public class GlobalSetup
    {
        [OneTimeSetUp]
        public void Setup()
        {
            SetConfiguration();
            MigrateDbToLatestVersion();
            Seed();
        }

        public void SetConfiguration()
        {
            //SqlProviderServices.SqlServerTypesAssemblyName = "Microsoft.SqlServer.Types, Version=13.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91";
            //SqlServerTypes.Utilities.LoadNativeAssemblies(AppDomain.CurrentDomain.BaseDirectory);
        }

        public void MigrateDbToLatestVersion()
        {
            DbContextInitializer<ApplicationDbContext>.SetInitializer(new DbContextFactory(), new ApplicationDbInitializerDropCreateForce(), true, true);

            var context = new ApplicationIdentityDbContextFactory().CreateDbContext(null);
            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();
            context.Database.Migrate();


            //var configuration = new ApplicationDbConfiguration();
            //var migrator = new DbMigrator(configuration);
            //migrator.Update();
        }

        public void Seed()
        {
            var context = new ApplicationIdentityDbContextFactory().CreateDbContext(null);

            if (context.Users.Any())
                return;

            context.Users.Add(new User { UserName = "user1", Name = "user1", Email = "-", PasswordHash = "-", EmailConfirmed = true });
            context.Users.Add(new User { UserName = "user2", Name = "user2", Email = "-", PasswordHash = "-", EmailConfirmed = true });
            context.SaveChanges();
        }
    }
}
