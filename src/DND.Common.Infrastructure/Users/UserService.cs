using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DND.Common.Infrastructure.Users
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _context;
        public UserService(IHttpContextAccessor context)
        {
            _context = context;
        }

        public ClaimsPrincipal User
        {
            get
            {
                return _context.HttpContext.User;
            }
        }
    }
}
