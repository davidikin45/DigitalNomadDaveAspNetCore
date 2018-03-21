using System;
using DND.Domain.Blog.Authors.Dtos;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DND.UnitTests.Dto
{
    [TestClass]
    public class DtoTests
    {
        [TestMethod]
        public void DtosAreEqual()
        {
            //Arrange
            var a = new TestDto() { A = "abc" };
            var b = new TestDto() { A = "abc" };

            //Act
            var isEqual = a.Equals(b);

            //Asset
            Assert.IsTrue(isEqual);
        }

        [TestMethod]
        public void DtosAreEqual2()
        {
            //Arrange
            var a = new TestDto() { A = "abc" };
            var b = new TestDto() { A = "abc" };

            //Act
            var isEqual = a == b;
            //Asset
            Assert.IsTrue(isEqual);
        }
    }
}
