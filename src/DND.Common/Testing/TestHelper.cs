using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.ModelBinding;
using System.Web.Routing;

namespace DND.Common.Testing
{
    public static class TestHelpers
    {
        public static DbSet<T> MockDbSet<T>() where T : class
        {
            return MockDbSet<T>(null);
        }

        public static DbSet<T> MockDbSet<T>(List<T> inMemoryData) where T : class
        {
            if (inMemoryData == null)
            {
                inMemoryData = new List<T>();
            }
            var mockDbSet = new Mock<DbSet<T>>();
            var queryableData = inMemoryData.AsQueryable();

            mockDbSet.Setup(m => m.Add(It.IsAny<T>())).Callback<T>(inMemoryData.Add);
            //mockDbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());
            mockDbSet.Setup(x => x.AsNoTracking()).Returns(mockDbSet.Object);
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockDbSet.As<IDbAsyncEnumerable<T>>()
                  .Setup(m => m.GetAsyncEnumerator())
                  .Returns(new TestDbAsyncEnumerator<T>(queryableData.GetEnumerator()));

            mockDbSet.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(new TestDbAsyncQueryProvider<T>(queryableData.Provider));

            return mockDbSet.Object;
        }

        public static void MockCurrentUser(this Controller controller, string userId, string username, string authenticationType)
        {
            controller.MockHttpContext(userId, username, authenticationType);
        }

        private static ClaimsPrincipal CreateClaimsPrincipal(string userId, string username, string authenticationType)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
           {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.NameIdentifier, userId),
            }, authenticationType));

            return user;
        }

        public static void MockHttpContext(this Controller controller, string userId, string username, string authenticationType)
        {
            var httpContext = FakeAuthenticatedHttpContext(userId, username, authenticationType);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        private static HttpContext FakeAuthenticatedHttpContext(string userId, string username, string authenticationType)
        {
            var context = new Mock<HttpContext>();

            var request = new Mock<HttpRequest>();
            var response = new Mock<HttpResponse>();
            var session = new Mock<ISession>();
            var user = CreateClaimsPrincipal(userId, username, authenticationType);
            var identity = new Mock<IIdentity>();

            context.SetupGet(r => r.RequestAborted).Returns(default(CancellationToken));
            context.Setup(ctx => ctx.Request).Returns(request.Object);
            context.Setup(ctx => ctx.Response).Returns(response.Object);

            context.Setup(ctx => ctx.Session).Returns(session.Object);
            context.Setup(ctx => ctx.User).Returns(user);

            return context.Object;
        }
    }
}
