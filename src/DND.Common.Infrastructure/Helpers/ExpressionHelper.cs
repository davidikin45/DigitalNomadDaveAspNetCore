using System;
using System.Linq.Expressions;
using System.Reflection;

namespace TopTal.Common.Infrastructure.Helpers
{
    public static class ExpressionHelper
    {

        public static PropertyInfo GetPropertyInfo<TSource, TValue>(
            this Expression<Func<TSource, TValue>> expression)
        {
            return (PropertyInfo)((MemberExpression)expression.Body).Member;
        }

    }
}
