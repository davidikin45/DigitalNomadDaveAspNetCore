using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Interfaces.Persistance;
using DND.Domain.Models;
using DND.Common.Implementation.ApplicationServices;
using DND.Common.Implementation.DomainServices;
using DND.Common.Infrastructure;
using DND.Common.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DND.DomainServices.Blog.Services
{
    public class BlogDomainService : BaseDomainService, IBlogDomainService
    {
        public IBlogPostDomainService BlogPostDomainService { get; private set; }
        public ICategoryDomainService CategoryDomainService { get; private set; }
        public ITagDomainService TagDomainService { get; private set; }
        public IAuthorDomainService AuthorDomainService { get; private set; }

        public BlogDomainService(
            IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory,
            IBlogPostDomainService blogPostDomainService,
            ICategoryDomainService categoryDomainService,
            ITagDomainService tagDomainService,
            IAuthorDomainService authorDomainService)
            : base(baseUnitOfWorkScopeFactory)
        {
            BlogPostDomainService = blogPostDomainService;
            CategoryDomainService = categoryDomainService;
            TagDomainService = tagDomainService;
            AuthorDomainService = authorDomainService;
        }   

    }
}