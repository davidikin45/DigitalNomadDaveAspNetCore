using AutoMapper;
using DND.Common.Infrastructure;
using DND.Common.Infrastructure.Helpers;
using DND.Common.Infrastructure.Interfaces.ApplicationServices;
using DND.Common.Infrastructure.Users;
using DND.Common.Infrastructure.Validation.Errors;
using DND.Common.Interfaces.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace DND.Common.Implementation.ApplicationServices
{
    public abstract class ApplicationServiceBase : IApplicationService
    {
        private readonly IFileSystemGenericRepositoryFactory _fileSystemGenericRepositoryFactory;
        public IMapper Mapper { get; }

        public string ServiceName { get; }
        public IAuthorizationService AuthorizationService { get; }
        public IUserService UserService { get; }

        public ApplicationServiceBase(string serviceName, IMapper mapper, IAuthorizationService authorizationService, IUserService userService)
        {
            ServiceName = serviceName;
            Mapper = mapper;
            AuthorizationService = authorizationService;
            UserService = userService;
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

        public async void AuthorizeOperationAsync(string operation)
        {
            var authorizationResult = await AuthorizationService.AuthorizeAsync(UserService.User, ServiceName + operation);
            if (!authorizationResult.Succeeded)
            {
                throw new UnauthorizedErrors(new GeneralError(String.Format(Messages.UnauthorisedServiceOperation, ServiceName + operation)));
            }
        }

        public async void AuthorizeResourceOperationAsync(string operation, object resource)
        {
            var authorizationResult = await AuthorizationService.AuthorizeAsync(UserService.User, resource, new OperationAuthorizationRequirement() { Name = ServiceName + operation });
            if (!authorizationResult.Succeeded)
            {
                throw new UnauthorizedErrors(new GeneralError(String.Format(Messages.UnauthorisedServiceOperation, ServiceName + operation)));
            }
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
