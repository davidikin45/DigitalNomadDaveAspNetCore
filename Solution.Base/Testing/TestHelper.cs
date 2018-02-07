using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Solution.Base.Testing
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

        public static void MockCurrentUser(this Controller controller, string userId, string username)
        {

            controller.MockHttpContext(userId, username);
        }

        private static IPrincipal CreatePrincipal(string userId, string username)
        {

            var identity = new GenericIdentity(username);
            identity.AddClaim(
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", username));
            identity.AddClaim(
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", userId));


            var principal = new GenericPrincipal(identity, null);

            return principal;

        }

        public static void MockHttpContext(this Controller controller, string userId, string username)
        {

            //var httpContext = FakeAuthenticatedHttpContext(userId, username);
            //ControllerContext context =
            //new ControllerContext(
            //new ActionContext(httpContext,
            //new RouteData()), controller);
            //controller.ControllerContext = context;

        }

        //private static HttpContextBase FakeAuthenticatedHttpContext(string userId, string username)
        //{
        //    var context = new Mock<HttpContextBase>();
        //    var request = new Mock<HttpRequestBase>();
        //    var response = new Mock<HttpResponseBase>();
        //    var session = new Mock<HttpSessionStateBase>();
        //    var server = new Mock<HttpServerUtilityBase>();
        //    var user = CreatePrincipal(userId, username);
        //    var identity = new Mock<IIdentity>();

        //    response.SetupGet(r => r.ClientDisconnectedToken).Returns(default(CancellationToken));

        //    context.Setup(ctx => ctx.Request).Returns(request.Object);
        //    context.Setup(ctx => ctx.Response).Returns(response.Object);

        //    context.Setup(ctx => ctx.Session).Returns(session.Object);
        //    context.Setup(ctx => ctx.Server).Returns(server.Object);
        //    context.Setup(ctx => ctx.User).Returns(user);

        //    //user.Setup(ctx => ctx.Identity).Returns(identity.Object);
        //    //identity.Setup(id => id.IsAuthenticated).Returns(true);
        //    //identity.Setup(id => id.Name).Returns(username);

        //    return context.Object;
        //}

        public static void BindModelToController(this Controller controller, object model)
        {
            //var modelBinder = new ModelBindingContext()
            //{
            //    ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
            //    ValueProvider = new NameValueCollectionValueProvider(new NameValueCollection(), CultureInfo.InvariantCulture)

            //};
            //var binder = new DefaultModelBinder().BindModel(new ControllerContext(), modelBinder);
            //controller.ModelState.Clear();
            //controller.ModelState.Merge(modelBinder.ModelState);
        }
    }
}
