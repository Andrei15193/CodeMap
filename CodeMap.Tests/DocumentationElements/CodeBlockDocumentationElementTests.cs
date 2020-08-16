using System;
using CodeMap.DocumentationElements;
using Xunit;

namespace CodeMap.Tests.DocumentationElements
{
    public class CodeBlockDocumentationElementTests : DocumentationElementTests
    {
        [Fact]
        public void CreatingCodeBlockWithNullValueThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("code", () => DocumentationElement.CodeBlock(null));

            Assert.Equal(new ArgumentNullException("code").Message, exception.Message);
        }

        [Fact]
        public void CodeBlockElementCallsVisitorMethods()
        {
            var codeBlock = DocumentationElement.CodeBlock("code block");
            var visitor = new DocumentationVisitorMock<CodeBlockDocumentationElement>(codeBlock);

            codeBlock.Accept(visitor);

            Assert.Equal(1, visitor.VisitCount);
        }
    }
}