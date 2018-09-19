using DND.Common.Testing;
using DND.Data;
using DND.Domain.Blog.Categories;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System.Linq;

namespace DND.IntegrationTestsNUnit.Data.DbContext
{
    [TestFixture]
    public class DbContextShould
    {
        private ApplicationContext _context;
        [SetUp]
        public void SetUp()
        {
            var connectionString = TestHelper.GetConfiguration("Integration").GetConnectionString("DefaultConnectionString");
            _context = TestHelper.GetContext<ApplicationContext>(connectionString, false);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test, Isolated]
        //When using isolated only the last saveChanges is kept
        public void DbContextLocal()
        {
            var cata = new Category() { Name = "Category 1", Description = "Category 1", UrlSlug = "category-1" };
            _context.Categories.Add(cata);
             _context.SaveChanges();

            var catb = new Category() { Name = "Category 2", Description = "Category 2", UrlSlug = "category-2" };
            _context.Categories.Add(catb);
             _context.SaveChanges();

            var categories = _context.Categories.ToList();

            //Post Commit Events should not be fired until transaction commited.

            Assert.AreEqual(2, categories.Count());
        }
    }
}
