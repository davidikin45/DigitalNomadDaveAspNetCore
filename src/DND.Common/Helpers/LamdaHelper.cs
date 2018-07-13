using AutoMapper;
using DND.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Common.Helpers
{
    public static class LamdaHelper
    {
        //Expression > Func yes
        //Func > Expression no compiled
        public static Expression<Func<TDestination, TProperty>> GetMappedSelector<TSource, TDestination, TProperty>(IMapper mapper, Expression<Func<TSource, TProperty>> selector)
        {
            return mapper.Map<Expression<Func<TDestination, TProperty>>>(selector);

            //if (selector == null)
            //    return null;
            //return Mapper.Map<Expression<Func<TDestination, TProperty>>>(selector);

            //return mapper.MapTo<>


            //var map = mapper.ConfigurationProvider.FindTypeMapFor<TSource, TDestination>();

            //var mInfo = ReflectionHelper.GetMemberInfo(selector);

            //if (mInfo == null)
            //{
            //    throw new Exception(string.Format(
            //        "Can't get PropertyMap. \"{0}\" is not a member expression", selector));
            //}

            //var sourceType = typeof(TSource);
            //var destinationType = typeof(TDestination);
            //var destinationProperty = mapper.GetDestinationMappedProperty(sourceType, destinationType, mInfo);

            //var param = Expression.Parameter(typeof(TDestination));
            //var body = Expression.MakeMemberAccess(param, destinationProperty);
            //var lambda = Expression.Lambda<Func<TDestination, TProperty>>(body, param);

            //return lambda;
        }


        public static Func<IQueryable<TDestination>, IOrderedQueryable<TDestination>> GetMappedOrderBy<TSource, TDestination>(IMapper mapper, Expression<Func<IQueryable<TSource>, IOrderedQueryable<TSource>>> orderBy)
        {
            return Mapper.Map<Func<IQueryable<TDestination>, IOrderedQueryable<TDestination>>>(orderBy);

            //if (orderBy == null)
            //    return null;

            //var orderType = ((MethodCallExpression)orderBy.Body).Method.Name;
            //var arguments = ((LambdaExpression)((UnaryExpression)((MethodCallExpression)orderBy.Body).Arguments[1]).Operand).Body;

            //MemberExpression ex = ((MemberExpression)arguments);

            //List<Type> sourceTypes = new List<Type>();

            //Type sourceType = ex.Expression.Type;

            //sourceTypes.Add(sourceType);

            //var sourceProperty = (PropertyInfo)ex.Member;

            //var orderColumn = sourceProperty.Name;

            //while (ex.Expression is MemberExpression)
            //{
            //    ex = ((MemberExpression)ex.Expression);
            //    sourceType = ex.Expression.Type;
            //    sourceTypes.Add(sourceType);

            //    sourceProperty = (PropertyInfo)ex.Member;

            //    orderColumn = sourceProperty.Name + "." + orderColumn;
            //}

            //sourceTypes.Reverse();

            //return GetOrderBy<TDestination>(orderColumn, orderType, mapper, sourceTypes).Compile();
        }
        public static Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> GetOrderByFunc<TEntity>(string orderColumn, string orderType, IMapper mapper = null, List<Type> sourceTypes = null)
        {
            return GetOrderBy<TEntity>(orderColumn, orderType, mapper, sourceTypes).Compile();
        }

        public static Expression<Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>> GetOrderBy<TEntity>(string orderColumn, string orderType, IMapper mapper = null, List<Type> sourceTypes = null)
        {
            if (string.IsNullOrEmpty(orderColumn))
                return null;

            Type typeQueryable = typeof(IQueryable<TEntity>);
            ParameterExpression argQueryable = Expression.Parameter(typeQueryable, "p");
            var outerExpression = Expression.Lambda(argQueryable, argQueryable);
            string[] props = orderColumn.Split('.');
            IQueryable<TEntity> query = new List<TEntity>().AsQueryable<TEntity>();
            Type type = typeof(TEntity);
            ParameterExpression arg = Expression.Parameter(type, "x");

            Expression expr = arg;
            int i = 0;
            foreach (string prop in props)
            {
                var targetProperty = prop;
                if (sourceTypes != null && mapper != null)
                {
                    targetProperty = mapper.GetDestinationMappedProperty(sourceTypes[i], type, prop).Name;
                }

                PropertyInfo pi = type.GetProperty(targetProperty, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
                i++;
            }
            LambdaExpression lambda = Expression.Lambda(expr, arg);

            string methodName = (orderType == "asc"  || orderType == "OrderBy") ? "OrderBy" : "OrderByDescending";

            var genericTypes = new Type[] { typeof(TEntity), type };

            MethodCallExpression resultExp =
                Expression.Call(typeof(Queryable), methodName, genericTypes, outerExpression.Body, Expression.Quote(lambda));

            var finalLambda = Expression.Lambda(resultExp, argQueryable);

            return (Expression<Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>>)finalLambda;
        }

        public static Func<IEnumerable<TEntity>, IOrderedEnumerable<TEntity>> GetOrderByIEnumerableFunc<TEntity>(string orderColumn, string orderType, IMapper mapper = null, List<Type> sourceTypes = null)
        {
            return GetOrderByIEnumerable<TEntity>(orderColumn, orderType, mapper, sourceTypes).Compile();
        }

        public static Expression<Func<IEnumerable<TEntity>, IOrderedEnumerable<TEntity>>> GetOrderByIEnumerable<TEntity>(string orderColumn, string orderType, IMapper mapper = null, List<Type> sourceTypes = null)
        {
            if (string.IsNullOrEmpty(orderColumn))
                return null;

            Type typeQueryable = typeof(IEnumerable<TEntity>);
            ParameterExpression argQueryable = Expression.Parameter(typeQueryable, "p");
            var outerExpression = Expression.Lambda(argQueryable, argQueryable);

            string[] props = orderColumn.Split('.');
            IEnumerable<TEntity> query = new List<TEntity>().AsEnumerable<TEntity>();
            Type type = typeof(TEntity);
            ParameterExpression arg = Expression.Parameter(type, "x");

            Expression expr = arg;
            int i = 0;
            foreach (string prop in props)
            {
                var targetProperty = prop;
                if (sourceTypes != null && mapper != null)
                {
                    targetProperty = mapper.GetDestinationMappedProperty(sourceTypes[i], type, prop).Name;
                }

                PropertyInfo pi = type.GetProperty(targetProperty, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
                i++;
            }
            LambdaExpression lambda = Expression.Lambda(expr, arg);

            string methodName = (orderType == "asc" || orderType == "OrderBy") ? "OrderBy" : "OrderByDescending";
            
            var resultExp =
                Expression.Call(typeof(Enumerable), methodName, new Type[] { typeof(TEntity), lambda.Body.Type }, argQueryable, lambda);

            var finalLambda = Expression.Lambda(resultExp, argQueryable);

            return (Expression<Func<IEnumerable<TEntity>, IOrderedEnumerable<TEntity>>>)finalLambda;
        }

        public static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> items, string property, bool ascending)
        {
            var MyObject = Expression.Parameter(typeof(T), "MyObject");
            var MyEnumeratedObject = Expression.Parameter(typeof(IEnumerable<T>), "MyEnumeratedObject");
            var MyProperty = Expression.Property(MyObject, property);
            var MyLamda = Expression.Lambda(MyProperty, MyObject);
            var MyMethod = Expression.Call(typeof(Enumerable), ascending ? "OrderBy" : "OrderByDescending", new[] { typeof(T), MyLamda.Body.Type }, MyEnumeratedObject, MyLamda);
            var MySortedLamda = Expression.Lambda<Func<IEnumerable<T>, IOrderedEnumerable<T>>>(MyMethod, MyEnumeratedObject).Compile();
            return MySortedLamda(items);
        }

        public static Expression<Func<TEntity, bool>> SearchForEntityByIds<TEntity>(IEnumerable<object> ids)
        {
            var item = Expression.Parameter(typeof(TEntity), "entity");
            var prop = Expression.PropertyOrField(item, "Id");

            var propType = typeof(TEntity).GetProperty("Id").PropertyType;

            var genericType = typeof(List<>).MakeGenericType(propType);
            var idList = Activator.CreateInstance(genericType);

            var add_method = idList.GetType().GetMethod("Add");
            foreach (var id in ids)
            {
                add_method.Invoke(idList, new object[] { (dynamic)Convert.ChangeType(id, propType) });
            }

            var contains_method = idList.GetType().GetMethod("Contains");
            var value_expression = Expression.Constant(idList);
            var contains_expression = Expression.Call(value_expression, contains_method, prop);
            var lamda = Expression.Lambda<Func<TEntity, bool>>(contains_expression, item);
            return lamda;
        }

        public static Expression<Func<TEntity, bool>> SearchForEntityById<TEntity>(object id)
        {
            var item = Expression.Parameter(typeof(TEntity), "entity");
            var prop = Expression.PropertyOrField(item, "Id");
            var propType = typeof(TEntity).GetProperty("Id").PropertyType;

            var value = Expression.Constant((dynamic)Convert.ChangeType(id, propType));

            var equal = Expression.Equal(prop, value);
            var lambda = Expression.Lambda<Func<TEntity, bool>>(equal, item);
            return lambda;
        }

        public static object SearchForEntityById(Type type, object id)
        {
            Type funcType = typeof(Func<,>).MakeGenericType(new[] { type, typeof(bool) });
          
            var item = Expression.Parameter(type, "entity");
            var prop = Expression.PropertyOrField(item, "Id");
            var propType = type.GetProperty("Id").PropertyType;

            var value = Expression.Constant((dynamic)Convert.ChangeType(id, propType));

            var equal = Expression.Equal(prop, value);

            return typeof(LamdaHelper).GetMethod(nameof(Lambda)).MakeGenericMethod(funcType).Invoke(null, new object[] { equal, new ParameterExpression[] { item } });
        }

        public static int Count<TSource>(IQueryable<TSource> source)
        {
            return Queryable.Count(source);
        }

        public static Task<int> CountEF6Async<TSource>(IQueryable<TSource> source, CancellationToken cancellationToken)
        {
            return System.Data.Entity.QueryableExtensions.CountAsync(source, cancellationToken);
        }

        public static Task<int> CountEFCoreAsync<TSource>(IQueryable<TSource> source, CancellationToken cancellationToken)
        {
            return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.CountAsync(source, cancellationToken);
        }

        public static Expression<TDelegate> Lambda<TDelegate>(Expression body, params ParameterExpression[] parameters)
        {
            return Expression.Lambda<TDelegate>(body, parameters);
        }

        public static IQueryable<TSource> Where<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            return Queryable.Where(source, predicate);
        }

        public static TSource FirstOrDefault<TSource>(IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return Enumerable.FirstOrDefault(source, predicate);
        }
    }
}
