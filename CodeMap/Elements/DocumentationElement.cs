using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.Elements
{
    /// <summary>A documentation element that is part of the documentation tree for an <see cref="System.Reflection.Assembly"/> and associated XML documentation.</summary>
    public abstract class DocumentationElement
    {
        /// <summary>Creates a <see cref="SummaryDocumentationElement"/> with the provided <paramref name="content"/>.</summary>
        /// <param name="content">The content of the <c>summary</c> XML element.</param>
        /// <returns>Returns a <see cref="SummaryDocumentationElement"/> with the provided <paramref name="content"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="content"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="content"/> contains <c>null</c> elements.</exception>
        public static SummaryDocumentationElement Summary(IEnumerable<BlockDocumentationElement> content)
            => new SummaryDocumentationElement(content, null);

        /// <summary>Creates a <see cref="SummaryDocumentationElement"/> with the provided <paramref name="content"/>.</summary>
        /// <param name="content">The content of the <c>summary</c> XML element.</param>
        /// <returns>Returns a <see cref="SummaryDocumentationElement"/> with the provided <paramref name="content"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="content"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="content"/> contains <c>null</c> elements.</exception>
        public static SummaryDocumentationElement Summary(params BlockDocumentationElement[] content)
            => new SummaryDocumentationElement(content, null);

        /// <summary>Creates a <see cref="SummaryDocumentationElement"/> with the provided <paramref name="content"/>.</summary>
        /// <param name="content">The content of the <c>summary</c> XML element.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the summary element.</param>
        /// <returns>Returns a <see cref="SummaryDocumentationElement"/> with the provided <paramref name="content"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="content"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="content"/> or <paramref name="xmlAttributes"/> contain <c>null</c> values.</exception>
        public static SummaryDocumentationElement Summary(IEnumerable<BlockDocumentationElement> content, IReadOnlyDictionary<string, string> xmlAttributes)
            => new SummaryDocumentationElement(content, xmlAttributes);

        /// <summary>Creates a <see cref="RemarksDocumentationElement"/> with the provided <paramref name="content"/>.</summary>
        /// <param name="content">The content of the <c>remarks</c> XML element.</param>
        /// <returns>Returns a <see cref="RemarksDocumentationElement"/> with the provided <paramref name="content"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="content"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="content"/> contains <c>null</c> elements.</exception>
        public static RemarksDocumentationElement Remarks(IEnumerable<BlockDocumentationElement> content)
            => new RemarksDocumentationElement(content, null);

        /// <summary>Creates a <see cref="RemarksDocumentationElement"/> with the provided <paramref name="content"/>.</summary>
        /// <param name="content">The content of the <c>remarks</c> XML element.</param>
        /// <returns>Returns a <see cref="RemarksDocumentationElement"/> with the provided <paramref name="content"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="content"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="content"/> contains <c>null</c> elements.</exception>
        public static RemarksDocumentationElement Remarks(params BlockDocumentationElement[] content)
            => new RemarksDocumentationElement(content, null);

        /// <summary>Creates a <see cref="RemarksDocumentationElement"/> with the provided <paramref name="content"/>.</summary>
        /// <param name="content">The content of the <c>remarks</c> XML element.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the remarks element.</param>
        /// <returns>Returns a <see cref="RemarksDocumentationElement"/> with the provided <paramref name="content"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="content"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="content"/> or <paramref name="xmlAttributes"/> contain <c>null</c> values.</exception>
        public static RemarksDocumentationElement Remarks(IEnumerable<BlockDocumentationElement> content, IReadOnlyDictionary<string, string> xmlAttributes)
            => new RemarksDocumentationElement(content, xmlAttributes);

        /// <summary>Creates an <see cref="ExampleDocumentationElement"/> with the provided <paramref name="content"/>.</summary>
        /// <param name="content">The content of the <c>example</c> XML element.</param>
        /// <returns>Returns an <see cref="ExampleDocumentationElement"/> with the provided <paramref name="content"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="content"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="content"/> contains <c>null</c> elements.</exception>
        public static ExampleDocumentationElement Example(IEnumerable<BlockDocumentationElement> content)
            => new ExampleDocumentationElement(content, null);

        /// <summary>Creates an <see cref="ExampleDocumentationElement"/> with the provided <paramref name="content"/>.</summary>
        /// <param name="content">The content of the <c>example</c> XML element.</param>
        /// <returns>Returns an <see cref="ExampleDocumentationElement"/> with the provided <paramref name="content"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="content"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="content"/> contains <c>null</c> elements.</exception>
        public static ExampleDocumentationElement Example(params BlockDocumentationElement[] content)
            => new ExampleDocumentationElement(content, null);

        /// <summary>Creates an <see cref="ExampleDocumentationElement"/> with the provided <paramref name="content"/>.</summary>
        /// <param name="content">The content of the <c>example</c> XML element.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the example element.</param>
        /// <returns>Returns an <see cref="ExampleDocumentationElement"/> with the provided <paramref name="content"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="content"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="content"/> or <paramref name="xmlAttributes"/> contain <c>null</c> values.</exception>
        public static ExampleDocumentationElement Example(IEnumerable<BlockDocumentationElement> content, IReadOnlyDictionary<string, string> xmlAttributes)
            => new ExampleDocumentationElement(content, xmlAttributes);

        /// <summary>Creates a <see cref="ValueDocumentationElement"/> with the provided <paramref name="content"/>.</summary>
        /// <param name="content">The content of the <c>value</c> XML element.</param>
        /// <returns>Returns a <see cref="ValueDocumentationElement"/> with the provided <paramref name="content"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="content"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="content"/> contains <c>null</c> elements.</exception>
        public static ValueDocumentationElement Value(IEnumerable<BlockDocumentationElement> content)
            => new ValueDocumentationElement(content, null);

        /// <summary>Creates a <see cref="ValueDocumentationElement"/> with the provided <paramref name="content"/>.</summary>
        /// <param name="content">The content of the <c>value</c> XML element.</param>
        /// <returns>Returns a <see cref="ValueDocumentationElement"/> with the provided <paramref name="content"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="content"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="content"/> contains <c>null</c> elements.</exception>
        public static ValueDocumentationElement Value(params BlockDocumentationElement[] content)
            => new ValueDocumentationElement(content, null);

        /// <summary>Creates a <see cref="ValueDocumentationElement"/> with the provided <paramref name="content"/>.</summary>
        /// <param name="content">The content of the <c>value</c> XML element.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the value element.</param>
        /// <returns>Returns a <see cref="ValueDocumentationElement"/> with the provided <paramref name="content"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="content"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="content"/> or <paramref name="xmlAttributes"/> contain <c>null</c> values.</exception>
        public static ValueDocumentationElement Value(IEnumerable<BlockDocumentationElement> content, IReadOnlyDictionary<string, string> xmlAttributes)
            => new ValueDocumentationElement(content, xmlAttributes);

        /// <summary>Creates a <see cref="BlockDescriptionDocumentationElement"/> with the provided <paramref name="blockElements"/>.</summary>
        /// <param name="blockElements">The <see cref="BlockDocumentationElement"/>s to wrap.</param>
        /// <returns>Returns a <see cref="BlockDescriptionDocumentationElement"/> with the provided <paramref name="blockElements"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="blockElements"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="blockElements"/> contain <c>null</c> elements.</exception>
        public static BlockDescriptionDocumentationElement BlockDescription(IEnumerable<BlockDocumentationElement> blockElements)
            => new BlockDescriptionDocumentationElement(blockElements, null);

        /// <summary>Creates a <see cref="BlockDescriptionDocumentationElement"/> with the provided <paramref name="blockElements"/>.</summary>
        /// <param name="blockElements">The <see cref="BlockDocumentationElement"/>s to wrap.</param>
        /// <returns>Returns a <see cref="BlockDescriptionDocumentationElement"/> with the provided <paramref name="blockElements"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="blockElements"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="blockElements"/> contain <c>null</c> elements.</exception>
        public static BlockDescriptionDocumentationElement BlockDescription(params BlockDocumentationElement[] blockElements)
            => new BlockDescriptionDocumentationElement(blockElements, null);

        /// <summary>Creates a <see cref="BlockDescriptionDocumentationElement"/> with the provided <paramref name="blockElements"/>.</summary>
        /// <param name="blockElements">The <see cref="BlockDocumentationElement"/>s to wrap.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the container element.</param>
        /// <returns>Returns a <see cref="BlockDescriptionDocumentationElement"/> with the provided <paramref name="blockElements"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="blockElements"/> or <paramref name="xmlAttributes"/> are <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="blockElements"/> or <paramref name="xmlAttributes"/> contain <c>null</c> values.</exception>
        public static BlockDescriptionDocumentationElement BlockDescription(IEnumerable<BlockDocumentationElement> blockElements, IReadOnlyDictionary<string, string> xmlAttributes)
            => new BlockDescriptionDocumentationElement(blockElements, xmlAttributes);

        /// <summary>Creates a <see cref="ParagraphDocumentationElement"/> with the provided <paramref name="content"/>.</summary>
        /// <param name="content">The content of the <c>para</c> XML element.</param>
        /// <returns>Returns a <see cref="ParagraphDocumentationElement"/> with the provided <paramref name="content"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="content"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="content"/> contains <c>null</c> elements.</exception>
        public static ParagraphDocumentationElement Paragraph(IEnumerable<InlineDocumentationElement> content)
            => new ParagraphDocumentationElement(content, null);

        /// <summary>Creates a <see cref="ParagraphDocumentationElement"/> with the provided <paramref name="content"/>.</summary>
        /// <param name="content">The content of the <c>para</c> XML element.</param>
        /// <returns>Returns a <see cref="ParagraphDocumentationElement"/> with the provided <paramref name="content"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="content"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="content"/> contains <c>null</c> elements.</exception>
        public static ParagraphDocumentationElement Paragraph(params InlineDocumentationElement[] content)
            => new ParagraphDocumentationElement(content, null);

        /// <summary>Creates a <see cref="ParagraphDocumentationElement"/> with the provided <paramref name="content"/>.</summary>
        /// <param name="content">The content of the <c>para</c> XML element.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the paragraph element.</param>
        /// <returns>Returns a <see cref="ParagraphDocumentationElement"/> with the provided <paramref name="content"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="content"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="content"/> or <paramref name="xmlAttributes"/> contain <c>null</c> values.</exception>
        public static ParagraphDocumentationElement Paragraph(IEnumerable<InlineDocumentationElement> content, IReadOnlyDictionary<string, string> xmlAttributes)
            => new ParagraphDocumentationElement(content, xmlAttributes);

        /// <summary>Creates a <see cref="CodeBlockDocumentationElement"/> for the provided <paramref name="code"/>.</summary>
        /// <param name="code">The code block inside the <c>code</c> element.</param>
        /// <returns>Returns a <see cref="CodeBlockDocumentationElement"/> for the provided <paramref name="code"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="code"/> is <c>null</c>.</exception>
        public static CodeBlockDocumentationElement CodeBlock(string code)
            => new CodeBlockDocumentationElement(code, null);

        /// <summary>Creates a <see cref="CodeBlockDocumentationElement"/> for the provided <paramref name="code"/>.</summary>
        /// <param name="code">The code block inside the <c>code</c> element.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the code block element.</param>
        /// <returns>Returns a <see cref="CodeBlockDocumentationElement"/> for the provided <paramref name="code"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="code"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="xmlAttributes"/> contain <c>null</c> values.</exception>
        public static CodeBlockDocumentationElement CodeBlock(string code, IReadOnlyDictionary<string, string> xmlAttributes)
            => new CodeBlockDocumentationElement(code, xmlAttributes);

        /// <summary>Creates an <see cref="OrderedListDocumentationElement"/> with the provided <paramref name="items"/>.</summary>
        /// <param name="items">The list items inside the <c>list</c> XML element.</param>
        /// <returns>Returns an <see cref="OrderedListDocumentationElement"/> with the provided <paramref name="items"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="items"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="items"/> contains <c>null</c> elements.</exception>
        public static OrderedListDocumentationElement OrderedList(IEnumerable<ListItemDocumentationElement> items)
            => new OrderedListDocumentationElement(items, null);

        /// <summary>Creates an <see cref="OrderedListDocumentationElement"/> with the provided <paramref name="items"/>.</summary>
        /// <param name="items">The list items inside the <c>list</c> XML element.</param>
        /// <returns>Returns an <see cref="OrderedListDocumentationElement"/> with the provided <paramref name="items"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="items"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="items"/> contains <c>null</c> elements.</exception>
        public static OrderedListDocumentationElement OrderedList(params ListItemDocumentationElement[] items)
            => new OrderedListDocumentationElement(items, null);

        /// <summary>Creates an <see cref="OrderedListDocumentationElement"/> with the provided <paramref name="items"/>.</summary>
        /// <param name="items">The list items inside the <c>list</c> XML element.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the ordered list element.</param>
        /// <returns>Returns an <see cref="OrderedListDocumentationElement"/> with the provided <paramref name="items"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="items"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="items"/> or <paramref name="xmlAttributes"/> contain <c>null</c> values.</exception>
        public static OrderedListDocumentationElement OrderedList(IEnumerable<ListItemDocumentationElement> items, IReadOnlyDictionary<string, string> xmlAttributes)
            => new OrderedListDocumentationElement(items, xmlAttributes);

        /// <summary>Creates an <see cref="UnorderedListDocumentationElement"/> with the provided <paramref name="items"/>.</summary>
        /// <param name="items">The list items inside the <c>list</c> XML element.</param>
        /// <returns>Returns an <see cref="UnorderedListDocumentationElement"/> with the provided <paramref name="items"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="items"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="items"/> contains <c>null</c> elements.</exception>
        public static UnorderedListDocumentationElement UnorderedList(IEnumerable<ListItemDocumentationElement> items)
            => new UnorderedListDocumentationElement(items, null);

        /// <summary>Creates an <see cref="UnorderedListDocumentationElement"/> with the provided <paramref name="items"/>.</summary>
        /// <param name="items">The list items inside the <c>list</c> XML element.</param>
        /// <returns>Returns an <see cref="UnorderedListDocumentationElement"/> with the provided <paramref name="items"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="items"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="items"/> contains <c>null</c> elements.</exception>
        public static UnorderedListDocumentationElement UnorderedList(params ListItemDocumentationElement[] items)
            => new UnorderedListDocumentationElement(items, null);

        /// <summary>Creates an <see cref="UnorderedListDocumentationElement"/> with the provided <paramref name="items"/>.</summary>
        /// <param name="items">The list items inside the <c>list</c> XML element.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the unordered list element.</param>
        /// <returns>Returns an <see cref="UnorderedListDocumentationElement"/> with the provided <paramref name="items"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="items"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="items"/> or <paramref name="xmlAttributes"/> contain <c>null</c> values.</exception>
        public static UnorderedListDocumentationElement UnorderedList(IEnumerable<ListItemDocumentationElement> items, IReadOnlyDictionary<string, string> xmlAttributes)
            => new UnorderedListDocumentationElement(items, xmlAttributes);

        /// <summary>Creates a <see cref="ListItemDocumentationElement"/> with the provided <paramref name="content"/>.</summary>
        /// <param name="content">The content inside the <c>item</c> XML element.</param>
        /// <returns>Returns a <see cref="ListItemDocumentationElement"/> with the provided <paramref name="content"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="content"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="content"/> contains <c>null</c> elements.</exception>
        public static ListItemDocumentationElement ListItem(IEnumerable<InlineDocumentationElement> content)
            => new ListItemDocumentationElement(content, null);

        /// <summary>Creates a <see cref="ListItemDocumentationElement"/> with the provided <paramref name="content"/>.</summary>
        /// <param name="content">The content inside the <c>item</c> XML element.</param>
        /// <returns>Returns a <see cref="ListItemDocumentationElement"/> with the provided <paramref name="content"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="content"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="content"/> contains <c>null</c> elements.</exception>
        public static ListItemDocumentationElement ListItem(params InlineDocumentationElement[] content)
            => new ListItemDocumentationElement(content, null);

        /// <summary>Creates a <see cref="ListItemDocumentationElement"/> with the provided <paramref name="content"/>.</summary>
        /// <param name="content">The content inside the <c>item</c> XML element.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the list item element.</param>
        /// <returns>Returns a <see cref="ListItemDocumentationElement"/> with the provided <paramref name="content"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="content"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="content"/> or <paramref name="xmlAttributes"/> contain <c>null</c> values.</exception>
        public static ListItemDocumentationElement ListItem(IEnumerable<InlineDocumentationElement> content, IReadOnlyDictionary<string, string> xmlAttributes)
            => new ListItemDocumentationElement(content, xmlAttributes);

        /// <summary>Creates a <see cref="DefinitionListDocumentationElement"/> with the provided <paramref name="items"/>.</summary>
        /// <param name="items">The list items inside the <c>list</c> XML element.</param>
        /// <returns>Returns a <see cref="DefinitionListDocumentationElement"/> with the provided <paramref name="items"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="items"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="items"/> contains <c>null</c> elements.</exception>
        public static DefinitionListDocumentationElement DefinitionList(IEnumerable<DefinitionListItemDocumentationElement> items)
            => new DefinitionListDocumentationElement(items, null);

        /// <summary>Creates a <see cref="DefinitionListDocumentationElement"/> with the provided <paramref name="items"/>.</summary>
        /// <param name="items">The list items inside the <c>list</c> XML element.</param>
        /// <returns>Returns a <see cref="DefinitionListDocumentationElement"/> with the provided <paramref name="items"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="items"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="items"/> contains <c>null</c> elements.</exception>
        public static DefinitionListDocumentationElement DefinitionList(params DefinitionListItemDocumentationElement[] items)
            => new DefinitionListDocumentationElement(items, null);

        /// <summary>Creates a <see cref="DefinitionListDocumentationElement"/> with the provided <paramref name="items"/>.</summary>
        /// <param name="items">The list items inside the <c>list</c> XML element.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the definition list element.</param>
        /// <returns>Returns a <see cref="DefinitionListDocumentationElement"/> with the provided <paramref name="items"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="items"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="items"/> or <paramref name="xmlAttributes"/> contain <c>null</c> values.</exception>
        public static DefinitionListDocumentationElement DefinitionList(IEnumerable<DefinitionListItemDocumentationElement> items, IReadOnlyDictionary<string, string> xmlAttributes)
            => new DefinitionListDocumentationElement(items, xmlAttributes);

        /// <summary>Creates a <see cref="DefinitionListDocumentationElement"/> with the provided <paramref name="items"/>.</summary>
        /// <param name="listTitle">The list title inside the <c>itemheader</c> XML element.</param>
        /// <param name="items">The list items inside the <c>list</c> XML element.</param>
        /// <returns>Returns a <see cref="DefinitionListDocumentationElement"/> with the provided <paramref name="items"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="items"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="listTitle"/> or <paramref name="items"/> contain <c>null</c> elements.</exception>
        public static DefinitionListDocumentationElement DefinitionList(IEnumerable<InlineDocumentationElement> listTitle, IEnumerable<DefinitionListItemDocumentationElement> items)
            => new DefinitionListDocumentationElement(listTitle, items, null);

        /// <summary>Creates a <see cref="DefinitionListDocumentationElement"/> with the provided <paramref name="items"/>.</summary>
        /// <param name="listTitle">The list title inside the <c>itemheader</c> XML element.</param>
        /// <param name="items">The list items inside the <c>list</c> XML element.</param>
        /// <returns>Returns a <see cref="DefinitionListDocumentationElement"/> with the provided <paramref name="items"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="items"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="listTitle"/> or <paramref name="items"/> contain <c>null</c> elements.</exception>
        public static DefinitionListDocumentationElement DefinitionList(IEnumerable<InlineDocumentationElement> listTitle, params DefinitionListItemDocumentationElement[] items)
            => new DefinitionListDocumentationElement(listTitle, items, null);

        /// <summary>Creates a <see cref="DefinitionListDocumentationElement"/> with the provided <paramref name="items"/>.</summary>
        /// <param name="listTitle">The list title inside the <c>itemheader</c> XML element.</param>
        /// <param name="items">The list items inside the <c>list</c> XML element.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the definition list element.</param>
        /// <returns>Returns a <see cref="DefinitionListDocumentationElement"/> with the provided <paramref name="items"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="items"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="listTitle"/>, <paramref name="items"/> or <paramref name="xmlAttributes"/> contain <c>null</c> values.</exception>
        public static DefinitionListDocumentationElement DefinitionList(IEnumerable<InlineDocumentationElement> listTitle, IEnumerable<DefinitionListItemDocumentationElement> items, IReadOnlyDictionary<string, string> xmlAttributes)
            => new DefinitionListDocumentationElement(listTitle, items, xmlAttributes);

        /// <summary>Creates a <see cref="DefinitionListItemDocumentationElement"/> with the provided <paramref name="term"/> and <paramref name="description"/>.</summary>
        /// <param name="term">The content inside the <c>term</c> XML element of an <c>item</c> XML element.</param>
        /// <param name="description">The content inside the <c>description</c> XML element of an <c>item</c> XML element.</param>
        /// <returns>Returns a <see cref="DefinitionListItemDocumentationElement"/> with the provided <paramref name="term"/> and <paramref name="description"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="term"/> or <paramref name="description"/> are <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="term"/> or <paramref name="description"/> contain <c>null</c> elements.</exception>
        public static DefinitionListItemDocumentationElement DefinitionListItem(IEnumerable<InlineDocumentationElement> term, IEnumerable<InlineDocumentationElement> description)
        {
            if (term == null)
                throw new ArgumentNullException(nameof(term));
            if (term.Contains(null))
                throw new ArgumentException("Cannot contain 'null' elements.", nameof(term));

            if (description == null)
                throw new ArgumentNullException(nameof(description));
            if (description.Contains(null))
                throw new ArgumentException("Cannot contain 'null' elements.", nameof(description));

            return new DefinitionListItemDocumentationElement(InlineDescription(term), InlineDescription(description), null);
        }

        /// <summary>Creates a <see cref="DefinitionListItemDocumentationElement"/> with the provided <paramref name="term"/> and <paramref name="description"/>.</summary>
        /// <param name="term">The content inside the <c>term</c> XML element of an <c>item</c> XML element.</param>
        /// <param name="description">The content inside the <c>description</c> XML element of an <c>item</c> XML element.</param>
        /// <returns>Returns a <see cref="DefinitionListItemDocumentationElement"/> with the provided <paramref name="term"/> and <paramref name="description"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="term"/> or <paramref name="description"/> are <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="term"/> or <paramref name="description"/> contain <c>null</c> elements.</exception>
        public static DefinitionListItemDocumentationElement DefinitionListItem(IEnumerable<InlineDocumentationElement> term, params InlineDocumentationElement[] description)
            => DefinitionListItem(term, description.AsEnumerable());

        /// <summary>Creates a <see cref="DefinitionListItemDocumentationElement"/> with the provided <paramref name="term"/> and <paramref name="description"/>.</summary>
        /// <param name="term">The content inside the <c>term</c> XML element of an <c>item</c> XML element.</param>
        /// <param name="description">The content inside the <c>description</c> XML element of an <c>item</c> XML element.</param>
        /// <returns>Returns a <see cref="DefinitionListItemDocumentationElement"/> with the provided <paramref name="term"/> and <paramref name="description"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="term"/> or <paramref name="description"/> are <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="term"/> or <paramref name="description"/> contain <c>null</c> elements.</exception>
        public static DefinitionListItemDocumentationElement DefinitionListItem(InlineDescriptionDocumentationElement term, InlineDescriptionDocumentationElement description)
            => new DefinitionListItemDocumentationElement(term, description, null);

        /// <summary>Creates a <see cref="DefinitionListItemDocumentationElement"/> with the provided <paramref name="term"/> and <paramref name="description"/>.</summary>
        /// <param name="term">The content inside the <c>term</c> XML element of an <c>item</c> XML element.</param>
        /// <param name="description">The content inside the <c>description</c> XML element of an <c>item</c> XML element.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the definition list item element.</param>
        /// <returns>Returns a <see cref="DefinitionListItemDocumentationElement"/> with the provided <paramref name="term"/> and <paramref name="description"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="term"/> or <paramref name="description"/> are <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="term"/>, <paramref name="description"/> or <paramref name="xmlAttributes"/> contain <c>null</c> values.</exception>
        public static DefinitionListItemDocumentationElement DefinitionListItem(InlineDescriptionDocumentationElement term, InlineDescriptionDocumentationElement description, IReadOnlyDictionary<string, string> xmlAttributes)
            => new DefinitionListItemDocumentationElement(term, description, xmlAttributes);

        /// <summary>Creates a <see cref="TableDocumentationElement"/> with the provided <paramref name="columns"/> and <paramref name="rows"/>.</summary>
        /// <param name="columns">The columns inside the <c>listheader</c> XML element of a <c>list</c> XML element.</param>
        /// <param name="rows">The rows corresponding to <c>item</c> XML elements of a <c>list</c> XML element.</param>
        /// <returns>Returns a <see cref="TableDocumentationElement"/> with the provided <paramref name="columns"/> and <paramref name="rows"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="columns"/> or <paramref name="rows"/> are <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="columns"/> or <paramref name="rows"/> contain <c>null</c> elements.</exception>
        /// <remarks>
        /// <para>
        /// The returned table is normalized in the sense that if there were more columns or rows with missing cells they will be filled with
        /// empty ones so that the table has equal number of columns for each row, including the header.
        /// </para>
        /// </remarks>
        public static TableDocumentationElement Table(IEnumerable<TableColumnDocumentationElement> columns, IEnumerable<TableRowDocumentationElement> rows)
            => new TableDocumentationElement(columns, rows, null);

        /// <summary>Creates a <see cref="TableDocumentationElement"/> with the provided <paramref name="columns"/> and <paramref name="rows"/>.</summary>
        /// <param name="columns">The columns inside the <c>listheader</c> XML element of a <c>list</c> XML element.</param>
        /// <param name="rows">The rows corresponding to <c>item</c> XML elements of a <c>list</c> XML element.</param>
        /// <returns>Returns a <see cref="TableDocumentationElement"/> with the provided <paramref name="columns"/> and <paramref name="rows"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="columns"/> or <paramref name="rows"/> are <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="columns"/> or <paramref name="rows"/> contain <c>null</c> elements.</exception>
        /// <remarks>
        /// <para>
        /// The returned table is normalized in the sense that if there were more columns or rows with missing cells they will be filled with
        /// empty ones so that the table has equal number of columns for each row, including the header.
        /// </para>
        /// </remarks>
        public static TableDocumentationElement Table(IEnumerable<TableColumnDocumentationElement> columns, params TableRowDocumentationElement[] rows)
            => new TableDocumentationElement(columns, rows, null);

        /// <summary>Creates a <see cref="TableDocumentationElement"/> with the provided <paramref name="columns"/> and <paramref name="rows"/>.</summary>
        /// <param name="columns">The columns inside the <c>listheader</c> XML element of a <c>list</c> XML element.</param>
        /// <param name="rows">The rows corresponding to <c>item</c> XML elements of a <c>list</c> XML element.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the table element.</param>
        /// <returns>Returns a <see cref="TableDocumentationElement"/> with the provided <paramref name="columns"/> and <paramref name="rows"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="columns"/> or <paramref name="rows"/> are <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="columns"/>, <paramref name="rows"/> or <paramref name="xmlAttributes"/> contain <c>null</c> values.</exception>
        /// <remarks>
        /// <para>
        /// The returned table is normalized in the sense that if there were more columns or rows with missing cells they will be filled with
        /// empty ones so that the table has equal number of columns for each row, including the header.
        /// </para>
        /// </remarks>
        public static TableDocumentationElement Table(IEnumerable<TableColumnDocumentationElement> columns, IEnumerable<TableRowDocumentationElement> rows, IReadOnlyDictionary<string, string> xmlAttributes)
            => new TableDocumentationElement(columns, rows, xmlAttributes);

        /// <summary>Creates a <see cref="TableDocumentationElement"/> with the provided <paramref name="rows"/>.</summary>
        /// <param name="rows">The rows corresponding to <c>item</c> XML elements of a <c>list</c> XML element.</param>
        /// <returns>Returns a <see cref="TableDocumentationElement"/> with the provided <paramref name="rows"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="rows"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="rows"/> contain <c>null</c> elements.</exception>
        /// <remarks>
        /// <para>
        /// The returned table is normalized in the sense that if there were rows with missing cells then they will be filled with
        /// empty ones so that the table has equal number of columns for each row.
        /// </para>
        /// </remarks>
        public static TableDocumentationElement Table(IEnumerable<TableRowDocumentationElement> rows)
            => new TableDocumentationElement(rows, null);

        /// <summary>Creates a <see cref="TableDocumentationElement"/> with the provided <paramref name="rows"/>.</summary>
        /// <param name="rows">The rows corresponding to <c>item</c> XML elements of a <c>list</c> XML element.</param>
        /// <returns>Returns a <see cref="TableDocumentationElement"/> with the provided <paramref name="rows"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="rows"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="rows"/> contain <c>null</c> elements.</exception>
        /// <remarks>
        /// <para>
        /// The returned table is normalized in the sense that if there were rows with missing cells then they will be filled with
        /// empty ones so that the table has equal number of columns for each row.
        /// </para>
        /// </remarks>
        public static TableDocumentationElement Table(params TableRowDocumentationElement[] rows)
            => new TableDocumentationElement(rows, null);

        /// <summary>Creates a <see cref="TableDocumentationElement"/> with the provided <paramref name="rows"/>.</summary>
        /// <param name="rows">The rows corresponding to <c>item</c> XML elements of a <c>list</c> XML element.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the table element.</param>
        /// <returns>Returns a <see cref="TableDocumentationElement"/> with the provided <paramref name="rows"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="rows"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="rows"/> or <paramref name="xmlAttributes"/> contain <c>null</c> values.</exception>
        /// <remarks>
        /// <para>
        /// The returned table is normalized in the sense that if there were rows with missing cells then they will be filled with
        /// empty ones so that the table has equal number of columns for each row.
        /// </para>
        /// </remarks>
        public static TableDocumentationElement Table(IEnumerable<TableRowDocumentationElement> rows, IReadOnlyDictionary<string, string> xmlAttributes)
            => new TableDocumentationElement(rows, xmlAttributes);

        /// <summary>Cretes a <see cref="TableCellDocumentationElement"/> for the provided <paramref name="name"/>.</summary>
        /// <param name="name">The content inside a <c>term</c> XML element inside the <c>listheader</c> XML element.</param>
        /// <returns>Returns a <see cref="TableCellDocumentationElement"/> for the provided <paramref name="name"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> contains <c>null</c> elements.</exception>
        public static TableColumnDocumentationElement TableColumn(IEnumerable<InlineDocumentationElement> name)
            => new TableColumnDocumentationElement(name, null);

        /// <summary>Cretes a <see cref="TableCellDocumentationElement"/> for the provided <paramref name="name"/>.</summary>
        /// <param name="name">The content inside a <c>term</c> XML element inside the <c>listheader</c> XML element.</param>
        /// <returns>Returns a <see cref="TableCellDocumentationElement"/> for the provided <paramref name="name"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> contains <c>null</c> elements.</exception>
        public static TableColumnDocumentationElement TableColumn(params InlineDocumentationElement[] name)
            => new TableColumnDocumentationElement(name, null);

        /// <summary>Cretes a <see cref="TableCellDocumentationElement"/> for the provided <paramref name="name"/>.</summary>
        /// <param name="name">The content inside a <c>term</c> XML element inside the <c>listheader</c> XML element.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the table column element.</param>
        /// <returns>Returns a <see cref="TableCellDocumentationElement"/> for the provided <paramref name="name"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> or <paramref name="xmlAttributes"/> contain <c>null</c> values.</exception>
        public static TableColumnDocumentationElement TableColumn(IEnumerable<InlineDocumentationElement> name, IReadOnlyDictionary<string, string> xmlAttributes)
            => new TableColumnDocumentationElement(name, xmlAttributes);

        /// <summary>Creates a <see cref="TableRowDocumentationElement"/> for the provided <paramref name="cells"/>.</summary>
        /// <param name="cells">The content corresponding to each <c>description</c> XML element inside an <c>item</c> XML element.</param>
        /// <returns>Returns a <see cref="TableRowDocumentationElement"/> for the provided <paramref name="cells"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="cells"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="cells"/> contain <c>null</c> elements.</exception>
        public static TableRowDocumentationElement TableRow(IEnumerable<TableCellDocumentationElement> cells)
            => new TableRowDocumentationElement(cells, null);

        /// <summary>Creates a <see cref="TableRowDocumentationElement"/> for the provided <paramref name="cells"/>.</summary>
        /// <param name="cells">The content corresponding to each <c>description</c> XML element inside an <c>item</c> XML element.</param>
        /// <returns>Returns a <see cref="TableRowDocumentationElement"/> for the provided <paramref name="cells"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="cells"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="cells"/> contain <c>null</c> elements.</exception>
        public static TableRowDocumentationElement TableRow(params TableCellDocumentationElement[] cells)
            => new TableRowDocumentationElement(cells, null);

        /// <summary>Creates a <see cref="TableRowDocumentationElement"/> for the provided <paramref name="cells"/>.</summary>
        /// <param name="cells">The content corresponding to each <c>description</c> XML element inside an <c>item</c> XML element.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the table row element.</param>
        /// <returns>Returns a <see cref="TableRowDocumentationElement"/> for the provided <paramref name="cells"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="cells"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="cells"/> or <paramref name="xmlAttributes"/> contain <c>null</c> values.</exception>
        public static TableRowDocumentationElement TableRow(IEnumerable<TableCellDocumentationElement> cells, IReadOnlyDictionary<string, string> xmlAttributes)
            => new TableRowDocumentationElement(cells, xmlAttributes);

        /// <summary>Creates a <see cref="TableCellDocumentationElement"/> with the provided <paramref name="content"/>.</summary>
        /// <param name="content">The content inside the <c>description</c> XML element of an <c>item</c> XML element.</param>
        /// <returns>Returns a <see cref="TableCellDocumentationElement"/> with the provided <paramref name="content"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="content"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="content"/> contains <c>null</c> elements.</exception>
        public static TableCellDocumentationElement TableCell(IEnumerable<InlineDocumentationElement> content)
            => new TableCellDocumentationElement(content, null);

        /// <summary>Creates a <see cref="TableCellDocumentationElement"/> with the provided <paramref name="content"/>.</summary>
        /// <param name="content">The content inside the <c>description</c> XML element of an <c>item</c> XML element.</param>
        /// <returns>Returns a <see cref="TableCellDocumentationElement"/> with the provided <paramref name="content"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="content"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="content"/> contains <c>null</c> elements.</exception>
        public static TableCellDocumentationElement TableCell(params InlineDocumentationElement[] content)
            => new TableCellDocumentationElement(content, null);

        /// <summary>Creates a <see cref="TableCellDocumentationElement"/> with the provided <paramref name="content"/>.</summary>
        /// <param name="content">The content inside the <c>description</c> XML element of an <c>item</c> XML element.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the table cell element.</param>
        /// <returns>Returns a <see cref="TableCellDocumentationElement"/> with the provided <paramref name="content"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="content"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="content"/> or <paramref name="xmlAttributes"/> contain <c>null</c> values.</exception>
        public static TableCellDocumentationElement TableCell(IEnumerable<InlineDocumentationElement> content, IReadOnlyDictionary<string, string> xmlAttributes)
            => new TableCellDocumentationElement(content, xmlAttributes);

        /// <summary>Creates a <see cref="InlineDescriptionDocumentationElement"/> with the provided <paramref name="inlineElements"/>.</summary>
        /// <param name="inlineElements">The <see cref="InlineDocumentationElement"/>s to wrap.</param>
        /// <returns>Returns a <see cref="InlineDescriptionDocumentationElement"/> with the provided <paramref name="inlineElements"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="inlineElements"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="inlineElements"/> contain <c>null</c> values.</exception>
        public static InlineDescriptionDocumentationElement InlineDescription(IEnumerable<InlineDocumentationElement> inlineElements)
            => new InlineDescriptionDocumentationElement(inlineElements, null);

        /// <summary>Creates a <see cref="InlineDescriptionDocumentationElement"/> with the provided <paramref name="inlineElements"/>.</summary>
        /// <param name="inlineElements">The <see cref="InlineDocumentationElement"/>s to wrap.</param>
        /// <returns>Returns a <see cref="InlineDescriptionDocumentationElement"/> with the provided <paramref name="inlineElements"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="inlineElements"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="inlineElements"/> contain <c>null</c> values.</exception>
        public static InlineDescriptionDocumentationElement InlineDescription(params InlineDocumentationElement[] inlineElements)
            => new InlineDescriptionDocumentationElement(inlineElements, null);

        /// <summary>Creates a <see cref="InlineDescriptionDocumentationElement"/> with the provided <paramref name="inlineElements"/>.</summary>
        /// <param name="inlineElements">The <see cref="InlineDocumentationElement"/>s to wrap.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the container element.</param>
        /// <returns>Returns a <see cref="InlineDescriptionDocumentationElement"/> with the provided <paramref name="inlineElements"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="inlineElements"/> or <paramref name="xmlAttributes"/> are <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="inlineElements"/> or <paramref name="xmlAttributes"/> contain <c>null</c> values.</exception>
        public static InlineDescriptionDocumentationElement InlineDescription(IEnumerable<InlineDocumentationElement> inlineElements, IReadOnlyDictionary<string, string> xmlAttributes)
            => new InlineDescriptionDocumentationElement(inlineElements, xmlAttributes);

        /// <summary>Creates a <see cref="TableDocumentationElement"/> with the provided <paramref name="text"/>.</summary>
        /// <param name="text">Plain text inside XML elements.</param>
        /// <returns>Returns a <see cref="TableDocumentationElement"/> with the provided <paramref name="text"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is <c>null</c>.</exception>
        public static TextDocumentationElement Text(string text)
            => new TextDocumentationElement(text);

        /// <summary>Creates an <see cref="InlineCodeDocumentationElement"/> with the provided <paramref name="code"/>.</summary>
        /// <param name="code">The code inside a <c>c</c> XML element.</param>
        /// <returns>Returns an <see cref="InlineCodeDocumentationElement"/> with the provided <paramref name="code"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="code"/> is <c>null</c>.</exception>
        public static InlineCodeDocumentationElement InlineCode(string code)
            => new InlineCodeDocumentationElement(code, null);

        /// <summary>Creates an <see cref="InlineCodeDocumentationElement"/> with the provided <paramref name="code"/>.</summary>
        /// <param name="code">The code inside a <c>c</c> XML element.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the inline code element.</param>
        /// <returns>Returns an <see cref="InlineCodeDocumentationElement"/> with the provided <paramref name="code"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="code"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="xmlAttributes"/> contain <c>null</c> values.</exception>
        public static InlineCodeDocumentationElement InlineCode(string code, IReadOnlyDictionary<string, string> xmlAttributes)
            => new InlineCodeDocumentationElement(code, xmlAttributes);

        /// <summary>Creates a <see cref="MemberInfoReferenceDocumentationElement"/> with the provided <paramref name="referredMember"/>.</summary>
        /// <param name="referredMember">The resolved <see cref="MemberInfo"/> referred by a canonical name using a <c>see</c> XML element.</param>
        /// <returns>Returns a <see cref="MemberInfoReferenceDocumentationElement"/> with the provided <paramref name="referredMember"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="referredMember"/> is <c>null</c>.</exception>
        public static MemberInfoReferenceDocumentationElement MemberReference(MemberInfo referredMember)
            => new MemberInfoReferenceDocumentationElement(referredMember, null);

        /// <summary>Creates a <see cref="MemberInfoReferenceDocumentationElement"/> with the provided <paramref name="referredMember"/>.</summary>
        /// <param name="referredMember">The resolved <see cref="MemberInfo"/> referred by a canonical name using a <c>see</c> XML element.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the member reference element.</param>
        /// <returns>Returns a <see cref="MemberInfoReferenceDocumentationElement"/> with the provided <paramref name="referredMember"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="referredMember"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="xmlAttributes"/> contain <c>null</c> values.</exception>
        public static MemberInfoReferenceDocumentationElement MemberReference(MemberInfo referredMember, IReadOnlyDictionary<string, string> xmlAttributes)
            => new MemberInfoReferenceDocumentationElement(referredMember, xmlAttributes);

        /// <summary>Creates a <see cref="MemberNameReferenceDocumentationElement"/> with the provided <paramref name="canonicalName"/>.</summary>
        /// <param name="canonicalName">The canonical name for a member referred using a <c>see</c> XML element.</param>
        /// <returns>Returns a <see cref="MemberNameReferenceDocumentationElement"/> with the provided <paramref name="canonicalName"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="canonicalName"/> is <c>null</c>.</exception>
        public static MemberNameReferenceDocumentationElement MemberReference(string canonicalName)
            => new MemberNameReferenceDocumentationElement(canonicalName, null);

        /// <summary>Creates a <see cref="MemberNameReferenceDocumentationElement"/> with the provided <paramref name="canonicalName"/>.</summary>
        /// <param name="canonicalName">The canonical name for a member referred using a <c>see</c> XML element.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the member reference element.</param>
        /// <returns>Returns a <see cref="MemberNameReferenceDocumentationElement"/> with the provided <paramref name="canonicalName"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="canonicalName"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="xmlAttributes"/> contain <c>null</c> values.</exception>
        public static MemberNameReferenceDocumentationElement MemberReference(string canonicalName, IReadOnlyDictionary<string, string> xmlAttributes)
            => new MemberNameReferenceDocumentationElement(canonicalName, xmlAttributes);

        /// <summary>Creates a <see cref="ParameterReferenceDocumentationElement"/> with the provided <paramref name="parameterName"/>.</summary>
        /// <param name="parameterName">The name of the referred parameter using the <c>paramref</c> XML element.</param>
        /// <returns>Returns a <see cref="ParameterReferenceDocumentationElement"/> with the provided <paramref name="parameterName"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="parameterName"/> is <c>null</c>.</exception>
        public static ParameterReferenceDocumentationElement ParameterReference(string parameterName)
            => new ParameterReferenceDocumentationElement(parameterName, null);

        /// <summary>Creates a <see cref="ParameterReferenceDocumentationElement"/> with the provided <paramref name="parameterName"/>.</summary>
        /// <param name="parameterName">The name of the referred parameter using the <c>paramref</c> XML element.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the parameter reference element.</param>
        /// <returns>Returns a <see cref="ParameterReferenceDocumentationElement"/> with the provided <paramref name="parameterName"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="parameterName"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="xmlAttributes"/> contain <c>null</c> values.</exception>
        public static ParameterReferenceDocumentationElement ParameterReference(string parameterName, IReadOnlyDictionary<string, string> xmlAttributes)
            => new ParameterReferenceDocumentationElement(parameterName, xmlAttributes);

        /// <summary>Creates a <see cref="GenericParameterReferenceDocumentationElement"/> with the provided <paramref name="genericParameterName"/>.</summary>
        /// <param name="genericParameterName">The name of the referred generic parameter using the <c>typeparamref</c> XML element.</param>
        /// <returns>Returns a <see cref="GenericParameterReferenceDocumentationElement"/> with the provided <paramref name="genericParameterName"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="genericParameterName"/> is <c>null</c>.</exception>
        public static GenericParameterReferenceDocumentationElement GenericParameterReference(string genericParameterName)
            => new GenericParameterReferenceDocumentationElement(genericParameterName, null);

        /// <summary>Creates a <see cref="GenericParameterReferenceDocumentationElement"/> with the provided <paramref name="genericParameterName"/>.</summary>
        /// <param name="genericParameterName">The name of the referred generic parameter using the <c>typeparamref</c> XML element.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the generic parameter reference element.</param>
        /// <returns>Returns a <see cref="GenericParameterReferenceDocumentationElement"/> with the provided <paramref name="genericParameterName"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="genericParameterName"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="xmlAttributes"/> contain <c>null</c> values.</exception>
        public static GenericParameterReferenceDocumentationElement GenericParameterReference(string genericParameterName, IReadOnlyDictionary<string, string> xmlAttributes)
            => new GenericParameterReferenceDocumentationElement(genericParameterName, xmlAttributes);

        /// <summary>Creates an <see cref="AssemblyDocumentationElement"/> from the provided <paramref name="assembly"/>.</summary>
        /// <param name="assembly">The <see cref="Assembly"/> from which to create a <see cref="AssemblyDocumentationElement"/>.</param>
        /// <returns>Returns an <see cref="AssemblyDocumentationElement"/> from the provided <paramref name="assembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assembly"/> is <c>null</c>.</exception>
        public static AssemblyDocumentationElement Create(Assembly assembly)
            => Create(assembly, new MemberDocumentationCollection(Enumerable.Empty<MemberDocumentation>()));

        /// <summary>Creates an <see cref="AssemblyDocumentationElement"/> from the provided <paramref name="assembly"/>.</summary>
        /// <param name="assembly">The <see cref="Assembly"/> from which to create a <see cref="AssemblyDocumentationElement"/>.</param>
        /// <param name="membersDocumentation">
        /// A <see cref="MemberDocumentationCollection"/> containing written documentation to associated to
        /// <see cref="DocumentationElement"/>s representing assembly member declarations.
        /// </param>
        /// <returns>Returns an <see cref="AssemblyDocumentationElement"/> from the provided <paramref name="assembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assembly"/> or <paramref name="membersDocumentation"/> are <c>null</c>.</exception>
        public static AssemblyDocumentationElement Create(Assembly assembly, MemberDocumentationCollection membersDocumentation)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));
            if (membersDocumentation == null)
                throw new ArgumentNullException(nameof(membersDocumentation));

            return new AssemblyDocumentationElementFactory(
                    new CanonicalNameResolver(new[] { assembly }.Concat(assembly.GetReferencedAssemblies().Select(Assembly.Load))),
                    membersDocumentation
                )
                .Create(assembly);
        }

        /// <summary>Creates an <see cref="AssemblyDocumentationElement"/> from the provided <paramref name="assembly"/>.</summary>
        /// <param name="assembly">The <see cref="Assembly"/> from which to create a <see cref="AssemblyDocumentationElement"/>.</param>
        /// <returns>Returns an <see cref="AssemblyDocumentationElement"/> from the provided <paramref name="assembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assembly"/> is <c>null</c>.</exception>
        /// <remarks>The associated XML documentation file is searched in the folder from where the assembly was loaded,
        /// if one is found then it is used, otherwise no written documentation is added to the result.</remarks>
        public static Task<AssemblyDocumentationElement> CreateAsync(Assembly assembly)
            => CreateAsync(assembly, CancellationToken.None);

        /// <summary>Creates an <see cref="AssemblyDocumentationElement"/> from the provided <paramref name="assembly"/>.</summary>
        /// <param name="assembly">The <see cref="Assembly"/> from which to create a <see cref="AssemblyDocumentationElement"/>.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns an <see cref="AssemblyDocumentationElement"/> from the provided <paramref name="assembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assembly"/> is <c>null</c>.</exception>
        /// <remarks>The associated XML documentation file is searched in the folder from where the assembly was loaded,
        /// if one is found then it is used, otherwise no written documentation is added to the result.</remarks>
        public static async Task<AssemblyDocumentationElement> CreateAsync(Assembly assembly, CancellationToken cancellationToken)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            var xmlDocumentationFileInfo = new FileInfo(Path.ChangeExtension(assembly.Location, ".xml"));
            if (xmlDocumentationFileInfo.Exists)
                using (var xmlDocumentationReader = xmlDocumentationFileInfo.OpenText())
                    return await CreateAsync(assembly, xmlDocumentationReader, cancellationToken).ConfigureAwait(false);
            else
                return Create(assembly);
        }

        /// <summary>Creates an <see cref="AssemblyDocumentationElement"/> from the provided <paramref name="assembly"/>.</summary>
        /// <param name="assembly">The <see cref="Assembly"/> from which to create a <see cref="AssemblyDocumentationElement"/>.</param>
        /// <param name="xmlDocumentationReader">A <see cref="TextReader"/> for reading the associated XML documentation file.</param>
        /// <returns>Returns an <see cref="AssemblyDocumentationElement"/> from the provided <paramref name="assembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assembly"/> or <paramref name="xmlDocumentationReader"/> are <c>null</c>.</exception>
        public static Task<AssemblyDocumentationElement> CreateAsync(Assembly assembly, TextReader xmlDocumentationReader)
            => CreateAsync(assembly, xmlDocumentationReader);

        /// <summary>Creates an <see cref="AssemblyDocumentationElement"/> from the provided <paramref name="assembly"/>.</summary>
        /// <param name="assembly">The <see cref="Assembly"/> from which to create a <see cref="AssemblyDocumentationElement"/>.</param>
        /// <param name="xmlDocumentationReader">A <see cref="TextReader"/> for reading the associated XML documentation file.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns an <see cref="AssemblyDocumentationElement"/> from the provided <paramref name="assembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assembly"/> or <paramref name="xmlDocumentationReader"/> are <c>null</c>.</exception>
        public static async Task<AssemblyDocumentationElement> CreateAsync(Assembly assembly, TextReader xmlDocumentationReader, CancellationToken cancellationToken)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));
            if (xmlDocumentationReader == null)
                throw new ArgumentNullException(nameof(xmlDocumentationReader));

            return Create(
                assembly,
                await new XmlDocumentationReader().ReadAsync(xmlDocumentationReader, cancellationToken).ConfigureAwait(false)
            );
        }

        /// <summary>Creates a <see cref="TypeDocumentationElement"/> from the provided <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> from which to create a <see cref="TypeDocumentationElement"/>.</param>
        /// <returns>Returns a <see cref="TypeDocumentationElement"/> from the provided <paramref name="type"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is <c>null</c>.</exception>
        /// <remarks>
        /// This method creates the entire <see cref="AssemblyDocumentationElement"/> and returns the <see cref="TypeDocumentationElement"/>
        /// for the provided <paramref name="type"/>.
        /// </remarks>
        public static TypeDocumentationElement Create(Type type)
            => Create(type, new MemberDocumentationCollection(Enumerable.Empty<MemberDocumentation>()));

        /// <summary>Creates a <see cref="TypeDocumentationElement"/> from the provided <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> from which to create a <see cref="TypeDocumentationElement"/>.</param>
        /// <param name="membersDocumentation">
        /// A <see cref="MemberDocumentationCollection"/> containing written documentation to associated to
        /// <see cref="DocumentationElement"/>s representing assembly member declarations.
        /// </param>
        /// <returns>Returns a <see cref="TypeDocumentationElement"/> from the provided <paramref name="type"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> or <paramref name="membersDocumentation"/> are <c>null</c>.</exception>
        /// <remarks>
        /// This method creates the entire <see cref="AssemblyDocumentationElement"/> and returns the <see cref="TypeDocumentationElement"/>
        /// for the provided <paramref name="type"/>.
        /// </remarks>
        public static TypeDocumentationElement Create(Type type, MemberDocumentationCollection membersDocumentation)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (membersDocumentation == null)
                throw new ArgumentNullException(nameof(membersDocumentation));

            return Create(type.Assembly, membersDocumentation)
                .Namespaces
                .Where(@namespace => string.Equals(@namespace.Name, type.Namespace, StringComparison.OrdinalIgnoreCase))
                .SelectMany(
                    @namespace => @namespace
                        .Enums
                        .AsEnumerable<TypeDocumentationElement>()
                        .Concat(@namespace.Delegates)
                        .Concat(@namespace.Interfaces)
                        .Concat(@namespace.Classes.SelectMany(_GetWithNested))
                        .Concat(@namespace.Structs.SelectMany(_GetWithNested))
                )
                .FirstOrDefault(typeDocumentationElement => typeDocumentationElement == type);
        }

        private static IEnumerable<TypeDocumentationElement> _GetWithNested(ClassDocumentationElement classDocumentationElement)
            => new[] { classDocumentationElement }
                .Concat(classDocumentationElement
                    .NestedEnums
                    .AsEnumerable<TypeDocumentationElement>()
                    .Concat(classDocumentationElement.NestedDelegates)
                    .Concat(classDocumentationElement.NestedInterfaces)
                    .Concat(classDocumentationElement.NestedClasses.SelectMany(_GetWithNested))
                    .Concat(classDocumentationElement.NestedStructs.SelectMany(_GetWithNested))
                );

        private static IEnumerable<TypeDocumentationElement> _GetWithNested(StructDocumentationElement structDocumentationElement)
            => new[] { structDocumentationElement }
                .Concat(structDocumentationElement
                    .NestedEnums
                    .AsEnumerable<TypeDocumentationElement>()
                    .Concat(structDocumentationElement.NestedDelegates)
                    .Concat(structDocumentationElement.NestedInterfaces)
                    .Concat(structDocumentationElement.NestedClasses.SelectMany(_GetWithNested))
                    .Concat(structDocumentationElement.NestedStructs.SelectMany(_GetWithNested))
                );

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