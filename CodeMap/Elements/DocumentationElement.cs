using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.Elements
{
    /// <summary>A documentation element that is part of the documentation tree for an <see cref="Assembly"/> and associated XML documentation.</summary>
    public abstract class DocumentationElement
    {
        /// <summary>Creates a <see cref="SummaryDocumentationElement"/> with the provided <paramref name="content"/>.</summary>
        /// <param name="content">The content of the <c>summary</c> XML element.</param>
        /// <returns>Returns a <see cref="SummaryDocumentationElement"/> with the provided <paramref name="content"/>.</returns>
        public static SummaryDocumentationElement Summary(IEnumerable<BlockDocumentationElement> content)
            => new SummaryDocumentationElement(content);

        /// <summary>Creates a <see cref="ReturnsDocumentationElement"/> with the provided <paramref name="returnType"/> and <paramref name="content"/>.</summary>
        /// <param name="returnType">The return type of the method.</param>
        /// <param name="content">The content of the <c>returns</c> XML element.</param>
        /// <returns>Returns a <see cref="ReturnsDocumentationElement"/> with the provided <paramref name="returnType"/> and <paramref name="content"/>.</returns>
        public static ReturnsDocumentationElement Returns(TypeReferenceData returnType, IEnumerable<BlockDocumentationElement> content)
            => new ReturnsDocumentationElement(returnType, content);

        /// <summary>Creates a <see cref="RemarksDocumentationElement"/> with the provided <paramref name="content"/>.</summary>
        /// <param name="content">The content of the <c>remarks</c> XML element.</param>
        /// <returns>Returns a <see cref="RemarksDocumentationElement"/> with the provided <paramref name="content"/>.</returns>
        public static RemarksDocumentationElement Remarks(IEnumerable<BlockDocumentationElement> content)
            => new RemarksDocumentationElement(content);

        /// <summary>Creates an <see cref="ExampleDocumentationElement"/> with the provided <paramref name="content"/>.</summary>
        /// <param name="content">The content of the <c>example</c> XML element.</param>
        /// <returns>Returns an <see cref="ExampleDocumentationElement"/> with the provided <paramref name="content"/>.</returns>
        public static ExampleDocumentationElement Example(IEnumerable<BlockDocumentationElement> content)
            => new ExampleDocumentationElement(content);

        /// <summary>Creates a <see cref="ValueDocumentationElement"/> with the provided <paramref name="content"/>.</summary>
        /// <param name="content">The content of the <c>value</c> XML element.</param>
        /// <returns>Returns a <see cref="ValueDocumentationElement"/> with the provided <paramref name="content"/>.</returns>
        public static ValueDocumentationElement Value(IEnumerable<BlockDocumentationElement> content)
            => new ValueDocumentationElement(content);

        /// <summary>Creates a <see cref="RelatedMembersList"/> for the provided <paramref name="relatedMembers"/>.</summary>
        /// <param name="relatedMembers">The referenced members using the <c>seealso</c> XML element.</param>
        /// <returns>Returns a <see cref="RelatedMembersList"/> for the provided <paramref name="relatedMembers"/>.</returns>
        public static RelatedMembersList RelatedMembersList(IEnumerable<MemberReferenceDocumentationElement> relatedMembers)
            => new RelatedMembersList(relatedMembers);

        /// <summary>Creates a <see cref="ParagraphDocumentationElement"/> with the provided <paramref name="content"/>.</summary>
        /// <param name="content">The content of the <c>para</c> XML element.</param>
        /// <returns>Returns a <see cref="ParagraphDocumentationElement"/> with the provided <paramref name="content"/>.</returns>
        public static ParagraphDocumentationElement Paragraph(IEnumerable<InlineDocumentationElement> content)
            => new ParagraphDocumentationElement(content);

        /// <summary>Creates a <see cref="CodeBlockDocumentationElement"/> for the provided <paramref name="code"/>.</summary>
        /// <param name="code">The code block inside the <c>code</c> element.</param>
        /// <returns>Returns a <see cref="CodeBlockDocumentationElement"/> for the provided <paramref name="code"/>.</returns>
        public static CodeBlockDocumentationElement CodeBlock(string code)
            => new CodeBlockDocumentationElement(code);

        /// <summary>Creates an <see cref="OrderedListDocumentationElement"/> with the provided <paramref name="items"/>.</summary>
        /// <param name="items">The list items inside the <c>list</c> XML element.</param>
        /// <returns>Returns an <see cref="OrderedListDocumentationElement"/> with the provided <paramref name="items"/>.</returns>
        public static OrderedListDocumentationElement OrderedList(IEnumerable<ListItemDocumentationElement> items)
            => new OrderedListDocumentationElement(items);

        /// <summary>Creates an <see cref="UnorderedListDocumentationElement"/> with the provided <paramref name="items"/>.</summary>
        /// <param name="items">The list items inside the <c>list</c> XML element.</param>
        /// <returns>Returns an <see cref="UnorderedListDocumentationElement"/> with the provided <paramref name="items"/>.</returns>
        public static UnorderedListDocumentationElement UnorderedList(IEnumerable<ListItemDocumentationElement> items)
            => new UnorderedListDocumentationElement(items);

        /// <summary>Creates a <see cref="ListItemDocumentationElement"/> with the provided <paramref name="content"/>.</summary>
        /// <param name="content">The content inside the <c>item</c> XML element.</param>
        /// <returns>Returns a <see cref="ListItemDocumentationElement"/> with the provided <paramref name="content"/>.</returns>
        public static ListItemDocumentationElement ListItem(IEnumerable<InlineDocumentationElement> content)
            => new ListItemDocumentationElement(content);

        /// <summary>Creates a <see cref="DefinitionListDocumentationElement"/> with the provided <paramref name="items"/>.</summary>
        /// <param name="items">The list items inside the <c>list</c> XML element.</param>
        /// <returns>Returns a <see cref="DefinitionListDocumentationElement"/> with the provided <paramref name="items"/>.</returns>
        public static DefinitionListDocumentationElement DefinitionList(IEnumerable<DefinitionListItemDocumentationElement> items)
            => new DefinitionListDocumentationElement(items);

        /// <summary>Creates a <see cref="DefinitionListDocumentationElement"/> with the provided <paramref name="items"/>.</summary>
        /// <param name="listTitle">The list title inside the <c>itemheader</c> XML element.</param>
        /// <param name="items">The list items inside the <c>list</c> XML element.</param>
        /// <returns>Returns a <see cref="DefinitionListDocumentationElement"/> with the provided <paramref name="items"/>.</returns>
        public static DefinitionListDocumentationElement DefinitionList(IEnumerable<InlineDocumentationElement> listTitle, IEnumerable<DefinitionListItemDocumentationElement> items)
            => new DefinitionListDocumentationElement(listTitle, items);

        /// <summary>Creates a <see cref="DefinitionListItemDocumentationElement"/> with the provided <paramref name="term"/> and <paramref name="description"/>.</summary>
        /// <param name="term">The content inside the <c>term</c> XML element of an <c>item</c> XML element.</param>
        /// <param name="description">The content inside the <c>description</c> XML element of an <c>item</c> XML element.</param>
        /// <returns>Returns a <see cref="DefinitionListItemDocumentationElement"/> with the provided <paramref name="term"/> and <paramref name="description"/>.</returns>
        public static DefinitionListItemDocumentationElement DefinitionListItem(IEnumerable<InlineDocumentationElement> term, IEnumerable<InlineDocumentationElement> description)
            => new DefinitionListItemDocumentationElement(term, description);

        /// <summary>Creates a <see cref="TableDocumentationElement"/> with the provided <paramref name="columns"/> and <paramref name="rows"/>.</summary>
        /// <param name="columns">The columns inside the <c>listheader</c> XML element of a <c>list</c> XML element.</param>
        /// <param name="rows">The rows corresponding to <c>item</c> XML elements of a <c>list</c> XML element.</param>
        /// <returns>Returns a <see cref="TableDocumentationElement"/> with the provided <paramref name="columns"/> and <paramref name="rows"/>.</returns>
        /// <remarks>
        /// <para>
        /// The returned table is normalized in the sense that if there were more columns or rows with missing cells they will be filled with
        /// empty ones so that the table has equal number of columns for each row, including the header.
        /// </para>
        /// </remarks>
        public static TableDocumentationElement Table(IEnumerable<TableColumnDocumentationElement> columns, IEnumerable<TableRowDocumentationElement> rows)
            => new TableDocumentationElement(columns, rows);

        /// <summary>Cretes a <see cref="TableCellDocumentationElement"/> for the provided <paramref name="name"/>.</summary>
        /// <param name="name">The content inside a <c>term</c> XML element inside the <c>listheader</c> XML element.</param>
        /// <returns>Returns a <see cref="TableCellDocumentationElement"/> for the provided <paramref name="name"/>.</returns>
        public static TableColumnDocumentationElement TableColumn(IEnumerable<InlineDocumentationElement> name)
            => new TableColumnDocumentationElement(name);

        /// <summary>Creates a <see cref="TableRowDocumentationElement"/> for the provided <paramref name="cells"/>.</summary>
        /// <param name="cells">The content corresponding to each <c>description</c> XML element inside an <c>item</c> XML element.</param>
        /// <returns>Returns a <see cref="TableRowDocumentationElement"/> for the provided <paramref name="cells"/>.</returns>
        public static TableRowDocumentationElement TableRow(IEnumerable<TableCellDocumentationElement> cells)
            => new TableRowDocumentationElement(cells);

        /// <summary>Creates a <see cref="TableCellDocumentationElement"/> with the provided <paramref name="content"/>.</summary>
        /// <param name="content">The content inside the <c>description</c> XML element of an <c>item</c> XML element.</param>
        /// <returns>Returns a <see cref="TableCellDocumentationElement"/> with the provided <paramref name="content"/>.</returns>
        public static TableCellDocumentationElement TableCell(IEnumerable<InlineDocumentationElement> content)
            => new TableCellDocumentationElement(content);

        /// <summary>Creates a <see cref="TableDocumentationElement"/> with the provided <paramref name="text"/>.</summary>
        /// <param name="text">Plain text inside XML elements.</param>
        /// <returns>Returns a <see cref="TableDocumentationElement"/> with the provided <paramref name="text"/>.</returns>
        public static TextDocumentationElement Text(string text)
            => new TextDocumentationElement(text);

        /// <summary>Creates an <see cref="InlineCodeDocumentationElement"/> with the provided <paramref name="code"/>.</summary>
        /// <param name="code">The code inside a <c>c</c> XML element.</param>
        /// <returns>Returns an <see cref="InlineCodeDocumentationElement"/> with the provided <paramref name="code"/>.</returns>
        public static InlineCodeDocumentationElement InlineCode(string code)
            => new InlineCodeDocumentationElement(code);

        /// <summary>Creates a <see cref="MemberInfoReferenceDocumentationElement"/> with the provided <paramref name="referredMember"/>.</summary>
        /// <param name="referredMember">The resolved <see cref="MemberInfo"/> referred by a canonical name using a <c>see</c> XML element.</param>
        /// <returns>Returns a <see cref="MemberInfoReferenceDocumentationElement"/> with the provided <paramref name="referredMember"/>.</returns>
        public static MemberInfoReferenceDocumentationElement MemberReference(MemberInfo referredMember)
            => new MemberInfoReferenceDocumentationElement(referredMember);

        /// <summary>Creates a <see cref="MemberNameReferenceDocumentationElement"/> with the provided <paramref name="canonicalName"/>.</summary>
        /// <param name="canonicalName">The canonical name for a member referred using a <c>see</c> XML element.</param>
        /// <returns>Returns a <see cref="MemberNameReferenceDocumentationElement"/> with the provided <paramref name="canonicalName"/>.</returns>
        public static MemberNameReferenceDocumentationElement MemberReference(string canonicalName)
            => new MemberNameReferenceDocumentationElement(canonicalName);

        /// <summary>Creates a <see cref="ParameterReferenceDocumentationElement"/> with the provided <paramref name="parameterName"/>.</summary>
        /// <param name="parameterName">The name of the referred parameter using the <c>paramref</c> XML element.</param>
        /// <returns>Returns a <see cref="ParameterReferenceDocumentationElement"/> with the provided <paramref name="parameterName"/>.</returns>
        public static ParameterReferenceDocumentationElement ParameterReference(string parameterName)
            => new ParameterReferenceDocumentationElement(parameterName);

        /// <summary>Creates a <see cref="GenericParameterReferenceDocumentationElement"/> with the provided <paramref name="genericParameterName"/>.</summary>
        /// <param name="genericParameterName">The name of the referred generic parameter using the <c>typeparamref</c> XML element.</param>
        /// <returns>Returns a <see cref="GenericParameterReferenceDocumentationElement"/> with the provided <paramref name="genericParameterName"/>.</returns>
        public static GenericParameterReferenceDocumentationElement GenericParameterReference(string genericParameterName)
            => new GenericParameterReferenceDocumentationElement(genericParameterName);

        internal DocumentationElement()
        {
        }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public abstract void Accept(DocumentationVisitor visitor);

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree asynchronously.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task AcceptAsync(DocumentationVisitor visitor)
            => AcceptAsync(visitor, CancellationToken.None);

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree asynchronously.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        public abstract Task AcceptAsync(DocumentationVisitor visitor, CancellationToken cancellationToken);
    }
}