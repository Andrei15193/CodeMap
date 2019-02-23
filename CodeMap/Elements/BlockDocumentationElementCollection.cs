using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.Elements
{
    /// <summary>Represents a collection of <see cref="BlockDocumentationElement"/> contained by an XML element.</summary>
    public sealed class BlockDocumentationElementCollection : DocumentationElement, IReadOnlyList<BlockDocumentationElement>
    {
        private readonly IReadOnlyList<BlockDocumentationElement> _blockDocumentationElements;

        /// <summary>Initializes a new instance of the <see cref="BlockDocumentationElementCollection"/> class.</summary>
        /// <param name="blockDocumentationElements">A collection of <see cref="BlockDocumentationElement"/>s to wrap.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="blockDocumentationElements"/> is <c>null</c>.</exception>
        public BlockDocumentationElementCollection(IEnumerable<BlockDocumentationElement> blockDocumentationElements)
            : this(blockDocumentationElements, new Dictionary<string, string>())
        {
        }

        /// <summary>Initializes a new instance of the <see cref="BlockDocumentationElementCollection"/> class.</summary>
        /// <param name="blockDocumentationElements">A collection of <see cref="BlockDocumentationElement"/>s to wrap.</param>
        /// <param name="xmlAttributes">A set of additional XML attributes.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="blockDocumentationElements"/> or <paramref name="xmlAttributes"/> are <c>null</c>.</exception>
        public BlockDocumentationElementCollection(IEnumerable<BlockDocumentationElement> blockDocumentationElements, IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _blockDocumentationElements = blockDocumentationElements
                .AsReadOnlyList()
                ?? throw new ArgumentNullException(nameof(blockDocumentationElements));
            XmlAttributes = xmlAttributes ?? throw new ArgumentNullException(nameof(xmlAttributes));
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
                    return _blockDocumentationElements[index];
                }
                catch (ArgumentOutOfRangeException)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, new ArgumentOutOfRangeException().Message);
                }
            }
        }

        /// <summary>Gets the number of <see cref="BlockDocumentationElement"/>s in the collection.</summary>
        public int Count
            => _blockDocumentationElements.Count;

        /// <summary>Gets an <see cref="IEnumerator"/> that iterates through the <see cref="BlockDocumentationElement"/> collection.</summary>
        /// <returns>Returns an <see cref="IEnumerator"/> that iterates through the <see cref="BlockDocumentationElement"/> collection.</returns>
        public IEnumerator<BlockDocumentationElement> GetEnumerator()
            => _blockDocumentationElements.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => _blockDocumentationElements.GetEnumerator();

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
        {
            foreach (var blockDocumentationElement in _blockDocumentationElements)
                blockDocumentationElement.Accept(visitor);
        }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree asynchronously.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        public override async Task AcceptAsync(DocumentationVisitor visitor, CancellationToken cancellationToken)
        {
            foreach (var blockDocumentationElement in _blockDocumentationElements)
                await blockDocumentationElement.AcceptAsync(visitor, cancellationToken);
        }
    }
}