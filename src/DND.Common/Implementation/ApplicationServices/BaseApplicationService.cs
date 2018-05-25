using AutoMapper;
using DND.Common.Interfaces.ApplicationServices;
using DND.Common.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DND.Common.Implementation.ApplicationServices
{
    public abstract class BaseApplicationService : IBaseApplicationService
    {
        private readonly IFileSystemGenericRepositoryFactory _fileSystemGenericRepositoryFactory;
        public IMapper Mapper { get; }

        public BaseApplicationService(IMapper mapper)
        {
            Mapper = mapper;
        }

        public BaseApplicationService(IFileSystemGenericRepositoryFactory fileSystemGenericRepositoryFactory, IMapper mapper)
            : this(fileSystemGenericRepositoryFactory)
        {
            Mapper = mapper;
        }

        public BaseApplicationService(IFileSystemGenericRepositoryFactory fileSystemGenericRepositoryFactory)
        {
            if (fileSystemGenericRepositoryFactory == null) throw new ArgumentNullException("fileSystemGenericRepositoryFactory");
            _fileSystemGenericRepositoryFactory = fileSystemGenericRepositoryFactory;
        }

        public BaseApplicationService()
        {

        }

        public IFileSystemGenericRepositoryFactory FileSytemGenericRepositoryFactory
        {
            get
            {
                return _fileSystemGenericRepositoryFactory;
            }
        }

        public Expression<Func<TDestination, Object>>[] GetMappedIncludes<TSource, TDestination>(Expression<Func<TSource, Object>>[] selectors)
        {
            if (selectors == null)
                return new Expression<Func<TDestination, Object>>[] { };

            List<Expression<Func<TDestination, Object>>> returnList = new List<Expression<Func<TDestination, Object>>>();

            foreach (var selector in selectors)
            {
                returnList.Add(Mapper.Map<Expression<Func<TDestination, Object>>>(selector));
            }

            return returnList.ToArray();
        }

        public Expression<Func<TDestination, TProperty>> GetMappedSelector<TSource, TDestination, TProperty>(Expression<Func<TSource, TProperty>> selector)
        {
            return Mapper.Map<Expression<Func<TDestination, TProperty>>>(selector);
        }

        public Func<IQueryable<TDestination>, IOrderedQueryable<TDestination>> GetMappedOrderBy<TSource, TDestination>(Expression<Func<IQueryable<TSource>, IOrderedQueryable<TSource>>> orderBy)
        {
            //return LamdaHelper.GetMappedOrderBy<TSource, TDestination>(Mapper, orderBy);
            if (orderBy == null)
                return null;

            return Mapper.Map<Expression<Func<IQueryable<TDestination>, IOrderedQueryable<TDestination>>>>(orderBy).Compile();
        }
    }
}
