/* 
 * Copyright (C) 2014 Mehdi El Gueddari
 * http://mehdi.me
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 */
using System;
using System.Data.Entity;
using Solution.Base.Interfaces.Persistance;

namespace Solution.Base.Interfaces.UnitOfWork
{
    /// <summary>
    /// Maintains a list of lazily-created DbContext instances.
    /// </summary>
    public interface IBaseDbContextCollection : IDisposable
    {
        /// <summary>
        /// Get or create a DbContext instance of the specified type. 
        /// </summary>
		TIDbContext Get<TIDbContext>() where TIDbContext : IBaseDbContext;
    }
}