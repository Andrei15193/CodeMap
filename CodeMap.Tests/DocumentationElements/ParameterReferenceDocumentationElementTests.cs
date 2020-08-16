using System;
using CodeMap.DocumentationElements;
using Xunit;

namespace CodeMap.Tests.DocumentationElements
{
    public class ParameterReferenceDocumentationElementTests : DocumentationElementTests
    {
        [Fact]
        public void CreatingParameterReferenceElementWithNullValueThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("parameterName", () => DocumentationElement.ParameterReference(null));

            Assert.Equal(new ArgumentNullException("parameterName").Message, exception.Message);
        }

        [Fact]
        public void ParameterReferenceElementCallsVisitorMethod()
        {
            var parameterReference = DocumentationElement.ParameterReference("parameter");
            var visitor = new DocumentationVisitorMock<ParameterReferenceDocumentationElement>(parameterReference);

            parameterReference.Accept(visitor);

            Assert.Equal(1, visitor.VisitCount);
        }
    }
}