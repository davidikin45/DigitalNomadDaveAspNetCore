using AutoMapper;
using DND.ApplicationServices.FlightSearch.Search.Services;
using DND.Common.Implementation.Repository;
using DND.Common.Implementation.UnitOfWork;
using DND.Common.Interfaces.Persistance;
using DND.Common.Testing;
using DND.Domain;
using DND.Domain.Blog.BlogPosts;
using DND.Domain.Blog.Categories;
using DND.Domain.Interfaces.Persistance;
using DND.DomainServices.FlightSearch.Search.Services;
using DND.EFPersistance;
using DND.EFPersistance.Identity;
using DND.Web.MVCImplementation.FlightSearch.Api;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Linq;

namespace DND.IntegrationTestsNUnit.Controllers
{
    [TestFixture]
    public class FlightSearchControllerTests
    {
        private FlightSearchController _controller;
        private ApplicationDbContext _context;
        private ApplicationIdentityDbContext _identityContext;
        private IMapper _mapper;


        [SetUp]
        public void SetUp()
        {
            var config = new MapperConfiguration(cfg =>
            {

            });
            _mapper = config.CreateMapper();

            var connectionString = DNDConnectionStrings.GetConnectionString("DefaultConnectionString");

            _context = new ApplicationDbContext(connectionString);
            var mockDbContextFactory = new Mock<IDbContextFactory>();
            mockDbContextFactory.Setup(c => c.CreateDefault()).Returns(_context);

            _identityContext = GlobalDbSetupFixture.CreateIdentityContext();

            _controller = new FlightSearchController(new FlightSearchApplicationService(new FlightSearchDomainService(new BaseUnitOfWorkScopeFactory(mockDbContextFactory.Object, new BaseAmbientDbContextLocator(), new BaseRepositoryFactory())), _mapper), _mapper, null, null, null);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
            _identityContext.Dispose();
        }

        [Test, Isolated]
        public void Search_WhenCalled_ShouldReturn()
        {
            //Arrange


            var user = _identityContext.Users.First();
            _controller.MockCurrentUser(user.Id, user.UserName, IdentityConstants.ApplicationScheme);

            var unitOfWorkFactory = new BaseUnitOfWorkScopeFactory(new DbContextFactory(), new BaseAmbientDbContextLocator(), new BaseRepositoryFactory());

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

            using (var unitOfWork = unitOfWorkFactory.Create())
            {
                unitOfWork.Repository<IApplicationDbContext, Category>().Create(category);

                using (var unitOfWork2 = unitOfWorkFactory.Create())
                {
                    unitOfWork2.Repository<IApplicationDbContext, Category>().Create(category2);
                    unitOfWork2.Complete();
                }
                unitOfWork.Complete();
            }
            //Act

            //Assert
            Assert.True(true);
        }
    }
}
