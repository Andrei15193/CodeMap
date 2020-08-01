using System;
using System.Linq;
using System.Linq.Expressions;
using CodeMap.DocumentationElements;
using Moq;

namespace CodeMap.Tests.DocumentationElements
{
    internal static class Extensions
    {
        public static void VerifyAcceptMethods(this Mock<IDocumentationVisitor> visitorMock, DocumentationElement documentationElement, params Expression<Action<IDocumentationVisitor>>[] methodsToCheck)
            => visitorMock.VerifyAcceptMethods(
                documentationElement,
                methodsToCheck.Select(methodToCheck => new InvocationCheck(methodToCheck, Times.Once())).ToArray()
            );

        public static void VerifyAcceptMethods(this Mock<IDocumentationVisitor> visitorMock, DocumentationElement documentationElement, params InvocationCheck[] invocationChecks)
        {
            documentationElement.Accept(new DocumentationVisitorAdapter(visitorMock.Object));

            foreach (var invocationCheck in invocationChecks)
                visitorMock.Verify(invocationCheck.Method, invocationCheck.InvocationTimes);
            visitorMock.VerifyNoOtherCalls();
        }
    }
}