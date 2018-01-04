using AutoMapper;
using Solution.Base.Interfaces.Repository;
using Solution.Base.Interfaces.Services;
using Solution.Base.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Solution.Base.Implementation.Services
{
    public abstract class BaseBusinessService : IBaseBusinessService
    {
        private readonly IFileSystemRepositoryFactory _fileSystemRepositoryFactory;
        private readonly IBaseUnitOfWorkScopeFactory _baseUnitOfWorkScopeFactory;
        public IMapper Mapper { get; }

        public BaseBusinessService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory, IMapper mapper)
            : this(baseUnitOfWorkScopeFactory)
        {
            Mapper = mapper;
        }

        public BaseBusinessService(IFileSystemRepositoryFactory fileSystemRepositoryFactory, IMapper mapper)
            : this(fileSystemRepositoryFactory)
        {
            Mapper = mapper;
        }

        public BaseBusinessService(IFileSystemRepositoryFactory fileSystemRepositoryFactory)
        {
            if (fileSystemRepositoryFactory == null) throw new ArgumentNullException("fileSystemRepositoryFactory");
            _fileSystemRepositoryFactory = fileSystemRepositoryFactory;
        }

        public BaseBusinessService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        {
            if (baseUnitOfWorkScopeFactory == null) throw new ArgumentNullException("baseUnitOfWorkScopeFactory");
            _baseUnitOfWorkScopeFactory = baseUnitOfWorkScopeFactory;
        }

        public IFileSystemRepositoryFactory FileSytemRepositoryFactory
        {
            get
            {
                return _fileSystemRepositoryFactory;
            }
        }

        public IBaseUnitOfWorkScopeFactory UnitOfWorkFactory
        {
            get
            {
                return _baseUnitOfWorkScopeFactory;
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
