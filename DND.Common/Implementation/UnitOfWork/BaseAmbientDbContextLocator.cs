/* 
 * Copyright (C) 2014 Mehdi El Gueddari
 * http://mehdi.me
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 */

using DND.Common.Interfaces.UnitOfWork;
using DND.Common.Interfaces.Persistance;

namespace DND.Common.Implementation.UnitOfWork
{
    public class BaseAmbientDbContextLocator : IBaseAmbientDbContextLocator
    {
        public ITDbContext Get<ITDbContext>() where ITDbContext : IBaseDbContext
        {
            var ambientUnitOfWorkScope = BaseUnitOfWorkTransactionScope.GetAmbientScope();
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