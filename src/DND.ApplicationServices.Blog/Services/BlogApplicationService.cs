using AutoMapper;
using DND.Common.Implementation.ApplicationServices;
using DND.Common.Infrastructure.Users;
using DND.Interfaces.Blog.ApplicationServices;
using Microsoft.AspNetCore.Authorization;

namespace DND.ApplicationServices.Blog.Services
{
    public class BlogApplicationService : ApplicationServiceBase, IBlogApplicationService
    {
        public IBlogPostApplicationService BlogPostApplicationService { get; private set; }
        public ICategoryApplicationService CategoryApplicationService { get; private set; }
        public ITagApplicationService TagApplicationService { get; private set; }
        public IAuthorApplicationService AuthorApplicationService { get; private set; }

        public BlogApplicationService(IMapper mapper,
            IBlogPostApplicationService blogPostApplicationService,
            ICategoryApplicationService categoryApplicationService,
            ITagApplicationService tagApplicationService,
            IAuthorApplicationService authorApplicationService, 
            IAuthorizationService authorizationService,
            IUserService userService)
            : base("blog.", mapper, authorizationService, userService)
        {
            BlogPostApplicationService = blogPostApplicationService;
            CategoryApplicationService = categoryApplicationService;
            TagApplicationService = tagApplicationService;
            AuthorApplicationService = authorApplicationService;
        }

    }
}