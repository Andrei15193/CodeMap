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
    }
}