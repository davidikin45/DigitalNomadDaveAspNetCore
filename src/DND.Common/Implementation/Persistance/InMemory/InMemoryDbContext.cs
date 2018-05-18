﻿using DND.Common.Implementation.Validation;
using DND.Common.Interfaces.Models;
using DND.Common.Interfaces.Persistance;
using RefactorThis.GraphDiff;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Common.Implementation.Persistance.InMemory
{
    public class InMemoryDataContext : IBaseDbContext
    {
        public class QueueItem
        {
            public QueueItem(Object entity)
            {
                Entity = entity;
                Timestamp = DateTime.Now;
            }
            public Object Entity { get; }
            public DateTime Timestamp { get; }
        }

        internal readonly ObjectRepresentationRepository repo;
        private readonly List<QueueItem> updateQueue = new List<QueueItem>();
        private readonly List<QueueItem> addQueue = new List<QueueItem>();
        private readonly List<QueueItem> removeQueue = new List<QueueItem>();

        public bool AutoDetectChanges { get; set; }

        public event EventHandler<BeforeSave> BeforeSave;
        public event EventHandler<AfterSave> AfterSave;

        public InMemoryDataContext()
        {
            repo = new ObjectRepresentationRepository();
            RegisterIIdentifiables();
        }

        internal InMemoryDataContext(ObjectRepresentationRepository repo)
        {
            this.repo = repo;
            RegisterIIdentifiables();
        }

        private void RegisterIIdentifiables()
        {
            RegisterIdentityStrategy(new IntegerIdentityStrategy<IBaseEntity<int>>(x => x.Id));
            RegisterIdentityStrategy(new ShortIdentityStrategy<IBaseEntity<short>>(x => x.Id));
            RegisterIdentityStrategy(new LongIdentityStrategy<IBaseEntity<long>>(x => x.Id));
            RegisterIdentityStrategy(new GuidIdentityStrategy<IBaseEntity<Guid>>(x => x.Id));
        }

        public void Dispose()
        {
        }

        /// <summary>
        /// This method allows you to register database "identity" like strategies for auto incrementing keys, or new guid keys, etc...
        /// </summary>
        /// <param name="identityStrategy">The strategy to use for an object</param>
        /// <typeparam name="T">The type to use it from</typeparam>
        public void RegisterIdentityStrategy<TEntity>(IIdentityStrategy<TEntity> identityStrategy) where TEntity : class
        {
            if (repo.IdentityStrategies.ContainsKey(typeof(TEntity)))
            {
                repo.IdentityStrategies[typeof(TEntity)] = obj => identityStrategy.Assign((TEntity)obj);
            }
            else
            {
                repo.IdentityStrategies.Add(typeof(TEntity), obj => identityStrategy.Assign((TEntity)obj));
            }
        }

        /// <summary>
        /// Processes the held but uncommitted adds and removes from the context
        /// </summary>
        protected void ProcessCommitQueues()
        {
            ClearUpdateQueue();
            AddAllFromQueueIntoRepository();
            RemoveAllFromQueueFromRepository();
        }

        private void AddAllFromQueueIntoRepository()
        {
            while (addQueue.Count > 0)
            {
                var first = addQueue.First();
                addQueue.RemoveAt(0);
                repo.Add(first.Entity);
            }
        }

        private void ClearUpdateQueue()
        {
            updateQueue.Clear();
        }

        private void RemoveAllFromQueueFromRepository()
        {
            while (removeQueue.Count > 0)
            {
                var first = removeQueue.First();
                removeQueue.RemoveAt(0);
                repo.Remove(first.Entity);
            }
        }

        public int SaveChanges()
        {
            BaseDbContext.AddTimestamps(addQueue.Select(x=>x.Entity), updateQueue.Select(x => x.Entity));
            BeforeSave?.Invoke(this, new BeforeSave());

            var all = addQueue.Select(x => x.Entity).Concat(updateQueue.Select(x => x.Entity)).Concat(removeQueue.Select(x => x.Entity));
            var update = updateQueue.Select(x => x.Entity);
            var insert = addQueue.Select(x => x.Entity);
            var delete = removeQueue.Select(x => x.Entity);

            BaseDbContext.RaiseDomainEventsPreCommit(all, insert, update, delete);

            ProcessCommitQueues();
            repo.Commit();

            BaseDbContext.RaiseDomainEventsPostCommit(all, insert, update, delete);

            AfterSave?.Invoke(this, new AfterSave());
            return 0;
        }

        public Task<int> SaveChangesAsync()
        {
            var task = new Task<int>(SaveChanges);
            task.Start();
            return task;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            var task = new Task<int>(SaveChanges);
            task.Start();
            return task;
        }

        public IBaseDbContextTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            throw new NotImplementedException();
        }

        public void AddEntity<TEntity>(TEntity entity) where TEntity : class
        {
            addQueue.Add(new QueueItem(entity));
        }

        public void AttachEntity<TEntity>(TEntity entity) where TEntity : class
        {
            //Don't need to do anything
        }

        public void RemoveEntity<TEntity>(TEntity entity) where TEntity : class
        {
            removeQueue.Add(new QueueItem(entity));
        }

        public TEntity FindEntity<TEntity>(object id) where TEntity : class
        {
            return repo.FindEntity<TEntity>(id);
        }

        TEntity IBaseDbContext.FindEntityLocal<TEntity>(object id)
        {
            return repo.FindEntity<TEntity>(id);
        }

        public IQueryable<TEntity> Queryable<TEntity>() where TEntity : class
        {
            return repo.Data<TEntity>();
        }

        public IQueryable<object> Queryable(Type type)
        {
            return repo.Data(type);
        }

        public void UpdateEntity(object existingEntity, object newEntity)
        {
            updateQueue.Add(new QueueItem(newEntity));
        }

        public TEntity UpdateGraph<TEntity>(TEntity entity, Expression<Func<IUpdateConfiguration<TEntity>, object>> mapping = null) where TEntity : class, new()
        {
            updateQueue.Add(new QueueItem(entity));
            return entity;
        }

        public bool IsEntityStateAdded(object entity)
        {
            return addQueue.Any(x => x.Entity == entity);
        }

        public void SetEntityStateAdded(object entity)
        {
            AddEntity(entity);
        }

        public bool IsEntityStateDeleted(object entity)
        {
            return removeQueue.Any(x => x.Entity == entity);
        }

        public void SetEntityStateDeleted(object entity)
        {
            RemoveEntity(entity);
        }

        public bool IsEntityStateModified(object entity)
        {
            return updateQueue.Any(x => x.Entity == entity);
        }

        public void SetEntityStateModified(object entity)
        {
            UpdateEntity(entity, entity);
        }

        public bool IsEntityStateDetached(object entity)
        {
            return !addQueue.Any(x => x.Entity == entity) && !updateQueue.Any(x => x.Entity == entity) && !removeQueue.Any(x => x.Entity == entity) && !repo.EntityExistsInRepository(entity);
        }

        public void SetEntityStateDetached(object entity)
        {
            if (addQueue.Any(x => x.Entity == entity))
            {
                addQueue.Remove(addQueue.First(x => x.Entity == entity));
            }

            if (updateQueue.Any(x => x.Entity == entity))
            {
                throw new Exception("If an item has been updated it can't be set to detached");
            }

            if (repo.EntityExistsInRepository(entity))
            {
                throw new Exception("If an item has been commit it can't be detached");
            }

            if (removeQueue.Any(x => x.Entity == entity))
            {
                removeQueue.Remove(removeQueue.First(x => x.Entity == entity));
            }
        }

        public bool IsEntityStateUnchanged(object entity)
        {
            return repo.EntityExistsInRepository(entity) && !updateQueue.Any(x => x.Entity == entity);
        }

        public void SetEntityStateUnchanged(object entity)
        {
            if (updateQueue.Any(x => x.Entity == entity))
            {
                throw new Exception("If an item has been updated it can't be set it to unchanged");
            }
        }

        public IEnumerable<DbEntityValidationResultBetter> GetValidationErrors()
        {
            var allEntityErrors = new List<DbEntityValidationResultBetter>();

            var addAndUpdate = addQueue.Select(x=> x.Entity).Concat(updateQueue.Select(x => x.Entity));

            foreach (var entity in addAndUpdate)
            {
                var errors = ValidationHelper.ValidateObject(entity);
                if (errors.Count() > 0)
                {
                    var result = new DbEntityValidationResultBetter();
                    foreach (var err in errors)
                    {
                        if (err.MemberNames.Count() > 0)
                        {
                            foreach (var prop in err.MemberNames)
                            {
                                result.AddModelError(prop, err.ErrorMessage);
                            }
                        }
                        else
                        {
                            result.AddModelError("", err.ErrorMessage);
                        }
                    }

                    allEntityErrors.Add(result);
                }
            }

            return allEntityErrors;
        }

        public IEnumerable<TResultType> SQLQueryNoTracking<TResultType>(string query, params object[] paramaters) where TResultType : class
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TResultType> SQLQueryTracking<TResultType>(string query, params object[] paramaters) where TResultType : class
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TResultType>> SQLQueryNoTrackingAsync<TResultType>(string query, params object[] paramaters) where TResultType : class
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TResultType>> SQLQueryTrackingAsync<TResultType>(string query, params object[] paramaters) where TResultType : class
        {
            throw new NotImplementedException();
        }
    }
}
