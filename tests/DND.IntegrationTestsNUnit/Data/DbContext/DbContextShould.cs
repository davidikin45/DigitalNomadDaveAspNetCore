using DND.Common.Interfaces.Data;
using DND.Data;
using DND.Domain.Blog.Categories;
using DND.Infrastructure;
using NUnit.Framework;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DND.IntegrationTestsNUnit.Data.DbContext
{
    [TestFixture]
    public class DbContextShould
    {
        private ApplicationDbContext _context;
        private IBaseDbContextTransaction _transaction;
        [SetUp]
        public void SetUp()
        {
            var connectionString = DNDConnectionStrings.GetConnectionString("DefaultConnectionString");
            _context = new ApplicationDbContext(connectionString, true);
            _transaction = _context.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        //When using isolated only the last saveChanges is kept
        public async Task DbContextLocal()
        {
            var cata = new Category() { Name = "Category 1", Description = "Category 1", UrlSlug = "category-1" };
            _context.Categories.Add(cata);
            await _context.SaveChangesAsync();

            var catb = new Category() { Name = "Category 2", Description = "Category 2", UrlSlug = "category-2" };
            _context.Categories.Add(catb);
            await _context.SaveChangesAsync();

            var categories = _context.Categories.ToList();

            _transaction.Rollback();

            //Post Commit Events should not be fired until transaction commited.

            Assert.AreEqual(2, categories.Count());
        }
    }
}
