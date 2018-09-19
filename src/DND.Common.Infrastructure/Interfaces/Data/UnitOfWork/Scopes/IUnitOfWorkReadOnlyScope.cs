/* 
 * Copyright (C) 2014 Mehdi El Gueddari
 * http://mehdi.me
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 */
using System;

namespace DND.Common.Infrastructure.Interfaces.Data.UnitOfWork
{
    /// <summary>
    /// A read-only UnitOfWorkScope. Refer to the comments for IUnitOfWorkScope
    /// for more details.
    /// </summary>
    public interface IUnitOfWorkReadOnlyScope : IDisposable, IUnitOfWorkScope
    {
        /// <summary>
        /// The DbContext instances that this UnitOfWorkScope manages.
        /// </summary>
        IDbContextCollection DbContexts { get; }

    }
}