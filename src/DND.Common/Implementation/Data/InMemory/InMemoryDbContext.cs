using DND.Common.DomainEvents;
using DND.Common.Helpers;
using DND.Common.Implementation.Validation;
using DND.Common.Interfaces.Models;
using DND.Common.Interfaces.Data;
using RefactorThis.GraphDiff;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using DND.Common.Extensions;

namespace DND.Common.Implementation.Data.InMemory
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
        public readonly List<QueueItem> updateQueue = new List<QueueItem>();
        public readonly List<QueueItem> addQueue = new List<QueueItem>();
        public readonly List<QueueItem> removeQueue = new List<QueueItem>();

        private readonly IDbContextDomainEvents _dbContextDomainEvents;
        private readonly DbContextTimestamps _dbContextTimestamps;

        public bool AutoDetectChanges { get; set; }

        public event EventHandler<BeforeSave> BeforeSave;
        public event EventHandler<AfterSave> AfterSave;

        //For domain events to fire, this needs to be set
        public IServiceProvider ServiceProvider { get; set; }

        public InMemoryDataContext()
        {
            _dbContextDomainEvents = new DbContextDomainEventsInMemoryAdapter(this, new DND.Common.DomainEvents.DomainEvents(ServiceProvider));
            _dbContextTimestamps = new DbContextTimestamps();

            repo = new ObjectRepresentationRepository();
            RegisterIIdentifiables();
        }

        internal InMemoryDataContext(ObjectRepresentationRepository repo)
        {
            _dbContextDomainEvents = new DbContextDomainEventsInMemoryAdapter(this, new DND.Common.DomainEvents.DomainEvents(null));
            _dbContextTimestamps = new DbContextTimestamps();

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
            //Update, Delete, Insert which is consistent with EF
            ClearUpdateQueue();
            RemoveAllFromQueueFromRepository();
            AddAllFromQueueIntoRepository();
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

        #region Validation
        public IEnumerable<DbEntityValidationResultBetter> GetValidationErrors()
        {
            return GetValidationErrorsForNewChanges(false);
        }

        public IEnumerable<DbEntityValidationResultBetter> GetValidationErrorsForNewChanges()
        {
            return GetValidationErrorsForNewChanges(true);
        }

        private IEnumerable<DbEntityValidationResultBetter> GetValidationErrorsForNewChanges(bool onlyNewChanges)
        {
            var allEntityErrors = new List<DbEntityValidationResultBetter>();

            var entities = addQueue.Select(x => x.Entity).Concat(updateQueue.Select(x => x.Entity));
            if (onlyNewChanges)
            {
                entities = entities.Where(x => !_dbContextDomainEvents.GetPreCommittedDeletedEntities().Contains(x) && !_dbContextDomainEvents.GetPreCommittedInsertedEntities().Contains(x));
            }

            foreach (var entity in entities)
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
        #endregion

        #region Interface Methods and Properties
        public void TriggerTrackChanges(object newEntity)
        {

        }

        public IBaseDbContextTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            throw new NotImplementedException();
        }

        public int CachedEntityCount()
        {
            return addQueue.Count() + repo._data.Count();
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
            if (IsEntityStateAdded(entity))
            {
                SetEntityStateDetached(entity);
            }
            else
            {
                removeQueue.Add(new QueueItem(entity));
            }
        }

        public IQueryable<TEntity> Queryable<TEntity>() where TEntity : class
        {
            return repo.Data<TEntity>();
        }

        public IQueryable<object> Queryable(Type type)
        {
            return repo.Data(type);
        }

        //Using this should give performance improvement.
        //https://msdn.microsoft.com/en-us/library/jj592677(v=vs.113).aspx
        //Note that only properties that are set to different values when copied from the other object will be marked as modified.
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
            return addQueue.Any(x => Equals(x.Entity, entity));
        }

        public void SetEntityStateAdded(object entity)
        {
            AddEntity(entity);
        }

        public bool IsEntityStateDeleted(object entity)
        {
            return removeQueue.Any(x => Equals(x.Entity, entity));
        }

        public void SetEntityStateDeleted(object entity)
        {
            RemoveEntity(entity);
        }

        public bool IsEntityStateModified(object entity)
        {
            return updateQueue.Any(x => Equals(x.Entity, entity));
        }

        public void SetEntityStateModified(object entity)
        {
            UpdateEntity(entity, entity);
        }

        public bool IsEntityStateDetached(object entity)
        {
            return !addQueue.Any(x => Equals(x.Entity, entity)) && !updateQueue.Any(x => Equals(x.Entity, entity)) && !removeQueue.Any(x => Equals(x.Entity, entity)) && !repo.EntityExistsInRepository(entity);
        }

        public void SetEntityStateDetached(object entity)
        {
            if (addQueue.Any(x => Equals(x.Entity, entity)))
            {
                addQueue.Remove(addQueue.First(x => Equals(x.Entity, entity)));
            }

            if (updateQueue.Any(x => Equals(x.Entity, entity)))
            {
                throw new Exception("If an item has been updated it can't be set to detached");
            }

            if (repo.EntityExistsInRepository(entity))
            {
                throw new Exception("If an item has been commit it can't be detached");
            }

            if (removeQueue.Any(x => Equals(x.Entity, entity)))
            {
                removeQueue.Remove(removeQueue.First(x => Equals(x.Entity, entity)));
            }
        }

        public bool IsEntityStateUnchanged(object entity)
        {
            return repo.EntityExistsInRepository(entity) && !updateQueue.Any(x => Equals(x.Entity, entity));
        }

        public void SetEntityStateUnchanged(object entity)
        {
            if (updateQueue.Any(x => Equals(x.Entity, entity)))
            {
                throw new Exception("If an item has been updated it can't be set it to unchanged");
            }
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
        #endregion

        #region Timestamps
        private void AddTimestamps()
        {
            var added = _dbContextDomainEvents.GetNewInsertedEntities();
            var modified = _dbContextDomainEvents.GetNewUpdatedEntities();
            var deleted = _dbContextDomainEvents.GetNewDeletedEntities();

            _dbContextTimestamps.AddTimestamps(added, modified, deleted);
        }
        #endregion

        #region Save Changes
        public int SaveChanges()
        {
            BeforeSave?.Invoke(this, new BeforeSave());

            AddTimestamps();

            ProcessCommitQueues();
            repo.Commit();

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
        #endregion

        #region Domain Events

        public void FirePreCommitEvents()
        {
            FirePreCommitEventsAsync().Wait();
        }

        public async Task FirePreCommitEventsAsync()
        {
            AddTimestamps();

            await _dbContextDomainEvents.FirePreCommitEventsAsync().ConfigureAwait(false);
        }

        public void FirePostCommitEvents()
        {
            FirePostCommitEventsAsync().Wait();
        }

        public async Task FirePostCommitEventsAsync()
        {
            await _dbContextDomainEvents.FirePostCommitEventsAsync().ConfigureAwait(false);
        }
        #endregion

        #region Collection Properties
        public void LoadCollectionProperty(object entity, string collectionProperty, string search = "", string orderBy = null, bool ascending = false, int? skip = null, int? take = null, object collectionItemId = null)
        {

        }

        public async Task LoadCollectionPropertyAsync(object entity, string collectionProperty, string search = "", string orderBy = null, bool ascending = false, int? skip = null, int? take = null, object collectionItemId = null, CancellationToken cancellationToken = default(CancellationToken))
        {

        }

        public int CollectionPropertyCount(object entity, string collectionProperty, string search = "")
        {
            return ((ICollection<Object>)entity.GetPropValue(collectionProperty)).Count();
        }

        public Task<int> CollectionPropertyCountAsync(object entity, string collectionProperty, string search, CancellationToken cancellationToken)
        {
            var task = new Task<int>(() => CollectionPropertyCount(entity, collectionProperty, search));
            task.Start();
            return task;
        }
        #endregion

        #region Local Entity Cache
        public bool EntityExistsLocal<TEntity>(TEntity entity) where TEntity : class
        {
            return addQueue.Any(x => Equals(x.Entity, entity)) || updateQueue.Any(x => Equals(x.Entity, entity)) || removeQueue.Any(x => Equals(x.Entity, entity));
        }

        public bool EntityExistsByIdLocal<TEntity>(object id) where TEntity : class
        {
            if (typeof(TEntity).HasProperty(nameof(IBaseEntity.Id)) && !Equals(typeof(TEntity).GetProperty(nameof(IBaseEntity.Id)).PropertyType.DefaultValue(), id))
            {
                var filter = LamdaHelper.SearchForEntityById<TEntity>(id);
                return addQueue.Where(x => x.Entity is TEntity && typeof(TEntity).HasProperty(nameof(IBaseEntity.Id))).Select(x => x.Entity).Cast<TEntity>().AsQueryable().Any(filter) ||
                updateQueue.Where(x => x.Entity is TEntity && typeof(TEntity).HasProperty(nameof(IBaseEntity.Id))).Select(x => x.Entity).Cast<TEntity>().AsQueryable().Any(filter) ||
                removeQueue.Where(x => x.Entity is TEntity && typeof(TEntity).HasProperty(nameof(IBaseEntity.Id))).Select(x => x.Entity).Cast<TEntity>().AsQueryable().Any(filter);
            }
            else
            {
                return false;
            }
        }

        public TEntity FindEntityByIdLocal<TEntity>(object id) where TEntity : class
        {
            if (typeof(TEntity).HasProperty(nameof(IBaseEntity.Id)) && !Equals(typeof(TEntity).GetProperty(nameof(IBaseEntity.Id)).PropertyType.DefaultValue(), id))
            {
                var filter = LamdaHelper.SearchForEntityById<TEntity>(id);
                return addQueue.Where(x => x.Entity is TEntity && typeof(TEntity).HasProperty(nameof(IBaseEntity.Id))).Select(x => x.Entity).Cast<TEntity>().AsQueryable().FirstOrDefault(filter) ??
                updateQueue.Where(x => x.Entity is TEntity && typeof(TEntity).HasProperty(nameof(IBaseEntity.Id))).Select(x => x.Entity).Cast<TEntity>().AsQueryable().FirstOrDefault(filter) ??
                removeQueue.Where(x => x.Entity is TEntity && typeof(TEntity).HasProperty(nameof(IBaseEntity.Id))).Select(x => x.Entity).Cast<TEntity>().AsQueryable().FirstOrDefault(filter);
            }
            else
            {
                return null;
            }
        }

        public TEntity FindEntityLocal<TEntity>(TEntity entity) where TEntity : class
        {
            return addQueue.Where(x => Equals(x.Entity, entity)).Select(x => (TEntity)x.Entity).FirstOrDefault() ?? updateQueue.Where(x => Equals(x.Entity, entity)).Select(x => (TEntity)x.Entity).FirstOrDefault() ?? removeQueue.Where(x => Equals(x.Entity, entity)).Select(x => (TEntity)x.Entity).FirstOrDefault();
        }
        #endregion

        #region Entity By Object

        public bool EntityExists<TEntity>(TEntity entity) where TEntity : class
        {
            var local = EntityExistsLocal(entity);
            if (local)
                return true;

            return repo.EntityExistsInRepository(entity);
        }

        public Task<bool> EntityExistsAsync<TEntity>(TEntity entity, CancellationToken cancellationToken) where TEntity : class
        {
            var task = new Task<bool>(() => EntityExists(entity));
            task.Start();
            return task;
        }

        public bool EntityExistsNoTracking<TEntity>(TEntity entity) where TEntity : class
        {
            return EntityExists(entity);
        }

        public Task<bool> EntityExistsNoTrackingAsync<TEntity>(TEntity entity, CancellationToken cancellationToken) where TEntity : class
        {
            return EntityExistsAsync(entity, cancellationToken);
        }

        public TEntity FindEntity<TEntity>(TEntity entity) where TEntity : class
        {
            var local = FindEntityLocal<TEntity>(entity);
            if (local != null)
                return local;

            if (entity.HasProperty(nameof(IBaseEntity.Id)) && !Equals(typeof(TEntity).GetProperty(nameof(IBaseEntity.Id)).PropertyType.DefaultValue(), entity.GetPropValue(nameof(IBaseEntity.Id))))
            {
                return repo.FindEntity<TEntity>(entity.GetPropValue(nameof(IBaseEntity.Id)));
            }

            return null;
        }

        public Task<TEntity> FindEntityAsync<TEntity>(TEntity entity, CancellationToken cancellationToken) where TEntity : class
        {
            var task = new Task<TEntity>(() => FindEntity<TEntity>(entity));
            task.Start();
            return task;
        }

        public TEntity FindEntityNoTracking<TEntity>(TEntity entity) where TEntity : class
        {
            return FindEntity(entity);
        }

        public Task<TEntity> FindEntityNoTrackingAsync<TEntity>(TEntity entity, CancellationToken cancellationToken) where TEntity : class
        {
            var task = new Task<TEntity>(() => FindEntity<TEntity>(entity));
            task.Start();
            return task;
        }
        #endregion

        #region Entity By Id
        public bool EntityExistsById<TEntity>(object id) where TEntity : class
        {
            var local = EntityExistsByIdLocal<TEntity>(id);
            if (local)
                return true;

            if (typeof(TEntity).HasProperty(nameof(IBaseEntity.Id)) && !Equals(typeof(TEntity).GetProperty(nameof(IBaseEntity.Id)).PropertyType.DefaultValue(), id))
            {
                return repo.EntityExistsInRepositoryById<TEntity>(id);
            }

            return false;
        }

        public Task<bool> EntityExistsByIdAsync<TEntity>(object id, CancellationToken cancellationToken) where TEntity : class
        {
            var task = new Task<bool>(() => EntityExistsById<TEntity>(id));
            task.Start();
            return task;
        }

        public bool EntityExistsByIdNoTracking<TEntity>(object id) where TEntity : class
        {
            return EntityExistsById<TEntity>(id);
        }

        public Task<bool> EntityExistsByIdNoTrackingAsync<TEntity>(object id, CancellationToken cancellationToken) where TEntity : class
        {
            var task = new Task<bool>(() => EntityExistsById<TEntity>(id));
            task.Start();
            return task;
        }

        public TEntity FindEntityById<TEntity>(object id) where TEntity : class
        {
            var local = FindEntityByIdLocal<TEntity>(id);
            if (local != null)
                return local;

            if (typeof(TEntity).HasProperty(nameof(IBaseEntity.Id)) && !Equals(typeof(TEntity).GetProperty(nameof(IBaseEntity.Id)).PropertyType.DefaultValue(), id))
            {
                var filter = LamdaHelper.SearchForEntityById<TEntity>(id);
                return repo.FindEntity<TEntity>(id);
            }

            return null;
        }

        public Task<TEntity> FindEntityByIdAsync<TEntity>(object id, CancellationToken cancellationToken) where TEntity : class
        {
            var task = new Task<TEntity>(() => FindEntityById<TEntity>(id));
            task.Start();
            return task;
        }

        public TEntity FindEntityByIdNoTracking<TEntity>(object id) where TEntity : class
        {
            return FindEntityById<TEntity>(id);
        }

        public Task<TEntity> FindEntityByIdNoTrackingAsync<TEntity>(object id, CancellationToken cancellationToken) where TEntity : class
        {
            var task = new Task<TEntity>(() => FindEntityById<TEntity>(id));
            task.Start();
            return task;
        }

        #endregion
    }
}
