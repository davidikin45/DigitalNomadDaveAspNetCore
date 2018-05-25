﻿using DND.Common.Implementation.Persistance.InMemory;
using DND.Common.Implementation.Repository;
using DND.Common.Implementation.Repository.EntityFramework;
using DND.Common.Implementation.UnitOfWork;
using DND.Common.Testing;
using DND.Domain.Blog.BlogPosts;
using DND.Domain.Interfaces.Persistance;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using Xunit;

namespace DND.UnitTests.Persistance.Repositories
{
    public class BlogPostRepositoryShould
    {
        private InMemoryDataContext _context;
        private GenericEFRepository<BlogPost> _repository;
       
        public BlogPostRepositoryShould()
        {
            _context = new InMemoryDataContext();
            var uowFactory = new UnitOfWorkScopeFactory(new FakeDbContextFactory(_context), new AmbientDbContextLocator(), new GenericRepositoryFactory());
            _repository = new GenericEFRepository<BlogPost>(_context, uowFactory.CreateReadOnly(), false);
        }

        [Fact]
        public async Task Add()
        {
            var post = new BlogPost() { Title = "Test Post" };
            _repository.Create(post);

            await _context.SaveChangesAsync();

            var result = await _repository.GetByIdAsync(1);

            Assert.NotNull(result);

            result.Title.Should().Be("Test Post");
        }

        [Fact]
        public async Task AddThenUpdate()
        {
            var post = new BlogPost() { Title = "Test Post" };
            _repository.Create(post);

            post.Title = "Test Post 2";
            _repository.Update(post);

            await _context.SaveChangesAsync();

            var count = await _repository.GetCountAsync();

            count.Should().Be(1);
        }

        [Fact]
        public async Task AddThenDelete()
        {
            var post = new BlogPost() { Title = "Test Post" };
            _repository.Create(post);

            _repository.Delete(post);

            await _context.SaveChangesAsync();

            var count = await _repository.GetCountAsync();

            count.Should().Be(0);
        }

        [Fact]
        public async Task Update()
        {
            var post = new BlogPost() { Title = "Test Post" };
            _repository.Create(post);

            await _context.SaveChangesAsync();

            var result = await _repository.GetByIdAsync(1);

            Assert.NotNull(result);

            result.Title = "Test Post 2";

            _repository.Update(result);
            await _context.SaveChangesAsync();

            result = await _repository.GetByIdAsync(1);

            Assert.NotNull(result);

            result.Title.Should().Be("Test Post 2");
        }

        [Fact]
        public async Task Delete()
        {
            var post = new BlogPost() { Title = "Test Post" };
            _repository.Create(post);

            await _context.SaveChangesAsync();

            var result = await _repository.GetByIdAsync(1);

            Assert.NotNull(result);

            _repository.Delete(result);
            await _context.SaveChangesAsync();

            var count = await _repository.GetCountAsync();

            count.Should().Be(0);
        }

        [Fact]
        public async Task ReturnAllPostsForGetCount()
        {
            var post = new BlogPost();
            var post2 = new BlogPost();
            _repository.Create(post);
            _repository.Create(post2);

            await _context.SaveChangesAsync();

            var result = await _repository.GetCountAsync();

            result.Should().Be(2);
        }
    }
}
