using System.Dynamic;
using CodeMap.ReferenceData;
using Xunit;

namespace CodeMap.Tests.ReferenceData
{
    public class DynamicTypeReferenceTests : MemberReferenceTests
    {
        [Fact]
        public void CreateDynamicTypeReference()
        {
            var typeReference = Factory.CreateDynamic();
            var visitor = new MemberReferenceVisitorMock<TypeReference>(typeReference);

            Assert.IsType<DynamicTypeReference>(typeReference);
            Assert.True(typeReference == typeof(object));
            Assert.True(typeReference == typeof(IDynamicMetaObjectProvider));
            Assert.True(typeReference == typeof(DynamicObject));
            Assert.True(typeReference != typeof(int));

            typeReference.Accept(visitor);
            Assert.Equal(1, visitor.VisitCount);
        }
    }
}