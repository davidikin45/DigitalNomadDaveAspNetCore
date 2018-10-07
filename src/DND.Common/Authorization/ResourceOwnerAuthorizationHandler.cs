using DND.Common.Infrastrucutre.Interfaces.Domain;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Authorization
{
    public class ResourceOwnerAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, IEntityAuditable>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       OperationAuthorizationRequirement requirement,
                                                       IEntityAuditable entity)
        {
            if (context.User.Claims.Where(c => c.Type == JwtClaimTypes.Scope && c.Value == requirement.Name).Count() > 0)
            {
                context.Succeed(requirement);
            }
            else if (context.User.Claims.Where(c => c.Type == JwtClaimTypes.Scope && c.Value == requirement.Name + "-if-owner").Count() > 0)
            {
                var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (entity.UserOwner == null || entity.UserOwner == userId)
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
