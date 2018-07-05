using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Common.Controllers.Api
{
    public static class ApiScopes
    {
        public const string Full = "api.full_access";
        public const string Create = "api.create_access";
        public const string Read = "api.read_access";
        public const string Update = "api.update_access";
        public const string Delete = "api.delete_access";
    }
}
