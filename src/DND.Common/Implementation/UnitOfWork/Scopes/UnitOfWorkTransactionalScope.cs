﻿/* 
 * Copyright (C) 2014 Mehdi El Gueddari
 * http://mehdi.me
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 */
using System;
using System.Collections;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Threading.Tasks;

using DND.Common.Interfaces.Repository;
using DND.Common.Interfaces.UnitOfWork;
using DND.Common.Interfaces.Persistance;
using DND.Common.Interfaces.Models;

namespace DND.Common.Implementation.UnitOfWork
{
    public class UnitOfWorkTransactionScope : BaseUnitOfWorkScope, IUnitOfWorkTransactionScope
    {
        public IBaseRepository<TEntity> Repository<TContext, TEntity>()
        where TContext : IBaseDbContext
        where TEntity : class, IBaseEntity, IBaseEntityAuditable, new()
        {
            return (IBaseRepository<TEntity>)_repositories.GetOrAdd(
                typeof(TEntity),
                t => _repositoryFactory.Get<TEntity>(DbContexts.Get<TContext>(), this, _cancellationToken)
            );
        }

        private bool _disposed;
        private bool _readOnly;
        private bool _completed;
        private bool _nested;
        private UnitOfWorkTransactionScope _parentScope;

        public UnitOfWorkTransactionScope(IDbContextFactory dbContextFactory = null, IAmbientDbContextLocator contextLocator = null, IGenericRepositoryFactory repositoryFactory = null, CancellationToken cancellationToken = default(CancellationToken)) :
            this(joiningOption: BaseUnitOfWorkScopeOption.JoinExisting, readOnly: false, isolationLevel: null, dbContextFactory: dbContextFactory, contextLocator : contextLocator, repositoryFactory : repositoryFactory, cancellationToken: cancellationToken)
        {}

        public UnitOfWorkTransactionScope(bool readOnly, IDbContextFactory dbContextFactory = null, IAmbientDbContextLocator contextLocator = null, IGenericRepositoryFactory repositoryFactory = null, CancellationToken cancellationToken = default(CancellationToken))
            : this(joiningOption: BaseUnitOfWorkScopeOption.JoinExisting, readOnly: readOnly, isolationLevel: null, dbContextFactory: dbContextFactory, contextLocator: contextLocator, repositoryFactory: repositoryFactory, cancellationToken: cancellationToken)
        {}

        public UnitOfWorkTransactionScope(BaseUnitOfWorkScopeOption joiningOption, bool readOnly, IsolationLevel? isolationLevel, IDbContextFactory dbContextFactory = null, IAmbientDbContextLocator contextLocator = null, IGenericRepositoryFactory repositoryFactory = null, CancellationToken cancellationToken = default(CancellationToken))
             :base(contextLocator: contextLocator, repositoryFactory: repositoryFactory, cancellationToken: cancellationToken)
        {
            if (isolationLevel.HasValue && joiningOption == BaseUnitOfWorkScopeOption.JoinExisting)
                throw new ArgumentException("Cannot join an ambient UnitOfWorkScope when an explicit database transaction is required. When requiring explicit database transactions to be used (i.e. when the 'isolationLevel' parameter is set), you must not also ask to join the ambient context (i.e. the 'joinAmbient' parameter must be set to false).");
            
            _disposed = false;
            _completed = false;
            _readOnly = readOnly;

            _parentScope = GetAmbientScope();
            if (_parentScope != null && joiningOption == BaseUnitOfWorkScopeOption.JoinExisting)
            {
                if (_parentScope._readOnly && !this._readOnly)
                {
                    throw new InvalidOperationException("Cannot nest a read/write UnitOfWorkScope within a read-only UnitOfWorkScope.");
                }

                _nested = true;
                _dbContexts = _parentScope._dbContexts;
            }
            else
            {
                _nested = false;
                _dbContexts = new DbContextCollection(readOnly, isolationLevel, dbContextFactory);
            }

            SetAmbientScope(this);
        }

