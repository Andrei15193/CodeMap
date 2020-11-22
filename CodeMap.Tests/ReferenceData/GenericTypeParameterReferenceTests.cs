using System;
using System.Collections.Generic;
using System.Linq;
using CodeMap.ReferenceData;
using Xunit;

namespace CodeMap.Tests.ReferenceData
{
    public class GenericTypeParameterReferenceTests : MemberReferenceTests
    {
        [Fact]
        public void CreateReferenceFromGenericTypeParameterReferenceType()
        {
            var genericParameterReference = (GenericTypeParameterReference)Factory.Create(_GetGenericTypeParameter());
            var visitor = new MemberReferenceVisitorMock<GenericTypeParameterReference>(genericParameterReference);

            Assert.Equal("T", genericParameterReference.Name);
            Assert.Equal(0, genericParameterReference.Position);
            Assert.True(genericParameterReference.DeclaringType == typeof(IEnumerable<>));
            Assert.True(genericParameterReference == _GetGenericTypeParameter());
            Assert.True(genericParameterReference.Assembly == typeof(IEnumerable<>).Assembly);

            genericParameterReference.Accept(visitor);
            Assert.Equal(1, visitor.VisitCount);

            Type _GetGenericTypeParameter()
                => typeof(IEnumerable<>).GetGenericArguments().Single();
        }
    }
}