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
        private BaseEFRepository<IApplicationDbContext, BlogPost> _repository;
        private DbSet<BlogPost> _dbSet;
        private List<BlogPost> _list;
       
        public BlogPostRepositoryShould()
        {
            _list = new List<BlogPost>();
            _dbSet = TestHelpers.MockDbSet<BlogPost>(_list);
            var mockContext = new Mock<IApplicationDbContext>();
            mockContext.SetupGet(c => c.BlogPosts).Returns(_dbSet);
            mockContext.Setup(c => c.BlogPosts).Returns(_dbSet);
            mockContext.Setup(c => c.Queryable<BlogPost>()).Returns(_dbSet);
            _repository = new BaseEFRepository<IApplicationDbContext, BlogPost>(mockContext.Object, true);
        }

        [Fact]
        public void ReturnAllRisksForGetCount()
        {
            var post = new BlogPost();
            _list.Add(post);
            _list.Add(post);

            var result = _repository.GetCount();

            result.Should().Be(2);
        }
    }
}
