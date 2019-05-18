using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.DocumentationElements
{
    /// <summary>Represents a collection of <see cref="BlockDocumentationElement"/> contained by an XML element.</summary>
    public sealed class BlockDescriptionDocumentationElement : DocumentationElement, IReadOnlyList<BlockDocumentationElement>
    {
        private readonly IReadOnlyList<BlockDocumentationElement> _blockElements;

        internal BlockDescriptionDocumentationElement(IEnumerable<BlockDocumentationElement> blockElements, IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _blockElements = blockElements
                .AsReadOnlyList()
                ?? throw new ArgumentNullException(nameof(blockElements));
            if (_blockElements.Contains(null))
                throw new ArgumentException("Cannot contain 'null' elements.", nameof(blockElements));

            XmlAttributes = xmlAttributes ?? new Dictionary<string, string>();
            if (XmlAttributes.Any(pair => pair.Value == null))
                throw new ArgumentException("Cannot contain 'null' values.", nameof(xmlAttributes));
        }

        /// <summary>The additional XML attributes on the containing element.</summary>
        public IReadOnlyDictionary<string, string> XmlAttributes { get; }

        /// <summary>Gets the <see cref="BlockDocumentationElement"/> at the specified index.</summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The <see cref="BlockDocumentationElement"/> at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index"/> is out of range.</exception>
        public BlockDocumentationElement this[int index]
        {
            get
            {
                try
                {
                    return _blockElements[index];
                }
                catch (ArgumentOutOfRangeException)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, new ArgumentOutOfRangeException().Message);
                }
            }
        }

        /// <summary>Gets the number of <see cref="BlockDocumentationElement"/>s in the collection.</summary>
        public int Count
            => _blockElements.Count;

        /// <summary>Gets an <see cref="IEnumerator{T}"/> that iterates through the <see cref="BlockDocumentationElement"/> collection.</summary>
        /// <returns>Returns an <see cref="IEnumerator{T}"/> that iterates through the <see cref="BlockDocumentationElement"/> collection.</returns>
        public IEnumerator<BlockDocumentationElement> GetEnumerator()
            => _blockElements.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => _blockElements.GetEnumerator();

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
        {
            foreach (var blockDocumentationElement in _blockElements)
                blockDocumentationElement.Accept(visitor);
        }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree asynchronously.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        public override async Task AcceptAsync(DocumentationVisitor visitor, CancellationToken cancellationToken)
        {
            foreach (var blockDocumentationElement in _blockElements)
                await blockDocumentationElement.AcceptAsync(visitor, cancellationToken);
        }
    }
}