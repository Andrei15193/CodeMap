using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace CodeMap.DocumentationElements
{
    /// <summary>Represents an XML documentation reader.</summary>
    public class XmlDocumentationReader
    {
        private readonly IEnumerable<XmlNodeType> _contentNodeTypes = new[]
        {
            XmlNodeType.Element,
            XmlNodeType.Text,
            XmlNodeType.Whitespace,
            XmlNodeType.SignificantWhitespace
        };

        /// <summary>Initializes a new instance of the <see cref="XmlDocumentationReader"/> class.</summary>
        public XmlDocumentationReader()
        {
        }

        /// <summary>Reads the XML documentation into <see cref="MemberDocumentation"/> items.</summary>
        /// <param name="input">The <see cref="TextReader"/> from which to load <see cref="MemberDocumentation"/> items.</param>
        /// <returns>Returns a collection of <see cref="MemberDocumentation"/> loaded from the provided <paramref name="input"/> wrapped in a <see cref="Task{TResult}"/>.</returns>
        public MemberDocumentationCollection Read(TextReader input)
        {
            XDocument documentation;
            using (var xmlReader = XmlReader.Create(input))
                documentation = XDocument.Load(xmlReader, LoadOptions.PreserveWhitespace);

            return new MemberDocumentationCollection(
                documentation
                    .Root
                    .Element("members")
                    .Elements("member")
                    .Where(member => member.Attribute("name") != null)
                    .Select(_ReadMemberDocumentation)
                );
        }

        private MemberDocumentation _ReadMemberDocumentation(XElement memberDocumentationXmlElement)
            => new MemberDocumentation(
                memberDocumentationXmlElement.Attribute("name").Value,
                _ReadSummary(memberDocumentationXmlElement),
                _ReadTypeParameters(memberDocumentationXmlElement),
                _ReadParameters(memberDocumentationXmlElement),
                _ReadReturns(memberDocumentationXmlElement),
                _ReadExceptions(memberDocumentationXmlElement),
                _ReadRemarks(memberDocumentationXmlElement),
                _ReadExamples(memberDocumentationXmlElement),
                _ReadValue(memberDocumentationXmlElement),
                _ReadRelatedMembers(memberDocumentationXmlElement)
            );

        private SummaryDocumentationElement _ReadSummary(XElement memberDocumentationXmlElement)
        {
            var summaryXmlElement = memberDocumentationXmlElement.Element("summary");
            if (summaryXmlElement == null)
                return null;

            return DocumentationElement.Summary(
                _ReadBlocks(summaryXmlElement),
                _ReadXmlAttributes(summaryXmlElement)
            );
        }

        private IReadOnlyDictionary<string, BlockDescriptionDocumentationElement> _ReadTypeParameters(XElement memberDocumentationXmlElement)
            => (
                from typeParamXmlElement in memberDocumentationXmlElement.Elements("typeparam")
                let typeParamNameAttribute = typeParamXmlElement.Attribute("name")
                where typeParamNameAttribute != null
                group typeParamXmlElement by typeParamNameAttribute.Value into typeParamXmlElementsByName
                let blockDocumentationElements = typeParamXmlElementsByName.SelectMany(_ReadBlocks)
                let xmlAttributes = _ReadXmlAttributesExcept(typeParamXmlElementsByName, "name")
                select new
                {
                    Name = typeParamXmlElementsByName.Key,
                    DescriptionBlockElements = DocumentationElement.BlockDescription(blockDocumentationElements, xmlAttributes)
                }
            )
            .ToDictionary(
                typeParam => typeParam.Name,
                typeParam => typeParam.DescriptionBlockElements,
                StringComparer.Ordinal
            );

        private IReadOnlyDictionary<string, BlockDescriptionDocumentationElement> _ReadParameters(XElement memberDocumentationXmlElement)
            => (
                from paramXmlElement in memberDocumentationXmlElement.Elements("param")
                let paramNameAttribute = paramXmlElement.Attribute("name")
                where paramNameAttribute != null
                group paramXmlElement by paramNameAttribute.Value into paramXmlElementsByName
                let blockDocumentationElements = paramXmlElementsByName.SelectMany(_ReadBlocks)
                let xmlAttributes = _ReadXmlAttributesExcept(paramXmlElementsByName, "name")
                select new
                {
                    Name = paramXmlElementsByName.Key,
                    DescriptionBlockElements = DocumentationElement.BlockDescription(blockDocumentationElements, xmlAttributes)
                }
            )
            .ToDictionary(
                param => param.Name,
                param => param.DescriptionBlockElements,
                StringComparer.Ordinal
            );

        private BlockDescriptionDocumentationElement _ReadReturns(XElement memberDocumentationXmlElement)
        {
            var returnsXmlElement = memberDocumentationXmlElement.Element("returns");
            if (returnsXmlElement == null)
                return null;

            return DocumentationElement.BlockDescription(
                _ReadBlocks(returnsXmlElement),
                _ReadXmlAttributes(returnsXmlElement)
            );
        }

        private IReadOnlyDictionary<string, BlockDescriptionDocumentationElement> _ReadExceptions(XElement memberDocumentationXmlElement)
            => (
                from exceptionXmlElement in memberDocumentationXmlElement.Elements("exception")
                let exceptionCrefAttribute = exceptionXmlElement.Attribute("cref")
                where exceptionCrefAttribute != null
                group exceptionXmlElement by exceptionCrefAttribute.Value into exceptionXmlElementsByType
                let blockDocumentationElements = exceptionXmlElementsByType.SelectMany(_ReadBlocks)
                let xmlAttributes = _ReadXmlAttributesExcept(exceptionXmlElementsByType, "cref")
                select new
                {
                    CanonicalName = exceptionXmlElementsByType.Key,
                    DescriptionBlockElements = DocumentationElement.BlockDescription(blockDocumentationElements, xmlAttributes)
                }
            )
            .ToDictionary(
                exception => exception.CanonicalName,
                exception => exception.DescriptionBlockElements,
                StringComparer.Ordinal
            );

        private RemarksDocumentationElement _ReadRemarks(XElement memberDocumentationXmlElement)
        {
            var remarksXmlElement = memberDocumentationXmlElement.Element("remarks");
            if (remarksXmlElement == null)
                return null;

            return DocumentationElement.Remarks(_ReadBlocks(remarksXmlElement), _ReadXmlAttributes(remarksXmlElement));
        }

        private IReadOnlyList<ExampleDocumentationElement> _ReadExamples(XElement memberDocumentationXmlElement)
            => memberDocumentationXmlElement
                .Elements("example")
                .Select(exampleXmlElement => DocumentationElement.Example(_ReadBlocks(exampleXmlElement), _ReadXmlAttributes(exampleXmlElement)))
                .ToList();

        private ValueDocumentationElement _ReadValue(XElement memberDocumentationXmlElement)
        {
            var valueXmlElement = memberDocumentationXmlElement.Element("value");
            if (valueXmlElement == null)
                return null;

            return DocumentationElement.Value(_ReadBlocks(valueXmlElement), _ReadXmlAttributes(valueXmlElement));
        }

        private IReadOnlyList<MemberReferenceDocumentationElement> _ReadRelatedMembers(XElement memberDocumentationXmlElement)
            => (
                from relatedMemberXmlElement in memberDocumentationXmlElement.Elements("seealso")
                let relatedMemberCrefAttribute = relatedMemberXmlElement.Attribute("cref")
                where relatedMemberCrefAttribute != null
                select DocumentationElement.MemberReference(relatedMemberCrefAttribute.Value, _ReadXmlAttributesExcept(relatedMemberXmlElement, "cref"))
            )
            .ToList();

        private static IReadOnlyDictionary<string, string> _ReadXmlAttributes(XElement xmlElement)
            => xmlElement
                .Attributes()
                .ToDictionary(
                    attribute => attribute.Name.LocalName,
                    attribute => attribute.Value,
                    StringComparer.Ordinal
                );

        private static IReadOnlyDictionary<string, string> _ReadXmlAttributes(IEnumerable<XElement> xmlElements)
            => xmlElements
                .Attributes()
                .GroupBy(attribute => attribute.Name.LocalName, StringComparer.Ordinal)
                .ToDictionary(
                    attributesByName => attributesByName.Key,
                    attributesByName => attributesByName.First().Value,
                    StringComparer.Ordinal
                );

        private static IReadOnlyDictionary<string, string> _ReadXmlAttributesExcept(XElement xmlElement, string attributeLocalName)
            => xmlElement
                .Attributes()
                .Where(attribute => attribute.Name.LocalName != attributeLocalName)
                .ToDictionary(
                    attribute => attribute.Name.LocalName,
                    attribute => attribute.Value,
                    StringComparer.Ordinal
                );

        private static IReadOnlyDictionary<string, string> _ReadXmlAttributesExcept(IEnumerable<XElement> xmlElements, string attributeLocalName)
            => xmlElements
                .Attributes()
                .Where(attribute => attribute.Name.LocalName != attributeLocalName)
                .GroupBy(attribute => attribute.Name.LocalName, StringComparer.Ordinal)
                .ToDictionary(
                    attributesByName => attributesByName.Key,
                    attributesByName => attributesByName.First().Value,
                    StringComparer.Ordinal
                );

        private IReadOnlyList<BlockDocumentationElement> _ReadBlocks(XElement sectionXmlElement)
        {
            var blockElements = new List<BlockDocumentationElement>();
            var inlineParagraphNodes = new List<XNode>();
            foreach (var childNode in sectionXmlElement.Nodes().Where(childNode => _contentNodeTypes.Contains(childNode.NodeType)))
                switch (childNode)
                {
                    case XElement childXmlElement when childXmlElement.Name.LocalName.Equals("para", StringComparison.Ordinal):
                        _AddInlineParagraphIfExists();
                        blockElements.Add(_ReadParagraph(childXmlElement));
                        break;

                    case XElement childXmlElement when childXmlElement.Name.LocalName.Equals("code", StringComparison.Ordinal):
                        _AddInlineParagraphIfExists();
                        blockElements.Add(_ReadCodeBlock(childXmlElement));
                        break;

                    case XElement childXmlElement when childXmlElement.Name.LocalName.Equals("list", StringComparison.Ordinal):
                        _AddInlineParagraphIfExists();
                        blockElements.Add(_ReadListOrTable(childXmlElement));
                        break;

                    default:
                        inlineParagraphNodes.Add(childNode);
                        break;
                }
            _AddInlineParagraphIfExists();

            return blockElements;

            void _AddInlineParagraphIfExists()
            {
                if (inlineParagraphNodes.Count > 0)
                {
                    var paragraphContent = _ReadContent(inlineParagraphNodes);
                    if (paragraphContent.Count > 0)
                        blockElements.Add(DocumentationElement.Paragraph(paragraphContent));
                    inlineParagraphNodes.Clear();
                }
            }
        }

        private static ParagraphDocumentationElement _ReadParagraph(XElement xmlElement)
            => DocumentationElement.Paragraph(
                _ReadContent(xmlElement.Nodes()),
                _ReadXmlAttributes(xmlElement)
            );

        private static CodeBlockDocumentationElement _ReadCodeBlock(XElement xmlElement)
        {
            var codeBlockLines = _ReadCodeBlockLines(xmlElement.Value);
            if (codeBlockLines.Count == 1)
                return DocumentationElement.CodeBlock(codeBlockLines.Single());

            var lineContentStartOffset = codeBlockLines
                .Skip(1)
                .Select(_GetIndentSize)
                .Min();

            var codeBlockBuilder = new StringBuilder();
            using (var line = codeBlockLines.GetEnumerator())
            {
                if (line.MoveNext() && !string.IsNullOrWhiteSpace(line.Current))
                    codeBlockBuilder.Append(line.Current);
                while (line.MoveNext())
                {
                    var indent = 0;
                    var startIndex = 0;
                    while (indent < lineContentStartOffset && char.IsWhiteSpace(line.Current, startIndex))
                    {
                        indent++;
                        startIndex += char.IsHighSurrogate(line.Current, startIndex) ? 2 : 1;
                    }
                    if (codeBlockBuilder.Length > 0)
                        codeBlockBuilder.Append('\n');
                    codeBlockBuilder.Append(line.Current, startIndex, line.Current.Length - startIndex);
                }
            }

            return DocumentationElement.CodeBlock(codeBlockBuilder.ToString(), _ReadXmlAttributes(xmlElement));
        }

        private static BlockDocumentationElement _ReadListOrTable(XElement xmlElement)
        {
            var listTypeAttribute = xmlElement.Attribute("type");
            if (listTypeAttribute != null)
                if (listTypeAttribute.Value.Equals("table", StringComparison.Ordinal))
                    return _ReadTable(xmlElement);
                else if (_IsDefinitionList())
                    return _ReadDefinitionList(xmlElement);
                else if (listTypeAttribute.Value.Equals("number", StringComparison.Ordinal))
                    return _ReadOrederedList(xmlElement);
                else
                    return _ReadUnorederedList(xmlElement);
            else if (_IsDefinitionList())
                return _ReadDefinitionList(xmlElement);
            else
                return _ReadUnorederedList(xmlElement);

            bool _IsDefinitionList()
                => xmlElement.Element("listheader") != null || xmlElement.Elements("item").Any(itemXmlElement => itemXmlElement.Element("term") != null);
        }

        private static UnorderedListDocumentationElement _ReadUnorederedList(XElement xmlElement)
            => DocumentationElement.UnorderedList(
                xmlElement.Elements("item").Select(_ReadListItem),
                _ReadXmlAttributesExcept(xmlElement, "type")
            );

        private static OrderedListDocumentationElement _ReadOrederedList(XElement xmlElement)
            => DocumentationElement.OrderedList(
                xmlElement.Elements("item").Select(_ReadListItem),
                _ReadXmlAttributesExcept(xmlElement, "type")
            );

        private static ListItemDocumentationElement _ReadListItem(XElement xmlElement)
        {
            var descriptionElement = xmlElement.Element("description");
            return DocumentationElement.ListItem(
                _ReadContent((descriptionElement ?? xmlElement).Nodes()),
                _ReadXmlAttributes(descriptionElement != null ? new[] { descriptionElement, xmlElement } : new[] { xmlElement })
            );
        }

        private static DefinitionListDocumentationElement _ReadDefinitionList(XElement xmlElement)
        {
            var definitionListItems = xmlElement
                .Elements("item")
                .Select(
                    itemXmlElement =>
                    {
                        var termElement = itemXmlElement.Element("term");
                        var descriptionElement = itemXmlElement.Element("description");
                        return DocumentationElement.DefinitionListItem(
                            DocumentationElement.DefinitionListItemTerm(
                                _ReadContent(termElement?.Nodes() ?? Enumerable.Empty<XNode>()),
                                termElement != null ? _ReadXmlAttributes(termElement) : null
                            ),
                            DocumentationElement.DefinitionListItemDescription(
                                _ReadContent(itemXmlElement.Element("description")?.Nodes() ?? Enumerable.Empty<XNode>()),
                                descriptionElement != null ? _ReadXmlAttributes(descriptionElement) : null
                            ),
                            _ReadXmlAttributes(itemXmlElement)
                        );
                    }
                );

            var listTitleXmlElement = xmlElement.Element("listheader");
            var listTitleNodes = (listTitleXmlElement?.Element("term") ?? listTitleXmlElement)?.Nodes();
            var listTile = listTitleNodes == null ? null : DocumentationElement.DefinitionListTitle(_ReadContent(listTitleNodes), _ReadXmlAttributes(listTitleXmlElement));
            return DocumentationElement.DefinitionList(
                listTile,
                definitionListItems,
                _ReadXmlAttributesExcept(xmlElement, "type")
            );
        }

        private static TableDocumentationElement _ReadTable(XElement xmlElement)
        {
            var rows = xmlElement
                .Elements("item")
                .Select(
                    rowXmlElement => DocumentationElement.TableRow(
                        rowXmlElement
                            .Elements("description")
                            .Select(
                                cellXmlElement => DocumentationElement.TableCell(
                                    _ReadContent(cellXmlElement.Nodes()),
                                    _ReadXmlAttributes(cellXmlElement)
                                )
                            ),
                        _ReadXmlAttributes(rowXmlElement)
                    )
                )
                .ToList();

            var tableHeaderXmlElement = xmlElement.Element("listheader");
            if (tableHeaderXmlElement != null)
            {
                var termElements = tableHeaderXmlElement
                    .Elements("term");
                var columns = termElements
                    .Select(
                        columnXmlElement => DocumentationElement.TableColumn(
                            _ReadContent(columnXmlElement.Nodes()),
                            _ReadXmlAttributes(new[] { columnXmlElement, tableHeaderXmlElement })
                        )
                    )
                    .Concat(
                        Enumerable.Repeat(
                            DocumentationElement.TableColumn(Enumerable.Empty<InlineDocumentationElement>(), _ReadXmlAttributes(tableHeaderXmlElement)),
                            Math.Max(0, (rows.Max(row => (int?)row.Cells.Count) ?? 0) - termElements.Count())
                        )
                    );
                return DocumentationElement.Table(columns, rows, _ReadXmlAttributesExcept(xmlElement, "type"));
            }
            else
                return DocumentationElement.Table(rows, _ReadXmlAttributesExcept(xmlElement, "type"));
        }

        private static IReadOnlyList<InlineDocumentationElement> _ReadContent(IEnumerable<XNode> xmlNodes)
        {
            var inlineElements = new List<InlineDocumentationElement>();
            var textBuilder = new StringBuilder();
            foreach (var xmlNode in xmlNodes)
            {
                switch (xmlNode)
                {
                    case XText textNode:
                        _Append(textNode.Value);
                        break;

                    case XElement xmlElement when xmlElement.Name.LocalName.Equals("see", StringComparison.Ordinal):
                        var memberReferenceCrefAttribute = xmlElement.Attribute("cref");
                        if (memberReferenceCrefAttribute != null)
                        {
                            _AddTextElementIfExists();
                            inlineElements.Add(DocumentationElement.MemberReference(memberReferenceCrefAttribute.Value, _ReadXmlAttributesExcept(xmlElement, "cref")));
                        }
                        break;

                    case XElement xmlElement when xmlElement.Name.LocalName.Equals("paramref", StringComparison.Ordinal):
                        var parameterReferenceNameAttribute = xmlElement.Attribute("name");
                        if (parameterReferenceNameAttribute != null)
                        {
                            _AddTextElementIfExists();
                            inlineElements.Add(DocumentationElement.ParameterReference(parameterReferenceNameAttribute.Value, _ReadXmlAttributesExcept(xmlElement, "name")));
                        }
                        break;

                    case XElement xmlElement when xmlElement.Name.LocalName.Equals("typeparamref", StringComparison.Ordinal):
                        var typeParameterReferenceNameAttribute = xmlElement.Attribute("name");
                        if (typeParameterReferenceNameAttribute != null)
                        {
                            _AddTextElementIfExists();
                            inlineElements.Add(DocumentationElement.GenericParameterReference(typeParameterReferenceNameAttribute.Value, _ReadXmlAttributesExcept(xmlElement, "name")));
                        }
                        break;

                    case XElement xmlElement when xmlElement.Name.LocalName.Equals("c", StringComparison.Ordinal):
                        _AddTextElementIfExists();
                        inlineElements.Add(DocumentationElement.InlineCode(xmlElement.Value, _ReadXmlAttributes(xmlElement)));
                        break;
                }
            }
            _AddTextElementIfExists(trimEnd: true);

            return inlineElements;

            void _Append(string value)
            {
                if (string.IsNullOrEmpty(value))
                    return;

                var index = 0;
                while (index < value.Length)
                {
                    var length = char.IsHighSurrogate(value, index) ? 2 : 1;
                    if (index + length <= value.Length)
                        if (!char.IsWhiteSpace(value, index))
                            textBuilder.Append(value, index, length);
                        else
                        {
                            if ((textBuilder.Length > 0 || inlineElements.Count > 0)
                                && (textBuilder.Length == 0 || textBuilder[textBuilder.Length - 1] != ' '))
                                textBuilder.Append(' ');
                        }
                    index += length;
                }
            }

            void _AddTextElementIfExists(bool trimEnd = false)
            {
                if (trimEnd)
                {
                    var contentLength = textBuilder.Length;
                    while (contentLength > 0 && textBuilder[contentLength - 1] == ' ')
                        contentLength--;
                    textBuilder.Length = contentLength;
                }

                if (textBuilder.Length > 0)
                {
                    inlineElements.Add(DocumentationElement.Text(textBuilder.ToString()));
                    textBuilder.Clear();
                }
            }
        }

        private static int _GetIndentSize(string text)
        {
            var indentSize = 0;
            var index = 0;
            while (index < text.Length && char.IsWhiteSpace(text, index))
            {
                indentSize++;
                index += char.IsHighSurrogate(text, index) ? 2 : 1;
            }
            return indentSize;
        }

        private static IReadOnlyList<string> _ReadCodeBlockLines(string codeBlock)
        {
            var lines = new List<string>();
            string previousLine = null;

            using (var line = _ReadLines(codeBlock).GetEnumerator())
                if (line.MoveNext())
                {
                    previousLine = line.Current;
                    while (line.MoveNext())
                    {
                        lines.Add(previousLine);
                        previousLine = line.Current;
                    }
                    if (!string.IsNullOrWhiteSpace(previousLine))
                        lines.Add(previousLine);
                }

            return lines;
        }

        private static IEnumerable<string> _ReadLines(string text)
        {
            var index = 0;
            var lineBuilder = new StringBuilder();
            while (index < text.Length)
            {
                var length = char.IsHighSurrogate(text, index) ? 2 : 1;
                if (index + length <= text.Length)
                    if (text[index] == '\n')
                    {
                        yield return lineBuilder.ToString();
                        lineBuilder.Clear();
                    }
                    else if (text[index] != '\r')
                        lineBuilder.Append(text, index, length);
                index += length;
            }
            if (lineBuilder.Length > 0)
                yield return lineBuilder.ToString();
        }
    }
}