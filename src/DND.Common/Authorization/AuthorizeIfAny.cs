using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;

namespace DND.Common.Authorization
{
    public class AuthorizeIfAnyScope : AuthorizeAttribute
    {
        public AuthorizeIfAnyScope(params string[] roles)
            :base(String.Join(",", roles.Select(x => x.ToString()).ToArray()))
        {

        }
    }
}
