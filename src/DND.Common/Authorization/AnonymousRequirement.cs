using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace DND.Common.Authorization
{
    public class AnonymousRequirement : IAuthorizationRequirement
    {

    }

    public class AnonymousHandler : AuthorizationHandler<AnonymousRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AnonymousRequirement requirement)
        {
           context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
