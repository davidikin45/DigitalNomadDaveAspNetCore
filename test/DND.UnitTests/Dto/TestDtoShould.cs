using System;
using DND.Domain.Blog.Authors.Dtos;
using Xunit;

namespace DND.UnitTests.Dto
{
    public class TestDtoShould
    {
        [Fact]
        public void BeEqual()
        {
            //Arrange 
            var a = new TestDto() { A = "abc" };
            var b = new TestDto() { A = "abc" };

            //Act
            var isEqual = a.Equals(b);

            //Asset
            Assert.True(isEqual);
        }

        [Fact]
        public void BeEqual2()
        {
            //Arrange
            var a = new TestDto() { A = "abc" };
            var b = new TestDto() { A = "abc" };

            //Act
            var isEqual = a == b;

            //Asset
            Assert.True(isEqual);
        }
    }
}
