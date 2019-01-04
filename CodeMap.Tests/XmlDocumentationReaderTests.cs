using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CodeMap;
using CodeMap.Elements;
using Xunit;

namespace CodeMap.Tests
{
    public class XmlDocumentationReaderTests
    {
        private static XmlDocumentationReader _XmlDocumentationReader { get; } = new XmlDocumentationReader();
        private static readonly string _richInlineContent = @"
                plain text
                <paramref name=""parameter reference""/>
                <typeparamref name=""generic parameter reference""/>
                <see cref=""member reference""/>
                <c>some code</c>"
            .Trim();
        private static readonly string _richBlockContent = $@"
                {_richInlineContent}
                <list type=""table"">
                    <item>
                        <description>{_richInlineContent}</description>
                    </item>
                    <item />
                </list>
                <code>
                    some code in a block
                </code>
                <list type=""bullet"">
                    <item>{_richInlineContent}</item>
                    <item>
                        <description>{_richInlineContent}</description>
                    </item>
                    <item />
                </list>
                <list type=""number"">
                    <item>{_richInlineContent}</item>
                    <item>
                        <description>{_richInlineContent}</description>
                    </item>
                    <item />
                </list>
                {_richInlineContent}
                <list>
                    <listheader>{_richInlineContent}</listheader>
                    <item>
                        <term>{_richInlineContent}</term>
                        <description>{_richInlineContent}</description>
                    </item>
                    <item>
                    </item>
                </list>"
            .Trim();
        private static readonly IEnumerable<InlineDocumentationElement> _richInlineElements = new InlineDocumentationElement[]
        {
                DocumentationElement.Text("plain text "),
                DocumentationElement.ParameterReference("parameter reference"),
                DocumentationElement.Text(" "),
                DocumentationElement.GenericParameterReference("generic parameter reference"),
                DocumentationElement.Text(" "),
                DocumentationElement.MemberReference("member reference"),
                DocumentationElement.Text(" "),
                DocumentationElement.InlineCode("some code"),
        };
        private static readonly IEnumerable<BlockDocumentationElement> _richBlockElements = new BlockDocumentationElement[]
        {
                DocumentationElement.Paragraph(
                    _richInlineElements
                ),
                DocumentationElement.Table(
                    DocumentationElement.TableRow(
                        DocumentationElement.TableCell(
                            _richInlineElements
                        )
                    ),
                    DocumentationElement.TableRow(
                        DocumentationElement.TableCell()
                    )
                ),
                DocumentationElement.CodeBlock(
                    "some code in a block"
                ),
                DocumentationElement.UnorderedList(
                    DocumentationElement.ListItem(
                        _richInlineElements
                    ),
                    DocumentationElement.ListItem(
                        _richInlineElements
                    ),
                    DocumentationElement.ListItem()
                ),
                DocumentationElement.OrderedList(
                    DocumentationElement.ListItem(
                        _richInlineElements
                    ),
                    DocumentationElement.ListItem(
                        _richInlineElements
                    ),
                    DocumentationElement.ListItem()
                ),
                DocumentationElement.Paragraph(
                    _richInlineElements
                ),
                DocumentationElement.DefinitionList(
                    _richInlineElements,
                    DocumentationElement.DefinitionListItem(
                        _richInlineElements,
                        _richInlineElements
                    ),
                    DocumentationElement.DefinitionListItem(
                        Enumerable.Empty<InlineDocumentationElement>(),
                        Enumerable.Empty<InlineDocumentationElement>()
                    )
                )
        };

