using System;
using CodeMap.ReferenceData;
using Xunit;

namespace CodeMap.Tests.ReferenceData
{
    public class NamespaceReferenceTests : MemberReferenceTests
    {
        [Fact]
        public void CreateNamespaceReferenceFromNullAssemblyThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("assembly", () => Factory.CreateNamespace(null, assembly: null));
            Assert.Equal(new ArgumentNullException("assembly").Message, exception.Message);
        }

        [Fact]
        public void CreateNamespaceReferenceFromNullAssemblyNameThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("assemblyName", () => Factory.CreateNamespace(null, assemblyName: null));
            Assert.Equal(new ArgumentNullException("assemblyName").Message, exception.Message);
        }

        [Fact]
        public void CreateNamespaceReferenceFromNameAndAssembly()
        {
            var namespaceReference = Factory.CreateNamespace("test-namespace", typeof(GlobalTestClass).Assembly);
            var visitor = new MemberReferenceVisitorMock<NamespaceReference>(namespaceReference);

            Assert.Equal("test-namespace", namespaceReference.Name);
            Assert.True(namespaceReference.Assembly == typeof(GlobalTestClass).Assembly);

            namespaceReference.Accept(visitor);
            Assert.Equal(1, visitor.VisitCount);
        }

        [Fact]
        public void CreateNamespaceReferenceFromNameAndAssemblyName()
        {
            var namespaceReference = Factory.CreateNamespace("test-namespace", typeof(GlobalTestClass).Assembly.GetName());
            var visitor = new MemberReferenceVisitorMock<NamespaceReference>(namespaceReference);

            Assert.Equal("test-namespace", namespaceReference.Name);
            Assert.True(namespaceReference.Assembly == typeof(GlobalTestClass).Assembly);

            namespaceReference.Accept(visitor);
            Assert.Equal(1, visitor.VisitCount);
        }

        [Fact]
        public void CreateNamespaceReferenceFromNullNameAndAssembly()
        {
            var namespaceReference = Factory.CreateNamespace(null, typeof(GlobalTestClass).Assembly);
            var visitor = new MemberReferenceVisitorMock<NamespaceReference>(namespaceReference);

            Assert.Empty(namespaceReference.Name);
            Assert.True(namespaceReference.Assembly == typeof(GlobalTestClass).Assembly);

            namespaceReference.Accept(visitor);
            Assert.Equal(1, visitor.VisitCount);
        }
    }
}