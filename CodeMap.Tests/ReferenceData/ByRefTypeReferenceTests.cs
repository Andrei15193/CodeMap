using CodeMap.ReferenceData;
using Xunit;

namespace CodeMap.Tests.ReferenceData
{
    public class ByRefTypeReferenceTests : MemberReferenceTests
    {
        [Fact]
        public void CreateReferenceFromByRefType()
        {
            var byRefTypeReference = (ByRefTypeReference)Factory.Create(typeof(int).MakeByRefType());
            var visitor = new MemberReferenceVisitorMock<ByRefTypeReference>(byRefTypeReference);

            Assert.True(byRefTypeReference.ReferentType == typeof(int));
            Assert.True(byRefTypeReference == typeof(int).MakeByRefType());
            Assert.True(byRefTypeReference != typeof(int));
            Assert.True(byRefTypeReference.Assembly == typeof(int).Assembly);

            byRefTypeReference.Accept(visitor);
            Assert.Equal(1, visitor.VisitCount);
        }
    }
}