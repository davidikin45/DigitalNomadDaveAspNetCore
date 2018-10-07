using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Infrastructure
{
    public class OperationConstants
    {
        public static readonly string Create = "create";
        public static readonly string Query = "query";
        public static readonly string Read = "read";
        public static readonly string Update = "update";
        public static readonly string Delete = "delete";
    }

    public class OperationScopeConstants
    {
        public static readonly string Create = "create";
        public static readonly string Query = "query";
        public static readonly string Read = "read";
        public static readonly string ReadOwn = "read-if-owner";
        public static readonly string Update = "update";
        public static readonly string UpdateOwn = "update-if-owner";
        public static readonly string Delete = "delete";
        public static readonly string DeleteOwn = "delete-if-owner";
    }
}
