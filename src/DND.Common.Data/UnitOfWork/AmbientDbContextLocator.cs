/* 
 * Copyright (C) 2014 Mehdi El Gueddari
 * http://mehdi.me
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 */



using DND.Common.Infrastructure.Interfaces.Data.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Toptal.Common.Data.UnitOfWork;

namespace DND.Common.Data.UnitOfWork
{
    public class AmbientDbContextLocator : IAmbientDbContextLocator
    {
        public ITDbContext Get<ITDbContext>() where ITDbContext : DbContext
        {
            var ambientUnitOfWorkScope = UnitOfWorkTransactionScope.GetAmbientScope();
            if (ambientUnitOfWorkScope == null)
            {
                return default(ITDbContext);
            }
            else
            {
                return ambientUnitOfWorkScope.DbContexts.Get<ITDbContext>();
            } 
        }
    }
}