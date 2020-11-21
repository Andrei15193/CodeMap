using CodeMap.ReferenceData;
using Xunit;

namespace CodeMap.Tests.ReferenceData
{
    public class VoidTypeReferenceTests : MemberReferenceTests
    {
        [Fact]
        public void CreateReferenceFromVoidType()
        {
            var typeReference = (VoidTypeReference)Factory.Create(typeof(void));
            var visitor = new MemberReferenceVisitorMock<TypeReference>(typeReference);

            Assert.IsType<VoidTypeReference>(typeReference);
            Assert.True(typeReference == typeof(void));
            Assert.True(typeReference != typeof(int));

            typeReference.Accept(visitor);
            Assert.Equal(1, visitor.VisitCount);
        }
    }
}