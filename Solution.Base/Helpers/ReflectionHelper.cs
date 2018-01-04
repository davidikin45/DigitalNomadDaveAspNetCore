using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Solution.Base.Helpers
{
    public static class ReflectionHelper
    {
        public static MemberInfo GetMemberInfo(Expression memberExpression)
        {
            var memberExpr = memberExpression as MemberExpression;

            if (memberExpr == null && memberExpression is LambdaExpression)
            {
                memberExpr = (memberExpression as LambdaExpression).Body as MemberExpression;
            }

            return memberExpr != null ? memberExpr.Member : null;
        }
    }
}
