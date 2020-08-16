using System;
using CodeMap.DocumentationElements;
using Xunit;

namespace CodeMap.Tests.DocumentationElements
{
    public class GenericParameterReferenceDocumentationElementTest : DocumentationElementTests
    {
        [Fact]
        public void CreatingGenericParameterReferenceElementWithNullValueThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("genericParameterName", () => DocumentationElement.GenericParameterReference(null));

            Assert.Equal(new ArgumentNullException("genericParameterName").Message, exception.Message);
        }

        [Fact]
        public void ParameterGenericReferenceElementCallsVisitorMethod()
        {
            var genericParameterReference = DocumentationElement.GenericParameterReference("genericParameter");
            var visitor = new DocumentationVisitorMock<GenericParameterReferenceDocumentationElement>(genericParameterReference);

            genericParameterReference.Accept(visitor);

            Assert.Equal(1, visitor.VisitCount);
        }
    }
}