using AutoMapper;
using DND.Common.Implementation.ApplicationServices;
using DND.Interfaces.Blog.ApplicationServices;

namespace DND.ApplicationServices.Blog.Services
{
    public class BlogApplicationService : BaseApplicationService, IBlogApplicationService
    {
        public IBlogPostApplicationService BlogPostApplicationService { get; private set; }
        public ICategoryApplicationService CategoryApplicationService { get; private set; }
        public ITagApplicationService TagApplicationService { get; private set; }
        public IAuthorApplicationService AuthorApplicationService { get; private set; }

        public BlogApplicationService(IMapper mapper,
            IBlogPostApplicationService blogPostApplicationService,
            ICategoryApplicationService categoryApplicationService,
            ITagApplicationService tagApplicationService,
            IAuthorApplicationService authorApplicationService)
            : base(mapper)
        {
            BlogPostApplicationService = blogPostApplicationService;
            CategoryApplicationService = categoryApplicationService;
            TagApplicationService = tagApplicationService;
            AuthorApplicationService = authorApplicationService;
        }

    }
}