using System.Reflection;
using CodeMap.ReferenceData;
using Xunit;

namespace CodeMap.Tests.ReferenceData
{
    public class FieldReferenceTests : MemberReferenceTests
    {
        [Fact]
        public void CreateReferenceFromFieldInfo()
        {
            var fieldReference = (FieldReference)Factory.Create(_GetFieldInfo());
            var visitor = new MemberReferenceVisitorMock<FieldReference>(fieldReference);

            Assert.Equal("Empty", fieldReference.Name);
            Assert.True(fieldReference.DeclaringType == typeof(string));
            Assert.True(fieldReference == _GetFieldInfo());

            fieldReference.Accept(visitor);
            Assert.Equal(1, visitor.VisitCount);

            FieldInfo _GetFieldInfo()
                => typeof(string).GetField(nameof(string.Empty));
        }
    }
}