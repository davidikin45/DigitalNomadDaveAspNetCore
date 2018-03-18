using AutoMapper;
using DND.ApplicationServices;
using DND.Controllers;
using DND.Domain;
using DND.Domain.Interfaces.Persistance;
using DND.Domain.Models;
using DND.DomainServices;
using DND.EFPersistance;
using DND.EFPersistance.Identity;
using Moq;
using NUnit.Framework;
using DND.Common.Implementation.Repository;
using DND.Common.Implementation.UnitOfWork;
using DND.Common.Interfaces.Persistance;
using DND.Common.Testing;
using System.Linq;

namespace DND.IntegrationTests.Controllers
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
            var config = new MapperConfiguration(cfg => {

            });
            _mapper = config.CreateMapper();

            _context = new ApplicationDbContext(DNDConnectionStrings.GetConnectionString("DefaultConnectionString"));
            var mockDbContextFactory = new Mock<IDbContextFactory>();
            mockDbContextFactory.Setup(c => c.CreateDefault()).Returns(_context);


            _controller = new FlightSearchController(new FlightSearchApplicationService(new FlightSearchDomainService(new BaseUnitOfWorkScopeFactory(mockDbContextFactory.Object, new BaseAmbientDbContextLocator(), new BaseRepositoryFactory())), _mapper), _mapper, null, null);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test, Isolated]
        public void Search_WhenCalled_ShouldReturn()
        {
            //Arrange
            var user = _identityContext.Users.First();
            _controller.MockCurrentUser(user.Id, user.UserName);


            var unitOfWorkFactory = new BaseUnitOfWorkScopeFactory(new DbContextFactory(), new BaseAmbientDbContextLocator(), new BaseRepositoryFactory());

            var post = new BlogPost();
            using (var unitOfWork = unitOfWorkFactory.Create())
            {
                unitOfWork.Repository<IApplicationDbContext, BlogPost>().Create(post);

                using (var unitOfWork2 = unitOfWorkFactory.Create())
                {
                    unitOfWork2.Repository<IApplicationDbContext, BlogPost>().Create(post);
                    unitOfWork2.Complete();
                }
                unitOfWork.Complete();
            }

            //Act


            //Assert
        }
    }
}
