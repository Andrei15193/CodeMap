using System;
using System.Linq;
using System.Reflection;
using CodeMap.ReferenceData;
using Xunit;

namespace CodeMap.Tests.ReferenceData
{
    public class ConstructorReferenceTests : MemberReferenceTests
    {
        [Fact]
        public void CreateReferenceFromConstructorInfo()
        {
            var constructorReference = (ConstructorReference)Factory.Create(_GetConstructorInfo());
            var visitor = new MemberReferenceVisitorMock<ConstructorReference>(constructorReference);

            Assert.True(constructorReference.DeclaringType == typeof(string));
            Assert.True(constructorReference
                .ParameterTypes
                .AsEnumerable<object>()
                .SequenceEqual(new[] { typeof(char), typeof(int) })
            );
            Assert.True(constructorReference == _GetConstructorInfo());
            Assert.True(constructorReference.Assembly == typeof(string).Assembly);

            constructorReference.Accept(visitor);
            Assert.Equal(1, visitor.VisitCount);

            ConstructorInfo _GetConstructorInfo()
                => typeof(string).GetConstructor(new[] { typeof(char), typeof(int) });
        }

        [Fact]
        public void CreateDefaultConstructorReferenceFromNullTypeThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("type", () => Factory.CreateDefaultConstructor(null));
            Assert.Equal(new ArgumentNullException("type").Message, exception.Message);
        }

        [Fact]
        public void CreateDefaultConstructorReferenceFromEnumTypeThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>("type", () => Factory.CreateDefaultConstructor(typeof(StringComparison)));
            Assert.Equal(new ArgumentException("Default constructor references can only be created for structs (value types).", "type").Message, exception.Message);
        }

        [Fact]
        public void CreateDefaultConstructorReferenceFromReferenceTypeThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>("type", () => Factory.CreateDefaultConstructor(typeof(Func<>)));
            Assert.Equal(new ArgumentException("Default constructor references can only be created for structs (value types).", "type").Message, exception.Message);
        }

        [Fact]
        public void CreateDefaultConstructorReferenceFromType()
        {
            var constructorReference = Factory.CreateDefaultConstructor(typeof(DateTime));
            var visitor = new MemberReferenceVisitorMock<ConstructorReference>(constructorReference);

            Assert.True(constructorReference.DeclaringType == typeof(DateTime));
            Assert.Empty(constructorReference.ParameterTypes);
            Assert.True(constructorReference.Assembly == typeof(string).Assembly);

            constructorReference.Accept(visitor);
            Assert.Equal(1, visitor.VisitCount);
        }
    }
}