        public int Complete()
        {
            if (_disposed)
                throw new ObjectDisposedException("UnitOfWorkScope");
            if (_completed)
                throw new InvalidOperationException("You cannot call SaveChanges() more than once on a UnitOfWorkScope. A UnitOfWorkScope is meant to encapsulate a business transaction: create the scope at the start of the business transaction and then call SaveChanges() at the end. Calling SaveChanges() mid-way through a business transaction doesn't make sense and most likely mean that you should refactor your service method into two separate service method that each create their own UnitOfWorkScope and each implement a single business transaction.");

            // Only save changes if we're not a nested scope. Otherwise, let the top-level scope 
            // decide when the changes should be saved.
            var c = 0;
            if (!_nested)
            {
                c = CommitInternal();
            }
            else
            {
                PreCommitInternal();
            }

            _completed = true;

            return c;
        }

        public Task<int> CompleteAsync()
        {
            return CompleteAsync(CancellationToken.None);
        }

        public async Task<int> CompleteAsync(CancellationToken cancelToken)
        {


            if (cancelToken == null)
                throw new ArgumentNullException("cancelToken");
            if (_disposed)
                throw new ObjectDisposedException("UnitOfWorkScope");
            if (_completed)
                throw new InvalidOperationException("You cannot call SaveChanges() more than once on a UnitOfWorkScope. A UnitOfWorkScope is meant to encapsulate a business transaction: create the scope at the start of the business transaction and then call SaveChanges() at the end. Calling SaveChanges() mid-way through a business transaction doesn't make sense and most likely mean that you should refactor your service method into two separate service method that each create their own UnitOfWorkScope and each implement a single business transaction.");

            // Only save changes if we're not a nested scope. Otherwise, let the top-level scope 
            // decide when the changes should be saved.
            var c = 0;
            if (!_nested)
            {
                c = await CommitInternalAsync(cancelToken).ConfigureAwait(false);
            }
            else
            {
                 await PreCommitInternalAsync(cancelToken).ConfigureAwait(false);
            }

            _completed = true;
            return c;


        }

        private void PreCommitInternal()
        {
            _dbContexts.PreCommit();
        }

        private Task PreCommitInternalAsync(CancellationToken cancelToken)
        {
            return _dbContexts.PreCommitAsync();
        }

        private int CommitInternal()
        {
            return _dbContexts.Commit();
        }

        private Task<int> CommitInternalAsync(CancellationToken cancelToken)
        {
            return _dbContexts.CommitAsync(cancelToken);
        }

        private void RollbackInternal()
        {
            _dbContexts.Rollback();
        }

