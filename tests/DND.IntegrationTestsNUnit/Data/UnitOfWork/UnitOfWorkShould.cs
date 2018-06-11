using DND.Common.Implementation.Repository;
using DND.Common.Implementation.Repository.EntityFramework;
using DND.Common.Implementation.UnitOfWork;
using DND.Common.Interfaces.UnitOfWork;
using DND.Common.Testing;
using DND.Data;
using DND.Domain.Blog.Categories;
using DND.Infrastructure;
using DND.Interfaces.Data;
using FluentAssertions;
using NUnit.Framework;
using System.Threading.Tasks;

namespace DND.IntegrationTestsNUnit.Data.UnitOfWork
{
    [TestFixture]
    public class UnitOfWorkShould
    {

        private UnitOfWorkScopeFactory _unitOfWorkFactory;

        [SetUp]
        public void SetUp()
        {
            _unitOfWorkFactory = new UnitOfWorkScopeFactory(new DbContextFactory(), new AmbientDbContextLocator(), new GenericRepositoryFactory());
        }

        [TearDown]
        public void TearDown()
        {

        }

        private void Seed()
        {
            var connectionString = DNDConnectionStrings.GetConnectionString("DefaultConnectionString");

            using (var con = new ApplicationDbContext(connectionString, true))
            {
                var uowFactory = new UnitOfWorkScopeFactory(new FakeSingleDbContextFactory(con), new AmbientDbContextLocator(), new GenericRepositoryFactory());
                using (var unitOfWork = uowFactory.Create(BaseUnitOfWorkScopeOption.ForceCreateNew))
                {
                    var repo = new GenericEFRepository<Category>(con, unitOfWork);

                    var cata = new Category() { Name = "Category 1", Description = "Category 1", UrlSlug = "category-1" };
                    var catb = new Category() { Name = "Category 2", Description = "Category 2", UrlSlug = "category-2" };
                    repo.Insert(cata);
                    repo.Insert(catb);

                    con.SaveChanges();
                }
            }
        }

        [Test, Isolated]
        public async Task NotSaveChangesUntilOuterUnitOfWorkSaves()
        {
            var category = new Category()
            {
                Name = "Test Category",
                Description = "Test Category",
                UrlSlug = "test-category"
            };

            var category2 = new Category()
            {
                Name = "Test Category 2",
                Description = "Test Category 2",
                UrlSlug = "test-category2"
            };

            using (var unitOfWork = _unitOfWorkFactory.Create())
            {
                unitOfWork.Repository<IApplicationDbContext, Category>().Insert(category);

                using (var unitOfWork2 = _unitOfWorkFactory.Create())
                {
                    unitOfWork2.Repository<IApplicationDbContext, Category>().Insert(category2);
                    await unitOfWork2.CompleteAsync();
                }

                var count = await unitOfWork.Repository<IApplicationDbContext, Category>().GetCountAsync();

                count.Should().Be(0);

                await unitOfWork.CompleteAsync();

                count = await unitOfWork.Repository<IApplicationDbContext, Category>().GetCountAsync();

                count.Should().Be(2);
            }
        }
    }
}
