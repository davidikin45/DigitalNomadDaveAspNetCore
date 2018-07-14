using AutoMapper;
using AutoMapper.Configuration.Internal;
using AutoMapper.Extensions.ExpressionMapping;
using AutoMapper.Internal;
using AutoMapper.Mappers.Internal;
using DND.Common.Helpers;
using DND.Common.Interfaces.ApplicationServices;
using DND.Common.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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
            return LamdaHelper.GetMappedIncludes<TSource, TDestination>(Mapper, selectors);
        }

        public Expression<Func<TDestination, TProperty>> GetMappedSelector<TSource, TDestination, TProperty>(Expression<Func<TSource, TProperty>> selector)
        {
            return LamdaHelper.GetMappedSelector<TSource, TDestination, TProperty>(Mapper, selector);
        }

        public Func<IQueryable<TDestination>, IOrderedQueryable<TDestination>> GetMappedOrderBy<TSource, TDestination>(Expression<Func<IQueryable<TSource>, IOrderedQueryable<TSource>>> orderBy)
        {
            //return LamdaHelper.GetMappedOrderBy<TSource, TDestination>(Mapper, orderBy);
            if (orderBy == null)
                return null;

            return LamdaHelper.GetMappedOrderByCompiled<TSource, TDestination>(Mapper, orderBy);
        }
    }
}
