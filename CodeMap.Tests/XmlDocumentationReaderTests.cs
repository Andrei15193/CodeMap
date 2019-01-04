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
                        DocumentationElement.Text("This contains"),
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
                        DocumentationElement.Text("This contains a"),
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
                        DocumentationElement.Text("This contains a"),
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
        public async Task ReadingFromNullThrowsException()
        {
            var exception = await Assert.ThrowsAsync<ArgumentNullException>("textReader", () => _XmlDocumentationReader.ReadAsync(null));
            Assert.Equal(new ArgumentNullException("textReader").Message, exception.Message);
        }

        private static void _AssertAreEqual(SummaryDocumentationElement expected, SummaryDocumentationElement actual)
        {
            Assert.Equal(expected.Content.Count, actual.Content.Count);
            foreach (var pair in expected.Content.Zip(actual.Content, (expectedContent, actualContent) => new { ExpectedContent = expectedContent, ActualContent = actualContent }))
                _AssertAreEqual(pair.ExpectedContent, pair.ActualContent);
        }

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

        private static void _AssertAreEqual(ParagraphDocumentationElement expected, ParagraphDocumentationElement actual)
        {
            Assert.Equal(expected.Content.Count, actual.Content.Count);
            foreach (var pair in expected.Content.Zip(actual.Content, (expectedContent, actualContent) => new { ExpectedContent = expectedContent, ActualContent = actualContent }))
                _AssertAreEqual(pair.ExpectedContent, pair.ActualContent);
        }

        private static void _AssertAreEqual(CodeBlockDocumentationElement expected, CodeBlockDocumentationElement actual)
        {
            Assert.Equal(expected.Code, actual.Code);
        }

        private static void _AssertAreEqual(OrderedListDocumentationElement expected, OrderedListDocumentationElement actual)
        {
            Assert.Equal(expected.Items.Count, actual.Items.Count);
            foreach (var pair in expected.Items.Zip(actual.Items, (expectedListItem, actualListItem) => new { ExpectedListItem = expectedListItem, ActualListItem = actualListItem }))
                _AssertAreEqual(pair.ExpectedListItem, pair.ActualListItem);
        }

        private static void _AssertAreEqual(UnorderedListDocumentationElement expected, UnorderedListDocumentationElement actual)
        {
            Assert.Equal(expected.Items.Count, actual.Items.Count);
            foreach (var pair in expected.Items.Zip(actual.Items, (expectedListItem, actualListItem) => new { ExpectedListItem = expectedListItem, ActualListItem = actualListItem }))
                _AssertAreEqual(pair.ExpectedListItem, pair.ActualListItem);
        }

        private static void _AssertAreEqual(DefinitionListDocumentationElement expected, DefinitionListDocumentationElement actual)
        {
            if (expected.ListTitle == null)
                Assert.Null(actual.ListTitle);
            else
            {
                Assert.Equal(expected.ListTitle.Count, actual.ListTitle.Count);
                foreach (var pair in expected.ListTitle.Zip(actual.ListTitle, (expectedContent, actualContent) => new { ExpectedContent = expectedContent, ActualContent = actualContent }))
                    _AssertAreEqual(pair.ExpectedContent, pair.ActualContent);
            }

            Assert.Equal(expected.Items.Count, actual.Items.Count);
            foreach (var pair in expected.Items.Zip(actual.Items, (expectedListItem, actualListItem) => new { ExpectedListItem = expectedListItem, ActualListItem = actualListItem }))
                _AssertAreEqual(pair.ExpectedListItem, pair.ActualListItem);
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

        private static void _AssertAreEqual(ListItemDocumentationElement expected, ListItemDocumentationElement actual)
        {
            Assert.Equal(expected.Content.Count, actual.Content.Count);
            foreach (var pair in expected.Content.Zip(actual.Content, (expectedContent, actualContent) => new { ExpectedContent = expectedContent, ActualContent = actualContent }))
                _AssertAreEqual(pair.ExpectedContent, pair.ActualContent);
        }

        private static void _AssertAreEqual(DefinitionListItemDocumentationElement expected, DefinitionListItemDocumentationElement actual)
        {
            Assert.Equal(expected.Term.Count, actual.Term.Count);
            foreach (var pair in expected.Term.Zip(actual.Term, (expectedContent, actualContent) => new { ExpectedContent = expectedContent, ActualContent = actualContent }))
                _AssertAreEqual(pair.ExpectedContent, pair.ActualContent);

            Assert.Equal(expected.Description.Count, actual.Description.Count);
            foreach (var pair in expected.Description.Zip(actual.Description, (expectedContent, actualContent) => new { ExpectedContent = expectedContent, ActualContent = actualContent }))
                _AssertAreEqual(pair.ExpectedContent, pair.ActualContent);
        }

        private static void _AssertAreEqual(TableColumnDocumentationElement expected, TableColumnDocumentationElement actual)
        {
            Assert.Equal(expected.Name.Count, actual.Name.Count);
            foreach (var pair in expected.Name.Zip(actual.Name, (expectedContent, actualContent) => new { ExpectedContent = expectedContent, ActualContent = actualContent }))
                _AssertAreEqual(pair.ExpectedContent, pair.ActualContent);
        }

        private static void _AssertAreEqual(TableRowDocumentationElement expected, TableRowDocumentationElement actual)
        {
            Assert.Equal(expected.Cells.Count, actual.Cells.Count);
            foreach (var pair in expected.Cells.Zip(actual.Cells, (expectedCell, actualCell) => new { ExpectedCell = expectedCell, ActualCell = actualCell }))
                _AssertAreEqual(pair.ExpectedCell, pair.ActualCell);
        }

        private static void _AssertAreEqual(TableCellDocumentationElement expected, TableCellDocumentationElement actual)
        {
            Assert.Equal(expected.Content.Count, actual.Content.Count);
            foreach (var pair in expected.Content.Zip(actual.Content, (expectedContent, actualContent) => new { ExpectedContent = expectedContent, ActualContent = actualContent }))
                _AssertAreEqual(pair.ExpectedContent, pair.ActualContent);
        }

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