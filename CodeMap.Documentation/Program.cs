using CodeMap.DeclarationNodes;
using CodeMap.DocumentationElements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CodeMap.Documentation
{
    public static class DocumentationAdditionApplier
    {
        public static AssemblyDeclaration Apply(this AssemblyDeclaration assemblyDeclaration, IEnumerable<AssemblyDocumentationAddition> additions)
        {
            if (assemblyDeclaration is null)
                throw new ArgumentNullException(nameof(assemblyDeclaration));
            if (additions is null)
                throw new ArgumentNullException(nameof(additions));

            var addition = _GetMatchingAddition(assemblyDeclaration, additions);
            if (addition != null)
            {
                assemblyDeclaration.Summary = addition.Summary ?? assemblyDeclaration.Summary;
                assemblyDeclaration.Remarks = addition.Remarks ?? assemblyDeclaration.Remarks;
                assemblyDeclaration.Examples = addition.Examples ?? assemblyDeclaration.Examples;
                assemblyDeclaration.RelatedMembers = addition.RelatedMembers ?? assemblyDeclaration.RelatedMembers;
                if (addition.NamespaceAdditions != null)
                    foreach (var @namespace in assemblyDeclaration.Namespaces)
                        if (addition.NamespaceAdditions.TryGetValue(@namespace.Name, out var namespaceAddition))
                        {
                            @namespace.Summary = namespaceAddition.Summary ?? @namespace.Summary;
                            @namespace.Remarks = namespaceAddition.Remarks ?? @namespace.Remarks;
                            @namespace.Examples = namespaceAddition.Examples ?? @namespace.Examples;
                            @namespace.RelatedMembers = namespaceAddition.RelatedMembers ?? @namespace.RelatedMembers;
                        }
            }
            return assemblyDeclaration;
        }

        public static AssemblyDeclaration Apply(this AssemblyDeclaration assemblyDeclaration, params AssemblyDocumentationAddition[] additions)
            => assemblyDeclaration.Apply((IEnumerable<AssemblyDocumentationAddition>)additions);

        private static AssemblyDocumentationAddition _GetMatchingAddition(AssemblyDeclaration assemblyDeclaration, IEnumerable<AssemblyDocumentationAddition> additions)
            => additions
                .Where(addition => addition.CanApply(assemblyDeclaration))
                .Concat(additions.Where(addition => addition.CanApply is null))
                .FirstOrDefault();
    }

    public class AssemblyDocumentationAddition
    {
        public Func<AssemblyDeclaration, bool> CanApply { get; set; }

        public SummaryDocumentationElement Summary { get; set; }

        public RemarksDocumentationElement Remarks { get; set; }

        public IReadOnlyList<ExampleDocumentationElement> Examples { get; set; }

        public IReadOnlyList<MemberReferenceDocumentationElement> RelatedMembers { get; set; }

        public IReadOnlyDictionary<string, NamespaceDocumentationAddition> NamespaceAdditions { get; set; }
    }

    public class NamespaceDocumentationAddition
    {
        public SummaryDocumentationElement Summary { get; set; }

        public RemarksDocumentationElement Remarks { get; set; }

        public IReadOnlyList<ExampleDocumentationElement> Examples { get; set; }

        public IReadOnlyList<MemberReferenceDocumentationElement> RelatedMembers { get; set; }

        public IReadOnlyDictionary<string, NamespaceDocumentationAddition> NamespaceAdditions { get; set; }
    }

    internal static class Program
    {
        internal static void Main(string[] args)
        {
            var arguments = Arguments.GetFrom(args);
            if (arguments.OutputPath == null)
                throw new ArgumentException("Expected -OutputPath", nameof(args));

            var documentation = DeclarationNode
                .Create(typeof(DocumentationElement).Assembly)
                .Apply(
                    new AssemblyDocumentationAddition
                    {
                        CanApply = assemblyDeclaration => assemblyDeclaration.Version.Major == 1,
                        Summary = DocumentationElement.Summary(
                            DocumentationElement.Paragraph(
                                DocumentationElement.Text("An extensible tool for generating documentation for your .NET libraries. Generate HTML, Markdown or in any other format, customise the documentation at your discretion.")
                            )
                        ),
                        Remarks = DocumentationElement.Remarks(
                            DocumentationElement.Paragraph(
                                DocumentationElement.Text("Visual Studio enables developers to write comprehensive documentation inside their code using XML in three slashed comments, see "),
                                DocumentationElement.Hyperlink("https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/xml-documentation-comments", "XML Documentation Comments (C# Programming Guide)"),
                                DocumentationElement.Text(" for more details about supported tags.")
                            ),
                            DocumentationElement.Paragraph(
                                DocumentationElement.Text("When we build a project that has been configured to generate XML documentation files (simply specify the output path where resulting files should be saved) Visual Studio will look for this file and display the respective documentation in IntelliSense.")
                            ),
                            DocumentationElement.Paragraph(
                                DocumentationElement.Text("This is great and all library owners should be writing documentation like this in their code, however there aren't that many tools out there that allow us to transform the XML generated documentation into something else, say JSON or custom HTML, to display the exact same documentation on the project's website.")
                            ),
                            DocumentationElement.Paragraph(
                                DocumentationElement.Text("Having a single source for documentation is the best way to ensure consistency, what you see in code is what you see on the project's website. There are no discrepancies. Writing all documentation in code may upset developers, but after all they know best what the code they are writing is all about.")
                            ),
                            DocumentationElement.Paragraph(
                                DocumentationElement.Text("CodeMap aims to bring a tool into the developers hands that allows them to generate any output format they wish from a given assembly and associated XML documentation.")
                            ),
                            DocumentationElement.Paragraph(
                                DocumentationElement.Text("This is done following the visitor design pattern, an assembly is being visited and all elements that make up an assembly get their turn. When you want to generate a specific output (say Markdown or Creole) you simply implement your own visitor.")
                            ),
                            DocumentationElement.Paragraph(
                                DocumentationElement.Text("There are some elements that do not have XML documentation correspondents, such as assemblies, modules and namespaces. You cannot write documentation for neither in code and have it generated in the resulting XML document.")
                            ),
                            DocumentationElement.Paragraph(
                                DocumentationElement.Text("This tool does not work directly on the XML file but rather on a representation of the file (XDocument), this allows for the document to be loaded, updated if necessary and only then processed. This enables the tool to support additions to the standard XML document format allowing for extra documentation to be manually added. If you want to add specific documentation to a namespace, you can, but you have to do it manually which means you follow the XML document structure as well.")
                            ),
                            DocumentationElement.Paragraph(
                                DocumentationElement.Text("This project exposes an XML Schema (.xsd) that is used for validating an XML document containing documentation to ensure that it is being correctly processed.")
                            ),

                            DocumentationElement.Paragraph(
                                new[] { DocumentationElement.Text("Some elements are ignored when processing the XML document because they either are complex to use or there is little support for them.") },
                                new Dictionary<string, string>
                                {
                                    { "section", "Exceptions" }
                                }
                            ),
                            DocumentationElement.UnorderedList(
                                DocumentationElement.ListItem(
                                    DocumentationElement.Hyperlink("https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/include", "include"),
                                    DocumentationElement.Text(" is being ignored because its usage is rather complex and can be misleading. The documentation is being kept separate from the source code which can easily lead to discrepancies when changes to code occur.")
                                ),
                                DocumentationElement.ListItem(
                                    DocumentationElement.Hyperlink("https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/permission", "permission"),
                                    DocumentationElement.Text(" is being ignored because its usage is more of an edge case and code permissions are not supported in .NET Core making this element obsolete in this case.")
                                )
                            ),

                            DocumentationElement.Paragraph(
                                new[] { DocumentationElement.Text("There are XML elements defined for various sections and limited markup support. The main sections for documentation are the following:") },
                                new Dictionary<string, string>
                                {
                                    { "section", "Interpretations" }
                                }
                            ),
                            DocumentationElement.UnorderedList(
                                DocumentationElement.ListItem(
                                    DocumentationElement.Hyperlink("https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/summary", "summary")
                                ),
                                DocumentationElement.ListItem(
                                    DocumentationElement.Hyperlink("https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/typeparam", "typeparam"),
                                    DocumentationElement.Text(" (available only for types and methods) in .NET Core making this element obsolete in this case.")
                                ),
                                DocumentationElement.ListItem(
                                    DocumentationElement.Hyperlink("https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/param", "param"),
                                    DocumentationElement.Text(" (available only for delegates, constructors, methods, properties with parameters)")
                                ),
                                DocumentationElement.ListItem(
                                    DocumentationElement.Hyperlink("https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/returns", "returns"),
                                    DocumentationElement.Text(" (available only for delegates and methods with result)")
                                ),
                                DocumentationElement.ListItem(
                                    DocumentationElement.Hyperlink("https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/exception", "exception"),
                                    DocumentationElement.Text(" (available only for delegates, constructors, methods and properties)")
                                ),
                                DocumentationElement.ListItem(
                                    DocumentationElement.Hyperlink("https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/remarks", "remarks")
                                ),
                                DocumentationElement.ListItem(
                                    DocumentationElement.Hyperlink("https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/example", "example")
                                ),
                                DocumentationElement.ListItem(
                                    DocumentationElement.Hyperlink("https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/value", "value"),
                                    DocumentationElement.Text(" (available only for properties)")
                                ),
                                DocumentationElement.ListItem(
                                    DocumentationElement.Hyperlink("https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/seealso", "seealso")
                                )
                            ),
                            DocumentationElement.Paragraph(
                                DocumentationElement.Text("All of the above sections can contain documentation made out of blocks, text and limited markup. seealso does not contain any content and are interpreted as references.")
                            ),
                            DocumentationElement.Paragraph(
                                DocumentationElement.Text("The content blocks are made using the following tags:")
                            ),
                            DocumentationElement.UnorderedList(
                                DocumentationElement.ListItem(
                                    DocumentationElement.Hyperlink("https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/para", "para")
                                ),
                                DocumentationElement.ListItem(
                                    DocumentationElement.Hyperlink("https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/code", "code")
                                ),
                                DocumentationElement.ListItem(
                                    DocumentationElement.Hyperlink("https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/list", "list")
                                )
                            ),
                            DocumentationElement.Paragraph(
                                DocumentationElement.Text("It is not mandatory to specify the "),
                                DocumentationElement.Hyperlink("https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/para", "para"),
                                DocumentationElement.Text(" element because it is inferred. It is mandatory to do so only when you want to distinguish two paragraphs that are one after the other, but if there is plain text that has a "),
                                DocumentationElement.Hyperlink("https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/code", "code"),
                                DocumentationElement.Text(" or a "),
                                DocumentationElement.Hyperlink("https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/list", "list"),
                                DocumentationElement.Text(" element then the text before the respective tag is considered a paragraph and the text afterwards is considered a separate paragraph.")
                            ),
                            DocumentationElement.Paragraph(
                                DocumentationElement.Text("For instance, the following "),
                                DocumentationElement.Hyperlink("https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/returns", "returns"),
                                DocumentationElement.Text(" section contains three paragraphs, a code block and a list.")
                            ),
                            DocumentationElement.CodeBlock(@"
                                <remarks>
                                This is my first paragraph.
                                <code>
                                This is a code block
                                </code>
                                This is my second paragraph.
                                <list>
                                    <item>Item 1</item>
                                    <item>Item 2</item>
                                    <item>Item 3</item>
                                </list>
                                This is my third paragraph.
                                </remarks>".CollapseIndentation(),
                                new Dictionary<string, string>
                                {
                                    { "language", "xml" }
                                }
                            ),
                            DocumentationElement.Paragraph(
                                DocumentationElement.Text("To distinguish two paragraphs one after the other you need to use the para tag.")
                            ),
                            DocumentationElement.CodeBlock(@"
                                <remarks>
                                <para>
                                    This is my first paragraph.
                                </para>
                                <para>
                                    This is my second paragraph.
                                </para>
                                </remarks>".CollapseIndentation(),
                                new Dictionary<string, string>
                                {
                                    { "language", "xml" }
                                }
                            ),
                            DocumentationElement.Paragraph(
                                DocumentationElement.Text("Each content block can contain plain text and limited markup using the following tags:")
                            ),
                            DocumentationElement.UnorderedList(
                                DocumentationElement.ListItem(
                                    DocumentationElement.Hyperlink("https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/code-inline", "c")
                                ),
                                DocumentationElement.ListItem(
                                    DocumentationElement.Hyperlink("https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/see", "see")
                                ),
                                DocumentationElement.ListItem(
                                    DocumentationElement.Hyperlink("https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/paramref", "paramref")
                                ),
                                DocumentationElement.ListItem(
                                    DocumentationElement.Hyperlink("https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/typeparamref", "typeparamref")
                                )
                            ),
                            DocumentationElement.Paragraph(
                                DocumentationElement.Text("Some XML elements lack a proper documentation and are not properly interpreted by Visual Studio.")
                            ),
                            DocumentationElement.Paragraph(
                                DocumentationElement.Hyperlink("https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/list", "List"),
                                DocumentationElement.Text(" is one of these elements, regardless of how you define the list element, Visual Studio will display the contents as plain text. This is understandable, there should not be such complex markup in the summary of a method or parameter description.")
                            ),
                            DocumentationElement.Paragraph(
                                DocumentationElement.Text("One the other hand, the lack of documentation for this element and the lack of proper interpretation allows for more flexibility when it comes to parsing lists. This tool parses lists as follows:")
                            ),
                            DocumentationElement.Paragraph(
                                new InlineDocumentationElement[]
                                {
                                    DocumentationElement.Text("This corresponds to list type "),
                                    DocumentationElement.InlineCode("bullet"),
                                    DocumentationElement.Text(" and "),
                                    DocumentationElement.InlineCode("number"),
                                    DocumentationElement.Text(" that have no "),
                                    DocumentationElement.InlineCode("listheader"),
                                    DocumentationElement.Text(" element.")
                                },
                                new Dictionary<string, string>
                                {
                                    { "subsection", "Simple Lists" }
                                }
                            ),
                            DocumentationElement.Paragraph(
                                DocumentationElement.Text("To define a list simply use the "),
                                DocumentationElement.InlineCode("list"),
                                DocumentationElement.Text(" element, specify a type (the default is "),
                                DocumentationElement.InlineCode("bullet"),
                                DocumentationElement.Text(") and then define each item using an "),
                                DocumentationElement.InlineCode("item"),
                                DocumentationElement.Text(" element. Optionally include a "),
                                DocumentationElement.InlineCode("description"),
                                DocumentationElement.Text(" subelement (this is more for compliance with the documentation, having a "),
                                DocumentationElement.InlineCode("description"),
                                DocumentationElement.Text(" element or not makes no difference).")
                            ),

                            DocumentationElement.Paragraph(
                                new[] { DocumentationElement.Text("Bullet (unordered) list:") },
                                new Dictionary<string, string>
                                {
                                    { "subsection-example", "Examples" }
                                }
                            ),
                            DocumentationElement.CodeBlock(@"
                                <list type=""bullet"">
                                    <item>this is an item</item>
                                    <item>
                                        <description>this is an item using description, it is the same as the first one</description>
                                    </item>
                                    <item>the description element is optional, use at your discretion</item>
                                </list>".CollapseIndentation(),
                                new Dictionary<string, string>
                                {
                                    { "language", "xml" }
                                }
                            ),
                            DocumentationElement.Paragraph(
                                DocumentationElement.Text("Number (ordered) list:")
                            ),
                            DocumentationElement.CodeBlock(@"
                                <list type=""number"">
                                    <item>first item</item>
                                    <item>second item</item>
                                    <item>third item</item>
                                </list>".CollapseIndentation(),
                                new Dictionary<string, string>
                                {
                                    { "language", "xml" }
                                }
                            ),

                            DocumentationElement.Paragraph(
                                new InlineDocumentationElement[]
                                {
                                    DocumentationElement.Text("This is where it can get a bit confusing. The definition list is inferred by the given list type ("),
                                    DocumentationElement.InlineCode("bullet"),
                                    DocumentationElement.Text(" or "),
                                    DocumentationElement.InlineCode("number"),
                                    DocumentationElement.Text(") or lack of this attribute. Definition lists cannot be ordered or unordered, they define a list of terms. This makes the type attribute rather useless in this case, it can be safely omitted since the default is "),
                                    DocumentationElement.InlineCode("bullet"),
                                    DocumentationElement.Text(" and alligns with the interpretation.")
                                },
                                new Dictionary<string, string>
                                {
                                    { "subsection", "Definition Lists" }
                                }
                            ),
                            DocumentationElement.Paragraph(
                                DocumentationElement.Text("To create a definition list it must either have a title or one of its items to contain the "),
                                DocumentationElement.InlineCode("term"),
                                DocumentationElement.Text(" element. The title is optional, however if the "),
                                DocumentationElement.InlineCode("list"),
                                DocumentationElement.Text(" element is empty it will not be inferred as a definition list!")
                            ),
                            DocumentationElement.Paragraph(
                                DocumentationElement.Text("Optionally, a definition list can have a title which is defined using the "),
                                DocumentationElement.InlineCode("listheader"),
                                DocumentationElement.Text(" element. The title can be written directly in the element or can be wrapped in a "),
                                DocumentationElement.InlineCode("term"),
                                DocumentationElement.Text(" element.")
                            ),

                            DocumentationElement.Paragraph(
                                new[] { DocumentationElement.Text("Simple definition list:") },
                                new Dictionary<string, string>
                                {
                                    { "subsection-example", "Examples" }
                                }
                            ),
                            DocumentationElement.CodeBlock(@"
                                <list>
                                    <item>
                                        <term>Exception</term>
                                        <description>Describes an abnormal behaviour</description>
                                    </item>
                                    <item>
                                        <term>Logging</term>
                                        <description>Something we should be doing</description>
                                    </item>
                                    <item>
                                        <term>HTTP</term>
                                        <description>HyperText Transfer Protocol, today's standard for communication</description>
                                    </item>
                                </list>".CollapseIndentation(),
                                new Dictionary<string, string>
                                {
                                    { "language", "xml" }
                                }
                            ),
                            DocumentationElement.Paragraph(
                                DocumentationElement.Text("Definition list with title:")
                            ),
                            DocumentationElement.CodeBlock(@"
                                <list>
                                    <listheader>Music Genres</listheader>
                                    <item>
                                        <term>Pop</term>
                                        <description>Very popular (hence the name) genre, mostly singing nonsence.</description>
                                    </item>
                                    <item>
                                        <term>Rock</term>
                                        <description>Experiences with depth that are very musical with some grotesque exceptions that seem more like yelling than singing.</description>
                                    </item>
                                    <item>
                                        <term>EDM</term>
                                        <description>Electronic Dance Music, less singing mostly used for background noise or to help get into a specific mood.</description>
                                    </item>
                                </list>".CollapseIndentation(),
                                new Dictionary<string, string>
                                {
                                    { "language", "xml" }
                                }
                            ),
                            DocumentationElement.Paragraph(
                                DocumentationElement.Text("Same definition list, we can wrap the title in a "),
                                DocumentationElement.InlineCode("term"),
                                DocumentationElement.Text("element to conform with the documentation:")
                            ),
                            DocumentationElement.CodeBlock(@"
                                <list>
                                    <listheader>
                                        <term>Music Genres</term>
                                    </listheader>
                                    <item>
                                        <term>Pop</term>
                                        <description>Very popular (hence the name) genre, mostly singing nonsence.</description>
                                    </item>
                                    <item>
                                        <term>Rock</term>
                                        <description>Experiences with depth that are very musical with some grotesque exceptions that seem more like yelling than singing.</description>
                                    </item>
                                    <item>
                                        <term>EDM</term>
                                        <description>Electronic Dance Music, less singing mostly used for background noise or to help get into a specific mood.</description>
                                    </item>
                                </list>".CollapseIndentation(),
                                new Dictionary<string, string>
                                {
                                    { "language", "xml" }
                                }
                            ),
                            DocumentationElement.Paragraph(
                                DocumentationElement.Text("A definition list without a term for its second item:")
                            ),
                            DocumentationElement.CodeBlock(@"
                                <list>
                                    <item>
                                        <term>LOL</term>
                                        <description>Usually meaning laughing out loud, can be confused with League of Legends, a very popular video game.</description>
                                    </item>
                                    <item>
                                        <description>For some reason I forgot to mention the term I am currently describing, Infer this!</description>
                                    </item>
                                </list>".CollapseIndentation(),
                                new Dictionary<string, string>
                                {
                                    { "language", "xml" }
                                }
                            ),

                            DocumentationElement.Paragraph(
                                new[] { DocumentationElement.Text("This is where things will get a bit interesting. Tables are useful in a lot of scenarios, however there is no explicit syntax for them.") },
                                new Dictionary<string, string>
                                {
                                    { "subsection", "Tables" }
                                }
                            ),
                            DocumentationElement.Paragraph(
                                DocumentationElement.Text("To define a table specify the "),
                                DocumentationElement.InlineCode("type"),
                                DocumentationElement.Text(" of a "),
                                DocumentationElement.InlineCode("list"),
                                DocumentationElement.Text(" to "),
                                DocumentationElement.InlineCode("table"),
                                DocumentationElement.Text(". To define the header rows (if any) use the "),
                                DocumentationElement.InlineCode("listheader"),
                                DocumentationElement.Text(" element, to define each column header specify multiple "),
                                DocumentationElement.InlineCode("term"),
                                DocumentationElement.Text(" elements.")
                            ),
                            DocumentationElement.Paragraph(
                                DocumentationElement.Text("To define each row use the "),
                                DocumentationElement.InlineCode("item"),
                                DocumentationElement.Text(" element and for each column use a "),
                                DocumentationElement.InlineCode("description"),
                                DocumentationElement.Text(" element.")
                            ),
                            DocumentationElement.Paragraph(
                                DocumentationElement.Text("The number of columns is determined by the maximum number of "),
                                DocumentationElement.InlineCode("term"),
                                DocumentationElement.Text(" or "),
                                DocumentationElement.InlineCode("description"),
                                DocumentationElement.Text(" elements found in an enclosing element ("),
                                DocumentationElement.InlineCode("listheader"),
                                DocumentationElement.Text(" or "),
                                DocumentationElement.InlineCode("item"),
                                DocumentationElement.Text("). If there are missing values for a column header or row then they will be filled with blank.")
                            ),

                            DocumentationElement.Paragraph(
                                new[] { DocumentationElement.Text("Simple table:") },
                                new Dictionary<string, string>
                                {
                                    { "subsection-examples", "Examples" }
                                }
                            ),
                            DocumentationElement.CodeBlock(@"
                                <list type=""table"">
                                    <listheader>
                                        <term>Column 1</term>
                                        <term>Column 2</term>
                                    </listheader>
                                    <item>
                                        <description>Row 1, Column 1</description>
                                        <description>Row 1, Column 2</description>
                                    </item>
                                    <item>
                                        <description>Row 2, Column 1</description>
                                        <description>Row 2, Column 2</description>
                                    </item>
                                    <item>
                                        <description>Row 3, Column 1</description>
                                        <description>Row 3, Column 2</description>
                                    </item>
                                </list>".CollapseIndentation(),
                                new Dictionary<string, string>
                                {
                                    { "language", "xml" }
                                }
                            ),
                            DocumentationElement.Paragraph(
                                DocumentationElement.Text("Table without heading:")
                            ),
                            DocumentationElement.CodeBlock(@"
                                <list type=""table"">
                                    <item>
                                        <description>Row 1, Column 1</description>
                                        <description>Row 1, Column 2</description>
                                    </item>
                                    <item>
                                        <description>Row 2, Column 1</description>
                                        <description>Row 2, Column 2</description>
                                    </item>
                                    <item>
                                        <description>Row 3, Column 1</description>
                                        <description>Row 3, Column 2</description>
                                    </item>
                                </list>".CollapseIndentation(),
                                new Dictionary<string, string>
                                {
                                    { "language", "xml" }
                                }
                            ),
                            DocumentationElement.Paragraph(
                                DocumentationElement.Text("Table with missing values for last column:")
                            ),
                            DocumentationElement.CodeBlock(@"
                                <list type=""table"">
                                    <listheader>
                                        <term>Column 1</term>
                                    </listheader>
                                    <item>
                                        <description>Row 1, Column 1</description>
                                    </item>
                                    <item>
                                        <description>Row 2, Column 1</description>
                                        <description>Row 2, Column 2</description>
                                    </item>
                                    <item>
                                        <description>Row 3, Column 1</description>
                                    </item>
                                </list>".CollapseIndentation(),
                                new Dictionary<string, string>
                                {
                                    { "language", "xml" }
                                }
                            ),

                            DocumentationElement.Paragraph(
                                new[] { DocumentationElement.Text("There are no additions at this moment.") },
                                new Dictionary<string, string>
                                {
                                    { "section", "Additions" }
                                }
                            )
                        ),
                        NamespaceAdditions = new Dictionary<string, NamespaceDocumentationAddition>
                        {
                            {
                                "CodeMap.DeclarationNodes",
                                new NamespaceDocumentationAddition
                                {
                                    Summary = DocumentationElement.Summary(
                                        DocumentationElement.Paragraph(
                                            DocumentationElement.Text("This is a test")
                                        )
                                    )
                                }
                            }
                        }
                    }
                );

            var outputDirectory = new DirectoryInfo(arguments.OutputPath);
            outputDirectory.Create();

            documentation.Accept(new HtmlWriterDeclarationNodeVisitor(outputDirectory));
        }

        private class Arguments
        {
            public static Arguments GetFrom(IEnumerable<string> args)
            {
                var result = new Arguments();
                string name = null;
                foreach (var arg in args)
                    if (arg.StartsWith('-'))
                        name = arg.Substring(1);
                    else if (name != null)
                    {
                        var property = typeof(Arguments).GetRuntimeProperty(name);
                        if (property != null)
                            property.SetValue(result, arg);
                        name = null;
                    }

                return result;
            }

            public string OutputPath { get; set; }
        }
    }
}