        public void RefreshEntitiesInParentScope(IEnumerable entities)
        {
            if (entities == null)
                return;

            if (_parentScope == null)
                return;

            if (_nested) // The parent scope uses the same DbContext instances as we do - no need to refresh anything
                return;

            // OK, so we must loop through all the DbContext instances in the parent scope
            // and see if their first-level cache (i.e. their ObjectStateManager) contains the provided entities. 
            // If they do, we'll need to force a refresh from the database. 

            // I'm sorry for this code but it's the only way to do this with the current version of Entity Framework 
            // as far as I can see.

            // What would be much nicer would be to have a way to merge all the modified / added / deleted
            // entities from one DbContext instance to another. NHibernate has support for this sort of stuff 
            // but EF still lags behind in this respect. But there is hope: https://entityframework.codeplex.com/workitem/864

			// NOTE: DbContext implements the ObjectContext property of the IObjectContextAdapter interface explicitely.
			// So we must cast the DbContext instances to IObjectContextAdapter in order to access their ObjectContext.
			// This cast is completely safe.

			foreach (IObjectContextAdapter contextInCurrentScope in _dbContexts.InitializedDbContexts.Values)
            {
                var correspondingParentContext =
                    _parentScope._dbContexts.InitializedDbContexts.Values.SingleOrDefault(parentContext => parentContext.GetType() == contextInCurrentScope.GetType())
					as IObjectContextAdapter;

                if (correspondingParentContext == null)
                    continue; // No DbContext of this type has been created in the parent scope yet. So no need to refresh anything for this DbContext type.

                // Both our scope and the parent scope have an instance of the same DbContext type. 
                // We can now look in the parent DbContext instance for entities that need to
                // be refreshed.
                foreach (var toRefresh in entities)
                {
                    // First, we need to find what the EntityKey for this entity is. 
                    // We need this EntityKey in order to check if this entity has
                    // already been loaded in the parent DbContext's first-level cache (the ObjectStateManager).
                    ObjectStateEntry stateInCurrentScope;
                    if (contextInCurrentScope.ObjectContext.ObjectStateManager.TryGetObjectStateEntry(toRefresh, out stateInCurrentScope))
                    {
                        var key = stateInCurrentScope.EntityKey;

                        // Now we can see if that entity exists in the parent DbContext instance and refresh it.
                        ObjectStateEntry stateInParentScope;
                        if (correspondingParentContext.ObjectContext.ObjectStateManager.TryGetObjectStateEntry(key, out stateInParentScope))
                        {
                            // Only refresh the entity in the parent DbContext from the database if that entity hasn't already been
                            // modified in the parent. Otherwise, let the whatever concurency rules the application uses
                            // apply.
                            if (stateInParentScope.State == EntityState.Unchanged)
                            {
                                correspondingParentContext.ObjectContext.Refresh(RefreshMode.StoreWins, stateInParentScope.Entity);
                            }
                        }
                    }
                }
            }
        }

