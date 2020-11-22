using System.Reflection;
using CodeMap.ReferenceData;
using Xunit;

namespace CodeMap.Tests.ReferenceData
{
    public class ConstantReferenceTests : MemberReferenceTests
    {
        [Fact]
        public void CreateReferenceFromConstantFieldInfo()
        {
            var constantReference = (ConstantReference)Factory.Create(_GetFieldInfo());
            var visitor = new MemberReferenceVisitorMock<ConstantReference>(constantReference);

            Assert.Equal("MaxValue", constantReference.Name);
            Assert.Equal(int.MaxValue, constantReference.Value);
            Assert.True(constantReference.DeclaringType == typeof(int));
            Assert.True(constantReference == _GetFieldInfo());
            Assert.True(constantReference.Assembly == typeof(int).Assembly);

            constantReference.Accept(visitor);
            Assert.Equal(1, visitor.VisitCount);

            FieldInfo _GetFieldInfo()
                => typeof(int).GetField(nameof(int.MaxValue));
        }
    }
}