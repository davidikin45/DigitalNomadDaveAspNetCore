﻿using DND.Common.Infrastructure;
using DND.Common.Interfaces.Data;
using DND.Common.Tasks;
using DND.Data.Initializers;
using DND.Infrastructure;

namespace DND.Data
{
    public class ApplicationDbInitializer : IRunOnWebHostStartup
    {
        private IDbContextFactoryProducerSingleton _dbContextFactory;
        public ApplicationDbInitializer(IDbContextFactoryProducerSingleton dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public void Execute()
        {
            if(bool.Parse(DNDConnectionStrings.GetConnectionString("UseSQLite")))
            {
                DbContextInitializer<ApplicationDbContext>.SetInitializer(_dbContextFactory, new ApplicationDbInitializerMigrateSQLite(), true, true);
            }
            else
            {
                DbContextInitializer<ApplicationDbContext>.SetInitializer(_dbContextFactory, new ApplicationDbInitializerMigrate(), true, true);
            }
        }
    }
}
