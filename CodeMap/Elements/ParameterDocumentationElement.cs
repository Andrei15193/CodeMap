using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.Elements
{
    /// <summary>Represents a documented parameter.</summary>
    public class ParameterDocumentationElement : DocumentationElement
    {
        internal ParameterDocumentationElement()
        {
        }

        /// <summary>The parameter name.</summary>
        public string Name { get; internal set; }

        /// <summary>The parameter type.</summary>
        public TypeReferenceDocumentationElement Type { get; internal set; }

        /// <summary>The parameter attributes.</summary>
        public IReadOnlyCollection<AttributeData> Attributes { get; internal set; }

        /// <summary>Indicates whether the parameter passed by reference and is input only (decorated with <c>in</c> in C#).</summary>
        public bool IsInputByReference { get; internal set; }

        /// <summary>Indicates whether the parameter passed by reference and is input and output (decorated with <c>ref</c> in C#).</summary>
        public bool IsInputOutputByReference { get; internal set; }

        /// <summary>Indicates whether the parameter passed by reference and is output only (decorated with <c>out</c> in C#).</summary>
        public bool IsOutputByReference { get; internal set; }

        /// <summary>Indicates whether the parameter has a default value.</summary>
        public bool HasDefaultValue { get; internal set; }

        /// <summary>The parameter default value</summary>
        /// <remarks>
        /// This property must be used in conjunction with <see cref="HasDefaultValue"/> as <c>null</c> can be a valid default value
        /// and therefore cannot be used to determine whether there is a default value.
        /// </remarks>
        public object DefaultValue { get; internal set; }

        /// <summary>The parameter description.</summary>
        public IReadOnlyList<BlockDocumentationElement> Description { get; internal set; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
        {
            throw new NotImplementedException();
        }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree asynchronously.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        public override Task AcceptAsync(DocumentationVisitor visitor, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}