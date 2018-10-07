/* 
 * Copyright (C) 2014 Mehdi El Gueddari
 * http://mehdi.me
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 */
using DND.Common.Data.DomainEvents;
using DND.Common.Infrastructure.Helpers;
using DND.Common.Infrastructure.Interfaces.Data;
using DND.Common.Infrastructure.Interfaces.Data.UnitOfWork;
using DND.Common.Infrastructure.Interfaces.DomainEvents;
using DND.Common.Infrastructure.Validation;
using DND.Common.Infrastructure.Validation.Errors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Common.Data.UnitOfWork
{
    /// <summary>
    /// As its name suggests, DbContextCollection maintains a collection of DbContext instances.
    /// 
    /// What it does in a nutshell:
    /// - Lazily instantiates DbContext instances when its Get Of TDbContext () method is called
    /// (and optionally starts an explicit database transaction).
    /// - Keeps track of the DbContext instances it created so that it can return the existing
    /// instance when asked for a DbContext of a specific type.
    /// - Takes care of committing / rolling back changes and transactions on all the DbContext
    /// instances it created when its Commit() or Rollback() method is called.
    /// 
    /// </summary>
    public class DbContextCollection : IDbContextCollection
    {
        private Dictionary<Type, (DbContext dbContext, DbContextDomainEventsEFCoreAdapter domainEvents)> _initializedDbContexts;
        private Dictionary<DbContext, IDbContextTransaction> _transactions;
        private IsolationLevel? _isolationLevel;
        private readonly IDbContextFactoryProducerSingleton _dbContextFactory;
        private readonly IDomainEvents _domainEvents;
        private bool _disposed;
        private bool _completed;
        private bool _readOnly;

        internal Dictionary<Type, (DbContext dbContext, DbContextDomainEventsEFCoreAdapter domainEvents)> InitializedDbContexts { get { return _initializedDbContexts; } }

        public DbContextCollection(bool readOnly = false, IsolationLevel? isolationLevel = null, IDbContextFactoryProducerSingleton dbContextFactory = null, IDomainEvents domainEvents = null)
        {
            _disposed = false;
            _completed = false;

            _initializedDbContexts = new Dictionary<Type, (DbContext dbContext, DbContextDomainEventsEFCoreAdapter domainEvents)>();
            _transactions = new Dictionary<DbContext, IDbContextTransaction>();

            _readOnly = readOnly;
            _isolationLevel = isolationLevel;
            _dbContextFactory = dbContextFactory;
            _domainEvents = domainEvents;
        }

        public TDbContext Get<TDbContext>() where TDbContext : DbContext
        {
            if (_disposed)
                throw new ObjectDisposedException("DbContextCollection");

            var requestedType = typeof(TDbContext);

            if (!_initializedDbContexts.ContainsKey(requestedType))
            {
                // First time we've been asked for this particular DbContext type.
                // Create one, cache it and start its database transaction if needed.
                var dbContext = _dbContextFactory != null
                    ? _dbContextFactory.GetFactory<TDbContext>().CreateDbContext()
                    : Activator.CreateInstance<TDbContext>();

                _initializedDbContexts.Add(requestedType, (dbContext, new DbContextDomainEventsEFCoreAdapter(dbContext, _domainEvents)));

                if (_readOnly)
                {
                    dbContext.ChangeTracker.AutoDetectChangesEnabled = false;
                }
                
                //EF uses a transaction when calling SaveChanges(). Only need explicit transaction if executing raw SQL
                if (_isolationLevel.HasValue)
                {
                    var tran = dbContext.Database.BeginTransaction(_isolationLevel.Value);
                    _transactions.Add(dbContext, tran);
                }
            }

            return (TDbContext)_initializedDbContexts[requestedType].dbContext;
        }

        public IEnumerable<DbEntityValidationResult> GetValidationErrors(DbContext context, DbContextDomainEventsEFCoreAdapter domainEvents, bool newChanges)
        {
            var list = new List<DbEntityValidationResult>();


            var entities = context.ChangeTracker.Entries().Where(e => ((e.State == EntityState.Added) || (e.State == EntityState.Modified)));
            if(newChanges)
            {
                entities= entities.Where(x => !domainEvents.GetPreCommittedDeletedEntities().Contains(x) && !domainEvents.GetPreCommittedInsertedEntities().Contains(x));
            }

            foreach (var entry in entities)
            {
                var entity = entry.Entity;

                var results = ValidationHelper.ValidateObject(entity);


                if (results.Count() > 0)
                {
                    var errors = results.Where(r => r != ValidationResult.Success);

                    if (errors.Count() > 0)
                    {
                        var dbValidationErrors = new List<DbValidationError>();
                        foreach (ValidationResult error in errors)
                        {
                            if (error.MemberNames.Count() > 0)
                            {
                                foreach (var prop in error.MemberNames)
                                {
                                    dbValidationErrors.Add(new DbValidationError(prop, error.ErrorMessage));
                                }
                            }
                            else
                            {
                                dbValidationErrors.Add(new DbValidationError("", error.ErrorMessage));
                            }
                        }

                        var validationResult = new DbEntityValidationResult(dbValidationErrors);

                        list.Add(validationResult);
                    }
                }
            }

            return list;
        }

        public Result PreCommit()
        {
            if (_disposed)
                throw new ObjectDisposedException("DbContextCollection");
            if (_completed)
                throw new InvalidOperationException("You can't call Commit() or Rollback() more than once on a DbContextCollection. All the changes in the DbContext instances managed by this collection have already been saved or rollback and all database transactions have been completed and closed. If you wish to make more data changes, create a new DbContextCollection and make your changes there.");

            // Best effort. You'll note that we're not actually implementing an atomic commit 
            // here. It entirely possible that one DbContext instance will be committed successfully
            // and another will fail. Implementing an atomic commit would require us to wrap
            // all of this in a TransactionScope. The problem with TransactionScope is that 
            // the database transaction it creates may be automatically promoted to a 
            // distributed transaction if our DbContext instances happen to be using different 
            // databases. And that would require the DTC service (Distributed Transaction Coordinator)
            // to be enabled on all of our live and dev servers as well as on all of our dev workstations.
            // Otherwise the whole thing would blow up at runtime. 

            // In practice, if our services are implemented following a reasonably DDD approach,
            // a business transaction (i.e. a service method) should only modify entities in a single
            // DbContext. So we should never find ourselves in a situation where two DbContext instances
            // contain uncommitted changes here. We should therefore never be in a situation where the below
            // would result in a partial commit. 

            ExceptionDispatchInfo lastError = null;

            foreach (var dbContext in _initializedDbContexts.Values)
            {
                if (!_readOnly)
                {
                    var errors = GetValidationErrors(dbContext.dbContext, dbContext.domainEvents, true);
                    if (errors.Count() > 0)
                    {
                        return Result.DatabaseErrors(errors);
                        //throw new DatabaseValidationErrors(errors);
                        //ThrowEnhancedValidationException(errors);
                    }
                }
            }

            foreach (var dbContext in _initializedDbContexts.Values)
            {
                try
                {
                    if (!_readOnly)
                    {
                        dbContext.domainEvents.FirePreCommitEventsAsync().Wait();
                    }

                }
                catch (Exception e)
                {
                    lastError = ExceptionDispatchInfo.Capture(e);
                }
            }

            if (lastError != null)
                lastError.Throw(); // Re-throw while maintaining the exception's original stack track

            return Result.Ok();
        }

        public Result<int> Commit()
        {
            if (_disposed)
                throw new ObjectDisposedException("DbContextCollection");
            if (_completed)
                throw new InvalidOperationException("You can't call Commit() or Rollback() more than once on a DbContextCollection. All the changes in the DbContext instances managed by this collection have already been saved or rollback and all database transactions have been completed and closed. If you wish to make more data changes, create a new DbContextCollection and make your changes there.");

            // Best effort. You'll note that we're not actually implementing an atomic commit 
            // here. It entirely possible that one DbContext instance will be committed successfully
            // and another will fail. Implementing an atomic commit would require us to wrap
            // all of this in a TransactionScope. The problem with TransactionScope is that 
            // the database transaction it creates may be automatically promoted to a 
            // distributed transaction if our DbContext instances happen to be using different 
            // databases. And that would require the DTC service (Distributed Transaction Coordinator)
            // to be enabled on all of our live and dev servers as well as on all of our dev workstations.
            // Otherwise the whole thing would blow up at runtime. 

            // In practice, if our services are implemented following a reasonably DDD approach,
            // a business transaction (i.e. a service method) should only modify entities in a single
            // DbContext. So we should never find ourselves in a situation where two DbContext instances
            // contain uncommitted changes here. We should therefore never be in a situation where the below
            // would result in a partial commit. 

            ExceptionDispatchInfo lastError = null;

            foreach (var dbContext in _initializedDbContexts.Values)
            {
                if (!_readOnly)
                {
                    var errors = GetValidationErrors(dbContext.dbContext, dbContext.domainEvents, false);
                    if (errors.Count() > 0)
                    {
                        return Result.DatabaseErrors<int>(errors);
                        //throw new DatabaseValidationErrors(errors);
                    }
                }
            }

            //Fire PreCommit Events to Chain Transactions. If any errors are thrown Save won't occur.
            foreach (var dbContext in _initializedDbContexts.Values)
            {
                try
                {
                    if (!_readOnly)
                    {
                        dbContext.domainEvents.FirePreCommitEventsAsync().Wait();
                        //Post commit events will fire automatically on transaction commit.
                    }
                }
                catch (Exception e)
                {
                    lastError = ExceptionDispatchInfo.Capture(e);
                }
            }

            var c = 0;

            if(lastError == null)
            {
                foreach (var dbContext in _initializedDbContexts.Values)
                {
                    try
                    {
                        if (!_readOnly)
                        {
                            dbContext.domainEvents.FirePreCommitEventsAsync().Wait();
                            c += dbContext.dbContext.SaveChanges();
                            dbContext.domainEvents.FirePostCommitEventsAsync().Wait();
                        }

                        // If we've started an explicit database transaction, time to commit it now.
                        var tran = GetValueOrDefault(_transactions, dbContext.dbContext);
                        if (tran != null)
                        {
                            tran.Commit();
                            tran.Dispose();
                        }
                    }
                    catch (Exception e)
                    {
                        lastError = ExceptionDispatchInfo.Capture(e);
                    }
                }
            }

            _transactions.Clear();
            _completed = true;

            if (lastError != null)
                lastError.Throw(); // Re-throw while maintaining the exception's original stack track

            return Result.Ok(c);
        }

        public Task<Result> PreCommitAsync()
        {
            return PreCommitAsync(CancellationToken.None);
        }

        public async Task<Result> PreCommitAsync(CancellationToken cancelToken)
        {
            if (cancelToken == null)
                throw new ArgumentNullException("cancelToken");
            if (_disposed)
                throw new ObjectDisposedException("DbContextCollection");
            if (_completed)
                throw new InvalidOperationException("You can't call Commit() or Rollback() more than once on a DbContextCollection. All the changes in the DbContext instances managed by this collection have already been saved or rollback and all database transactions have been completed and closed. If you wish to make more data changes, create a new DbContextCollection and make your changes there.");

            // See comments in the sync version of this method for more details.

            ExceptionDispatchInfo lastError = null;

            foreach (var dbContext in _initializedDbContexts.Values)
            {
                if (!_readOnly)
                {
                    var errors = GetValidationErrors(dbContext.dbContext, dbContext.domainEvents, true);
                    if (errors.Count() > 0)
                    {
                        return Result.DatabaseErrors(errors);
                        //throw new DatabaseValidationErrors(errors);
                        //ThrowEnhancedValidationException(errors);
                    }
                }
            }

            foreach (var dbContext in _initializedDbContexts.Values)
            {
                try
                {
                    if (!_readOnly)
                    {

                        await dbContext.domainEvents.FirePreCommitEventsAsync().ConfigureAwait(false);

                    }
                }
                catch (Exception e)
                {
                    lastError = ExceptionDispatchInfo.Capture(e);
                }
            }

            if (lastError != null)
                lastError.Throw(); // Re-throw while maintaining the exception's original stack track

            return Result.Ok();
        }

        public Task<Result<int>> CommitAsync()
        {
            return CommitAsync(CancellationToken.None);
        }

        public async Task<Result<int>> CommitAsync(CancellationToken cancelToken)
        {
            if (cancelToken == null)
                throw new ArgumentNullException("cancelToken");
            if (_disposed)
                throw new ObjectDisposedException("DbContextCollection");
            if (_completed)
                throw new InvalidOperationException("You can't call Commit() or Rollback() more than once on a DbContextCollection. All the changes in the DbContext instances managed by this collection have already been saved or rollback and all database transactions have been completed and closed. If you wish to make more data changes, create a new DbContextCollection and make your changes there.");

            // See comments in the sync version of this method for more details.

            ExceptionDispatchInfo lastError = null;

            foreach (var dbContext in _initializedDbContexts.Values)
            {
                if (!_readOnly)
                {
                    var errors = GetValidationErrors(dbContext.dbContext, dbContext.domainEvents, false);
                    if (errors.Count() > 0)
                    {
                        return Result.DatabaseErrors<int>(errors);
                        //throw new DatabaseValidationErrors(errors);
                    }
                }
            }

            //Fire PreCommit Events to Chain Transactions. If any errors are thrown Save won't occur.
            foreach (var dbContext in _initializedDbContexts.Values)
            {
                try
                {
                    if (!_readOnly)
                    {
                       await dbContext.domainEvents.FirePreCommitEventsAsync();
                    }
                }
                catch (Exception e)
                {
                    lastError = ExceptionDispatchInfo.Capture(e);
                }
            }

            var c = 0;

            if(lastError == null)
            {
                foreach (var dbContext in _initializedDbContexts.Values)
                {
                    try
                    {
                        if (!_readOnly)
                        {
                            await dbContext.domainEvents.FirePreCommitEventsAsync();
                            c += await dbContext.dbContext.SaveChangesAsync(cancelToken).ConfigureAwait(false);
                            await dbContext.domainEvents.FirePostCommitEventsAsync();
                        }

                        // If we've started an explicit database transaction, time to commit it now.
                        var tran = GetValueOrDefault(_transactions, dbContext.dbContext);
                        if (tran != null)
                        {
                            tran.Commit();
                            tran.Dispose();
                        }
                    }
                    catch (Exception e)
                    {
                        lastError = ExceptionDispatchInfo.Capture(e);
                    }
                }
            }

            _transactions.Clear();
            _completed = true;

            if (lastError != null)
                lastError.Throw(); // Re-throw while maintaining the exception's original stack track

            return Result.Ok(c);
        }

        public void Rollback()
        {
            if (_disposed)
                throw new ObjectDisposedException("DbContextCollection");
            if (_completed)
                throw new InvalidOperationException("You can't call Commit() or Rollback() more than once on a DbContextCollection. All the changes in the DbContext instances managed by this collection have already been saved or rollback and all database transactions have been completed and closed. If you wish to make more data changes, create a new DbContextCollection and make your changes there.");

            ExceptionDispatchInfo lastError = null;

            foreach (var dbContext in _initializedDbContexts.Values)
            {
                // There's no need to explicitly rollback changes in a DbContext as
                // DbContext doesn't save any changes until its SaveChanges() method is called.
                // So "rolling back" for a DbContext simply means not calling its SaveChanges()
                // method. 

                // But if we've started an explicit database transaction, then we must roll it back.
                var tran = GetValueOrDefault(_transactions, dbContext.dbContext);
                if (tran != null)
                {
                    try
                    {
                        tran.Rollback();
                        tran.Dispose();
                    }
                    catch (Exception e)
                    {
                        lastError = ExceptionDispatchInfo.Capture(e);
                    }
                }
            }

            _transactions.Clear();
            _completed = true;

            if (lastError != null)
                lastError.Throw(); // Re-throw while maintaining the exception's original stack track
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            // Do our best here to dispose as much as we can even if we get errors along the way.
            // Now is not the time to throw. Correctly implemented applications will have called
            // either Commit() or Rollback() first and would have got the error there.

            if (!_completed)
            {
                try
                {
                    if (_readOnly) Commit();
                    else Rollback();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                }
            }

            foreach (var dbContext in _initializedDbContexts.Values)
            {
                try
                {
                    dbContext.dbContext.Dispose();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                }
            }

            _initializedDbContexts.Clear();
            _disposed = true;
        }

        /// <summary>
        /// Returns the value associated with the specified key or the default 
        /// value for the TValue  type.
        /// </summary>
        private static TValue GetValueOrDefault<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue value;
            return dictionary.TryGetValue(key, out value) ? value : default(TValue);
        }
    }
}