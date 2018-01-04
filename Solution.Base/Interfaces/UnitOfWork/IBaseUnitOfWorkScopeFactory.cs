/* 
 * Copyright (C) 2014 Mehdi El Gueddari
 * http://mehdi.me
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 */
using System;
using System.Data;
using System.Threading;

namespace Solution.Base.Interfaces.UnitOfWork
{
    /// <summary>
    /// Convenience methods to create a new ambient UnitOfWorkScope. This is the prefered method
    /// to create a UnitOfWorkScope.
    /// </summary>
    public interface IBaseUnitOfWorkScopeFactory
    {
        /// <summary>
        /// Creates a new UnitOfWorkScope.
        /// 
        /// By default, the new scope will join the existing ambient scope. This
        /// is what you want in most cases. This ensures that the same DbContext instances
        /// are used by all services methods called within the scope of a business transaction.
        /// 
        /// Set 'joiningOption' to 'ForceCreateNew' if you want to ignore the ambient scope
        /// and force the creation of new DbContext instances within that scope. Using 'F---orceCreateNew'
        /// is an advanced feature that should be used with great care and only if you fully understand the
        /// implications of doing this.
        /// </summary>
        IBaseUnitOfWorkTransactionScope Create(BaseUnitOfWorkScopeOption joiningOption = BaseUnitOfWorkScopeOption.JoinExisting, CancellationToken cancellationToken = default(CancellationToken) );

        /// <summary>
        /// Creates a new UnitOfWorkScope for read-only queries.
        /// 
        /// By default, the new scope will join the existing ambient scope. This
        /// is what you want in most cases. This ensures that the same DbContext instances
        /// are used by all services methods called within the scope of a business transaction.
        /// 
        /// Set 'joiningOption' to 'ForceCreateNew' if you want to ignore the ambient scope
        /// and force the creation of new DbContext instances within that scope. Using 'ForceCreateNew'
        /// is an advanced feature that should be used with great care and only if you fully understand the
        /// implications of doing this.
        /// </summary>
        IBaseUnitOfWorkReadOnlyScope CreateReadOnly(BaseUnitOfWorkScopeOption joiningOption = BaseUnitOfWorkScopeOption.JoinExisting, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Forces the creation of a new ambient UnitOfWorkScope (i.e. does not
        /// join the ambient scope if there is one) and wraps all DbContext instances
        /// created within that scope in an explicit database transaction with 
        /// the provided isolation level. 
        /// 
        /// WARNING: the database transaction will remain open for the whole 
        /// duration of the scope! So keep the scope as short-lived as possible.
        /// Don't make any remote API calls or perform any long running computation 
        /// within that scope.
        /// 
        /// This is an advanced feature that you should use very carefully
        /// and only if you fully understand the implications of doing this.
        /// </summary>
        IBaseUnitOfWorkTransactionScope CreateWithTransaction(IsolationLevel isolationLevel, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Forces the creation of a new ambient read-only UnitOfWorkScope (i.e. does not
        /// join the ambient scope if there is one) and wraps all DbContext instances
        /// created within that scope in an explicit database transaction with 
        /// the provided isolation level. 
        /// 
        /// WARNING: the database transaction will remain open for the whole 
        /// duration of the scope! So keep the scope as short-lived as possible.
        /// Don't make any remote API calls or perform any long running computation 
        /// within that scope.
        /// 
        /// This is an advanced feature that you should use very carefully
        /// and only if you fully understand the implications of doing this.
        /// </summary>
        IBaseUnitOfWorkReadOnlyScope CreateReadOnlyWithTransaction(IsolationLevel isolationLevel, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Temporarily suppresses the ambient UnitOfWorkScope. 
        /// 
        /// Always use this if you need to  kick off parallel tasks within a UnitOfWorkScope. 
        /// This will prevent the parallel tasks from using the current ambient scope. If you
        /// were to kick off parallel tasks within a UnitOfWorkScope without suppressing the ambient
        /// context first, all the parallel tasks would end up using the same ambient UnitOfWorkScope, which 
        /// would result in multiple threads accesssing the same DbContext instances at the same 
        /// time.
        /// </summary>
        IDisposable SuppressAmbientContext();
    }
}
