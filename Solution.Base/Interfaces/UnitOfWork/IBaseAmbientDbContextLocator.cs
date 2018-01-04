/* 
 * Copyright (C) 2014 Mehdi El Gueddari
 * http://mehdi.me
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 */
using System.Data.Entity;
using Solution.Base.Interfaces.Persistance;

namespace Solution.Base.Interfaces.UnitOfWork
{
    /// <summary>
    /// Convenience methods to retrieve ambient DbContext instances. 
    /// </summary>
    public interface IBaseAmbientDbContextLocator
    {
        /// <summary>
        /// If called within the scope of a UnitOfWorkScope, gets or creates 
        /// the ambient DbContext instance for the provided DbContext type. 
        /// 
        /// Otherwise returns null. 
        /// </summary>
        ITDbContext Get<ITDbContext>() where ITDbContext : IBaseDbContext;
    }
}