        [Fact]
        public async Task ReadEmptySummary()
        {
            IReadOnlyList<MemberDocumentation> result;
            using (var stringReader = new StringReader(@"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>CodeMap.Tests</name>
    </assembly>
    <members>
        <member name=""canonical name"">
            <summary>
            </summary>
        </member>
    </members>
</doc>
"))
                result = await _XmlDocumentationReader.ReadAsync(stringReader);

            Assert.Equal(1, result.Count);
            _AssertAreEqual(
                DocumentationElement.Summary(),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public async Task ReadSummaryWithOneParagraph()
        {
            IReadOnlyList<MemberDocumentation> result;
            using (var stringReader = new StringReader(@"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>CodeMap.Tests</name>
    </assembly>
    <members>
        <member name=""canonical name"">
            <summary>This is a test.</summary>
        </member>
    </members>
</doc>
"))
                result = await _XmlDocumentationReader.ReadAsync(stringReader);

            Assert.Equal(1, result.Count);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.Paragraph(
                        DocumentationElement.Text("This is a test.")
                    )
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public async Task ReadSummaryWithMultiParagraphSummarySection()
        {
            IReadOnlyList<MemberDocumentation> result;
            using (var stringReader = new StringReader(@"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>CodeMap.Tests</name>
    </assembly>
    <members>
        <member name=""canonical name"">
            <summary>
                This is the first paragraph.
                <para>
                    This is the second paragraph.
                </para>
                This is the third paragraph.
            </summary>
        </member>
    </members>
</doc>
"))
                result = await _XmlDocumentationReader.ReadAsync(stringReader);

            Assert.Equal(1, result.Count);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.Paragraph(
                        DocumentationElement.Text("This is the first paragraph.")
                    ),
                    DocumentationElement.Paragraph(
                        DocumentationElement.Text("This is the second paragraph.")
                    ),
                    DocumentationElement.Paragraph(
                        DocumentationElement.Text("This is the third paragraph.")
                    )
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public async Task ReadSummaryWithEmptyParagraph()
        {
            IReadOnlyList<MemberDocumentation> result;
            using (var stringReader = new StringReader(@"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>CodeMap.Tests</name>
    </assembly>
    <members>
        <member name=""canonical name"">
            <summary>
                <para>
                </para>
            </summary>
        </member>
    </members>
</doc>
"))
                result = await _XmlDocumentationReader.ReadAsync(stringReader);

            Assert.Equal(1, result.Count);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.Paragraph()
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public async Task ReadSummaryWithMultilineParagraphCollapsesWhiteSpace()
        {
            IReadOnlyList<MemberDocumentation> result;
            using (var stringReader = new StringReader(@"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>CodeMap.Tests</name>
    </assembly>
    <members>
        <member name=""canonical name"">
            <summary>
                This is the first line.
                This is the second line.
            </summary>
        </member>
    </members>
</doc>
"))
                result = await _XmlDocumentationReader.ReadAsync(stringReader);

            Assert.Equal(1, result.Count);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.Paragraph(
                        DocumentationElement.Text("This is the first line. This is the second line.")
                    )
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public async Task ReadSummaryWithInlineCode()
        {
            IReadOnlyList<MemberDocumentation> result;
            using (var stringReader = new StringReader(@"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>CodeMap.Tests</name>
    </assembly>
    <members>
        <member name=""canonical name"">
            <summary>
                This contains <c>some code</c>.
            </summary>
        </member>
    </members>
</doc>
"))
                result = await _XmlDocumentationReader.ReadAsync(stringReader);

            Assert.Equal(1, result.Count);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.Paragraph(
                        DocumentationElement.Text("This contains "),
                        DocumentationElement.InlineCode("some code"),
                        DocumentationElement.Text(".")
                    )
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public async Task ReadSummaryDoesNotCollapseInlineCodeWhiteSpace()
        {
            IReadOnlyList<MemberDocumentation> result;
            using (var stringReader = new StringReader(@"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>CodeMap.Tests</name>
    </assembly>
    <members>
        <member name=""canonical name"">
            <summary>
                <c>  some  code  </c>
            </summary>
        </member>
    </members>
</doc>
"))
                result = await _XmlDocumentationReader.ReadAsync(stringReader);

            Assert.Equal(1, result.Count);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.Paragraph(
                        DocumentationElement.InlineCode("  some  code  ")
                    )
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public async Task ReadSummaryWithMemberReference()
        {
            IReadOnlyList<MemberDocumentation> result;
            using (var stringReader = new StringReader(@"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>CodeMap.Tests</name>
    </assembly>
    <members>
        <member name=""canonical name"">
            <summary>
                This contains a <see cref=""referred canonical name""/> reference.
            </summary>
        </member>
    </members>
</doc>
"))
                result = await _XmlDocumentationReader.ReadAsync(stringReader);

            Assert.Equal(1, result.Count);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.Paragraph(
                        DocumentationElement.Text("This contains a "),
                        DocumentationElement.MemberReference("referred canonical name"),
                        DocumentationElement.Text(" reference.")
                    )
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public async Task ReadSummaryWithParameterReference()
        {
            IReadOnlyList<MemberDocumentation> result;
            using (var stringReader = new StringReader(@"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>CodeMap.Tests</name>
    </assembly>
    <members>
        <member name=""canonical name"">
            <summary>
                This contains a <paramref name=""referred parameter name""/> reference.
            </summary>
        </member>
    </members>
</doc>
"))
                result = await _XmlDocumentationReader.ReadAsync(stringReader);

            Assert.Equal(1, result.Count);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.Paragraph(
                        DocumentationElement.Text("This contains a "),
                        DocumentationElement.ParameterReference("referred parameter name"),
                        DocumentationElement.Text(" reference.")
                    )
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public async Task ReadSummaryCodeBlock()
        {
            IReadOnlyList<MemberDocumentation> result;
            using (var stringReader = new StringReader(@"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>CodeMap.Tests</name>
    </assembly>
    <members>
        <member name=""canonical name"">
            <summary>
                <code>
                    this is a code block
                </code>
            </summary>
        </member>
    </members>
</doc>
"))
                result = await _XmlDocumentationReader.ReadAsync(stringReader);

            Assert.Equal(1, result.Count);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.CodeBlock("this is a code block")
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public async Task ReadSummaryCodeBlockWithMultilineAndIndentedCode()
        {
            IReadOnlyList<MemberDocumentation> result;
            using (var stringReader = new StringReader(@"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>CodeMap.Tests</name>
    </assembly>
    <members>
        <member name=""canonical name"">
            <summary>
                <code>
                    this is a code block
                        with indentation
                    and 3 lines
                </code>
            </summary>
        </member>
    </members>
</doc>
"))
                result = await _XmlDocumentationReader.ReadAsync(stringReader);

            Assert.Equal(1, result.Count);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.CodeBlock(@"
this is a code block
    with indentation
and 3 lines".Trim().Replace("\r", string.Empty)
                    )
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public async Task ReadSummaryCodeBlockWithMultilineNormalizesSpacesToLineFeed()
        {
            IReadOnlyList<MemberDocumentation> result;
            using (var stringReader = new StringReader(@"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>CodeMap.Tests</name>
    </assembly>
    <members>
        <member name=""canonical name"">
            <summary>
                <code>"
                + "first line\r\nsecond line \r\nthird line\r\nfourth line \r\n fifth line"
                + @"</code>
            </summary>
        </member>
    </members>
</doc>
"))
                result = await _XmlDocumentationReader.ReadAsync(stringReader);

            Assert.Equal(1, result.Count);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.CodeBlock(@"
first line
second line 
third line
fourth line 
 fifth line".Trim().Replace("\r", string.Empty)
                    )
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public async Task ReadSummaryCodeBlockOnOneLinePreservesWhiteSpace()
        {
            IReadOnlyList<MemberDocumentation> result;
            using (var stringReader = new StringReader(@"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>CodeMap.Tests</name>
    </assembly>
    <members>
        <member name=""canonical name"">
            <summary>
                <code>  this contains some white space  </code>
            </summary>
        </member>
    </members>
</doc>
"))
                result = await _XmlDocumentationReader.ReadAsync(stringReader);

            Assert.Equal(1, result.Count);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.CodeBlock("  this contains some white space  ")
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public async Task ReadSummaryCodeBlockPreservesWhiteSpaceOnTheSameLineWithCodeTag()
        {
            IReadOnlyList<MemberDocumentation> result;
            using (var stringReader = new StringReader(@"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>CodeMap.Tests</name>
    </assembly>
    <members>
        <member name=""canonical name"">
            <summary>
                <code>  this contains some white space  
                    the second line
                </code>
            </summary>
        </member>
    </members>
</doc>
"))
                result = await _XmlDocumentationReader.ReadAsync(stringReader);

            Assert.Equal(1, result.Count);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.CodeBlock("  this contains some white space  \nthe second line")
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public async Task ReadSummaryCodeBlockWithCodeOnSameLineWithCodeEndingTag()
        {
            IReadOnlyList<MemberDocumentation> result;
            using (var stringReader = new StringReader(@"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>CodeMap.Tests</name>
    </assembly>
    <members>
        <member name=""canonical name"">
            <summary>
                <code>start
                    end</code>
            </summary>
        </member>
    </members>
</doc>
"))
                result = await _XmlDocumentationReader.ReadAsync(stringReader);

            Assert.Equal(1, result.Count);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.CodeBlock("start\nend")
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public async Task ReadSummaryUnorderedList()
        {
            IReadOnlyList<MemberDocumentation> result;
            using (var stringReader = new StringReader(@"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>CodeMap.Tests</name>
    </assembly>
    <members>
        <member name=""canonical name"">
            <summary>
                <list>
                    <item>The first item</item>
                    <item><description>The second item</description></item>
                    <item>
                        <description>The third item</description>
                    </item>
                    <item>
                        <description>
                            The fourth item
                        </description>
                    </item>
                </list>
            </summary>
        </member>
    </members>
</doc>
"))
                result = await _XmlDocumentationReader.ReadAsync(stringReader);

            Assert.Equal(1, result.Count);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.UnorderedList(
                        DocumentationElement.ListItem(
                            DocumentationElement.Text("The first item")
                        ),
                        DocumentationElement.ListItem(
                            DocumentationElement.Text("The second item")
                        ),
                        DocumentationElement.ListItem(
                            DocumentationElement.Text("The third item")
                        ),
                        DocumentationElement.ListItem(
                            DocumentationElement.Text("The fourth item")
                        )
                    )
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public async Task ReadSummaryEmptyUnorderedList()
        {
            IReadOnlyList<MemberDocumentation> result;
            using (var stringReader = new StringReader(@"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>CodeMap.Tests</name>
    </assembly>
    <members>
        <member name=""canonical name"">
            <summary>
                <list>
                </list>
            </summary>
        </member>
    </members>
</doc>
"))
                result = await _XmlDocumentationReader.ReadAsync(stringReader);

            Assert.Equal(1, result.Count);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.UnorderedList()
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public async Task ReadSummaryExplicitUnorderedList()
        {
            IReadOnlyList<MemberDocumentation> result;
            using (var stringReader = new StringReader(@"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>CodeMap.Tests</name>
    </assembly>
    <members>
        <member name=""canonical name"">
            <summary>
                <list type=""bullet"">
                    <item>The first item</item>
                    <item><description>The second item</description></item>
                    <item>
                        <description>The third item</description>
                    </item>
                    <item>
                        <description>
                            The fourth item
                        </description>
                    </item>
                </list>
            </summary>
        </member>
    </members>
</doc>
"))
                result = await _XmlDocumentationReader.ReadAsync(stringReader);

            Assert.Equal(1, result.Count);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.UnorderedList(
                        DocumentationElement.ListItem(
                            DocumentationElement.Text("The first item")
                        ),
                        DocumentationElement.ListItem(
                            DocumentationElement.Text("The second item")
                        ),
                        DocumentationElement.ListItem(
                            DocumentationElement.Text("The third item")
                        ),
                        DocumentationElement.ListItem(
                            DocumentationElement.Text("The fourth item")
                        )
                    )
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public async Task ReadSummaryOrderedList()
        {
            IReadOnlyList<MemberDocumentation> result;
            using (var stringReader = new StringReader(@"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>CodeMap.Tests</name>
    </assembly>
    <members>
        <member name=""canonical name"">
            <summary>
                <list type=""number"">
                    <item>The first item</item>
                    <item><description>The second item</description></item>
                    <item>
                        <description>The third item</description>
                    </item>
                    <item>
                        <description>
                            The fourth item
                        </description>
                    </item>
                </list>
            </summary>
        </member>
    </members>
</doc>
"))
                result = await _XmlDocumentationReader.ReadAsync(stringReader);

            Assert.Equal(1, result.Count);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.OrderedList(
                        DocumentationElement.ListItem(
                            DocumentationElement.Text("The first item")
                        ),
                        DocumentationElement.ListItem(
                            DocumentationElement.Text("The second item")
                        ),
                        DocumentationElement.ListItem(
                            DocumentationElement.Text("The third item")
                        ),
                        DocumentationElement.ListItem(
                            DocumentationElement.Text("The fourth item")
                        )
                    )
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public async Task ReadSummaryEmptyOrderedList()
        {
            IReadOnlyList<MemberDocumentation> result;
            using (var stringReader = new StringReader(@"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>CodeMap.Tests</name>
    </assembly>
    <members>
        <member name=""canonical name"">
            <summary>
                <list type=""number"">
                </list>
            </summary>
        </member>
    </members>
</doc>
"))
                result = await _XmlDocumentationReader.ReadAsync(stringReader);

            Assert.Equal(1, result.Count);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.OrderedList()
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public async Task ReadSummaryDefinitionList()
        {
            IReadOnlyList<MemberDocumentation> result;
            using (var stringReader = new StringReader(@"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>CodeMap.Tests</name>
    </assembly>
    <members>
        <member name=""canonical name"">
            <summary>
                <list>
                    <item><term>term 1</term><description>The first item</description></item>
                    <item>
                        <term>term 2</term>
                        <description>The second item</description>
                    </item>
                    <item>
                        <term>
                            term 3
                        </term>
                        <description>
                            The third item
                        </description>
                    </item>
                    <item>
                        <description>
                            The fourth item
                        </description>
                    </item>
                    <item>
                        <term>
                            term 5
                        </term>
                    </item>
                </list>
            </summary>
        </member>
    </members>
</doc>
"))
                result = await _XmlDocumentationReader.ReadAsync(stringReader);

            Assert.Equal(1, result.Count);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.DefinitionList(
                        DocumentationElement.DefinitionListItem(
                            new[] { DocumentationElement.Text("term 1") },
                            DocumentationElement.Text("The first item")
                        ),
                        DocumentationElement.DefinitionListItem(
                            new[] { DocumentationElement.Text("term 2") },
                            DocumentationElement.Text("The second item")
                        ),
                        DocumentationElement.DefinitionListItem(
                            new[] { DocumentationElement.Text("term 3") },
                            DocumentationElement.Text("The third item")
                        ),
                        DocumentationElement.DefinitionListItem(
                            Enumerable.Empty<InlineDocumentationElement>(),
                            DocumentationElement.Text("The fourth item")
                        ),
                        DocumentationElement.DefinitionListItem(
                            new[] { DocumentationElement.Text("term 5") },
                            Enumerable.Empty<InlineDocumentationElement>()
                        )
                    )
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public async Task ReadSummaryDefinitionListWithTypeBullet()
        {
            IReadOnlyList<MemberDocumentation> result;
            using (var stringReader = new StringReader(@"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>CodeMap.Tests</name>
    </assembly>
    <members>
        <member name=""canonical name"">
            <summary>
                <list type=""bullet"">
                    <item>
                        <term>term 1</term>
                        <description>The first item</description>
                    </item>
                    <item>
                        <term>term 2</term>
                        <description>The second item</description>
                    </item>
                </list>
            </summary>
        </member>
    </members>
</doc>
"))
                result = await _XmlDocumentationReader.ReadAsync(stringReader);

            Assert.Equal(1, result.Count);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.DefinitionList(
                        DocumentationElement.DefinitionListItem(
                            new[] { DocumentationElement.Text("term 1") },
                            DocumentationElement.Text("The first item")
                        ),
                        DocumentationElement.DefinitionListItem(
                            new[] { DocumentationElement.Text("term 2") },
                            DocumentationElement.Text("The second item")
                        )
                    )
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public async Task ReadSummaryDefinitionListWithTypeNumber()
        {
            IReadOnlyList<MemberDocumentation> result;
            using (var stringReader = new StringReader(@"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>CodeMap.Tests</name>
    </assembly>
    <members>
        <member name=""canonical name"">
            <summary>
                <list type=""bullet"">
                    <item>
                        <term>term 1</term>
                        <description>The first item</description>
                    </item>
                    <item>
                        <term>term 2</term>
                        <description>The second item</description>
                    </item>
                </list>
            </summary>
        </member>
    </members>
</doc>
"))
                result = await _XmlDocumentationReader.ReadAsync(stringReader);

            Assert.Equal(1, result.Count);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.DefinitionList(
                        DocumentationElement.DefinitionListItem(
                            new[] { DocumentationElement.Text("term 1") },
                            DocumentationElement.Text("The first item")
                        ),
                        DocumentationElement.DefinitionListItem(
                            new[] { DocumentationElement.Text("term 2") },
                            DocumentationElement.Text("The second item")
                        )
                    )
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public async Task ReadSummaryDefinitionListWithTitle()
        {
            IReadOnlyList<MemberDocumentation> result;
            using (var stringReader = new StringReader(@"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>CodeMap.Tests</name>
    </assembly>
    <members>
        <member name=""canonical name"">
            <summary>
                <list>
                    <listheader>
                        this is a title
                    </listheader>
                    <item>
                        <term>term 1</term>
                        <description>The first item</description>
                    </item>
                    <item>
                        <term>term 2</term>
                        <description>The second item</description>
                    </item>
                </list>
            </summary>
        </member>
    </members>
</doc>
"))
                result = await _XmlDocumentationReader.ReadAsync(stringReader);

            Assert.Equal(1, result.Count);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.DefinitionList(
                        new[] { DocumentationElement.Text("this is a title") },
                        DocumentationElement.DefinitionListItem(
                            new[] { DocumentationElement.Text("term 1") },
                            DocumentationElement.Text("The first item")
                        ),
                        DocumentationElement.DefinitionListItem(
                            new[] { DocumentationElement.Text("term 2") },
                            DocumentationElement.Text("The second item")
                        )
                    )
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public async Task ReadSummaryDefinitionListWithTitleUsingTerm()
        {
            IReadOnlyList<MemberDocumentation> result;
            using (var stringReader = new StringReader(@"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>CodeMap.Tests</name>
    </assembly>
    <members>
        <member name=""canonical name"">
            <summary>
                <list>
                    <listheader>
                        <term>this is a title</term>
                    </listheader>
                    <item>
                        <term>term 1</term>
                        <description>The first item</description>
                    </item>
                    <item>
                        <term>term 2</term>
                        <description>The second item</description>
                    </item>
                </list>
            </summary>
        </member>
    </members>
</doc>
"))
                result = await _XmlDocumentationReader.ReadAsync(stringReader);

            Assert.Equal(1, result.Count);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.DefinitionList(
                        new[] { DocumentationElement.Text("this is a title") },
                        DocumentationElement.DefinitionListItem(
                            new[] { DocumentationElement.Text("term 1") },
                            DocumentationElement.Text("The first item")
                        ),
                        DocumentationElement.DefinitionListItem(
                            new[] { DocumentationElement.Text("term 2") },
                            DocumentationElement.Text("The second item")
                        )
                    )
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public async Task ReadSummaryEmptyDefinitionList()
        {
            IReadOnlyList<MemberDocumentation> result;
            using (var stringReader = new StringReader(@"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>CodeMap.Tests</name>
    </assembly>
    <members>
        <member name=""canonical name"">
            <summary>
                <list>
                    <listheader>
                        this is a title
                    </listheader>
                </list>
            </summary>
        </member>
    </members>
</doc>
"))
                result = await _XmlDocumentationReader.ReadAsync(stringReader);

            Assert.Equal(1, result.Count);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.DefinitionList(
                        new[] { DocumentationElement.Text("this is a title") }
                    )
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public async Task ReadSummaryEmptyDefinitionListWithEmptyTitle()
        {
            IReadOnlyList<MemberDocumentation> result;
            using (var stringReader = new StringReader(@"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>CodeMap.Tests</name>
    </assembly>
    <members>
        <member name=""canonical name"">
            <summary>
                <list>
                    <listheader>
                    </listheader>
                </list>
            </summary>
        </member>
    </members>
</doc>
"))
                result = await _XmlDocumentationReader.ReadAsync(stringReader);

            Assert.Equal(1, result.Count);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.DefinitionList(
                        Enumerable.Empty<InlineDocumentationElement>()
                    )
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public async Task ReadSummaryTable()
        {
            IReadOnlyList<MemberDocumentation> result;
            using (var stringReader = new StringReader(@"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>CodeMap.Tests</name>
    </assembly>
    <members>
        <member name=""canonical name"">
            <summary>
                <list type=""table"">
                    <listheader>
                        <term>first column</term>
                        <term>second column</term>
                    </listheader>
                    <item>
                        <description>First row, first cell</description>
                        <description>First row, second cell</description>
                    </item>
                    <item>
                        <description>Second row, first cell</description>
                        <description>Second row, second cell</description>
                    </item>
                </list>
            </summary>
        </member>
    </members>
</doc>
"))
                result = await _XmlDocumentationReader.ReadAsync(stringReader);

            Assert.Equal(1, result.Count);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.Table(
                        new[]
                        {
                            DocumentationElement.TableColumn(
                                DocumentationElement.Text("first column")
                            ),
                            DocumentationElement.TableColumn(
                                DocumentationElement.Text("second column")
                            )
                        },
                        DocumentationElement.TableRow(
                            DocumentationElement.TableCell(
                                DocumentationElement.Text("First row, first cell")
                            ),
                            DocumentationElement.TableCell(
                                DocumentationElement.Text("First row, second cell")
                            )
                        ),
                        DocumentationElement.TableRow(
                            DocumentationElement.TableCell(
                                DocumentationElement.Text("Second row, first cell")
                            ),
                            DocumentationElement.TableCell(
                                DocumentationElement.Text("Second row, second cell")
                            )
                        )
                    )
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public async Task ReadSummaryTableWithoutEqualNumberOfColumns()
        {
            IReadOnlyList<MemberDocumentation> result;
            using (var stringReader = new StringReader(@"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>CodeMap.Tests</name>
    </assembly>
    <members>
        <member name=""canonical name"">
            <summary>
                <list type=""table"">
                    <listheader>
                        <term>first column</term>
                    </listheader>
                    <item>
                        <description>First row, first cell</description>
                        <description>First row, second cell</description>
                    </item>
                    <item>
                        <description>Second row, first cell</description>
                    </item>
                </list>
            </summary>
        </member>
    </members>
</doc>
"))
                result = await _XmlDocumentationReader.ReadAsync(stringReader);

            Assert.Equal(1, result.Count);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.Table(
                        new[]
                        {
                            DocumentationElement.TableColumn(
                                DocumentationElement.Text("first column")
                            ),
                            DocumentationElement.TableColumn(
                                Enumerable.Empty<InlineDocumentationElement>()
                            )
                        },
                        DocumentationElement.TableRow(
                            DocumentationElement.TableCell(
                                DocumentationElement.Text("First row, first cell")
                            ),
                            DocumentationElement.TableCell(
                                DocumentationElement.Text("First row, second cell")
                            )
                        ),
                        DocumentationElement.TableRow(
                            DocumentationElement.TableCell(
                                DocumentationElement.Text("Second row, first cell")
                            ),
                            DocumentationElement.TableCell(
                                Enumerable.Empty<InlineDocumentationElement>()
                            )
                        )
                    )
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public async Task ReadSummaryTableWithoutHeader()
        {
            IReadOnlyList<MemberDocumentation> result;
            using (var stringReader = new StringReader(@"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>CodeMap.Tests</name>
    </assembly>
    <members>
        <member name=""canonical name"">
            <summary>
                <list type=""table"">
                    <item>
                        <description>First row, first cell</description>
                        <description>First row, second cell</description>
                    </item>
                    <item>
                        <description>Second row, first cell</description>
                    </item>
                </list>
            </summary>
        </member>
    </members>
</doc>
"))
                result = await _XmlDocumentationReader.ReadAsync(stringReader);

            Assert.Equal(1, result.Count);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.Table(
                        DocumentationElement.TableRow(
                            DocumentationElement.TableCell(
                                DocumentationElement.Text("First row, first cell")
                            ),
                            DocumentationElement.TableCell(
                                DocumentationElement.Text("First row, second cell")
                            )
                        ),
                        DocumentationElement.TableRow(
                            DocumentationElement.TableCell(
                                DocumentationElement.Text("Second row, first cell")
                            ),
                            DocumentationElement.TableCell(
                                Enumerable.Empty<InlineDocumentationElement>()
                            )
                        )
                    )
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public async Task ReadSummaryEmptyTable()
        {
            IReadOnlyList<MemberDocumentation> result;
            using (var stringReader = new StringReader(@"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>CodeMap.Tests</name>
    </assembly>
    <members>
        <member name=""canonical name"">
            <summary>
                <list type=""table"">
                </list>
            </summary>
        </member>
    </members>
</doc>
"))
                result = await _XmlDocumentationReader.ReadAsync(stringReader);

            Assert.Equal(1, result.Count);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.Table()
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public async Task ReadComplexSummary()
        {
            IReadOnlyList<MemberDocumentation> result;
            using (var stringReader = new StringReader($@"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>CodeMap.Tests</name>
    </assembly>
    <members>
        <member name=""canonical name"">
            <summary>
                {_richBlockContent}
            </summary>
        </member>
    </members>
</doc>
"))
                result = await _XmlDocumentationReader.ReadAsync(stringReader);

            Assert.Equal(1, result.Count);
            _AssertAreEqual(
                DocumentationElement.Summary(_richBlockElements),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public async Task ReadComplexTypeParameters()
        {
            IReadOnlyList<MemberDocumentation> result;
            using (var stringReader = new StringReader($@"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>CodeMap.Tests</name>
    </assembly>
    <members>
        <member name=""canonical name"">
            <typeparam name=""typeParameter1"">{_richBlockContent}</typeparam>
            <typeparam name=""typeParameter2"">{_richBlockContent}</typeparam>
            <typeparam name=""typeParameter2"">{_richBlockContent}</typeparam>
        </member>
    </members>
</doc>
"))
                result = await _XmlDocumentationReader.ReadAsync(stringReader);

            Assert.Equal(1, result.Count);
            _AssertAreEqual(
                new Dictionary<string, IEnumerable<BlockDocumentationElement>>(StringComparer.Ordinal)
                {
                    { "typeParameter1", _richBlockElements },
                    { "typeParameter2", _richBlockElements.Concat(_richBlockElements) }
                },
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").GenericParameters
            );
        }

        [Fact]
        public async Task ReadComplexParameters()
        {
            IReadOnlyList<MemberDocumentation> result;
            using (var stringReader = new StringReader($@"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>CodeMap.Tests</name>
    </assembly>
    <members>
        <member name=""canonical name"">
            <param name=""parameter1"">{_richBlockContent}</param>
            <param name=""parameter2"">{_richBlockContent}</param>
            <param name=""parameter2"">{_richBlockContent}</param>
        </member>
    </members>
</doc>
"))
                result = await _XmlDocumentationReader.ReadAsync(stringReader);

            Assert.Equal(1, result.Count);
            _AssertAreEqual(
                new Dictionary<string, IEnumerable<BlockDocumentationElement>>(StringComparer.Ordinal)
                {
                    { "parameter1", _richBlockElements },
                    { "parameter2", _richBlockElements.Concat(_richBlockElements) }
                },
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Parameters
            );
        }

        [Fact]
        public async Task ReadComplexReturns()
        {
            IReadOnlyList<MemberDocumentation> result;
            using (var stringReader = new StringReader($@"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>CodeMap.Tests</name>
    </assembly>
    <members>
        <member name=""canonical name"">
            <returns>{_richBlockContent}</returns>
        </member>
    </members>
</doc>
"))
                result = await _XmlDocumentationReader.ReadAsync(stringReader);

            Assert.Equal(1, result.Count);
            _AssertAreEqual(
                _richBlockElements,
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Returns
            );
        }

        [Fact]
        public async Task ReadComplexExceptions()
        {
            IReadOnlyList<MemberDocumentation> result;
            using (var stringReader = new StringReader($@"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>CodeMap.Tests</name>
    </assembly>
    <members>
        <member name=""canonical name"">
            <exception cref=""exception1"">{_richBlockContent}</exception>
            <exception cref=""exception2"">{_richBlockContent}</exception>
            <exception cref=""exception2"">{_richBlockContent}</exception>
        </member>
    </members>
</doc>
"))
                result = await _XmlDocumentationReader.ReadAsync(stringReader);

            Assert.Equal(1, result.Count);
            _AssertAreEqual(
                new Dictionary<string, IEnumerable<BlockDocumentationElement>>(StringComparer.Ordinal)
                {
                    { "exception1", _richBlockElements },
                    { "exception2", _richBlockElements.Concat(_richBlockElements) }
                },
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Exceptions
            );
        }

        [Fact]
        public async Task ReadComplexRemarks()
        {
            IReadOnlyList<MemberDocumentation> result;
            using (var stringReader = new StringReader($@"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>CodeMap.Tests</name>
    </assembly>
    <members>
        <member name=""canonical name"">
            <remarks>
                {_richBlockContent}
            </remarks>
        </member>
    </members>
</doc>
"))
                result = await _XmlDocumentationReader.ReadAsync(stringReader);

            Assert.Equal(1, result.Count);
            _AssertAreEqual(
                DocumentationElement.Remarks(_richBlockElements),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Remarks
            );
        }

        [Fact]
        public async Task ReadComplexExamples()
        {
            IReadOnlyList<MemberDocumentation> result;
            using (var stringReader = new StringReader($@"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>CodeMap.Tests</name>
    </assembly>
    <members>
        <member name=""canonical name"">
            <example>
                {_richBlockContent}
            </example>
            <example>
                {_richBlockContent}
                {_richBlockContent}
            </example>
        </member>
    </members>
</doc>
"))
                result = await _XmlDocumentationReader.ReadAsync(stringReader);

            Assert.Equal(1, result.Count);
            _AssertAreEqual(
                new[]
                {
                    DocumentationElement.Example(_richBlockElements),
                    DocumentationElement.Example(_richBlockElements.Concat(_richBlockElements))
                },
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Examples
            );
        }

        [Fact]
        public async Task ReadComplexValue()
        {
            IReadOnlyList<MemberDocumentation> result;
            using (var stringReader = new StringReader($@"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>CodeMap.Tests</name>
    </assembly>
    <members>
        <member name=""canonical name"">
            <value>
                {_richBlockContent}
            </value>
        </member>
    </members>
</doc>
"))
                result = await _XmlDocumentationReader.ReadAsync(stringReader);

            Assert.Equal(1, result.Count);
            _AssertAreEqual(
                DocumentationElement.Value(_richBlockElements),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Value
            );
        }

        [Fact]
        public async Task ReadRelatedMembersList()
        {
            IReadOnlyList<MemberDocumentation> result;
            using (var stringReader = new StringReader($@"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>CodeMap.Tests</name>
    </assembly>
    <members>
        <member name=""canonical name"">
            <seealso cref=""member1"" />
            <seealso cref=""member2"" />
            <seealso cref=""member2"" />
        </member>
    </members>
</doc>
"))
                result = await _XmlDocumentationReader.ReadAsync(stringReader);

            Assert.Equal(1, result.Count);
            _AssertAreEqual(
                DocumentationElement.RelatedMembersList(
                    DocumentationElement.MemberReference("member1"),
                    DocumentationElement.MemberReference("member2"),
                    DocumentationElement.MemberReference("member2")
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").RelatedMembersList
            );
        }

        [Fact]
        public async Task ReadEmptyMemberDocumentation()
        {
            IReadOnlyList<MemberDocumentation> result;
            using (var stringReader = new StringReader(@"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>CodeMap.Tests</name>
    </assembly>
    <members>
        <member name=""canonical name"" />
    </members>
</doc>
"))
                result = await _XmlDocumentationReader.ReadAsync(stringReader);

            Assert.Equal(1, result.Count);
            var emptyMemberDocumentation = result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name");
            Assert.Null(emptyMemberDocumentation.Summary);
            Assert.Empty(emptyMemberDocumentation.GenericParameters);
            Assert.Empty(emptyMemberDocumentation.Parameters);
            Assert.Null(emptyMemberDocumentation.Returns);
            Assert.Empty(emptyMemberDocumentation.Exceptions);
            Assert.Null(emptyMemberDocumentation.Remarks);
            Assert.Empty(emptyMemberDocumentation.Examples);
            Assert.Null(emptyMemberDocumentation.Value);
            Assert.Empty(emptyMemberDocumentation.RelatedMembersList);
        }

        [Fact]
        public async Task ReadingFromNullThrowsException()
        {
            var exception = await Assert.ThrowsAsync<ArgumentNullException>("textReader", () => _XmlDocumentationReader.ReadAsync(null));
            Assert.Equal(new ArgumentNullException("textReader").Message, exception.Message);
        }

        private static void _AssertAreEqual(SummaryDocumentationElement expected, SummaryDocumentationElement actual)
            => _AssertAreEqual(expected.Content, actual.Content);

        private static void _AssertAreEqual(IReadOnlyDictionary<string, IEnumerable<BlockDocumentationElement>> expected, ILookup<string, BlockDocumentationElement> actual)
        {
            Assert.Equal(expected.Count, actual.Count);

            foreach (var pair in expected
                .OrderBy(expectedParameter => expectedParameter.Key, StringComparer.Ordinal)
                .Zip(
                    actual.OrderBy(actualParameter => actualParameter.Key, StringComparer.Ordinal),
                    (expectedParameter, actualParameter) => new
                    {
                        ExpectedParameter = expectedParameter.Key,
                        ExpectedContent = expectedParameter.Value.ToList(),
                        ActualParameter = actualParameter.Key,
                        ActualContent = actualParameter.ToList()
                    }
                )
            )
            {
                Assert.Equal(pair.ExpectedParameter, pair.ActualParameter);
                _AssertAreEqual(pair.ExpectedContent, pair.ActualContent);
            }
        }

        private static void _AssertAreEqual(RemarksDocumentationElement expected, RemarksDocumentationElement actual)
            => _AssertAreEqual(expected.Content, actual.Content);

        private static void _AssertAreEqual(ExampleDocumentationElement expected, ExampleDocumentationElement actual)
            => _AssertAreEqual(expected.Content, actual.Content);

        private static void _AssertAreEqual(IEnumerable<ExampleDocumentationElement> expected, IEnumerable<ExampleDocumentationElement> actual)
        {
            Assert.Equal(expected.Count(), actual.Count());
            foreach (var pair in expected.Zip(actual, (expectedElement, actualElement) => new { ExpectedElement = expectedElement, ActualElement = actualElement }))
                _AssertAreEqual(pair.ExpectedElement, pair.ActualElement);
        }

        private static void _AssertAreEqual(ValueDocumentationElement expected, ValueDocumentationElement actual)
            => _AssertAreEqual(expected.Content, actual.Content);

        private static void _AssertAreEqual(BlockDocumentationElement expectedContent, BlockDocumentationElement actualContent)
        {
            Assert.IsType(expectedContent.GetType(), actualContent);
            switch (expectedContent)
            {
                case ParagraphDocumentationElement expectedParagraph:
                    _AssertAreEqual(expectedParagraph, (ParagraphDocumentationElement)actualContent);
                    break;

                case CodeBlockDocumentationElement expectedCodeBlock:
                    _AssertAreEqual(expectedCodeBlock, (CodeBlockDocumentationElement)actualContent);
                    break;

                case OrderedListDocumentationElement expectedOrderedList:
                    _AssertAreEqual(expectedOrderedList, (OrderedListDocumentationElement)actualContent);
                    break;

                case UnorderedListDocumentationElement expectedUnorderedList:
                    _AssertAreEqual(expectedUnorderedList, (UnorderedListDocumentationElement)actualContent);
                    break;

                case DefinitionListDocumentationElement expectedDefinitionList:
                    _AssertAreEqual(expectedDefinitionList, (DefinitionListDocumentationElement)actualContent);
                    break;

                case TableDocumentationElement expectedTable:
                    _AssertAreEqual(expectedTable, (TableDocumentationElement)actualContent);
                    break;
            }
        }

        private static void _AssertAreEqual(IEnumerable<BlockDocumentationElement> expected, IEnumerable<BlockDocumentationElement> actual)
        {
            Assert.Equal(expected.Count(), actual.Count());
            foreach (var pair in expected.Zip(actual, (expectedElement, actualElement) => new { ExpectedElement = expectedElement, ActualElement = actualElement }))
                _AssertAreEqual(pair.ExpectedElement, pair.ActualElement);
        }

        private static void _AssertAreEqual(ParagraphDocumentationElement expected, ParagraphDocumentationElement actual)
            => _AssertAreEqual(expected.Content, actual.Content);

        private static void _AssertAreEqual(CodeBlockDocumentationElement expected, CodeBlockDocumentationElement actual)
        {
            Assert.Equal(expected.Code, actual.Code);
        }

        private static void _AssertAreEqual(UnorderedListDocumentationElement expected, UnorderedListDocumentationElement actual)
        {
            Assert.Equal(expected.Items.Count, actual.Items.Count);
            foreach (var pair in expected.Items.Zip(actual.Items, (expectedListItem, actualListItem) => new { ExpectedListItem = expectedListItem, ActualListItem = actualListItem }))
                _AssertAreEqual(pair.ExpectedListItem, pair.ActualListItem);
        }

        private static void _AssertAreEqual(OrderedListDocumentationElement expected, OrderedListDocumentationElement actual)
        {
            Assert.Equal(expected.Items.Count, actual.Items.Count);
            foreach (var pair in expected.Items.Zip(actual.Items, (expectedListItem, actualListItem) => new { ExpectedListItem = expectedListItem, ActualListItem = actualListItem }))
                _AssertAreEqual(pair.ExpectedListItem, pair.ActualListItem);
        }

        private static void _AssertAreEqual(ListItemDocumentationElement expected, ListItemDocumentationElement actual)
            => _AssertAreEqual(expected.Content, actual.Content);

        private static void _AssertAreEqual(DefinitionListDocumentationElement expected, DefinitionListDocumentationElement actual)
        {
            if (expected.ListTitle == null)
                Assert.Null(actual.ListTitle);
            else
                _AssertAreEqual(expected.ListTitle, actual.ListTitle);

            Assert.Equal(expected.Items.Count, actual.Items.Count);
            foreach (var pair in expected.Items.Zip(actual.Items, (expectedListItem, actualListItem) => new { ExpectedListItem = expectedListItem, ActualListItem = actualListItem }))
                _AssertAreEqual(pair.ExpectedListItem, pair.ActualListItem);
        }

        private static void _AssertAreEqual(DefinitionListItemDocumentationElement expected, DefinitionListItemDocumentationElement actual)
        {
            _AssertAreEqual(expected.Term, actual.Term);
            _AssertAreEqual(expected.Description, actual.Description);
        }

        private static void _AssertAreEqual(TableDocumentationElement expected, TableDocumentationElement actual)
        {
            Assert.Equal(expected.Columns.Count, actual.Columns.Count);
            foreach (var pair in expected.Columns.Zip(actual.Columns, (expectedColumn, actualColumn) => new { ExpectedColumn = expectedColumn, ActualColumn = actualColumn }))
                _AssertAreEqual(pair.ExpectedColumn, pair.ActualColumn);

            Assert.Equal(expected.Rows.Count, actual.Rows.Count);
            foreach (var pair in expected.Rows.Zip(actual.Rows, (expectedRow, actualRow) => new { ExpectedRow = expectedRow, ActualRow = actualRow }))
                _AssertAreEqual(pair.ExpectedRow, pair.ActualRow);
        }

        private static void _AssertAreEqual(TableColumnDocumentationElement expected, TableColumnDocumentationElement actual)
            => _AssertAreEqual(expected.Name, actual.Name);

        private static void _AssertAreEqual(TableRowDocumentationElement expected, TableRowDocumentationElement actual)
        {
            Assert.Equal(expected.Cells.Count, actual.Cells.Count);
            foreach (var pair in expected.Cells.Zip(actual.Cells, (expectedCell, actualCell) => new { ExpectedCell = expectedCell, ActualCell = actualCell }))
                _AssertAreEqual(pair.ExpectedCell, pair.ActualCell);
        }

        private static void _AssertAreEqual(TableCellDocumentationElement expected, TableCellDocumentationElement actual)
            => _AssertAreEqual(expected.Content, actual.Content);

        private static void _AssertAreEqual(InlineDocumentationElement expected, InlineDocumentationElement actual)
        {
            Assert.IsType(expected.GetType(), actual);
            switch (expected)
            {
                case TextDocumentationElement expectedText:
                    _AssertAreEqual(expectedText, (TextDocumentationElement)actual);
                    break;

                case InlineCodeDocumentationElement expectedInlineCode:
                    _AssertAreEqual(expectedInlineCode, (InlineCodeDocumentationElement)actual);
                    break;

                case MemberInfoReferenceDocumentationElement expectedMemberInfoReference:
                    _AssertAreEqual(expectedMemberInfoReference, (MemberInfoReferenceDocumentationElement)actual);
                    break;

                case MemberNameReferenceDocumentationElement expectedMemberNameReference:
                    _AssertAreEqual(expectedMemberNameReference, (MemberNameReferenceDocumentationElement)actual);
                    break;

                case GenericParameterReferenceDocumentationElement expectedGenericParameterReference:
                    _AssertAreEqual(expectedGenericParameterReference, (GenericParameterReferenceDocumentationElement)actual);
                    break;

                case ParameterReferenceDocumentationElement expectedParameterReference:
                    _AssertAreEqual(expectedParameterReference, (ParameterReferenceDocumentationElement)actual);
                    break;
            }
        }

        private static void _AssertAreEqual(IEnumerable<InlineDocumentationElement> expected, IEnumerable<InlineDocumentationElement> actual)
        {
            Assert.Equal(expected.Count(), actual.Count());
            foreach (var pair in expected.Zip(actual, (expectedElement, actualElement) => new { ExpectedElement = expectedElement, ActualElement = actualElement }))
                _AssertAreEqual(pair.ExpectedElement, pair.ActualElement);
        }

        private static void _AssertAreEqual(TextDocumentationElement expected, TextDocumentationElement actual)
        {
            Assert.Equal(expected.Text, actual.Text);
        }

        private static void _AssertAreEqual(InlineCodeDocumentationElement expected, InlineCodeDocumentationElement actual)
        {
            Assert.Equal(expected.Code, actual.Code);
        }

        private static void _AssertAreEqual(MemberInfoReferenceDocumentationElement expected, MemberInfoReferenceDocumentationElement actual)
        {
            Assert.Equal(expected.ReferredMember, actual.ReferredMember);
        }

        private static void _AssertAreEqual(MemberNameReferenceDocumentationElement expected, MemberNameReferenceDocumentationElement actual)
        {
            Assert.Equal(expected.CanonicalName, actual.CanonicalName);
        }

        private static void _AssertAreEqual(GenericParameterReferenceDocumentationElement expected, GenericParameterReferenceDocumentationElement actual)
        {
            Assert.Equal(expected.GenericParameterName, actual.GenericParameterName);
        }

        private static void _AssertAreEqual(ParameterReferenceDocumentationElement expected, ParameterReferenceDocumentationElement actual)
        {
            Assert.Equal(expected.ParameterName, actual.ParameterName);
        }
    }
}