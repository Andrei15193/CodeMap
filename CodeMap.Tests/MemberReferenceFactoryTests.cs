using CodeMap.ReferenceData;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace CodeMap.Tests
{
    public class MemberReferenceFactoryTests
    {
        private MemberReferenceFactory _Factory { get; } = new MemberReferenceFactory();

        [Fact]
        public void CreateFromAssembly_ReturnsAssemblyReference()
        {
            var assemblyReference = _Factory.Create(typeof(GlobalTestClass).Assembly);

            Assert.Equal("CodeMap.Tests.Data", assemblyReference.Name);
            Assert.Equal(new Version(1, 2, 3, 4), assemblyReference.Version);
            Assert.Empty(assemblyReference.Culture);
            Assert.Empty(assemblyReference.PublicKeyToken);
        }

        [Fact]
        public void CreateFromAssemblyName_ReturnsAssemblyReference()
        {
            var assemblyReference = _Factory.Create(typeof(GlobalTestClass).Assembly.GetName());

            Assert.Equal("CodeMap.Tests.Data", assemblyReference.Name);
            Assert.Equal(new Version(1, 2, 3, 4), assemblyReference.Version);
            Assert.Empty(assemblyReference.Culture);
            Assert.Empty(assemblyReference.PublicKeyToken);
        }

        [Fact]
        public void CreateFromNullAssembly_ThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("assembly", () => _Factory.Create(assembly: null));
            Assert.Equal(new ArgumentNullException("assembly").Message, exception.Message);
        }

        [Fact]
        public void CreateFromNullAssemblyName_ThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("assemblyName", () => _Factory.Create(assemblyName: null));
            Assert.Equal(new ArgumentNullException("assemblyName").Message, exception.Message);
        }
    }
}