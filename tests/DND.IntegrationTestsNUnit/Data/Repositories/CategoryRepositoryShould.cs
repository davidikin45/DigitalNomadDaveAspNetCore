using DND.Common.Data.Repository.GenericEF;
using DND.Common.Testing;
using DND.Data;
using DND.Domain.Blog.Categories;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System.Threading;

namespace DND.IntegrationTestsNUnit.Data.Repositories
{
    [TestFixture]
    public class CategoryRepositoryShould
    {
        private ApplicationContext _context;
        private GenericEFRepository<Category> _repository;

        [SetUp]
        public void SetUp()
        {
            var connectionString = TestHelper.GetConfiguration("Integration").GetConnectionString("DefaultConnection");
            _context = TestHelper.GetContext<ApplicationContext>(connectionString, false);

            _repository = new GenericEFRepository<Category>(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

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
            }
        }

        [Test, Isolated]
        public void InsertDeleteUpdate()
        {
            Seed();

            var cat1 = _repository.GetOne(c => c.Name == "Category 1");
            var cat2  = _repository.GetOne(c => c.Name == "Category 2");

            //Insert, Delete, Update
            var cat3 = new Category() { Name = "Category 3", Description = "Category 3", UrlSlug = "category-3" };
            _context.Categories.Add(cat3);
            //_repository.Delete(cat2.Id);
            _context.Categories.Remove(cat2);
            cat1.Name = "Category 4";
            _context.Categories.Update(cat1);

            _context.SaveChanges();
            //Update, Delete, Insert
        }

        [Test, Isolated]
        public void InsertUpdateDelete()
        {
            Seed();

            var cat1 = _repository.GetOne(c => c.Name == "Category 1");
            var cat2 = _repository.GetOne(c => c.Name == "Category 2");

            //Insert, Update, Delete
            var cat3 = new Category() { Name = "Category 3", Description = "Category 3", UrlSlug = "category-3" };
            _repository.Insert(cat3);
            cat1.Name = "Category 4";
            _repository.Update(cat1);
            _repository.Delete(cat2.Id);

            _context.SaveChanges();
            //Update, Delete, Insert
        }

        [Test, Isolated]
        public void DeleteInsertUpdate()
        {
            Seed();

            var cat1 = _repository.GetOne(c => c.Name == "Category 1");
            var cat2 = _repository.GetOne(c => c.Name == "Category 2");

            //Delete, Insert, Update
            var cat3 = new Category() { Name = "Category 3", Description = "Category 3", UrlSlug = "category-3" };
            _repository.Delete(cat2.Id);
            _repository.Insert(cat3);
            cat1.Name = "Category 4";
            _repository.Update(cat1);

            _context.SaveChanges();
            //Update, Delete, Insert
        }

        [Test, Isolated]
        public void DeleteUpdateInsert()
        {
            Seed();

            var cat1 = _repository.GetOne(c => c.Name == "Category 1");
            var cat2 = _repository.GetOne(c => c.Name == "Category 2");

            //Delete, Update, Insert
            var cat3 = new Category() { Name = "Category 3", Description = "Category 3", UrlSlug = "category-3" };
            _repository.Delete(cat2.Id);
            cat1.Name = "Category 4";
            _repository.Update(cat1);
            _repository.Insert(cat3);

            _context.SaveChanges();
            //Update, Delete, Insert
        }

        [Test, Isolated]
        public void UpdateDeleteInsert()
        {
            Seed();

            var cat1 = _repository.GetOne(c => c.Name == "Category 1");
            var cat2 = _repository.GetOne(c => c.Name == "Category 2");

            //Update, Delete, Insert
            var cat3 = new Category() { Name = "Category 3", Description = "Category 3", UrlSlug = "category-3" };
            cat1.Name = "Category 4";
            _repository.Update(cat1);
            _repository.Delete(cat2.Id);
            _repository.Insert(cat3);

            _context.SaveChanges();
            //Update, Delete, Insert
        }

        [Test, Isolated]
        public void UpdateInsertDelete()
        {
            Seed();

            var cat1 = _repository.GetOne(c => c.Name == "Category 1");
            var cat2 = _repository.GetOne(c => c.Name == "Category 2");

            //Update, Insert, Delete
            var cat3 = new Category() { Name = "Category 3", Description = "Category 3", UrlSlug = "category-3" };
            cat1.Name = "Category 4";
            _repository.Update(cat1);
            _repository.Insert(cat3);
            _repository.Delete(cat2.Id);

            _context.SaveChanges();
            //Update, Delete, Insert
        }

        [Test, Isolated]
        public void InsertDelete()
        {
            //Insert, Delete, Update
            var cat3 = new Category() { Name = "Category 3", Description = "Category 3", UrlSlug = "category-3" };
            _repository.Insert(cat3);
            _repository.Delete(cat3);
         
            _context.SaveChanges();
            //No save occurs
        }
    }
}
