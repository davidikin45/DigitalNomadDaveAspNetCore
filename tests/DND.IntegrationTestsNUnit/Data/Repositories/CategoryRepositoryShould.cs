using DND.Common.Implementation.Repository;
using DND.Common.Implementation.Repository.EntityFramework;
using DND.Common.Implementation.UnitOfWork;
using DND.Common.Interfaces.UnitOfWork;
using DND.Common.Testing;
using DND.Data;
using DND.Domain.Blog.Categories;
using DND.Infrastructure;
using NUnit.Framework;
using System.Threading.Tasks;

namespace DND.IntegrationTestsNUnit.Data.Repositories
{
    [TestFixture]
    public class CategoryRepositoryShould
    {
        private ApplicationDbContext _context;
        private GenericEFRepository<Category> _repository;
        private IUnitOfWorkReadOnlyScope _ouw;

        [SetUp]
        public void SetUp()
        {
            var connectionString = DNDConnectionStrings.GetConnectionString("DefaultConnectionString");
            _context = new ApplicationDbContext(connectionString, true);

            var uowFactory = new UnitOfWorkScopeFactory(new FakeSingleDbContextFactory(_context), new AmbientDbContextLocator(), new GenericRepositoryFactory());
            _ouw = uowFactory.CreateReadOnly();
            _repository = new GenericEFRepository<Category>(_context, _ouw, false);
        }

        [TearDown]
        public void TearDown()
        {
            _ouw.Dispose();
            _context.Dispose();
        }

        public void Seed()
        {
            var connectionString = DNDConnectionStrings.GetConnectionString("DefaultConnectionString");
            using (var con = new ApplicationDbContext(connectionString, true))
            {
                var uowFactory = new UnitOfWorkScopeFactory(new FakeSingleDbContextFactory(con), new AmbientDbContextLocator(), new GenericRepositoryFactory());

                using (var unitOfWork = uowFactory.Create(BaseUnitOfWorkScopeOption.ForceCreateNew))
                {
                    var repo = new GenericEFRepository<Category>(con, unitOfWork, false);

                    var cata = new Category() { Name = "Category 1", Description = "Category 1", UrlSlug = "category-1" };
                    var catb = new Category() { Name = "Category 2", Description = "Category 2", UrlSlug = "category-2" };
                    repo.Insert(cata);
                    repo.Insert(catb);

                    con.SaveChanges();
                }
            }
        }

        [Test, Isolated]
        public async Task InsertDeleteUpdate()
        {
            Seed();

            var cat1 = _repository.GetOne(c => c.Name == "Category 1");
            var cat2  = _repository.GetOne(c => c.Name == "Category 2");

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
        public async Task InsertUpdateDelete()
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

            await _context.SaveChangesAsync();
            //Update, Delete, Insert
        }

        [Test, Isolated]
        public async Task DeleteInsertUpdate()
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

            await _context.SaveChangesAsync();
            //Update, Delete, Insert
        }

        [Test, Isolated]
        public async Task DeleteUpdateInsert()
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

            await _context.SaveChangesAsync();
            //Update, Delete, Insert
        }

        [Test, Isolated]
        public async Task UpdateDeleteInsert()
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

            await _context.SaveChangesAsync();
            //Update, Delete, Insert
        }

        [Test, Isolated]
        public async Task UpdateInsertDelete()
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

            await _context.SaveChangesAsync();
            //Update, Delete, Insert
        }

        [Test, Isolated]
        public async Task InsertDelete()
        {
            //Insert, Delete, Update
            var cat3 = new Category() { Name = "Category 3", Description = "Category 3", UrlSlug = "category-3" };
            _repository.Insert(cat3);
            _repository.Delete(cat3);
         
            await _context.SaveChangesAsync();
            //No save occurs
        }
    }
}
