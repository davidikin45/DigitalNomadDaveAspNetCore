using AutoMapper;
using DND.Common.Infrastructure.Helpers;
using DND.Common.Infrastructure.Interfaces.ApplicationServices;
using DND.Common.Interfaces.Repository;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace DND.Common.Implementation.ApplicationServices
{
    public abstract class ApplicationServiceBase : IApplicationService
    {
        private readonly IFileSystemGenericRepositoryFactory _fileSystemGenericRepositoryFactory;
        public IMapper Mapper { get; }

        public ApplicationServiceBase(IMapper mapper)
        {
            Mapper = mapper;
        }


        public ApplicationServiceBase(IFileSystemGenericRepositoryFactory fileSystemGenericRepositoryFactory, IMapper mapper)
            : this(fileSystemGenericRepositoryFactory)
        {
            Mapper = mapper;
        }

        public ApplicationServiceBase(IFileSystemGenericRepositoryFactory fileSystemGenericRepositoryFactory)
        {
            if (fileSystemGenericRepositoryFactory == null) throw new ArgumentNullException("fileSystemGenericRepositoryFactory");
            _fileSystemGenericRepositoryFactory = fileSystemGenericRepositoryFactory;
        }

        public ApplicationServiceBase()
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
            return AutoMapperHelper.GetMappedIncludes<TSource, TDestination>(Mapper, selectors);
        }

        public Expression<Func<TDestination, TProperty>> GetMappedSelector<TSource, TDestination, TProperty>(Expression<Func<TSource, TProperty>> selector)
        {
            return AutoMapperHelper.GetMappedSelector<TSource, TDestination, TProperty>(Mapper, selector);
        }

        public Func<IQueryable<TDestination>, IOrderedQueryable<TDestination>> GetMappedOrderBy<TSource, TDestination>(Expression<Func<IQueryable<TSource>, IOrderedQueryable<TSource>>> orderBy)
        {
            //return LamdaHelper.GetMappedOrderBy<TSource, TDestination>(Mapper, orderBy);
            if (orderBy == null)
                return null;

            return AutoMapperHelper.GetMappedOrderByCompiled<TSource, TDestination>(Mapper, orderBy);
        }
    }
}
