using CodeMap.ReferenceData;
using Xunit;

namespace CodeMap.Tests.ReferenceData
{
    public class ArrayTypeReferenceTests : MemberReferenceTests
    {
        [Fact]
        public void CreateReferenceFromArrayType()
        {
            var arrayTypeReference = (ArrayTypeReference)Factory.Create(typeof(decimal[][,]));
            var visitor = new MemberReferenceVisitorMock<ArrayTypeReference>(arrayTypeReference);

            Assert.Equal(1, arrayTypeReference.Rank);
            var itemArrayTypeReference = Assert.IsType<ArrayTypeReference>(arrayTypeReference.ItemType);
            Assert.Equal(2, itemArrayTypeReference.Rank);
            Assert.True(itemArrayTypeReference.ItemType == typeof(decimal));
            Assert.True(arrayTypeReference == typeof(decimal[][,]));
            Assert.True(arrayTypeReference != typeof(decimal[,][]));

            arrayTypeReference.Accept(visitor);
            Assert.Equal(1, visitor.VisitCount);
        }
    }
}