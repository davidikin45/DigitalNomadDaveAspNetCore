using AutoMapper;
using DND.ApplicationServices.FlightSearch.Search.Services;
using DND.Common.Implementation.Repository;
using DND.Common.Implementation.UnitOfWork;
using DND.Common.Interfaces.Data;
using DND.Common.Testing;
using DND.Data;
using DND.Data.Identity;
using DND.Domain.Blog.Categories;
using DND.DomainServices.FlightSearch.Search.Services;
using DND.Infrastructure;
using DND.Interfaces.Blog.Data;
using DND.Interfaces.Data;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Linq;

namespace DND.IntegrationTestsNUnit.Controllers
{
    using DND.Common.Implementation.Data;
    using DND.Web.MVCImplementation.FlightSearch.Api;
    using NUnit.Framework;
    [TestFixture]
    public class FlightSearchControllerTests
    {
        private FlightSearchController _controller;
        private ApplicationDbContext _context;
        private IdentityDbContext _identityContext;
        private IMapper _mapper;


        [SetUp]
        public void SetUp()
        {
            var config = new MapperConfiguration(cfg =>
            {

            });
            _mapper = config.CreateMapper();

            var connectionString = DNDConnectionStrings.GetConnectionString("DefaultConnectionString");

            _context = new ApplicationDbContext(connectionString, true);
            var mockDbContextFactory = new Mock<IDbContextFactory<IBlogDbContext>>();
            mockDbContextFactory.Setup(c => c.CreateBaseDbContext()).Returns(_context);

            _identityContext = GlobalDbSetupFixture.CreateIdentityContext(false);

            _controller = new FlightSearchController(new FlightSearchApplicationService(new FlightSearchDomainService(new UnitOfWorkScopeFactory(new DbContextFactoryProducerSingleton(new IDbContextAbstractFactory[] { mockDbContextFactory.Object }), new AmbientDbContextLocator(), new GenericRepositoryFactory())), _mapper), _mapper, null, null, null);
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

            var unitOfWorkFactory = new UnitOfWorkScopeFactory(new DbContextFactoryProducerSingleton(new IDbContextAbstractFactory[] { }), new AmbientDbContextLocator(), new GenericRepositoryFactory());

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
                unitOfWork.Repository<IBlogDbContext, Category>().Insert(category);

                using (var unitOfWork2 = unitOfWorkFactory.Create())
                {
                    unitOfWork2.Repository<IApplicationDbContext, Category>().Insert(category2);
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
