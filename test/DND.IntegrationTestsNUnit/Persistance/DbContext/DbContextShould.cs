﻿using DND.Common.Implementation.Repository;
using DND.Common.Implementation.Repository.EntityFramework;
using DND.Common.Implementation.UnitOfWork;
using DND.Common.Testing;
using DND.Domain;
using DND.Domain.Blog.Categories;
using DND.Domain.Interfaces.Persistance;
using DND.EFPersistance;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.IntegrationTestsNUnit.Persistance.UnitOfWork
{
    [TestFixture]
    public class DbContextShould
    {
        private ApplicationDbContext _context;
        [SetUp]
        public void SetUp()
        {
            var connectionString = DNDConnectionStrings.GetConnectionString("DefaultConnectionString");
            _context = new ApplicationDbContext(connectionString, true);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test, Isolated]
        public async Task DbContextLocal()
        {
            Assert.True(true);
        }
    }
}
