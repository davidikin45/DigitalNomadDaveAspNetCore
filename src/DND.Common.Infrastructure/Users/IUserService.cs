using System.Security.Claims;

namespace DND.Common.Infrastructure.Users
{
    public interface IUserService
    {
        ClaimsPrincipal User { get; }
    }
}
