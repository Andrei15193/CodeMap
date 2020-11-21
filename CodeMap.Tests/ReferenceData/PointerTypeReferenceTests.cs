using CodeMap.ReferenceData;
using Xunit;

namespace CodeMap.Tests.ReferenceData
{
    public class PointerTypeReferenceTests : MemberReferenceTests
    {
        [Fact]
        public void CreateReferenceFromPointerType()
        {
            var pointerTypeReference = (PointerTypeReference)Factory.Create(typeof(int**));
            var visitor = new MemberReferenceVisitorMock<PointerTypeReference>(pointerTypeReference);

            var referentPointerTypeReference = Assert.IsType<PointerTypeReference>(pointerTypeReference.ReferentType);
            Assert.True(referentPointerTypeReference.ReferentType == typeof(int));
            Assert.True(referentPointerTypeReference == typeof(int*));
            Assert.True(referentPointerTypeReference != typeof(int));
            Assert.True(pointerTypeReference == typeof(int**));
            Assert.True(pointerTypeReference != typeof(int*));

            pointerTypeReference.Accept(visitor);
            Assert.Equal(1, visitor.VisitCount);
        }
    }
}