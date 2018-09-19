using DND.Common.Data.Repository.GenericEF;
using DND.Common.Testing;
using DND.Data;
using DND.Domain.Blog.BlogPosts;
using FluentAssertions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace DND.UnitTests.Blog.Data.Repositories
{
    public class BlogPostRepositoryShould : IDisposable
    {
        private ApplicationContext _context;
        private GenericEFRepository<BlogPost> _repository;
        private readonly ITestOutputHelper _testOutputHelper;

        public BlogPostRepositoryShould(ITestOutputHelper testOutputHelper)
        {
            _context = TestHelper.GetContextInMemory<ApplicationContext>();     
            _repository = new GenericEFRepository<BlogPost>(_context);
            _testOutputHelper = testOutputHelper;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public async Task Add()
        {
            var post = new BlogPost() { Id = 1, Title = "Test Post" };
            _repository.Insert(post);

            await _context.SaveChangesAsync();

            _testOutputHelper.WriteLine(post.Id.ToString());

            var result = await _repository.GetByIdAsync(CancellationToken.None, 1);

            Assert.NotNull(result);

            result.Title.Should().Be("Test Post");
        }

        [Fact]
        public async Task AddThenUpdate()
        {
            var post = new BlogPost() { Id = 1, Title = "Test Post" };
            _repository.Insert(post);

            post.Title = "Test Post 2";
            _repository.Update(post);

            await _context.SaveChangesAsync();

            var count = await _repository.GetCountAsync(CancellationToken.None);

            count.Should().Be(1);
        }

        [Fact]
        public async Task AddThenDelete()
        {
            var post = new BlogPost() { Id = 1, Title = "Test Post" };
            _repository.Insert(post);

            _repository.Delete(post);

            await _context.SaveChangesAsync();

            var count = await _repository.GetCountAsync(CancellationToken.None);

            count.Should().Be(0);
        }

        [Fact]
        public async Task Update()
        {
            var post = new BlogPost() { Id = 1, Title = "Test Post" };
            _repository.Insert(post);

            await _context.SaveChangesAsync();

            var result = await _repository.GetByIdAsync(CancellationToken.None,1);

            Assert.NotNull(result);

            result.Title = "Test Post 2";

            _repository.Update(result);
            await _context.SaveChangesAsync();

            result = await _repository.GetByIdAsync(CancellationToken.None,1);

            Assert.NotNull(result);

            result.Title.Should().Be("Test Post 2");
        }

        [Fact]
        public async Task Delete()
        {
            var post = new BlogPost() { Id = 1, Title = "Test Post" };
            _repository.Insert(post);

            await _context.SaveChangesAsync();

            var result = await _repository.GetByIdAsync(CancellationToken.None,1);

            Assert.NotNull(result);

            _repository.Delete(result);
            await _context.SaveChangesAsync();

            var count = await _repository.GetCountAsync(CancellationToken.None);

            count.Should().Be(0);
        }

        [Fact]
        public async Task ReturnAllPostsForGetCount()
        {
            var post = new BlogPost() { Id = 1 };
            var post2 = new BlogPost() { Id = 2 };
            _repository.Insert(post);
            _repository.Insert(post2);

            await _context.SaveChangesAsync();

            var result = await _repository.GetCountAsync(CancellationToken.None);

            result.Should().Be(2);
        }
    }
}
