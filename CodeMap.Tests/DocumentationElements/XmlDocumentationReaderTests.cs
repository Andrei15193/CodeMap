using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeMap.DocumentationElements;
using Xunit;

namespace CodeMap.Tests.DocumentationElements
{
    public class XmlDocumentationReaderTests
    {
        private static XmlDocumentationReader _XmlDocumentationReader { get; } = new XmlDocumentationReader();
        private static readonly string _richInlineContent = @"
                plain text
                <paramref name=""parameter reference"" test=""paramref""/>
                <typeparamref name=""generic parameter reference"" test=""typeparamref""/>
                <see cref=""member reference"" test=""see""/>
                <c test=""c"">some code</c>"
            .Trim();
        private static readonly string _richBlockContent = $@"
                {_richInlineContent}
                <list type=""table"" test=""table"">
                    <listheader test=""listheader"" test2=""old term"">
                        <term test2=""term"">{_richInlineContent}</term>
                    </listheader>
                    <item test=""item"" test2=""old description"">
                        <description test2=""description"">{_richInlineContent}</description>
                        <description test2=""description"">{_richInlineContent}</description>
                    </item>
                    <item test=""item"" test2=""old description""/>
                </list>
                <code test=""code"">
                    some code in a block
                </code>
                <list type=""bullet"" test=""unordered list"">
                    <item test=""item"" test2=""description"">{_richInlineContent}</item>
                    <item test=""item"" test2=""old description"">
                        <description test2=""description"">{_richInlineContent}</description>
                    </item>
                    <item test=""item"" test2=""description""/>
                </list>
                <list type=""number"" test=""ordered list"">
                    <item test=""item"" test2=""description"">{_richInlineContent}</item>
                    <item test=""item"" test2=""old description"">
                        <description test2=""description"">{_richInlineContent}</description>
                    </item>
                    <item test=""item"" test2=""description""/>
                </list>
                {_richInlineContent}
                <list test=""definition list"">
                    <listheader test=""listheader"">{_richInlineContent}</listheader>
                    <item test=""item"" test2=""old description"">
                        <term test=""term"">{_richInlineContent}</term>
                        <description test2=""description"">{_richInlineContent}</description>
                    </item>
                    <item test=""item"" test2=""old description"">
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
        private static readonly BlockDescriptionDocumentationElement _richBlockElements = DocumentationElement.BlockDescription(
            new BlockDocumentationElement[]
            {
                    DocumentationElement.Paragraph(
                        _richInlineElements
                    ),
                    DocumentationElement.Table(
                        new []
                        {
                            DocumentationElement.TableColumn(
                                _richInlineElements
                            )
                        },
                        DocumentationElement.TableRow(
                            DocumentationElement.TableCell(
                                _richInlineElements
                            ),
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
                        DocumentationElement.DefinitionListTitle(
                            _richInlineElements
                        ),
                        DocumentationElement.DefinitionListItem(
                            DocumentationElement.DefinitionListItemTerm(_richInlineElements),
                            DocumentationElement.DefinitionListItemDescription(_richInlineElements)
                        ),
                        DocumentationElement.DefinitionListItem(
                            DocumentationElement.DefinitionListItemTerm(Enumerable.Empty<InlineDocumentationElement>()),
                            DocumentationElement.DefinitionListItemDescription(Enumerable.Empty<InlineDocumentationElement>())
                        )
                    )
            }
        );

        [Fact]
        public void ReadEmptySummary()
        {
            MemberDocumentationCollection result;
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
                result = _XmlDocumentationReader.Read(stringReader);

            Assert.Single(result);
            _AssertAreEqual(
                DocumentationElement.Summary(),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public void ReadSummaryWithOneParagraph()
        {
            MemberDocumentationCollection result;
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
                result = _XmlDocumentationReader.Read(stringReader);

            Assert.Single(result);
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
        public void ReadSummaryWithMultiParagraphSummarySection()
        {
            MemberDocumentationCollection result;
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
                result = _XmlDocumentationReader.Read(stringReader);

            Assert.Single(result);
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
        public void ReadSummaryWithEmptyParagraph()
        {
            MemberDocumentationCollection result;
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
                result = _XmlDocumentationReader.Read(stringReader);

            Assert.Single(result);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.Paragraph()
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public void ReadSummaryWithMultilineParagraphCollapsesWhiteSpace()
        {
            MemberDocumentationCollection result;
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
                result = _XmlDocumentationReader.Read(stringReader);

            Assert.Single(result);
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
        public void ReadSummaryWithInlineCode()
        {
            MemberDocumentationCollection result;
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
                result = _XmlDocumentationReader.Read(stringReader);

            Assert.Single(result);
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
        public void ReadSummaryDoesNotCollapseInlineCodeWhiteSpace()
        {
            MemberDocumentationCollection result;
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
                result = _XmlDocumentationReader.Read(stringReader);

            Assert.Single(result);
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
        public void ReadSummaryWithMemberReference()
        {
            MemberDocumentationCollection result;
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
                result = _XmlDocumentationReader.Read(stringReader);

            Assert.Single(result);
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
        public void ReadSummaryWithParameterReference()
        {
            MemberDocumentationCollection result;
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
                result = _XmlDocumentationReader.Read(stringReader);

            Assert.Single(result);
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
        public void ReadSummaryCodeBlock()
        {
            MemberDocumentationCollection result;
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
                result = _XmlDocumentationReader.Read(stringReader);

            Assert.Single(result);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.CodeBlock("this is a code block")
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public void ReadSummaryCodeBlockWithMultilineAndIndentedCode()
        {
            MemberDocumentationCollection result;
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
                result = _XmlDocumentationReader.Read(stringReader);

            Assert.Single(result);
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
        public void ReadSummaryCodeBlockWithMultilineNormalizesSpacesToLineFeed()
        {
            MemberDocumentationCollection result;
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
                result = _XmlDocumentationReader.Read(stringReader);

            Assert.Single(result);
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
        public void ReadSummaryCodeBlockOnOneLinePreservesWhiteSpace()
        {
            MemberDocumentationCollection result;
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
                result = _XmlDocumentationReader.Read(stringReader);

            Assert.Single(result);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.CodeBlock("  this contains some white space  ")
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public void ReadSummaryCodeBlockPreservesWhiteSpaceOnTheSameLineWithCodeTag()
        {
            MemberDocumentationCollection result;
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
                result = _XmlDocumentationReader.Read(stringReader);

            Assert.Single(result);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.CodeBlock("  this contains some white space  \nthe second line")
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public void ReadSummaryCodeBlockWithCodeOnSameLineWithCodeEndingTag()
        {
            MemberDocumentationCollection result;
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
                result = _XmlDocumentationReader.Read(stringReader);

            Assert.Single(result);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.CodeBlock("start\nend")
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public void ReadSummaryUnorderedList()
        {
            MemberDocumentationCollection result;
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
                result = _XmlDocumentationReader.Read(stringReader);

            Assert.Single(result);
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
        public void ReadSummaryEmptyUnorderedList()
        {
            MemberDocumentationCollection result;
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
                result = _XmlDocumentationReader.Read(stringReader);

            Assert.Single(result);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.UnorderedList()
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public void ReadSummaryExplicitUnorderedList()
        {
            MemberDocumentationCollection result;
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
                result = _XmlDocumentationReader.Read(stringReader);

            Assert.Single(result);
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
        public void ReadSummaryOrderedList()
        {
            MemberDocumentationCollection result;
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
                result = _XmlDocumentationReader.Read(stringReader);

            Assert.Single(result);
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
        public void ReadSummaryEmptyOrderedList()
        {
            MemberDocumentationCollection result;
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
                result = _XmlDocumentationReader.Read(stringReader);

            Assert.Single(result);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.OrderedList()
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public void ReadSummaryDefinitionList()
        {
            MemberDocumentationCollection result;
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
                result = _XmlDocumentationReader.Read(stringReader);

            Assert.Single(result);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.DefinitionList(
                        DocumentationElement.DefinitionListItem(
                            DocumentationElement.DefinitionListItemTerm(DocumentationElement.Text("term 1")),
                            DocumentationElement.DefinitionListItemDescription(DocumentationElement.Text("The first item"))
                        ),
                        DocumentationElement.DefinitionListItem(
                            DocumentationElement.DefinitionListItemTerm(DocumentationElement.Text("term 2")),
                            DocumentationElement.DefinitionListItemDescription(DocumentationElement.Text("The second item"))
                        ),
                        DocumentationElement.DefinitionListItem(
                            DocumentationElement.DefinitionListItemTerm(DocumentationElement.Text("term 3")),
                            DocumentationElement.DefinitionListItemDescription(DocumentationElement.Text("The third item"))
                        ),
                        DocumentationElement.DefinitionListItem(
                            DocumentationElement.DefinitionListItemTerm(),
                            DocumentationElement.DefinitionListItemDescription(DocumentationElement.Text("The fourth item"))
                        ),
                        DocumentationElement.DefinitionListItem(
                            DocumentationElement.DefinitionListItemTerm(DocumentationElement.Text("term 5")),
                            DocumentationElement.DefinitionListItemDescription()
                        )
                    )
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public void ReadSummaryDefinitionListWithTypeBullet()
        {
            MemberDocumentationCollection result;
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
                result = _XmlDocumentationReader.Read(stringReader);

            Assert.Single(result);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.DefinitionList(
                        DocumentationElement.DefinitionListItem(
                            DocumentationElement.DefinitionListItemTerm(DocumentationElement.Text("term 1")),
                            DocumentationElement.DefinitionListItemDescription(DocumentationElement.Text("The first item"))
                        ),
                        DocumentationElement.DefinitionListItem(
                            DocumentationElement.DefinitionListItemTerm(DocumentationElement.Text("term 2")),
                            DocumentationElement.DefinitionListItemDescription(DocumentationElement.Text("The second item"))
                        )
                    )
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public void ReadSummaryDefinitionListWithTypeNumber()
        {
            MemberDocumentationCollection result;
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
                result = _XmlDocumentationReader.Read(stringReader);

            Assert.Single(result);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.DefinitionList(
                        DocumentationElement.DefinitionListItem(
                            DocumentationElement.DefinitionListItemTerm(DocumentationElement.Text("term 1")),
                            DocumentationElement.DefinitionListItemDescription(DocumentationElement.Text("The first item"))
                        ),
                        DocumentationElement.DefinitionListItem(
                            DocumentationElement.DefinitionListItemTerm(DocumentationElement.Text("term 2")),
                            DocumentationElement.DefinitionListItemDescription(DocumentationElement.Text("The second item"))
                        )
                    )
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public void ReadSummaryDefinitionListWithTitle()
        {
            MemberDocumentationCollection result;
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
                result = _XmlDocumentationReader.Read(stringReader);

            Assert.Single(result);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.DefinitionList(
                        DocumentationElement.DefinitionListTitle(
                            DocumentationElement.Text("this is a title")
                        ),
                        DocumentationElement.DefinitionListItem(
                            DocumentationElement.DefinitionListItemTerm(DocumentationElement.Text("term 1")),
                            DocumentationElement.DefinitionListItemDescription(DocumentationElement.Text("The first item"))
                        ),
                        DocumentationElement.DefinitionListItem(
                            DocumentationElement.DefinitionListItemTerm(DocumentationElement.Text("term 2")),
                            DocumentationElement.DefinitionListItemDescription(DocumentationElement.Text("The second item"))
                        )
                    )
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public void ReadSummaryDefinitionListWithTitleUsingTerm()
        {
            MemberDocumentationCollection result;
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
                result = _XmlDocumentationReader.Read(stringReader);

            Assert.Single(result);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.DefinitionList(
                        DocumentationElement.DefinitionListTitle(
                            DocumentationElement.Text("this is a title")
                        ),
                        DocumentationElement.DefinitionListItem(
                            DocumentationElement.DefinitionListItemTerm(DocumentationElement.Text("term 1")),
                            DocumentationElement.DefinitionListItemDescription(DocumentationElement.Text("The first item"))
                        ),
                        DocumentationElement.DefinitionListItem(
                            DocumentationElement.DefinitionListItemTerm(DocumentationElement.Text("term 2")),
                            DocumentationElement.DefinitionListItemDescription(DocumentationElement.Text("The second item"))
                        )
                    )
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public void ReadSummaryEmptyDefinitionList()
        {
            MemberDocumentationCollection result;
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
                result = _XmlDocumentationReader.Read(stringReader);

            Assert.Single(result);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.DefinitionList(
                        DocumentationElement.DefinitionListTitle(
                            DocumentationElement.Text("this is a title")
                        )
                    )
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public void ReadSummaryEmptyDefinitionListWithEmptyTitle()
        {
            MemberDocumentationCollection result;
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
                result = _XmlDocumentationReader.Read(stringReader);

            Assert.Single(result);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.DefinitionList(
                        DocumentationElement.DefinitionListTitle()
                    )
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public void ReadSummaryTable()
        {
            MemberDocumentationCollection result;
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
                result = _XmlDocumentationReader.Read(stringReader);

            Assert.Single(result);
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
        public void ReadSummaryTableWithoutEqualNumberOfColumns()
        {
            MemberDocumentationCollection result;
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
                result = _XmlDocumentationReader.Read(stringReader);

            Assert.Single(result);
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
        public void ReadSummaryTableWithoutHeader()
        {
            MemberDocumentationCollection result;
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
                result = _XmlDocumentationReader.Read(stringReader);

            Assert.Single(result);
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
        public void ReadSummaryEmptyTable()
        {
            MemberDocumentationCollection result;
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
                result = _XmlDocumentationReader.Read(stringReader);

            Assert.Single(result);
            _AssertAreEqual(
                DocumentationElement.Summary(
                    DocumentationElement.Table()
                ),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public void ReadComplexSummary()
        {
            MemberDocumentationCollection result;
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
                result = _XmlDocumentationReader.Read(stringReader);

            Assert.Single(result);
            _AssertAreEqual(
                DocumentationElement.Summary(_richBlockElements),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary
            );
        }

        [Fact]
        public void ReadComplexTypeParameters()
        {
            MemberDocumentationCollection result;
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
                result = _XmlDocumentationReader.Read(stringReader);

            Assert.Single(result);
            _AssertAreEqual(
                new Dictionary<string, BlockDescriptionDocumentationElement>(StringComparer.Ordinal)
                {
                    { "typeParameter1", _richBlockElements },
                    { "typeParameter2", DocumentationElement.BlockDescription(_richBlockElements.Concat(_richBlockElements)) }
                },
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").GenericParameters
            );
        }

        [Fact]
        public void ReadComplexParameters()
        {
            MemberDocumentationCollection result;
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
                result = _XmlDocumentationReader.Read(stringReader);

            Assert.Single(result);
            _AssertAreEqual(
                new Dictionary<string, BlockDescriptionDocumentationElement>(StringComparer.Ordinal)
                {
                    { "parameter1", _richBlockElements },
                    { "parameter2", DocumentationElement.BlockDescription(_richBlockElements.Concat(_richBlockElements)) }
                },
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Parameters
            );
        }

        [Fact]
        public void ReadComplexReturns()
        {
            MemberDocumentationCollection result;
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
                result = _XmlDocumentationReader.Read(stringReader);

            Assert.Single(result);
            _AssertAreEqual(
                _richBlockElements,
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Returns
            );
        }

        [Fact]
        public void ReadComplexExceptions()
        {
            MemberDocumentationCollection result;
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
                result = _XmlDocumentationReader.Read(stringReader);

            Assert.Single(result);
            _AssertAreEqual(
                new Dictionary<string, BlockDescriptionDocumentationElement>(StringComparer.Ordinal)
                {
                    { "exception1", _richBlockElements },
                    { "exception2", DocumentationElement.BlockDescription(_richBlockElements.Concat(_richBlockElements)) }
                },
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Exceptions
            );
        }

        [Fact]
        public void ReadComplexRemarks()
        {
            MemberDocumentationCollection result;
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
                result = _XmlDocumentationReader.Read(stringReader);

            Assert.Single(result);
            _AssertAreEqual(
                DocumentationElement.Remarks(_richBlockElements),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Remarks
            );
        }

        [Fact]
        public void ReadComplexExamples()
        {
            MemberDocumentationCollection result;
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
                result = _XmlDocumentationReader.Read(stringReader);

            Assert.Single(result);
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
        public void ReadComplexValue()
        {
            MemberDocumentationCollection result;
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
                result = _XmlDocumentationReader.Read(stringReader);

            Assert.Single(result);
            _AssertAreEqual(
                DocumentationElement.Value(_richBlockElements),
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Value
            );
        }

        [Fact]
        public void ReadRelatedMembersList()
        {
            MemberDocumentationCollection result;
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
                result = _XmlDocumentationReader.Read(stringReader);

            Assert.Single(result);
            _AssertAreEqual(
                new[]
                {
                    DocumentationElement.MemberReference("member1"),
                    DocumentationElement.MemberReference("member2"),
                    DocumentationElement.MemberReference("member2")
                },
                result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").RelatedMembers
            );
        }

        [Fact]
        public void ReadEmptyMemberDocumentation()
        {
            MemberDocumentationCollection result;
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
                result = _XmlDocumentationReader.Read(stringReader);

            Assert.Single(result);
            var emptyMemberDocumentation = result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name");
            Assert.Empty(emptyMemberDocumentation.Summary.Content);
            Assert.Empty(emptyMemberDocumentation.GenericParameters);
            Assert.Empty(emptyMemberDocumentation.Parameters);
            Assert.Empty(emptyMemberDocumentation.Returns);
            Assert.Empty(emptyMemberDocumentation.Exceptions);
            Assert.Empty(emptyMemberDocumentation.Remarks.Content);
            Assert.Empty(emptyMemberDocumentation.Examples);
            Assert.Empty(emptyMemberDocumentation.Value.Content);
            Assert.Empty(emptyMemberDocumentation.RelatedMembers);
        }

        [Fact]
        public void ReadMemberDocumentationWithXmlAttributes()
        {
            MemberDocumentationCollection result;
            using (var stringReader = new StringReader($@"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>CodeMap.Tests</name>
    </assembly>
    <members>
        <member name=""canonical name"">
            <summary test=""summary"">{_richBlockContent}</summary>
            <typeparam name=""TParam"" test=""typeparam"">{_richBlockContent}</typeparam>
            <param name=""param"" test=""param"">{_richBlockContent}</param>
            <returns test=""returns"">{_richBlockContent}</returns>
            <exception cref=""exception1"" test=""exception"">{_richBlockContent}</exception>
            <remarks test=""remarks"">{_richBlockContent}</remarks>
            <example test=""example"">{_richBlockContent}</example>
            <value test=""value"">{_richBlockContent}</value>
            <seealso cref=""reference"" test=""seealso""/>
        </member>
    </members>
</doc>
"))
                result = _XmlDocumentationReader.Read(stringReader);

            Assert.Single(result);
            _AssertXmlAttributes(result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name"));
        }

        [Fact]
        public void ReadMemberDocumentationParagraphWithXmlAttributes()
        {
            MemberDocumentationCollection result;
            using (var stringReader = new StringReader($@"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>CodeMap.Tests</name>
    </assembly>
    <members>
        <member name=""canonical name"">
            <summary>
                <para test=""para"">paragraph</para>
            </summary>
        </member>
    </members>
</doc>
"))
                result = _XmlDocumentationReader.Read(stringReader);

            Assert.Single(result);
            var summary = result.Single(memberDocumentation => memberDocumentation.CanonicalName == "canonical name").Summary;
            Assert.Single(summary.Content);
            var paragraph = summary.Content.Cast<ParagraphDocumentationElement>().Single();
            Assert.Single(paragraph.XmlAttributes);
            Assert.Equal("para", paragraph.XmlAttributes["test"]);
        }

        [Fact]
        public void ReadingFromNullThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("input", () => _XmlDocumentationReader.Read(null));
            Assert.Equal(new ArgumentNullException("input").Message, exception.Message);
        }

        private static void _AssertAreEqual(SummaryDocumentationElement expected, SummaryDocumentationElement actual)
            => _AssertAreEqual(expected.Content, actual.Content);

        private static void _AssertAreEqual(IReadOnlyDictionary<string, BlockDescriptionDocumentationElement> expected, IReadOnlyDictionary<string, BlockDescriptionDocumentationElement> actual)
        {
            Assert.Equal(expected.Count, actual.Count);

            foreach (var (expectedParameter, expectedContent, actualParameter, actualContent) in expected
                .OrderBy(expectedParameter => expectedParameter.Key, StringComparer.Ordinal)
                .Zip(
                    actual.OrderBy(actualParameter => actualParameter.Key, StringComparer.Ordinal),
                    (expectedParameter, actualParameter) => (
                        expectedParameter: expectedParameter.Key,
                        expectedContent: expectedParameter.Value,
                        actualParameter: actualParameter.Key,
                        actualContent: actualParameter.Value
                    )
                )
            )
            {
                Assert.Equal(expectedParameter, actualParameter);
                _AssertAreEqual(expectedContent, actualContent);
            }
        }

        private static void _AssertAreEqual(RemarksDocumentationElement expected, RemarksDocumentationElement actual)
            => _AssertAreEqual(expected.Content, actual.Content);

        private static void _AssertAreEqual(ExampleDocumentationElement expected, ExampleDocumentationElement actual)
            => _AssertAreEqual(expected.Content, actual.Content);

        private static void _AssertAreEqual(IEnumerable<ExampleDocumentationElement> expected, IEnumerable<ExampleDocumentationElement> actual)
        {
            Assert.Equal(expected.Count(), actual.Count());
            foreach (var (expectedElement, actualElement) in expected.Zip(actual, (expectedElement, actualElement) => (expectedElement, actualElement)))
                _AssertAreEqual(expectedElement, actualElement);
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
            foreach (var (expectedElement, actualElement) in expected.Zip(actual, (expectedElement, actualElement) => (expectedElement, actualElement)))
                _AssertAreEqual(expectedElement, actualElement);
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
            foreach (var (expectedListItem, actualListItem) in expected.Items.Zip(actual.Items, (expectedListItem, actualListItem) => (expectedListItem, actualListItem)))
                _AssertAreEqual(expectedListItem, actualListItem);
        }

        private static void _AssertAreEqual(OrderedListDocumentationElement expected, OrderedListDocumentationElement actual)
        {
            Assert.Equal(expected.Items.Count, actual.Items.Count);
            foreach (var (expectedListItem, actualListItem) in expected.Items.Zip(actual.Items, (expectedListItem, actualListItem) => (expectedListItem, actualListItem)))
                _AssertAreEqual(expectedListItem, actualListItem);
        }

        private static void _AssertAreEqual(ListItemDocumentationElement expected, ListItemDocumentationElement actual)
            => _AssertAreEqual(expected.Content, actual.Content);

        private static void _AssertAreEqual(DefinitionListDocumentationElement expected, DefinitionListDocumentationElement actual)
        {
            if (expected.ListTitle == null)
                Assert.Null(actual.ListTitle);
            else
                _AssertAreEqual(expected.ListTitle.Content, actual.ListTitle.Content);

            Assert.Equal(expected.Items.Count, actual.Items.Count);
            foreach (var (expectedListItem, actualListItem) in expected.Items.Zip(actual.Items, (expectedListItem, actualListItem) => (expectedListItem, actualListItem)))
                _AssertAreEqual(expectedListItem, actualListItem);
        }

        private static void _AssertAreEqual(DefinitionListItemDocumentationElement expected, DefinitionListItemDocumentationElement actual)
        {
            _AssertAreEqual(expected.Term.Content, actual.Term.Content);
            _AssertAreEqual(expected.Description.Content, actual.Description.Content);
        }

        private static void _AssertAreEqual(TableDocumentationElement expected, TableDocumentationElement actual)
        {
            Assert.Equal(expected.Columns.Count, actual.Columns.Count);
            foreach (var (expectedColumn, actualColumn) in expected.Columns.Zip(actual.Columns, (expectedColumn, actualColumn) => (expectedColumn, actualColumn)))
                _AssertAreEqual(expectedColumn, actualColumn);

            Assert.Equal(expected.Rows.Count, actual.Rows.Count);
            foreach (var (expectedRow, actualRow) in expected.Rows.Zip(actual.Rows, (expectedRow, actualRow) => (expectedRow, actualRow)))
                _AssertAreEqual(expectedRow, actualRow);
        }

        private static void _AssertAreEqual(TableColumnDocumentationElement expected, TableColumnDocumentationElement actual)
            => _AssertAreEqual(expected.Name, actual.Name);

        private static void _AssertAreEqual(TableRowDocumentationElement expected, TableRowDocumentationElement actual)
        {
            Assert.Equal(expected.Cells.Count, actual.Cells.Count);
            foreach (var (expectedCell, actualCell) in expected.Cells.Zip(actual.Cells, (expectedCell, actualCell) => (expectedCell, actualCell)))
                _AssertAreEqual(expectedCell, actualCell);
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

                case ReferenceDataDocumentationElement expectedMemberInfoReference:
                    _AssertAreEqual(expectedMemberInfoReference, (ReferenceDataDocumentationElement)actual);
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
            foreach (var (expectedElement, actualElement) in expected.Zip(actual, (expectedElement, actualElement) => (expectedElement, actualElement)))
                _AssertAreEqual(expectedElement, actualElement);
        }

        private static void _AssertAreEqual(TextDocumentationElement expected, TextDocumentationElement actual)
        {
            Assert.Equal(expected.Text, actual.Text);
        }

        private static void _AssertAreEqual(InlineCodeDocumentationElement expected, InlineCodeDocumentationElement actual)
        {
            Assert.Equal(expected.Code, actual.Code);
        }

        private static void _AssertAreEqual(ReferenceDataDocumentationElement expected, ReferenceDataDocumentationElement actual)
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

        private static void _AssertXmlAttributes(MemberDocumentation memberDocumentation)
        {
            _AssertSummaryXmlAttributes(memberDocumentation.Summary);
            foreach (var genericParameter in memberDocumentation.GenericParameters.Values)
                _AssertGenericParameterXmlAttributes(genericParameter);
            foreach (var parameter in memberDocumentation.Parameters.Values)
                _AssertParameterXmlAttributes(parameter);
            _AssertReturnsXmlAttributes(memberDocumentation.Returns);
            foreach (var exception in memberDocumentation.Exceptions.Values)
                _AssertExceptionXmlAttributes(exception);
            _AssertRemarksXmlAttributes(memberDocumentation.Remarks);
            foreach (var example in memberDocumentation.Examples)
                _AssertExampleXmlAttributes(example);
            _AssertValueXmlAttributes(memberDocumentation.Value);
            foreach (var relatedMember in memberDocumentation.RelatedMembers)
                _AssertRelatedMemberXmlAttributes(relatedMember);
        }

        private static void _AssertSummaryXmlAttributes(SummaryDocumentationElement summary)
        {
            Assert.Single(summary.XmlAttributes);
            Assert.Equal("summary", summary.XmlAttributes["test"]);
            _AssertBlockElementsXmlAttributes(summary.Content);
        }

        private static void _AssertGenericParameterXmlAttributes(BlockDescriptionDocumentationElement genericParameter)
        {
            Assert.Single(genericParameter.XmlAttributes);
            Assert.Equal("typeparam", genericParameter.XmlAttributes["test"]);
            _AssertBlockElementsXmlAttributes(genericParameter);
        }

        private static void _AssertParameterXmlAttributes(BlockDescriptionDocumentationElement parameter)
        {
            Assert.Single(parameter.XmlAttributes);
            Assert.Equal("param", parameter.XmlAttributes["test"]);
            _AssertBlockElementsXmlAttributes(parameter);
        }

        private static void _AssertReturnsXmlAttributes(BlockDescriptionDocumentationElement returns)
        {
            Assert.Single(returns.XmlAttributes);
            Assert.Equal("returns", returns.XmlAttributes["test"]);
            _AssertBlockElementsXmlAttributes(returns);
        }

        private static void _AssertExceptionXmlAttributes(BlockDescriptionDocumentationElement exception)
        {
            Assert.Single(exception.XmlAttributes);
            Assert.Equal("exception", exception.XmlAttributes["test"]);
            _AssertBlockElementsXmlAttributes(exception);
        }

        private static void _AssertRemarksXmlAttributes(RemarksDocumentationElement remarks)
        {
            Assert.Single(remarks.XmlAttributes);
            Assert.Equal("remarks", remarks.XmlAttributes["test"]);
            _AssertBlockElementsXmlAttributes(remarks.Content);
        }

        private static void _AssertExampleXmlAttributes(ExampleDocumentationElement example)
        {
            Assert.Single(example.XmlAttributes);
            Assert.Equal("example", example.XmlAttributes["test"]);
            _AssertBlockElementsXmlAttributes(example.Content);
        }

        private static void _AssertValueXmlAttributes(ValueDocumentationElement value)
        {
            Assert.Single(value.XmlAttributes);
            Assert.Equal("value", value.XmlAttributes["test"]);
            _AssertBlockElementsXmlAttributes(value.Content);
        }

        private static void _AssertRelatedMemberXmlAttributes(MemberReferenceDocumentationElement relatedMember)
        {
            switch (relatedMember)
            {
                case ReferenceDataDocumentationElement memberInfoReference:
                    Assert.Single(memberInfoReference.XmlAttributes);
                    Assert.Equal("seealso", memberInfoReference.XmlAttributes["test"]);
                    break;

                case MemberNameReferenceDocumentationElement memberNameReference:
                    Assert.Single(memberNameReference.XmlAttributes);
                    Assert.Equal("seealso", memberNameReference.XmlAttributes["test"]);
                    break;
            }
        }

        private static void _AssertBlockElementsXmlAttributes(IEnumerable<BlockDocumentationElement> blockElements)
        {
            foreach (var blockElement in blockElements)
                switch (blockElement)
                {
                    case ParagraphDocumentationElement paragraph:
                        _AssertParagraphXmlAttributes(paragraph);
                        break;

                    case UnorderedListDocumentationElement unorderedListDocumentationElement:
                        _AssertUnoeredListXmlAttributes(unorderedListDocumentationElement);
                        break;

                    case OrderedListDocumentationElement orderedListDocumentationElement:
                        _AssertOrderedListXmlAttributes(orderedListDocumentationElement);
                        break;

                    case DefinitionListDocumentationElement definitionListDocumentationElement:
                        _AssertDefinitionListXmlAttributes(definitionListDocumentationElement);
                        break;

                    case TableDocumentationElement tableDocumentationElement:
                        _AssertTableXmlAttributes(tableDocumentationElement);
                        break;

                    case CodeBlockDocumentationElement codeBlockDocumentationElement:
                        _AssertCodeBlockXmlAttributes(codeBlockDocumentationElement);
                        break;
                }
        }

        private static void _AssertParagraphXmlAttributes(ParagraphDocumentationElement paragraph)
        {
            Assert.Empty(paragraph.XmlAttributes);
            _AssertInlineElementsXmlAttributes(paragraph.Content);
        }

        private static void _AssertUnoeredListXmlAttributes(UnorderedListDocumentationElement unorderedList)
        {
            Assert.Single(unorderedList.XmlAttributes);
            Assert.Equal("unordered list", unorderedList.XmlAttributes["test"]);
            _AssertListItemsXmlAttributes(unorderedList.Items);
        }

        private static void _AssertOrderedListXmlAttributes(OrderedListDocumentationElement orderedList)
        {
            Assert.Single(orderedList.XmlAttributes);
            Assert.Equal("ordered list", orderedList.XmlAttributes["test"]);
            _AssertListItemsXmlAttributes(orderedList.Items);
        }

        private static void _AssertListItemsXmlAttributes(IEnumerable<ListItemDocumentationElement> items)
        {
            foreach (var item in items)
            {
                Assert.Equal(2, item.XmlAttributes.Count);
                Assert.Equal("item", item.XmlAttributes["test"]);
                Assert.Equal("description", item.XmlAttributes["test2"]);
                _AssertInlineElementsXmlAttributes(item.Content);
            }
        }

        private static void _AssertDefinitionListXmlAttributes(DefinitionListDocumentationElement definitionList)
        {
            Assert.Single(definitionList.XmlAttributes);
            Assert.Equal("definition list", definitionList.XmlAttributes["test"]);
            _AssertDefinitionListItemsXmlAttributes(definitionList.Items);
        }

        private static void _AssertDefinitionListItemsXmlAttributes(IEnumerable<DefinitionListItemDocumentationElement> items)
        {
            foreach (var item in items)
            {
                Assert.Equal(2, item.XmlAttributes.Count);
                Assert.Equal("item", item.XmlAttributes["test"]);
                Assert.Equal("old description", item.XmlAttributes["test2"]);

                if (item.Term.Content.Any())
                {
                    Assert.Single(item.Term.XmlAttributes);
                    Assert.Equal("term", item.Term.XmlAttributes["test"]);
                    _AssertInlineElementsXmlAttributes(item.Term.Content);
                }

                if (item.Description.Content.Any())
                {
                    Assert.Single(item.Description.XmlAttributes);
                    Assert.Equal("description", item.Description.XmlAttributes["test2"]);
                    _AssertInlineElementsXmlAttributes(item.Description.Content);
                }
            }
        }

        private static void _AssertTableXmlAttributes(TableDocumentationElement table)
        {
            Assert.Single(table.XmlAttributes);
            Assert.Equal("table", table.XmlAttributes["test"]);
            _AssertTableColumnsXmlAttributes(table.Columns);
            _AssertTableRowsXmlAttributes(table.Rows);
        }

        private static void _AssertTableColumnsXmlAttributes(IEnumerable<TableColumnDocumentationElement> tableColumns)
        {
            foreach (var tableColumn in tableColumns)
                if (tableColumn.Name.Any())
                {
                    Assert.Equal(2, tableColumn.XmlAttributes.Count);
                    Assert.Equal("listheader", tableColumn.XmlAttributes["test"]);
                    Assert.Equal("term", tableColumn.XmlAttributes["test2"]);
                }
                else
                {
                    Assert.Equal(2, tableColumn.XmlAttributes.Count);
                    Assert.Equal("listheader", tableColumn.XmlAttributes["test"]);
                    Assert.Equal("old term", tableColumn.XmlAttributes["test2"]);
                }
        }

        private static void _AssertTableRowsXmlAttributes(IEnumerable<TableRowDocumentationElement> tableRows)
        {
            foreach (var tableRow in tableRows)
            {
                Assert.Equal(2, tableRow.XmlAttributes.Count);
                Assert.Equal("item", tableRow.XmlAttributes["test"]);
                Assert.Equal("old description", tableRow.XmlAttributes["test2"]);
                _AssertTableCellsXmlAttributes(tableRow.Cells);
            }
        }

        private static void _AssertTableCellsXmlAttributes(IReadOnlyList<TableCellDocumentationElement> tableCells)
        {
            foreach (var tableCell in tableCells)
                if (tableCell.Content.Any())
                {
                    Assert.Single(tableCell.XmlAttributes);
                    Assert.Equal("description", tableCell.XmlAttributes["test2"]);
                }
        }

        private static void _AssertCodeBlockXmlAttributes(CodeBlockDocumentationElement codeBlock)
        {
            Assert.Single(codeBlock.XmlAttributes);
            Assert.Equal("code", codeBlock.XmlAttributes["test"]);
        }

        private static void _AssertInlineElementsXmlAttributes(IEnumerable<InlineDocumentationElement> inlineElements)
        {
            foreach (var inlineElement in inlineElements)
                switch (inlineElement)
                {
                    case ReferenceDataDocumentationElement memberInfoReferenceDocumentationElement:
                        _AssertMemberReferenceXmlAttributes(memberInfoReferenceDocumentationElement);
                        break;

                    case MemberNameReferenceDocumentationElement memberNameReferenceDocumentationElement:
                        _AssertMemberReferenceXmlAttributes(memberNameReferenceDocumentationElement);
                        break;

                    case ParameterReferenceDocumentationElement parameterReferenceDocumentationElement:
                        _AssertParameterReferenceXmlAttributes(parameterReferenceDocumentationElement);
                        break;

                    case GenericParameterReferenceDocumentationElement genericParameterReferenceDocumentationElement:
                        _AssertGenericParameterReferenceXmlAttributes(genericParameterReferenceDocumentationElement);
                        break;

                    case InlineCodeDocumentationElement inlineCodeDocumentationElement:
                        _AssertInlineCodeXmlAttributes(inlineCodeDocumentationElement);
                        break;
                }
        }

        private static void _AssertMemberReferenceXmlAttributes(ReferenceDataDocumentationElement memberInfoReference)
        {
            Assert.Single(memberInfoReference.XmlAttributes);
            Assert.Equal("see", memberInfoReference.XmlAttributes["test"]);
        }

        private static void _AssertMemberReferenceXmlAttributes(MemberNameReferenceDocumentationElement memberNameReference)
        {
            Assert.Single(memberNameReference.XmlAttributes);
            Assert.Equal("see", memberNameReference.XmlAttributes["test"]);
        }

        private static void _AssertParameterReferenceXmlAttributes(ParameterReferenceDocumentationElement parameterReference)
        {
            Assert.Single(parameterReference.XmlAttributes);
            Assert.Equal("paramref", parameterReference.XmlAttributes["test"]);
        }

        private static void _AssertGenericParameterReferenceXmlAttributes(GenericParameterReferenceDocumentationElement genericParameterReference)
        {
            Assert.Single(genericParameterReference.XmlAttributes);
            Assert.Equal("typeparamref", genericParameterReference.XmlAttributes["test"]);
        }

        private static void _AssertInlineCodeXmlAttributes(InlineCodeDocumentationElement inlineCode)
        {
            Assert.Single(inlineCode.XmlAttributes);
            Assert.Equal("c", inlineCode.XmlAttributes["test"]);
        }
    }
}