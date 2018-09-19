using AutoMapper;
using DND.Common.Extensions;
using DND.Common.Infrastructure.Helpers;
using RefactorThis.GraphDiff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Common.Helpers
{
    public static class LamdaHelper
    {
        //Expression > Func yes
        //Func > Expression no compiled
        public static Expression<Func<TDestination, Object>>[] GetMappedIncludes<TSource, TDestination>(IMapper mapper, Expression<Func<TSource, Object>>[] selectors)
        {
            if (selectors == null)
                return new Expression<Func<TDestination, Object>>[] { };

            List<Expression<Func<TDestination, Object>>> returnList = new List<Expression<Func<TDestination, Object>>>();

            foreach (var selector in selectors)
            {
                returnList.Add(mapper.Map<Expression<Func<TDestination, Object>>>(selector));
            }

            return returnList.ToArray();
        }

        public static Expression<Func<TDestination, TProperty>> GetMappedSelector<TSource, TDestination, TProperty>(IMapper mapper, Expression<Func<TSource, TProperty>> selector)
        {
            return mapper.Map<Expression<Func<TDestination, TProperty>>>(selector);
        }

        public static Expression<Func<IQueryable<TDestination>, IOrderedQueryable<TDestination>>> GetMappedOrderBy<TSource, TDestination>(IMapper mapper, Expression<Func<IQueryable<TSource>, IOrderedQueryable<TSource>>> orderBy)
        {
            if (orderBy == null)
                return null;

            return mapper.Map<Expression<Func<IQueryable<TDestination>, IOrderedQueryable<TDestination>>>>(orderBy);
        }

        public static Func<IQueryable<TDestination>, IOrderedQueryable<TDestination>> GetMappedOrderByCompiled<TSource, TDestination>(IMapper mapper, Expression<Func<IQueryable<TSource>, IOrderedQueryable<TSource>>> orderBy)
        {
            if (orderBy == null)
                return null;

            return mapper.Map<Expression<Func<IQueryable<TDestination>, IOrderedQueryable<TDestination>>>>(orderBy).Compile();
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

        public static IOrderedQueryable<T> QueryableOrderBy<T>(this IQueryable<T> items, string property, bool ascending)
        {
            var MyObject = Expression.Parameter(typeof(T), "MyObject");
            var MyEnumeratedObject = Expression.Parameter(typeof(IQueryable<T>), "MyQueryableObject");
            var MyProperty = Expression.Property(MyObject, property);
            var MyLamda = Expression.Lambda(MyProperty, MyObject);
            var MyMethod = Expression.Call(typeof(Queryable), ascending ? "OrderBy" : "OrderByDescending", new[] { typeof(T), MyLamda.Body.Type }, MyEnumeratedObject, MyLamda);
            var MySortedLamda = Expression.Lambda<Func<IQueryable<T>, IOrderedQueryable<T>>>(MyMethod, MyEnumeratedObject).Compile();
            return MySortedLamda(items);
        }

        public static IQueryable<T> CreateSearchQuery<T>(IQueryable<T> query, string values)
        {
            List<Expression> andExpressions = new List<Expression>();

            ParameterExpression parameter = Expression.Parameter(typeof(T), "p");

            MethodInfo contains_method = typeof(string).GetMethod("Contains", new[] { typeof(string) });

            var ignore = new List<string>() { "UserDeleted"};

            foreach (var value in values.Split('&'))
            {
                List<Expression> orExpressions = new List<Expression>();

                foreach (PropertyInfo prop in typeof(T).GetProperties().Where(x => x.PropertyType == typeof(string) && !ignore.Contains(x.Name) ))
                {
                    MemberExpression member_expression = Expression.PropertyOrField(parameter, prop.Name);

                    ConstantExpression value_expression = Expression.Constant(value, typeof(string));

                    MethodCallExpression contains_expression = Expression.Call(member_expression, contains_method, value_expression);

                    orExpressions.Add(contains_expression);
                }

                if (orExpressions.Count == 0)
                    return query;

                Expression or_expression = orExpressions[0];

                for (int i = 1; i < orExpressions.Count; i++)
                {
                    or_expression = Expression.OrElse(or_expression, orExpressions[i]);
                }

                andExpressions.Add(or_expression);
            }

            if (andExpressions.Count == 0)
                return query;

            Expression and_expression = andExpressions[0];

            for (int i = 1; i < andExpressions.Count; i++)
            {
                and_expression = Expression.AndAlso(and_expression, andExpressions[i]);
            }

            Expression<Func<T, bool>> expression = Expression.Lambda<Func<T, bool>>(
                and_expression, parameter);

            return query.Where(expression);
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

        public static Object SearchForEntityByProperty(Type type, string property, IEnumerable<object> values)
        {
            Type funcType = typeof(Func<,>).MakeGenericType(new[] { type, typeof(bool) });

            var item = Expression.Parameter(type, "entity");
            var prop = Expression.PropertyOrField(item, property);

            var propType = type.GetProperty(property).PropertyType;

            var genericType = typeof(List<>).MakeGenericType(propType);
            var idList = Activator.CreateInstance(genericType);

            var add_method = idList.GetType().GetMethod("Add");
            foreach (var value in values)
            {
                add_method.Invoke(idList, new object[] { (dynamic)Convert.ChangeType(value, propType) });
            }

            var contains_method = idList.GetType().GetMethod("Contains");
            var value_expression = Expression.Constant(idList);
            var contains_expression = Expression.Call(value_expression, contains_method, prop);

            return typeof(LamdaHelper).GetMethod(nameof(Lambda)).MakeGenericMethod(funcType).Invoke(null, new object[] { contains_expression, new ParameterExpression[] { item } });
        }

        public static Object SearchForEntityByIds(Type type, IEnumerable<object> ids)
        {
            Type funcType = typeof(Func<,>).MakeGenericType(new[] { type, typeof(bool) });

            var item = Expression.Parameter(type, "entity");
            var prop = Expression.PropertyOrField(item, "Id");

            var propType = type.GetProperty("Id").PropertyType;

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

            return typeof(LamdaHelper).GetMethod(nameof(Lambda)).MakeGenericMethod(funcType).Invoke(null, new object[] { contains_expression, new ParameterExpression[] { item } });
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

        public static object SearchForEntityByIdCompile(Type type, object id)
        {
            Type funcType = typeof(Func<,>).MakeGenericType(new[] { type, typeof(bool) });

            var item = Expression.Parameter(type, "entity");
            var prop = Expression.PropertyOrField(item, "Id");
            var propType = type.GetProperty("Id").PropertyType;

            var value = Expression.Constant((dynamic)Convert.ChangeType(id, propType));

            var equal = Expression.Equal(prop, value);

            return typeof(LamdaHelper).GetMethod(nameof(LambdaCompile)).MakeGenericMethod(funcType).Invoke(null, new object[] { equal, new ParameterExpression[] { item } });
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

        public static object SourceDestinationEquivalentExpressionById(Type sourceType, Type destinationType)
        {
            Type funcType = typeof(Func<,,>).MakeGenericType(new[] { sourceType, destinationType, typeof(bool) });

            var itemSource = Expression.Parameter(sourceType, "source");
            var propSource = Expression.PropertyOrField(itemSource, "Id");
            var propTypeSource = sourceType.GetProperty("Id").PropertyType;

            var itemDestination = Expression.Parameter(destinationType, "destination");
            var propDestination = Expression.PropertyOrField(itemDestination, "Id");
            var propTypeDestination = destinationType.GetProperty("Id").PropertyType;

            var equal = Expression.Equal(propSource, propDestination);

            return typeof(LamdaHelper).GetMethod(nameof(Lambda)).MakeGenericMethod(funcType).Invoke(null, new object[] { equal, new ParameterExpression[] { itemSource, itemDestination } });
        }

        public static Expression GraphDiffConfiguration(Type entityType, string mapSuffix = "")
        {
            Type iUpdateConfigurationType =  typeof(IUpdateConfiguration<>).MakeGenericType(new[] { entityType });
            Type funcType = typeof(Func<,>).MakeGenericType(new[] { iUpdateConfigurationType,  typeof(object) });

            MethodInfo ownedCollectionMethod2 = typeof(UpdateConfigurationExtensions).GetMethods().Where(m => m.Name == nameof(UpdateConfigurationExtensions.OwnedCollection) && m.GetParameters().Count() == 2).First();
            MethodInfo ownedCollectionMethod3 = typeof(UpdateConfigurationExtensions).GetMethods().Where(m => m.Name == nameof(UpdateConfigurationExtensions.OwnedCollection) && m.GetParameters().Count() == 3).First();

            var entity = Expression.Parameter(entityType, "entity");
            var compositionProperties = RelationshipHelper.GetAllCompositionAndAggregationRelationshipPropertyIncludes(true, entityType).Where(p => !p.Contains("."));
            if(compositionProperties.Count() > 0)
            {
                var map = Expression.Parameter(iUpdateConfigurationType, "map" + mapSuffix);
                MethodCallExpression ownedExpression = null;

                foreach (var compositionProperty in compositionProperties)
                {
                    var collectionProperty = Expression.PropertyOrField(entity, compositionProperty);
                    var collectionItemType = entityType.GetProperty(compositionProperty).PropertyType.GetGenericArguments().First();

                    Type iCollectionType = typeof(ICollection<>).MakeGenericType(new[] { collectionItemType });
                    Type funcTypeExpression = typeof(Func<,>).MakeGenericType(new[] { entityType, iCollectionType });

                    var lambdaExpression = (Expression)typeof(LamdaHelper).GetMethod(nameof(Lambda)).MakeGenericMethod(funcTypeExpression).Invoke(null, new object[] { collectionProperty, new ParameterExpression[] { entity } });

                    MethodInfo ownedCollectionMethod2Generic = ownedCollectionMethod2.MakeGenericMethod(entityType, collectionItemType);
                    MethodInfo ownedCollectionMethod3Generic = ownedCollectionMethod3.MakeGenericMethod(entityType, collectionItemType);

                    var collectionPropertyconfiguration = GraphDiffConfiguration(collectionItemType, mapSuffix + compositionProperty);

                    if(collectionPropertyconfiguration == null)
                    {
                        if (ownedExpression == null)
                        {
                            ownedExpression = Expression.Call(ownedCollectionMethod2Generic, map, lambdaExpression);
                        }
                        else
                        {
                            ownedExpression = Expression.Call(ownedCollectionMethod2Generic, ownedExpression, lambdaExpression);
                        }
                    }
                    else
                    {
                        if (ownedExpression == null)
                        {
                            ownedExpression = Expression.Call(ownedCollectionMethod3Generic, map, lambdaExpression, collectionPropertyconfiguration);
                        }
                        else
                        {
                            ownedExpression = Expression.Call(ownedCollectionMethod3Generic, ownedExpression, lambdaExpression, collectionPropertyconfiguration);
                        }
                    }
                }

                return (Expression)typeof(LamdaHelper).GetMethod(nameof(Lambda)).MakeGenericMethod(funcType).Invoke(null, new object[] { ownedExpression, new ParameterExpression[] { map } });
            }

            return null;
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

        public static TDelegate LambdaCompile<TDelegate>(Expression body, params ParameterExpression[] parameters)
        {
            return Expression.Lambda<TDelegate>(body, parameters).Compile();
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
