using DND.Common.Data.Helpers;
using DND.Common.Data.Repository.GenericEF;
using DND.Common.Extensions;
using DND.Common.Testing;
using DND.Data;
using DND.Domain.Blog.Categories;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DND.IntegrationTestsNUnit.Data.Repositories
{
    [TestFixture]
    public class GenericRepositoryShould
    {
        private StringBuilder _logBuilder = new StringBuilder();
        private ApplicationContext _context;
        private GenericEFRepository<Category> _repository;

        [SetUp]
        public void SetUp()
        {
            var connectionString = TestHelper.GetConfiguration("Integration").GetConnectionString("DefaultConnection");
            _context = TestHelper.GetContext<ApplicationContext>(connectionString, false);
            ApplicationContext.LoggerFactory.ConfigureLogging(s => BuildLogString(s), LoggingCategories.SQL);

            _repository = new GenericEFRepository<Category>(_context);
            _logBuilder = new StringBuilder();
        }

        private void BuildLogString(string message)
        {
            _logBuilder.AppendLine(message);
        }

        private string _log
        {
            get
            {
                return _logBuilder.ToString();
            }
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        private int insertedCategoryId;

        public void Seed()
        {
            var connectionString = TestHelper.GetConfiguration("Integration").GetConnectionString("DefaultConnection");
            using (var seedContext = TestHelper.GetContext<ApplicationContext>(connectionString, false))
            {
                var cata = new Category() { Name = "Category 1", Description = "Category 1", UrlSlug = "category-1" };
                var catb = new Category() { Name = "Category 2", Description = "Category 2", UrlSlug = "category-2" };
                seedContext.Categories.Add(cata);
                seedContext.Categories.Add(catb);

                seedContext.SaveChanges();

                insertedCategoryId = cata.Id;
            }
            _logBuilder = new StringBuilder();
        }

        [Test, Isolated]
        public void InsertDeleteUpdate()
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

            _context.SaveChanges();
            //Update, Delete, Insert
        }

        [Test, Isolated]
        public void CanFindByCategoryByIdWithDynamicLambda()
        {
            Seed();
            _repository.GetById(1);
            Assert.IsTrue(_log.Contains("FROM [Category"));
        }

        [Test, Isolated]
        public void CanFindByCategoryByIdsWithDynamicLambda()
        {
            Seed();
            var ids = new List<object>() { 1, 2 };
            var entity = _repository.GetByIdsNoTracking(ids);
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
  
            Assert.AreEqual(0, _context.CachedEntityCount());
        }

        [Test, Isolated]
        public void NoCacheEntitiesWhenExistsNoTrackingsCalled()
        {
            Seed();
            var exists = _repository.ExistsNoTracking();
            Assert.AreEqual(0, _context.CachedEntityCount());
        }

        [Test, Isolated]
        public void CacheEntitiesWhenExistsIsCalled()
        {
            Seed();
            var exists = _repository.Exists();
            Assert.AreNotEqual(0, _context.CachedEntityCount());
        }

        [Test, Isolated]
        public void AllowInsertThenDelete()
        {
            var category = new Category { Name = "test", Description = "test", UrlSlug = "test" };
            _repository.Insert(category);
            _repository.Delete(category);
            _context.SaveChanges();
            Assert.IsTrue(!_log.Contains("Insert"));
        }

        [Test, Isolated]
        public void OnlyFetchEntityFromDbOnce()
        {
            Seed();
            var exists = _repository.ExistsById(insertedCategoryId);
            Assert.AreEqual(1, _context.CachedEntityCount());
            var category = _repository.GetById(insertedCategoryId);
            Assert.AreEqual(1, _context.CachedEntityCount());
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


            //Select only once
            Assert.AreEqual(1, _log.CountStringOccurrences("[Description]"));
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

            //Inital non tracking select + select from update
            Assert.AreEqual(2, _log.CountStringOccurrences("[Description]"));
            //Update only updated fields
            Assert.IsTrue(!_log.Contains("[Description] = "));
        }

        [Test, Isolated]
        public void CanQueryWithSinglePredicate()
        {
            _repository.Get(c => c.Name.StartsWith("L"));
            Assert.IsTrue(_log.Contains("N'L' + N'%'"));
        }

        [Test, Isolated]
        public void CanQueryWithDualPredicate()
        {
            var date = new DateTime(2001, 1, 1);
            _repository
               .Get(c => c.Name.StartsWith("L") && c.DateCreated >= date);
            Assert.IsTrue(_log.Contains("N'L' + N'%'") && _log.Contains("2001-01-01"));
        }

        [Test, Isolated]
        public void ComposedOnAllListExecutedInMemory()
        {
            _repository.GetAll().Where(c => c.Name == "Julie").ToList();
            Assert.IsFalse(_log.Contains("Julie"));
        }

        [Test, Isolated]
        public void CanInsertCategory()
        {
            var category = new Category { Name = "test", Description = "test", UrlSlug = "test" };
            _repository.Insert(category);
            _context.SaveChanges();
 
            Assert.AreNotEqual(0, category.Id);
        }
    }
}
