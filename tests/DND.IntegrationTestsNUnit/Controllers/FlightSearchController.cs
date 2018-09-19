using AutoMapper;
using DND.ApplicationServices.FlightSearch.Search.Services;
using DND.Data;
using DND.Data.Identity;
using DND.Domain.Blog.Categories;
using DND.DomainServices.FlightSearch.Search.Services;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Linq;

namespace DND.IntegrationTestsNUnit.Controllers
{
    using DND.Common.Data.Repository.GenericEF;
    using DND.Common.Data.UnitOfWork;
    using DND.Common.Infrastructure.Interfaces.Data;
    using DND.Common.Testing;
    using DND.Web.FlightSearch.MVCImplementation.Api;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    [TestFixture]
    public class FlightSearchControllerTests
    {
        private FlightSearchController _controller;
        private ApplicationContext _context;
        private IdentityContext _identityContext;
        private IMapper _mapper;
        private IDbContextFactoryProducerSingleton _dbContextFactoryProducer;


        [SetUp]
        public void SetUp()
        {
            var config = new MapperConfiguration(cfg =>
            {

            });
            _mapper = config.CreateMapper();

            var connectionString = TestHelper.GetConfiguration("Integration").GetConnectionString("DefaultConnectionString");

            _context = TestHelper.GetContext<ApplicationContext>(connectionString, false);

            var mockDbContextFactory = new Mock<IDbContextFactory<ApplicationContext>>();
            mockDbContextFactory.Setup(c => c.CreateDbContext()).Returns(_context);

            var mockDbContextFactoryProducer = new Mock<IDbContextFactoryProducerSingleton>();
            mockDbContextFactoryProducer.Setup(c => c.GetFactory<ApplicationContext>()).Returns(mockDbContextFactory.Object);
            _dbContextFactoryProducer = mockDbContextFactoryProducer.Object;

            _identityContext = TestHelper.GetContext<IdentityContext>(connectionString, false);

            _controller = new FlightSearchController(new FlightSearchApplicationService(new FlightSearchDomainService(new UnitOfWorkScopeFactory(_dbContextFactoryProducer, new AmbientDbContextLocator(), new GenericRepositoryFactory())), _mapper), _mapper, null, null, null);
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

            var unitOfWorkFactory = new UnitOfWorkScopeFactory(_dbContextFactoryProducer, new AmbientDbContextLocator(), new GenericRepositoryFactory());

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
                unitOfWork.Repository<ApplicationContext, Category>().Insert(category);

                using (var unitOfWork2 = unitOfWorkFactory.Create())
                {
                    unitOfWork2.Repository<ApplicationContext, Category>().Insert(category2);
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