        public async Task RefreshEntitiesInParentScopeAsync(IEnumerable entities)
        {
            // See comments in the sync version of this method for an explanation of what we're doing here.

            if (entities == null)
                return;

            if (_parentScope == null)
                return;

            if (_nested) 
                return;

			foreach (IObjectContextAdapter contextInCurrentScope in _dbContexts.InitializedDbContexts.Values)
            {
                var correspondingParentContext =
                    _parentScope._dbContexts.InitializedDbContexts.Values.SingleOrDefault(parentContext => parentContext.GetType() == contextInCurrentScope.GetType())
					as IObjectContextAdapter;

                if (correspondingParentContext == null)
                    continue; 

                foreach (var toRefresh in entities)
                {
                    ObjectStateEntry stateInCurrentScope;
                    if (contextInCurrentScope.ObjectContext.ObjectStateManager.TryGetObjectStateEntry(toRefresh, out stateInCurrentScope))
                    {
                        var key = stateInCurrentScope.EntityKey;

                        ObjectStateEntry stateInParentScope;
                        if (correspondingParentContext.ObjectContext.ObjectStateManager.TryGetObjectStateEntry(key, out stateInParentScope))
                        {
                            if (stateInParentScope.State == EntityState.Unchanged)
                            {
                                await correspondingParentContext.ObjectContext.RefreshAsync(RefreshMode.StoreWins, stateInParentScope.Entity).ConfigureAwait(false);
                            }
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            // Commit / Rollback and dispose all of our DbContext instances
            if (!_nested)
            {
                if (!_completed)
                {
                    // Do our best to clean up as much as we can but don't throw here as it's too late anyway.
                    try
                    {
                        if (_readOnly)
                        {
                            // Disposing a read-only scope before having called its SaveChanges() method
                            // is the normal and expected behavior. Read-only scopes get committed automatically.
                            CommitInternal();
                        }
                        else
                        {
                            // Disposing a read/write scope before having called its SaveChanges() method
                            // indicates that something went wrong and that all changes should be rolled-back.
                            RollbackInternal();
                        }
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(e);
                    }

                    _completed = true;
                }

                _dbContexts.Dispose();
            }

            // Pop ourself from the ambient scope stack
            var currentAmbientScope = GetAmbientScope();
            if (currentAmbientScope != this) // This is a serious programming error. Worth throwing here.
                throw new InvalidOperationException("UnitOfWorkScope instances must be disposed of in the order in which they were created!");

            RemoveAmbientScope();

            if (_parentScope != null)
            {
                if (_parentScope._disposed)
                {
                    /*
                     * If our parent scope has been disposed before us, it can only mean one thing:
                     * someone started a parallel flow of execution and forgot to suppress the
                     * ambient context before doing so. And we've been created in that parallel flow.
                     * 
                     * Since the CallContext flows through all async points, the ambient scope in the 
                     * main flow of execution ended up becoming the ambient scope in this parallel flow
                     * of execution as well. So when we were created, we captured it as our "parent scope". 
                     * 
                     * The main flow of execution then completed while our flow was still ongoing. When 
                     * the main flow of execution completed, the ambient scope there (which we think is our 
                     * parent scope) got disposed of as it should.
                     * 
                     * So here we are: our parent scope isn't actually our parent scope. It was the ambient
                     * scope in the main flow of execution from which we branched off. We should never have seen 
                     * it. Whoever wrote the code that created this parallel task should have suppressed
                     * the ambient context before creating the task - that way we wouldn't have captured
                     * this bogus parent scope.
                     * 
                     * While this is definitely a programming error, it's not worth throwing here. We can only 
                     * be in one of two scenario:
                     * 
                     * - If the developer who created the parallel task was mindful to force the creation of 
                     * a new scope in the parallel task (with IUnitOfWorkScopeFactory.CreateNew() instead of 
                     * JoinOrCreate()) then no harm has been done. We haven't tried to access the same DbContext
                     * instance from multiple threads.
                     * 
                     * - If this was not the case, they probably already got an exception complaining about the same
                     * DbContext or ObjectContext being accessed from multiple threads simultaneously (or a related
                     * error like multiple active result sets on a DataReader, which is caused by attempting to execute
                     * several queries in parallel on the same DbContext instance). So the code has already blow up.
                     * 
                     * So just record a warning here. Hopefully someone will see it and will fix the code.
                     */

                    var message = @"PROGRAMMING ERROR - When attempting to dispose a UnitOfWorkScope, we found that our parent UnitOfWorkScope has already been disposed! This means that someone started a parallel flow of execution (e.g. created a TPL task, created a thread or enqueued a work item on the ThreadPool) within the context of a UnitOfWorkScope without suppressing the ambient context first. 

In order to fix this:
1) Look at the stack trace below - this is the stack trace of the parallel task in question.
2) Find out where this parallel task was created.
3) Change the code so that the ambient context is suppressed before the parallel task is created. You can do this with IUnitOfWorkScopeFactory.SuppressAmbientContext() (wrap the parallel task creation code block in this). 

Stack Trace:
" + Environment.StackTrace;

                    System.Diagnostics.Debug.WriteLine(message);
                }
                else
                {
                    SetAmbientScope(_parentScope);
                }
            }

            _disposed = true;

        }

        #region Ambient Context Logic

        /*
         * This is where all the magic happens. And there is not much of it.
         * 
         * This implementation is inspired by the source code of the
         * TransactionScope class in .NET 4.5.1 (the TransactionScope class
         * is prior versions of the .NET Fx didn't have support for async
         * operations).
         * 
         * In order to understand this, you'll need to be familiar with the
         * concept of async points. You'll also need to be familiar with the
         * ExecutionContext and CallContext and understand how and why they 
         * flow through async points. Stephen Toub has written an
         * excellent blog post about this - it's a highly recommended read:
         * http://blogs.msdn.com/b/pfxteam/archive/2012/06/15/executioncontext-vs-synchronizationcontext.aspx
         * 
         * Overview: 
         * 
         * We want our UnitOfWorkScope instances to be ambient within 
         * the context of a logical flow of execution. This flow may be 
         * synchronous or it may be asynchronous.
         * 
         * If we only wanted to support the synchronous flow scenario, 
         * we could just store our UnitOfWorkScope instances in a ThreadStatic 
         * variable. That's the "traditional" (i.e. pre-async) way of implementing
         * an ambient context in .NET. You can see an example implementation of 
         * a TheadStatic-based ambient DbContext here: http://coding.abel.nu/2012/10/make-the-dbcontext-ambient-with-unitofworkscope/ 
         * 
         * But that would be hugely limiting as it would prevent us from being
         * able to use the new async features added to Entity Framework
         * in EF6 and .NET 4.5.
         * 
         * So we need a storage place for our UnitOfWorkScope instances 
         * that can flow through async points so that the ambient context is still 
         * available after an await (or any other async point). And this is exactly 
         * what CallContext is for.
         * 
         * There are however two issues with storing our UnitOfWorkScope instances 
         * in the CallContext:
         * 
         * 1) Items stored in the CallContext should be serializable. That's because
         * the CallContext flows not just through async points but also through app domain 
         * boundaries. I.e. if you make a remoting call into another app domain, the
         * CallContext will flow through this call (which will require all the values it
         * stores to get serialized) and get restored in the other app domain.
         * 
         * In our case, our UnitOfWorkScope instances aren't serializable. And in any case,
         * we most definitely don't want them to be flown accross app domains. So we'll
         * use the trick used by the TransactionScope class to work around this issue.
         * Instead of storing our UnitOfWorkScope instances themselves in the CallContext,
         * we'll just generate a unique key for each instance and only store that key in 
         * the CallContext. We'll then store the actual UnitOfWorkScope instances in a static
         * Dictionary against their key. 
         * 
         * That way, if an app domain boundary is crossed, the keys will be flown accross
         * but not the UnitOfWorkScope instances since a static variable is stored at the 
         * app domain level. The code executing in the other app domain won't see the ambient
         * UnitOfWorkScope created in the first app domain and will therefore be able to create
         * their own ambient UnitOfWorkScope if necessary.
         * 
         * 2) The CallContext is flow through *all* async points. This means that if someone
         * decides to create multiple threads within the scope of a UnitOfWorkScope, our ambient scope
         * will flow through all the threads. Which means that all the threads will see that single 
         * UnitOfWorkScope instance as being their ambient DbContext. So clients need to be 
         * careful to always suppress the ambient context before kicking off a parallel operation
         * to avoid our DbContext instances from being accessed from multiple threads.
         * 
         */

        private static readonly string AmbientUnitOfWorkScopeKey = "AmbientDbcontext_" + Guid.NewGuid();

        // Use a ConditionalWeakTable instead of a simple ConcurrentDictionary to store our UnitOfWorkScope instances 
        // in order to prevent leaking UnitOfWorkScope instances if someone doesn't dispose them properly.
        //
        // For example, if we used a ConcurrentDictionary and someone let go of a UnitOfWorkScope instance without 
        // disposing it, our ConcurrentDictionary would still have a reference to it, preventing
        // the GC from being able to collect it => leak. With a ConditionalWeakTable, we don't hold a reference
        // to the UnitOfWorkScope instances we store in there, allowing them to get GCed.
        // The doc for ConditionalWeakTable isn't the best. This SO anser does a good job at explaining what 
        // it does: http://stackoverflow.com/a/18613811
        private static readonly ConditionalWeakTable<InstanceIdentifier, UnitOfWorkTransactionScope> UnitOfWorkScopeInstances = new ConditionalWeakTable<InstanceIdentifier, UnitOfWorkTransactionScope>();

        private InstanceIdentifier _instanceIdentifier = new InstanceIdentifier();

        /// <summary>
        /// Makes the provided 'UnitOfWorkScope' available as the the ambient scope via the CallContext.
        /// </summary>
        internal static void SetAmbientScope(UnitOfWorkTransactionScope newAmbientScope)
        {
            if (newAmbientScope == null)
                throw new ArgumentNullException("newAmbientScope");

            var current = CallContext.LogicalGetData(AmbientUnitOfWorkScopeKey) as InstanceIdentifier;

            if (current == newAmbientScope._instanceIdentifier)
                return;

            // Store the new scope's instance identifier in the CallContext, making it the ambient scope
            CallContext.LogicalSetData(AmbientUnitOfWorkScopeKey, newAmbientScope._instanceIdentifier);

            // Keep track of this instance (or do nothing if we're already tracking it)
            UnitOfWorkScopeInstances.GetValue(newAmbientScope._instanceIdentifier, key => newAmbientScope);
        }

        /// <summary>
        /// Clears the ambient scope from the CallContext and stops tracking its instance. 
        /// Call this when a UnitOfWorkScope is being disposed.
        /// </summary>
        internal static void RemoveAmbientScope()
        {
            var current = CallContext.LogicalGetData(AmbientUnitOfWorkScopeKey) as InstanceIdentifier;
            CallContext.LogicalSetData(AmbientUnitOfWorkScopeKey, null);

            // If there was an ambient scope, we can stop tracking it now
            if (current != null)
            {
                UnitOfWorkScopeInstances.Remove(current);
            }
        }

        /// <summary>
        /// Clears the ambient scope from the CallContext but keeps tracking its instance. Call this to temporarily 
        /// hide the ambient context (e.g. to prevent it from being captured by parallel task).
        /// </summary>
        internal static void HideAmbientScope()
        {
            CallContext.LogicalSetData(AmbientUnitOfWorkScopeKey, null);
        }

        /// <summary>
        /// Get the current ambient scope or null if no ambient scope has been setup.
        /// </summary>
        internal static UnitOfWorkTransactionScope GetAmbientScope()
        {
            // Retrieve the identifier of the ambient scope (if any)
            var instanceIdentifier = CallContext.LogicalGetData(AmbientUnitOfWorkScopeKey) as InstanceIdentifier;
            if (instanceIdentifier == null)
                return null; // Either no ambient context has been set or we've crossed an app domain boundary and have (intentionally) lost the ambient context

            // Retrieve the UnitOfWorkScope instance corresponding to this identifier
            UnitOfWorkTransactionScope ambientScope;
            if (UnitOfWorkScopeInstances.TryGetValue(instanceIdentifier, out ambientScope))
                return ambientScope;

            // We have an instance identifier in the CallContext but no corresponding instance
            // in our UnitOfWorkScopeInstances table. This should never happen! The only place where
            // we remove the instance from the UnitOfWorkScopeInstances table is in RemoveAmbientScope(),
            // which also removes the instance identifier from the CallContext. 
            //
            // There's only one scenario where this could happen: someone let go of a UnitOfWorkScope 
            // instance without disposing it. In that case, the CallContext
            // would still contain a reference to the scope and we'd still have that scope's instance
            // in our UnitOfWorkScopeInstances table. But since we use a ConditionalWeakTable to store 
            // our UnitOfWorkScope instances and are therefore only holding a weak reference to these instances, 
            // the GC would be able to collect it. Once collected by the GC, our ConditionalWeakTable will return
            // null when queried for that instance. In that case, we're OK. This is a programming error 
            // but our use of a ConditionalWeakTable prevented a leak.
            System.Diagnostics.Debug.WriteLine("Programming error detected. Found a reference to an ambient UnitOfWorkScope in the CallContext but didn't have an instance for it in our UnitOfWorkScopeInstances table. This most likely means that this UnitOfWorkScope instance wasn't disposed of properly. UnitOfWorkScope instance must always be disposed. Review the code for any UnitOfWorkScope instance used outside of a 'using' block and fix it so that all UnitOfWorkScope instances are disposed of.");
            return null;
        }

        #endregion
    }

    /*
     * The idea of using an object reference as our instance identifier 
     * instead of simply using a unique string (which we could have generated
     * with Guid.NewGuid() for example) comes from the TransactionScope
     * class. As far as I can make out, a string would have worked just fine.
     * I'm guessing that this is done for optimization purposes. Creating
     * an empty class is cheaper and uses up less memory than generating
     * a unique string.
    */
    internal class InstanceIdentifier : MarshalByRefObject
    { }
}

    