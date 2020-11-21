using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeMap.ReferenceData;
using Xunit;

namespace CodeMap.Tests.ReferenceData
{
    public class PropertyReferenceTests : MemberReferenceTests
    {
        [Fact]
        public void CreateReferenceFromPropertyInfo()
        {
            var propertyReference = (PropertyReference)Factory.Create(_GetPropertyInfo());
            var visitor = new MemberReferenceVisitorMock<PropertyReference>(propertyReference);

            Assert.Equal("Item", propertyReference.Name);
            Assert.True(propertyReference.DeclaringType == typeof(IDictionary<string, string>));
            Assert.True(propertyReference
                .ParameterTypes
                .AsEnumerable<object>()
                .SequenceEqual(new[] { typeof(string) })
            );
            Assert.True(propertyReference == _GetPropertyInfo());

            propertyReference.Accept(visitor);
            Assert.Equal(1, visitor.VisitCount);

            PropertyInfo _GetPropertyInfo()
                => typeof(IDictionary<string, string>).GetDefaultMembers().OfType<PropertyInfo>().Single();
        }
    }
}