using System;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.Elements
{
    /// <summary>Represents a code block element corresponding to the <c>code</c> XML element.</summary>
    public sealed class CodeBlockDocumentationElement : BlockDocumentationElement
    {
        internal CodeBlockDocumentationElement(string code)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
        }

        /// <summary>The code inside the code block element.</summary>
        public string Code { get; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
            => visitor.VisitCodeBlock(Code);

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree asynchronously.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        public override Task AcceptAsync(DocumentationVisitor visitor, CancellationToken cancellationToken)
            => visitor.VisitCodeBlockAsync(Code, cancellationToken);
    }
}