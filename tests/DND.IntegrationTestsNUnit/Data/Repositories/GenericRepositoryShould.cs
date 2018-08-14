using DND.Common.Extensions;
using DND.Common.Implementation.Data;
using DND.Common.Implementation.Repository;
using DND.Common.Implementation.Repository.EntityFramework;
using DND.Common.Implementation.UnitOfWork;
using DND.Common.Interfaces.Data;
using DND.Common.Interfaces.UnitOfWork;
using DND.Common.Testing;
using DND.Data;
using DND.Domain.Blog.Categories;
using DND.Infrastructure;
using DND.Interfaces.Blog.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.IntegrationTestsNUnit.Data.Repositories
{
    [TestFixture]
    public class GenericRepositoryShould
    {
        private StringBuilder _logBuilder = new StringBuilder();
        private string _log;
        private ApplicationDbContext _context;
        private GenericEFRepository<Category> _repository;
        private IUnitOfWorkReadOnlyScope _ouw;

        [SetUp]
        public void SetUp()
        {
            var connectionString = DNDConnectionStrings.GetConnectionString("DefaultConnectionString");
            _context = new ApplicationDbContext(connectionString, true);

            var uowFactory = new UnitOfWorkScopeFactory(new DbContextFactoryProducerSingleton(new IDbContextAbstractFactory[] { new FakeSingleDbContextFactory<IBlogDbContext>(_context) }), new AmbientDbContextLocator(), new GenericRepositoryFactory());
            _ouw = uowFactory.CreateReadOnly();
            _repository = new GenericEFRepository<Category>(_context, _ouw);
            _log = "";
            _logBuilder = new StringBuilder();
            SetupLogging();
        }

        private void WriteLog()
        {
            Debug.WriteLine(_log);
        }

        private void SetupLogging()
        {
            _context.Database.Log = BuildLogString;
        }

        private void BuildLogString(string message)
        {
            _logBuilder.Append(message);
            _log = _logBuilder.ToString();
        }

        [TearDown]
        public void TearDown()
        {
            _ouw.Dispose();
            _context.Dispose();
        }

        private int insertedCategoryId;

        public void Seed()
        {
            var connectionString = DNDConnectionStrings.GetConnectionString("DefaultConnectionString");
            using (var con = new ApplicationDbContext(connectionString, true))
            {
                var uowFactory = new UnitOfWorkScopeFactory(new DbContextFactoryProducerSingleton(new IDbContextAbstractFactory[] { new FakeSingleDbContextFactory<IBlogDbContext>(con) }), new AmbientDbContextLocator(), new GenericRepositoryFactory());

                using (var unitOfWork = uowFactory.Create(BaseUnitOfWorkScopeOption.ForceCreateNew))
                {
                    var repo = new GenericEFRepository<Category>(con, unitOfWork);

                    var cata = new Category() { Name = "Category 1", Description = "Category 1", UrlSlug = "category-1" };
                    var catb = new Category() { Name = "Category 2", Description = "Category 2", UrlSlug = "category-2" };
                    repo.Insert(cata);
                    repo.Insert(catb);

                    con.SaveChanges();

                    insertedCategoryId = cata.Id;

                }
            }
        }


        [Test, Isolated]
        public async Task InsertDeleteUpdate()
        {
            Seed();

            var cat1 = _repository.GetOne(c => c.Name == "Category 1");
            var cat2 = _repository.GetOne(c => c.Name == "Category 2");

            //Insert, Delete, Update
            var cat3 = new Category() { Name = "Category 3", Description = "Category 3", UrlSlug = "category-3" };
            _repository.Insert(cat3);
            _repository.Delete(cat2.Id);
            cat1.Name = "Category 4";
            _repository.Update(cat1);

            await _context.SaveChangesAsync();
            //Update, Delete, Insert
        }

        [Test, Isolated]
        public void CanFindByCategoryByIdWithDynamicLambda()
        {
            Seed();
            _repository.GetById(1);
            WriteLog();
            Assert.IsTrue(_log.Contains("FROM [dbo].[Category"));
        }

        [Test, Isolated]
        public void CanFindByCategoryByIdsWithDynamicLambda()
        {
            Seed();
            var ids = new List<object>() { 1, 2 };
            var entity = _repository.GetByIdsNoTracking(ids);
            WriteLog();
            Assert.AreEqual(0, _context.CachedEntityCount());
        }

        [Test, Isolated]
        public void NoTrackingQueriesDoNotCacheObjects()
        {
            Seed();
            var entity = _repository.GetByIdNoTracking(1);
            Assert.AreEqual(0, _context.CachedEntityCount());
        }

        [Test, Isolated]
        public void NoCacheEntitiesWhenCountisCalled()
        {
            Seed();
            var entity = _repository.GetCount();
            WriteLog();
            Assert.AreEqual(0, _context.CachedEntityCount());
        }

        [Test, Isolated]
        public void NoCacheEntitiesWhenExistsNoTrackingsCalled()
        {
            Seed();
            var exists = _repository.ExistsNoTracking();
            WriteLog();
            Assert.AreEqual(0, _context.CachedEntityCount());
        }

        [Test, Isolated]
        public void CacheEntitiesWhenExistsIsCalled()
        {
            Seed();
            var exists = _repository.Exists();
            WriteLog();
            Assert.AreNotEqual(0, _context.CachedEntityCount());
        }

        [Test, Isolated]
        public void AllowInsertThenDelete()
        {
            var category = new Category { Name = "test", Description = "test", UrlSlug = "test" };
            _repository.Insert(category);
            _repository.Delete(category);
            _context.SaveChanges();
            WriteLog();
            Assert.IsTrue(!_log.Contains("Insert"));
        }

        [Test, Isolated]
        public void OnlyFetchEntityFromDbOnce()
        {
            Seed();
            var exists = _repository.ExistsById(insertedCategoryId);
            var category = _repository.GetById(insertedCategoryId);
            Assert.AreEqual(1, _context.CachedEntityCount());
            WriteLog();
            Assert.AreEqual(1, _log.CountStringOccurrences("SELECT"));
        }

        [Test, Isolated]
        public void OnlyFetchEntityFromDbOnceAndUpdateUpdatedFieldsOnlyWhenConnectedUpdateOccurs()
        {
            Seed();
            var category =_repository.GetById(insertedCategoryId);
            Assert.AreEqual(1, _context.CachedEntityCount());

            category.Name = "Updated Name";

            _repository.Update(category);
            _context.SaveChanges();

            WriteLog();

            //Select only once
            Assert.AreEqual(1, _log.CountStringOccurrences("[Description] AS [Description]"));
            //Update only updated fields
            Assert.IsTrue(!_log.Contains("[Description] = "));
        }

        [Test, Isolated]
        public void StoreEntityInCacheWhenExistsIsCalled()
        {
            Seed();
            var category = _repository.GetByIdNoTracking(insertedCategoryId);
            _repository.Exists(category);

            Assert.AreEqual(1, _context.CachedEntityCount());
        }

        [Test, Isolated]
        public void OnlyFetchEntityFromDbOnceWhenDisconnectedUpdatedOccurs()
        {
            Seed();
            var category = _repository.GetByIdNoTracking(insertedCategoryId);
            Assert.AreEqual(0, _context.CachedEntityCount());

            category.Name = "Updated Name";

            _repository.Update(category);

            _context.SaveChanges();

            WriteLog();

            //Inital non tracking select + select from update
            Assert.AreEqual(2, _log.CountStringOccurrences("[Description] AS [Description]"));
            //Update only updated fields
            Assert.IsTrue(!_log.Contains("[Description] = "));
        }

        [Test, Isolated]
        public void CanQueryWithSinglePredicate()
        {
            _repository.Get(c => c.Name.StartsWith("L"));
            WriteLog();
            Assert.IsTrue(_log.Contains("'L%'"));
        }

        [Test, Isolated]
        public void CanQueryWithDualPredicate()
        {
            var date = new DateTime(2001, 1, 1);
            _repository
               .Get(c => c.Name.StartsWith("L") && c.DateCreated >= date);
            WriteLog();
            Assert.IsTrue(_log.Contains("'L%'") && _log.Contains("1/01/2001"));
        }

        [Test, Isolated]
        public void ComposedOnAllListExecutedInMemory()
        {
            _repository.GetAll().Where(c => c.Name == "Julie").ToList();
            WriteLog();
            Assert.IsFalse(_log.Contains("Julie"));
        }

        [Test, Isolated]
        public void CanInsertCategory()
        {
            var category = new Category { Name = "test", Description = "test", UrlSlug = "test" };
            _repository.Insert(category);
            _context.SaveChanges();
            WriteLog();
            Assert.AreNotEqual(0, category.Id);
        }
    }
}
