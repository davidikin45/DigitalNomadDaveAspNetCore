using DND.Domain.Interfaces.Persistance;
using DND.Domain.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using DND.Common.Implementation.Repository.EntityFramework;
using DND.Common.Testing;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DND.Domain.Blog.BlogPosts;

namespace DND.UnitTests.Persistance.Repositories
{
    [TestClass]
    public class BlogPostRepositoryTests
    {
        private BaseEFRepository<IApplicationDbContext, BlogPost> _repository;
        private DbSet<BlogPost> _dbSet;
        private List<BlogPost> _list;

        [TestInitialize]
        public void TestInitialize()
        {
            _list = new List<BlogPost>();
            _dbSet = TestHelpers.MockDbSet<BlogPost>(_list);
            var mockContext = new Mock<IApplicationDbContext>();
            mockContext.SetupGet(c => c.BlogPosts).Returns(_dbSet);
            mockContext.Setup(c => c.BlogPosts).Returns(_dbSet);
            _repository = new BaseEFRepository<IApplicationDbContext, BlogPost>(mockContext.Object, true);
        }

        [TestMethod]
        public void GetAllRisks_All_ShouldReturnAll()
        {
            var post = new BlogPost();
            _list.Add(post);
            _list.Add(post);

            var result = _repository.GetCount();

            result.Should().Be(2);
        }
    }
}
