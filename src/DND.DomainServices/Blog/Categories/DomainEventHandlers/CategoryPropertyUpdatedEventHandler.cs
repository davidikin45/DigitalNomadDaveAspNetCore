using DND.Common.DomainEvents;
using DND.Common.Implementation.Validation;
using DND.Domain.Blog.BlogPosts;
using DND.Domain.Blog.Categories;
using DND.Domain.Blog.Tags;
using DND.Domain.Interfaces.DomainServices;
using System.Threading.Tasks;

namespace DND.DomainServices.Blog.BlogPosts.DomainEventHandlers
{
    public class CategoryPropertyUpdatedEventHandler : IDomainEventHandler<EntityPropertyUpdatedEvent<Category>>
    {
        private ITagDomainService _tagService;
        public CategoryPropertyUpdatedEventHandler(ITagDomainService tagService)
        {
            _tagService = tagService;
        }

        public async Task<Result> HandlePostCommitAsync(EntityPropertyUpdatedEvent<Category> domainEvent)
        {
            var after = domainEvent.Entity;
            if (domainEvent.PropertyName == "Name")
            {
                //var tag = new Tag() { Name = "Tag 1", Description = "Tag 2", UrlSlug = "tag-1" };
                //await _tagService.CreateAsync(tag, domainEvent.UpdatedBy).ConfigureAwait(false);
            }

            return Result.Ok();
        }

        public async Task<Result> HandlePreCommitAsync(EntityPropertyUpdatedEvent<Category> domainEvent)
        {
            var before = domainEvent.Entity;
            if(domainEvent.PropertyName == "Name")
            {
                //var tag = new Tag() { Name = "Tag 1", Description = "Tag 2", UrlSlug = "tag-1" };
                //await _tagService.CreateAsync(tag, domainEvent.UpdatedBy).ConfigureAwait(false);
            }

            return Result.Ok();
        }
    }
}
