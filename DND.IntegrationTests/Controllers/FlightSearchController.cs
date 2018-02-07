using AutoMapper;
using DND.Controllers;
using DND.Domain;
using DND.Domain.Interfaces.Persistance;
using DND.Domain.Models;
using DND.EFPersistance;
using DND.EFPersistance.Identity;
using DND.Services;
using Moq;
using NUnit.Framework;
using Solution.Base.Implementation.Repository;
using Solution.Base.Implementation.UnitOfWork;
using Solution.Base.Interfaces.Persistance;
using Solution.Base.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


            _controller = new FlightSearchController(new FlightSearchService(new BaseUnitOfWorkScopeFactory(mockDbContextFactory.Object, new BaseAmbientDbContextLocator(), new BaseRepositoryFactory()), _mapper), _mapper);
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
