using System;
using CodeMap.ReferenceData;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests.ReferenceData
{
    public class AssemblyReferenceTests : MemberReferenceTests
    {
        [Fact]
        public void CreateReferenceFromNullAssemblyThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("assembly", () => Factory.Create(assembly: null));
            Assert.Equal(new ArgumentNullException("assembly").Message, exception.Message);
        }

        [Fact]
        public void CreateReferenceFromNullAssemblyNameThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("assemblyName", () => Factory.Create(assemblyName: null));
            Assert.Equal(new ArgumentNullException("assemblyName").Message, exception.Message);
        }

        [Fact]
        public void CreateReferenceFromAssembly()
        {
            var assemblyReference = Factory.Create(typeof(TestClass<>).Assembly);
            var visitor = new MemberReferenceVisitorMock<AssemblyReference>(assemblyReference);

            Assert.Equal("CodeMap.Tests.Data", assemblyReference.Name);
            Assert.Equal(new Version(1, 2, 3, 4), assemblyReference.Version);
            Assert.Empty(assemblyReference.Culture);
#if DEBUG
            Assert.Empty(assemblyReference.PublicKeyToken);
#else
            Assert.Equal("4919ac5af74d53e8", assemblyReference.PublicKeyToken);
#endif
            Assert.Same(assemblyReference, assemblyReference.Assembly);

            assemblyReference.Accept(visitor);
            Assert.Equal(1, visitor.VisitCount);
        }

        [Fact]
        public void CreateReferenceFromAssemblyName()
        {
            var assemblyReference = Factory.Create(typeof(TestClass<>).Assembly.GetName());
            var visitor = new MemberReferenceVisitorMock<AssemblyReference>(assemblyReference);

            Assert.Equal("CodeMap.Tests.Data", assemblyReference.Name);
            Assert.Equal(new Version(1, 2, 3, 4), assemblyReference.Version);
            Assert.Empty(assemblyReference.Culture);
#if DEBUG
            Assert.Empty(assemblyReference.PublicKeyToken);
#else
            Assert.Equal("4919ac5af74d53e8", assemblyReference.PublicKeyToken);
#endif
            Assert.Same(assemblyReference, assemblyReference.Assembly);

            assemblyReference.Accept(visitor);
            Assert.Equal(1, visitor.VisitCount);
        }

        [Fact]
        public void EqualityComparison()
        {
            var assemblyName = typeof(GlobalTestClass).Assembly.GetName();
            var otherAssemblyName = typeof(MemberReferenceTests).Assembly.GetName();

            var assemblyReference = Factory.Create(assemblyName);
            var visitor = new MemberReferenceVisitorMock<AssemblyReference>(assemblyReference);

            Assert.True(assemblyReference == assemblyName);
            Assert.True(assemblyName == assemblyReference);
            Assert.True(assemblyReference != otherAssemblyName);
            Assert.True(otherAssemblyName != assemblyReference);

            assemblyReference.Accept(visitor);
            Assert.Equal(1, visitor.VisitCount);
        }
    }
}