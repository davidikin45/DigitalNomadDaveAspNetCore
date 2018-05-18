using DND.Common.Implementation.Persistance.InMemory;
using DND.Common.Implementation.Repository.EntityFramework;
using DND.Common.Testing;
using DND.Domain.Blog.BlogPosts;
using DND.Domain.Interfaces.Persistance;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using System.Data.Entity;
using Xunit;

namespace DND.UnitTests.Persistance.Repositories
{
    public class BlogPostRepositoryShould
    {
        private InMemoryDataContext _context;
        private BaseEFRepository<BlogPost> _repository;
       
        public BlogPostRepositoryShould()
        {
            _context = new InMemoryDataContext();
            _repository = new BaseEFRepository<BlogPost>(_context, false);
        }

        [Fact]
        public void ReturnAllPostsForGetCount()
        {
            var post = new BlogPost();
            var post2 = new BlogPost();
            _repository.Create(post);
            _repository.Create(post2);

            _context.SaveChanges();

            var result = _repository.GetCount();

            result.Should().Be(2);
        }
    }
}
