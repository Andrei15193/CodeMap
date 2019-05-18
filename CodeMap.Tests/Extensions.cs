using CodeMap.DocumentationElements;
using Moq;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CodeMap.Tests
{
    internal static class Extensions
    {
        public static Task VerifyAcceptMethods(this Mock<IDocumentationVisitor> visitorMock, DocumentationElement documentationElement, params Expression<Action<IDocumentationVisitor>>[] methodsToCheck)
            => visitorMock.VerifyAcceptMethods(
                documentationElement,
                methodsToCheck.Select(methodToCheck => new InvocationCheck(methodToCheck, Times.Once())).ToArray()
            );

        public static async Task VerifyAcceptMethods(this Mock<IDocumentationVisitor> visitorMock, DocumentationElement documentationElement, params InvocationCheck[] invocationChecks)
        {
            var adapter = new DocumentationVisitorAdapter(visitorMock.Object);

            await documentationElement.AcceptAsync(adapter);
            foreach (var invocationCheck in invocationChecks)
                visitorMock.Verify(invocationCheck.Method, invocationCheck.InvocationTimes);
            visitorMock.VerifyNoOtherCalls();

            visitorMock.Reset();

            documentationElement.Accept(adapter);
            foreach (var invocationCheck in invocationChecks)
                visitorMock.Verify(invocationCheck.Method, invocationCheck.InvocationTimes);
            visitorMock.VerifyNoOtherCalls();
        }
    }
}