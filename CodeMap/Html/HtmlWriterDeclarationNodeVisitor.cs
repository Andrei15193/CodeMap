using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CodeMap.DeclarationNodes;
using CodeMap.DocumentationElements;
using CodeMap.ReferenceData;

namespace CodeMap.Html
{
    /// <summary>
    /// A rudimentary HTML generator for <see cref="DeclarationNode"/>s. This is the most basic way of generating
    /// HTML documentation pages out of a <see cref="DeclarationNode"/> with customisation options.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The entire documentation for all <see cref="DeclarationNode"/>s is written to a single output. Basic hash
    /// navigation is added through a simple JavaScript block allowing for only one element to be displayed at a
    /// given time. This enables sharing of links and bookmarking a specific <see cref="DeclarationNode"/>.
    /// </para>
    /// <para>
    /// For each <see cref="DeclarationNode"/> a <c>section</c> element is generated where the ID is set to the
    /// full name reference (<see cref="DeclarationNodeExtensions.GetFullNameReference(DeclarationNode)"/>. This
    /// ID can be used to reference each declaration and it is used for navigation.
    /// </para>
    /// <para>
    /// Most likely there are references between assemblies for which hyperlinks need to be generated. At the very
    /// least there are the type references to .NET Framework. For this an <see cref="IMemberReferenceResolver"/>
    /// needs to be provided, there are implementation already available to get started with this quickly.
    /// </para>
    /// </remarks>
    /// <example>
    /// To get started, simply generate a <see cref="DeclarationNode"/> using one of the factory methods then
    /// pass it to an <see cref="HtmlWriterDeclarationNodeVisitor"/> along side an <see cref="IMemberReferenceResolver"/>.
    /// <code lang="c#">
    /// var codeMapAssemblyDeclarationNode = DeclarationNode.Create(typeof(DeclarationNode).Assembly);
    ///
    /// var defaultMemberReferenceResolver = new MicrosoftDocsMemberReferenceResolver("netstandard-2.1", "en-US");
    /// var memberReferenceResolver = new MemberReferenceResolver(defaultMemberReferenceResolver)
    /// {
    ///     // Don't forget to encode URLs, the full name reference may contain characters (such as &lt; and &gt;) that are forbidden.
    ///     // As mentioned, the generated HTML page uses basic hash navigation, all elements can be accessed using `#FullNameReference`,
    ///     // similar to classic in-page navigation, https://stackoverflow.com/questions/24739126/scroll-to-a-specific-element-using-html
    ///     { typeof(DeclarationNode).Assembly, MemberReferenceResolver.Create(memberReference => $"#{Uri.EscapeDataString(memberReference.GetFullNameReference())}") }
    /// };
    ///
    /// // Create output file stream.
    /// var outputFileInfo = new FileInfo(arguments.OutputFilePath);
    /// outputFileInfo.Directory.Create();
    /// using var outputFileStream = new FileStream(outputFileInfo.FullName, FileMode.Create, FileAccess.Write, FileShare.Read);
    /// using var outputFileStreamWriter = new StreamWriter(outputFileStream);
    ///
    /// // Instantiate the visitor.
    /// var htmlWriterDeclarationNodeVisitor = new CodeMalHtmlWriterDocumentaitonNodeVisitor(outputFileStreamWriter, memberReferenceResolver);
    ///
    /// // After all this setup, it is time to generate the HTML page.
    /// codeMapAssemblyDeclaration.Accept(htmlWriterDeclarationNodeVisitor);
    /// </code>
    /// </example>
    /// <seealso cref="HtmlWriterDocumentationVisitor"/>
    public class HtmlWriterDeclarationNodeVisitor : DeclarationNodeVisitor
    {
        private bool _isHtmlInitialized = false;
        private int _declarationDepth = 0;

        /// <summary>Initializes a new instance of the <see cref="HtmlWriterDeclarationNodeVisitor"/> class.</summary>
        /// <param name="textWriter">The <see cref="TextWriter"/> to which to write the HTML output.</param>
        /// <param name="memberReferenceResolver">The <see cref="IMemberReferenceResolver"/> used to generate URLs for <see cref="MemberReference"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="textWriter"/> or <paramref name="memberReferenceResolver"/> are <c>null</c>.</exception>
        public HtmlWriterDeclarationNodeVisitor(TextWriter textWriter, IMemberReferenceResolver memberReferenceResolver)
        {
            TextWriter = textWriter;
            MemberReferenceResolver = memberReferenceResolver ?? throw new ArgumentNullException(nameof(memberReferenceResolver));
            HasDefaultSection = false;
        }

        /// <summary>The <see cref="TextWriter"/> to which the HTML document is being written to.</summary>
        public TextWriter TextWriter { get; set; }

        /// <summary>The <see cref="IMemberReferenceResolver"/> used to generate URLs for <see cref="MemberReference"/>s.</summary>
        public IMemberReferenceResolver MemberReferenceResolver { get; }

        /// <summary>An internal flag which indicates whether there is a <see cref="DeclarationNode"/> <c>section</c> element generated.</summary>
        /// <remarks>
        /// The first <c>section</c> element that is generated is considered the default, typically this would be for the
        /// <see cref="AssemblyDeclaration"/>, but HTML pages can be generated only for a type and its declared members as well.
        /// </remarks>
        protected bool HasDefaultSection { get; private set; }

        /// <summary>Provides a <see cref="DocumentationVisitor"/> for outputting the related <see cref="DocumentationElement"/>s of a <see cref="DeclarationNode"/>.</summary>
        /// <returns>Returns a <see cref="DocumentationVisitor"/> for outputting the related <see cref="DocumentationElement"/>s of a <see cref="DeclarationNode"/>.</returns>
        /// <remarks>
        /// Implicitly this returns <see cref="HtmlWriterDocumentationVisitor"/> which writers HTML to a provided <see cref="TextWriter"/>.
        /// For customisation, this method can be overridden and have a different <see cref="DocumentationVisitor"/> provided.
        /// </remarks>
        /// <seealso cref="HtmlWriterDocumentationVisitor"/>
        protected virtual DocumentationVisitor CreateDocumentationVisitor()
            => new HtmlWriterDocumentationVisitor(TextWriter, MemberReferenceResolver);

        /// <summary>Visits an <see cref="AssemblyDeclaration"/>.</summary>
        /// <param name="assembly">The <see cref="AssemblyDeclaration"/> to visit.</param>
        protected internal sealed override void VisitAssembly(AssemblyDeclaration assembly)
        {
            _WriteHtmlDocumentBeginning(assembly);

            WriteAssemblyDeclaration(assembly);
            HasDefaultSection = true;

            _declarationDepth++;
            foreach (var @namespace in assembly.Namespaces)
                @namespace.Accept(this);
            _declarationDepth--;

            _WriteHtmlDocumentEnding(assembly);
        }

        /// <summary>Writes the HTML documentation section for the provided <paramref name="assembly"/>.</summary>
        /// <param name="assembly">The <see cref="AssemblyDeclaration"/> for which to write the documentation.</param>
        protected virtual void WriteAssemblyDeclaration(AssemblyDeclaration assembly)
        {
            WriteDeclarationSectionBeginning(assembly);

            WriteNavigation(assembly);
            WritePageHeading($"{assembly.Name}@{assembly.GetInformalVersion()}");

            WriteSummary(assembly.Summary);
            WriteNamespacesList(assembly.Namespaces);

            WriteExamples(assembly.Examples);
            WriteRemarks(assembly.Remarks);
            WriteRelatedMembers(assembly.RelatedMembers);

            WriteAttributes(assembly.Attributes, "attributes", "Attributes");

            WriteDeclarationSectionEnding(assembly);
        }

        /// <summary>Writes an HTML table for the provided <paramref name="namespaces"/>.</summary>
        /// <param name="namespaces">The <see cref="NamespaceDeclaration"/> for which to write the HTML table.</param>
        /// <remarks>
        /// The table contains two columns, one is the namespace itself, which a hyperlink towards the documentation section.
        /// The second column contains the first paragraph of the namespace summary, if there is one.
        /// </remarks>
        /// <seealso cref="WriteFirstSummaryParagraph(SummaryDocumentationElement)"/>
        protected virtual void WriteNamespacesList(IEnumerable<NamespaceDeclaration> namespaces)
        {
            if (namespaces.Any())
            {
                TextWriter.Write("<section data-sectionId=\"namespaces\">");

                TextWriter.Write("<table>");

                TextWriter.Write("<thead>");
                TextWriter.Write("<tr>");
                TextWriter.Write("<th>");
                WriteSafeHtml("Namespace");
                TextWriter.Write("</th>");
                TextWriter.Write("<th>");
                WriteSafeHtml("Description");
                TextWriter.Write("</th>");
                TextWriter.Write("</tr>");
                TextWriter.Write("</thead>");

                TextWriter.Write("<tbody>");
                foreach (var @namespace in namespaces)
                {
                    TextWriter.Write("<tr>");
                    TextWriter.Write("<td>");
                    TextWriter.Write("<a href=\"#");
                    TextWriter.Write(Uri.EscapeDataString(@namespace.GetFullNameReference()));
                    TextWriter.Write("\">");
                    WriteSafeHtml(@namespace.GetSimpleNameReference());
                    TextWriter.Write("</a>");
                    TextWriter.Write("</td>");
                    TextWriter.Write("<td>");
                    WriteFirstSummaryParagraph(@namespace.Summary);
                    TextWriter.Write("</td>");
                    TextWriter.Write("</tr>");
                }
                TextWriter.Write("</tbody>");

                TextWriter.Write("</table>");

                TextWriter.Write("</section>");
            }
        }

        /// <summary>Visits a <see cref="NamespaceDeclaration"/>.</summary>
        /// <param name="namespace">The <see cref="NamespaceDeclaration"/> to visit.</param>
        protected internal sealed override void VisitNamespace(NamespaceDeclaration @namespace)
        {
            _WriteHtmlDocumentBeginning(@namespace);

            WriteNamespaceDeclaration(@namespace);
            HasDefaultSection = true;

            _declarationDepth++;
            foreach (var type in @namespace.DeclaredTypes)
                type.Accept(this);
            _declarationDepth--;

            _WriteHtmlDocumentEnding(@namespace);
        }

        /// <summary>Writes the HTML documentation section for the provided <paramref name="namespace"/>.</summary>
        /// <param name="namespace">The <see cref="NamespaceDeclaration"/> for which to write the documentation.</param>
        protected virtual void WriteNamespaceDeclaration(NamespaceDeclaration @namespace)
        {
            WriteDeclarationSectionBeginning(@namespace);

            WriteNavigation(@namespace);
            WritePageHeading($"{@namespace.Name} Namespace");

            WriteSummary(@namespace.Summary);
            WriteTypesList(@namespace.Enums, "Enums", "enums");
            WriteTypesList(@namespace.Delegates, "Delegates", "delegates");
            WriteTypesList(@namespace.Interfaces, "Interfaces", "interfaces");
            WriteTypesList(@namespace.Classes, "Classes", "classes");
            WriteTypesList(@namespace.Records, "Records", "records");
            WriteTypesList(@namespace.Structs, "DeclaredStructs", "structs");

            WriteExamples(@namespace.Examples);
            WriteRemarks(@namespace.Remarks);
            WriteRelatedMembers(@namespace.RelatedMembers);

            WriteDeclarationSectionEnding(@namespace);
        }

        /// <summary>Visits an <see cref="EnumDeclaration"/>.</summary>
        /// <param name="enum">The <see cref="EnumDeclaration"/> to visit.</param>
        protected internal sealed override void VisitEnum(EnumDeclaration @enum)
        {
            _WriteHtmlDocumentBeginning(@enum);

            WriteEnumDeclaration(@enum);
            HasDefaultSection = true;

            _WriteHtmlDocumentEnding(@enum);
        }

        /// <summary>Writes the HTML documentation section for the provided <paramref name="enum"/>.</summary>
        /// <param name="enum">The <see cref="EnumDeclaration"/> for which to write the documentation.</param>
        protected virtual void WriteEnumDeclaration(EnumDeclaration @enum)
        {
            WriteDeclarationSectionBeginning(@enum);

            WriteNavigation(@enum);
            WritePageHeading($"{@enum.GetSimpleNameReference()} Enum", @enum.AccessModifier);

            WriteSummary(@enum.Summary);

            TextWriter.Write("<p>");
            WriteSafeHtml("This enum is ");
            WriteAccessModifier(@enum.AccessModifier);
            WriteSafeHtml(".");
            TextWriter.Write("</p>");

            WriteEnumMembersList(@enum.Members);

            WriteExamples(@enum.Examples);
            WriteRemarks(@enum.Remarks);
            WriteRelatedMembers(@enum.RelatedMembers);

            WriteDeclarationSectionEnding(@enum);
        }

        /// <summary>Writes an HTML table for the provided enum <paramref name="members"/>.</summary>
        /// <param name="members">The enum <see cref="ConstantDeclaration"/>s for which to write the HTML table.</param>
        /// <remarks>
        /// The HTML table contains 3 columns. The first one is the member name, the second is the value of the member and
        /// the last contains the first paragraph of the summary of the related member.
        /// </remarks>
        /// <seealso cref="WriteFirstSummaryParagraph(SummaryDocumentationElement)"/>
        protected virtual void WriteEnumMembersList(IEnumerable<ConstantDeclaration> members)
        {
            if (members.Any())
            {
                TextWriter.Write("<section data-sectionId=\"members\">");

                TextWriter.Write("<table>");

                TextWriter.Write("<caption>");
                WriteSafeHtml("Members");
                TextWriter.Write("</caption>");

                TextWriter.Write("<thead>");
                TextWriter.Write("<tr>");
                TextWriter.Write("<th>");
                WriteSafeHtml("Name");
                TextWriter.Write("</th>");
                TextWriter.Write("<th>");
                WriteSafeHtml("Value");
                TextWriter.Write("</th>");
                TextWriter.Write("<th>");
                WriteSafeHtml("Description");
                TextWriter.Write("</th>");
                TextWriter.Write("</tr>");
                TextWriter.Write("</thead>");

                TextWriter.Write("<tbody>");
                foreach (var member in members)
                {
                    TextWriter.Write("<tr>");
                    TextWriter.Write("<td><code>");
                    WriteSafeHtml(member.GetSimpleNameReference());
                    TextWriter.Write("</code></td>");
                    TextWriter.Write("<td><code>");
                    WriteValue(member.Value);
                    TextWriter.Write("</code></td>");
                    TextWriter.Write("<td>");
                    WriteFirstSummaryParagraph(member.Summary);
                    TextWriter.Write("</td>");
                    TextWriter.Write("</tr>");
                }
                TextWriter.Write("</tbody>");

                TextWriter.Write("</table>");

                TextWriter.Write("</section>");
            }
        }

        /// <summary>Visits a <see cref="DelegateDeclaration"/>.</summary>
        /// <param name="delegate">The <see cref="DelegateDeclaration"/> to visit.</param>
        protected internal sealed override void VisitDelegate(DelegateDeclaration @delegate)
        {
            _WriteHtmlDocumentBeginning(@delegate);

            WriteDelegateDeclaration(@delegate);
            HasDefaultSection = true;

            _WriteHtmlDocumentEnding(@delegate);
        }

        /// <summary>Writes the HTML documentation section for the provided <paramref name="delegate"/>.</summary>
        /// <param name="delegate">The <see cref="DelegateDeclaration"/> for which to write the documentation.</param>
        protected void WriteDelegateDeclaration(DelegateDeclaration @delegate)
        {
            WriteDeclarationSectionBeginning(@delegate);

            WriteNavigation(@delegate);
            WritePageHeading($"{@delegate.GetSimpleNameReference()} Delegate", @delegate.AccessModifier);

            WriteSummary(@delegate.Summary);

            TextWriter.Write("<p>");
            WriteSafeHtml("This delegate is ");
            WriteAccessModifier(@delegate.AccessModifier);
            WriteSafeHtml(".");
            TextWriter.Write("</p>");

            WriteGenericParameters(@delegate.GenericParameters);
            WriteParameters(@delegate.Parameters);

            WriteReturn(@delegate.Return);
            WriteExceptions(@delegate.Exceptions);
            WriteExamples(@delegate.Examples);
            WriteRemarks(@delegate.Remarks);
            WriteRelatedMembers(@delegate.RelatedMembers);

            WriteDeclarationSectionEnding(@delegate);
        }

        /// <summary>Visits an <see cref="InterfaceDeclaration"/>.</summary>
        /// <param name="interface">The <see cref="InterfaceDeclaration"/> to visit.</param>
        protected internal sealed override void VisitInterface(InterfaceDeclaration @interface)
        {
            _WriteHtmlDocumentBeginning(@interface);

            WriteInterfaceDeclaration(@interface);
            HasDefaultSection = true;

            foreach (var member in @interface.Members)
                member.Accept(this);

            _WriteHtmlDocumentEnding(@interface);
        }

        /// <summary>Writes the HTML documentation section for the provided <paramref name="interface"/>.</summary>
        /// <param name="interface">The <see cref="InterfaceDeclaration"/> for which to write the documentation.</param>
        protected virtual void WriteInterfaceDeclaration(InterfaceDeclaration @interface)
        {
            WriteDeclarationSectionBeginning(@interface);

            WriteNavigation(@interface);
            WritePageHeading($"{@interface.GetSimpleNameReference()} Interface", @interface.AccessModifier);

            WriteSummary(@interface.Summary);

            TextWriter.Write("<p>");
            WriteSafeHtml("This interface is ");
            WriteAccessModifier(@interface.AccessModifier);
            WriteSafeHtml(".");
            TextWriter.Write("</p>");

            if (@interface.BaseInterfaces.Any())
            {
                TextWriter.Write("<p>");
                WriteSafeHtml("This interface extends the following interfaces: ");
                var isFrist = true;
                foreach (var baseInterface in @interface.BaseInterfaces)
                {
                    if (isFrist)
                        isFrist = false;
                    else
                        WriteSafeHtml(", ");

                    WriteConstructedTypeReference(baseInterface);
                }
                WriteSafeHtml(".");
                TextWriter.Write("</p>");
            }

            WriteGenericParameters(@interface.GenericParameters);
            WriteMembersList(@interface.Events, "Events", "events", hideAccessModifier: true);
            WriteMembersList(@interface.Properties, "Properties", "properties", hideAccessModifier: true);
            WriteMembersList(@interface.Methods, "Methods", "methods", hideAccessModifier: true);

            WriteExamples(@interface.Examples);
            WriteRemarks(@interface.Remarks);
            WriteRelatedMembers(@interface.RelatedMembers);

            WriteDeclarationSectionEnding(@interface);
        }

        /// <summary>Visits a <see cref="ClassDeclaration"/>.</summary>
        /// <param name="class">The <see cref="ClassDeclaration"/> to visit.</param>
        protected internal sealed override void VisitClass(ClassDeclaration @class)
        {
            _WriteHtmlDocumentBeginning(@class);

            WriteClassDeclaration(@class);
            HasDefaultSection = true;

            foreach (var type in @class.NestedTypes)
                type.Accept(this);
            foreach (var member in @class.Members)
                member.Accept(this);

            _WriteHtmlDocumentEnding(@class);
        }

        /// <summary>Writes the HTML documentation section for the provided <paramref name="class"/>.</summary>
        /// <param name="class">The <see cref="ClassDeclaration"/> for which to write the documentation.</param>
        protected virtual void WriteClassDeclaration(ClassDeclaration @class)
        {
            WriteDeclarationSectionBeginning(@class);

            WriteNavigation(@class);
            WritePageHeading($"{@class.GetSimpleNameReference()} Class", @class.AccessModifier);

            WriteSummary(@class.Summary);

            TextWriter.Write("<p>");
            WriteSafeHtml("This class is ");
            WriteAccessModifier(@class.AccessModifier);
            WriteSafeHtml(".");
            TextWriter.Write("</p>");

            if (@class.BaseClass != typeof(object))
            {
                TextWriter.Write("<p>");
                WriteSafeHtml("This class extends ");
                WriteConstructedTypeReference(@class.BaseClass);
                WriteSafeHtml(".");
                TextWriter.Write("</p>");
            }
            if (@class.ImplementedInterfaces.Any())
            {
                TextWriter.Write("<p>");
                WriteSafeHtml("This class implements the following interfaces: ");
                var isFrist = true;
                foreach (var imlementedInterface in @class.ImplementedInterfaces)
                {
                    if (isFrist)
                        isFrist = false;
                    else
                        WriteSafeHtml(", ");

                    WriteConstructedTypeReference(imlementedInterface);
                }
                WriteSafeHtml(".");
                TextWriter.Write("</p>");
            }

            WriteGenericParameters(@class.GenericParameters);
            WriteMembersList(@class.Constants, "Constants", "constants");
            WriteMembersList(@class.Fields, "Fields", "fields");
            WriteMembersList(@class.Constructors, "Constructors", "constructors");
            WriteMembersList(@class.Events, "Events", "events");
            WriteMembersList(@class.Properties, "Properties", "properties");
            WriteMembersList(@class.Methods, "Methods", "methods");

            WriteExamples(@class.Examples);
            WriteRemarks(@class.Remarks);
            WriteRelatedMembers(@class.RelatedMembers);

            WriteDeclarationSectionEnding(@class);
        }

        /// <summary>Visits a <see cref="RecordDeclaration"/>.</summary>
        /// <param name="record">The <see cref="RecordDeclaration"/> to visit.</param>
        protected internal sealed override void VisitRecord(RecordDeclaration record)
        {
            _WriteHtmlDocumentBeginning(record);

            WriteRecordDeclaration(record);
            HasDefaultSection = true;

            foreach (var type in record.NestedTypes)
                type.Accept(this);
            foreach (var member in record.Members)
                member.Accept(this);

            _WriteHtmlDocumentEnding(record);
        }

        /// <summary>Writes the HTML documentation section for the provided <paramref name="record"/>.</summary>
        /// <param name="record">The <see cref="RecordDeclaration"/> for which to write the documentation.</param>
        protected virtual void WriteRecordDeclaration(RecordDeclaration record)
        {
            WriteDeclarationSectionBeginning(record);

            WriteNavigation(record);
            WritePageHeading($"{record.GetSimpleNameReference()} Record", record.AccessModifier);

            WriteSummary(record.Summary);

            TextWriter.Write("<p>");
            WriteSafeHtml("This record is ");
            WriteAccessModifier(record.AccessModifier);
            WriteSafeHtml(".");
            TextWriter.Write("</p>");

            if (record.BaseRecord != typeof(object))
            {
                TextWriter.Write("<p>");
                WriteSafeHtml("This class extends ");
                WriteConstructedTypeReference(record.BaseRecord);
                WriteSafeHtml(".");
                TextWriter.Write("</p>");
            }
            if (record.ImplementedInterfaces.Any())
            {
                TextWriter.Write("<p>");
                WriteSafeHtml("This class implements the following interfaces: ");
                var isFrist = true;
                foreach (var imlementedInterface in record.ImplementedInterfaces)
                {
                    if (isFrist)
                        isFrist = false;
                    else
                        WriteSafeHtml(", ");

                    WriteConstructedTypeReference(imlementedInterface);
                }
                WriteSafeHtml(".");
                TextWriter.Write("</p>");
            }

            WriteGenericParameters(record.GenericParameters);
            WriteMembersList(record.Constants, "Constants", "constants");
            WriteMembersList(record.Fields, "Fields", "fields");
            WriteMembersList(record.Constructors, "Constructors", "constructors");
            WriteMembersList(record.Events, "Events", "events");
            WriteMembersList(record.Properties, "Properties", "properties");
            WriteMembersList(record.Methods, "Methods", "methods");

            WriteExamples(record.Examples);
            WriteRemarks(record.Remarks);
            WriteRelatedMembers(record.RelatedMembers);

            WriteDeclarationSectionEnding(record);
        }

        /// <summary>Visits a <see cref="StructDeclaration"/>.</summary>
        /// <param name="struct">The <see cref="StructDeclaration"/> to visit.</param>
        protected internal sealed override void VisitStruct(StructDeclaration @struct)
        {
            _WriteHtmlDocumentBeginning(@struct);

            WriteStructDeclaration(@struct);
            HasDefaultSection = true;

            foreach (var type in @struct.NestedTypes)
                type.Accept(this);
            foreach (var member in @struct.Members)
                member.Accept(this);

            _WriteHtmlDocumentEnding(@struct);
        }

        /// <summary>Writes the HTML documentation section for the provided <paramref name="struct"/>.</summary>
        /// <param name="struct">The <see cref="StructDeclaration"/> for which to write the documentation.</param>
        protected virtual void WriteStructDeclaration(StructDeclaration @struct)
        {
            WriteDeclarationSectionBeginning(@struct);

            WriteNavigation(@struct);
            WritePageHeading($"{@struct.GetSimpleNameReference()} Struct", @struct.AccessModifier);

            WriteSummary(@struct.Summary);

            TextWriter.Write("<p>");
            WriteSafeHtml("This struct is ");
            WriteAccessModifier(@struct.AccessModifier);
            WriteSafeHtml(".");
            TextWriter.Write("</p>");

            if (@struct.ImplementedInterfaces.Any())
            {
                TextWriter.Write("<p>");
                WriteSafeHtml("This class implements the following interfaces: ");
                var isFrist = true;
                foreach (var imlementedInterface in @struct.ImplementedInterfaces)
                {
                    if (isFrist)
                        isFrist = false;
                    else
                        WriteSafeHtml(", ");

                    WriteConstructedTypeReference(imlementedInterface);
                }
                WriteSafeHtml(".");
                TextWriter.Write("</p>");
            }

            WriteGenericParameters(@struct.GenericParameters);
            WriteMembersList(@struct.Constants, "Constants", "constants");
            WriteMembersList(@struct.Fields, "Fields", "fields");
            WriteMembersList(@struct.Constructors, "Constructors", "constructors");
            WriteMembersList(@struct.Events, "Events", "events");
            WriteMembersList(@struct.Properties, "Properties", "properties");
            WriteMembersList(@struct.Methods, "Methods", "methods");

            WriteExamples(@struct.Examples);
            WriteRemarks(@struct.Remarks);
            WriteRelatedMembers(@struct.RelatedMembers);

            WriteDeclarationSectionEnding(@struct);
        }

        /// <summary>Visits a <see cref="ConstantDeclaration"/>.</summary>
        /// <param name="constant">The <see cref="ConstantDeclaration"/> to visit.</param>
        protected internal sealed override void VisitConstant(ConstantDeclaration constant)
        {
            _WriteHtmlDocumentBeginning(constant);

            WriteConstantDeclaration(constant);
            HasDefaultSection = true;

            _WriteHtmlDocumentEnding(constant);
        }

        /// <summary>Writes the HTML documentation section for the provided <paramref name="constant"/>.</summary>
        /// <param name="constant">The <see cref="ConstantDeclaration"/> for which to write the documentation.</param>
        protected virtual void WriteConstantDeclaration(ConstantDeclaration constant)
        {
            WriteDeclarationSectionBeginning(constant);

            WriteNavigation(constant);
            WritePageHeading($"{constant.GetSimpleNameReference()} Constant", constant.AccessModifier);

            WriteSummary(constant.Summary);
            TextWriter.Write("<p>");
            WriteSafeHtml("The value of this constant is ");
            TextWriter.Write("<code>");
            WriteValue(constant.Value);
            TextWriter.Write("</code>");
            WriteSafeHtml(".");
            TextWriter.Write("</p>");

            TextWriter.Write("<p>");
            WriteSafeHtml("This constant is ");
            WriteAccessModifier(constant.AccessModifier);
            WriteSafeHtml(".");
            TextWriter.Write("</p>");

            WriteExamples(constant.Examples);
            WriteRemarks(constant.Remarks);
            WriteRelatedMembers(constant.RelatedMembers);

            WriteDeclarationSectionEnding(constant);
        }

        /// <summary>Visits a <see cref="FieldDeclaration"/>.</summary>
        /// <param name="field">The <see cref="FieldDeclaration"/> to visit.</param>
        protected internal sealed override void VisitField(FieldDeclaration field)
        {
            _WriteHtmlDocumentBeginning(field);

            WriteFieldDeclaration(field);
            HasDefaultSection = true;

            _WriteHtmlDocumentEnding(field);
        }

        /// <summary>Writes the HTML documentation section for the provided <paramref name="field"/>.</summary>
        /// <param name="field">The <see cref="FieldDeclaration"/> for which to write the documentation.</param>
        protected virtual void WriteFieldDeclaration(FieldDeclaration field)
        {
            WriteDeclarationSectionBeginning(field);

            WriteNavigation(field);
            WritePageHeading($"{field.GetSimpleNameReference()} Field", field.AccessModifier);

            WriteSummary(field.Summary);

            TextWriter.Write("<p>");
            WriteSafeHtml("This field is ");
            WriteAccessModifier(field.AccessModifier);
            WriteSafeHtml(".");
            TextWriter.Write("</p>");

            WriteExamples(field.Examples);
            WriteRemarks(field.Remarks);
            WriteRelatedMembers(field.RelatedMembers);

            WriteDeclarationSectionEnding(field);
        }

        /// <summary>Visits a <see cref="ConstructorDeclaration"/>.</summary>
        /// <param name="constructor">The <see cref="ConstructorDeclaration"/> to visit.</param>
        protected internal sealed override void VisitConstructor(ConstructorDeclaration constructor)
        {
            _WriteHtmlDocumentBeginning(constructor);

            WriteConstructorDeclaration(constructor);
            HasDefaultSection = true;

            _WriteHtmlDocumentEnding(constructor);
        }

        /// <summary>Writes the HTML documentation section for the provided <paramref name="constructor"/>.</summary>
        /// <param name="constructor">The <see cref="ConstructorDeclaration"/> for which to write the documentation.</param>
        private void WriteConstructorDeclaration(ConstructorDeclaration constructor)
        {
            WriteDeclarationSectionBeginning(constructor);

            WriteNavigation(constructor);
            WritePageHeading($"{constructor.GetSimpleNameReference()} Constructor", constructor.AccessModifier);

            WriteSummary(constructor.Summary);

            TextWriter.Write("<p>");
            WriteSafeHtml("This constructor is ");
            WriteAccessModifier(constructor.AccessModifier);
            WriteSafeHtml(".");
            TextWriter.Write("</p>");

            WriteParameters(constructor.Parameters);

            WriteExceptions(constructor.Exceptions);
            WriteExamples(constructor.Examples);
            WriteRemarks(constructor.Remarks);
            WriteRelatedMembers(constructor.RelatedMembers);

            WriteDeclarationSectionEnding(constructor);
        }

        /// <summary>Visits a <see cref="EventDeclaration"/>.</summary>
        /// <param name="event">The <see cref="EventDeclaration"/> to visit.</param>
        protected internal sealed override void VisitEvent(EventDeclaration @event)
        {
            _WriteHtmlDocumentBeginning(@event);

            WriteEventDeclaration(@event);
            HasDefaultSection = true;

            _WriteHtmlDocumentEnding(@event);
        }

        /// <summary>Writes the HTML documentation section for the provided <paramref name="event"/>.</summary>
        /// <param name="event">The <see cref="EventDeclaration"/> for which to write the documentation.</param>
        protected virtual void WriteEventDeclaration(EventDeclaration @event)
        {
            WriteDeclarationSectionBeginning(@event);

            WriteNavigation(@event);
            WritePageHeading($"{@event.GetSimpleNameReference()} Event", @event.AccessModifier);

            WriteSummary(@event.Summary);
            TextWriter.Write("<p>");
            WriteSafeHtml("The type of this event is ");
            WriteConstructedTypeReference(@event.Type);
            WriteSafeHtml(".");
            TextWriter.Write("</p>");

            TextWriter.Write("<p>");
            WriteSafeHtml("This event is ");
            WriteAccessModifier(@event.AccessModifier);
            WriteSafeHtml(".");
            TextWriter.Write("</p>");

            if (@event.IsAbstract)
            {
                TextWriter.Write("<p>");
                WriteSafeHtml("This event is abstract.");
                TextWriter.Write("</p>");
            }
            if (@event.IsVirtual)
            {
                TextWriter.Write("<p>");
                WriteSafeHtml("This event is virtual.");
                TextWriter.Write("</p>");
            }
            if (@event.IsOverride)
            {
                TextWriter.Write("<p>");
                WriteSafeHtml("This event is an override.");
                TextWriter.Write("</p>");
            }
            if (@event.IsSealed)
            {
                TextWriter.Write("<p>");
                WriteSafeHtml("This event is sealed.");
                TextWriter.Write("</p>");
            }
            if (@event.IsShadowing)
            {
                TextWriter.Write("<p>");
                WriteSafeHtml("This event is hiding an inherited member with the same name.");
                TextWriter.Write("</p>");
            }
            if (@event.IsStatic)
            {
                TextWriter.Write("<p>");
                WriteSafeHtml("This event is static.");
                TextWriter.Write("</p>");
            }

            WriteExceptions(@event.Exceptions);
            WriteExamples(@event.Examples);
            WriteRemarks(@event.Remarks);
            WriteRelatedMembers(@event.RelatedMembers);

            WriteDeclarationSectionEnding(@event);
        }

        /// <summary>Visits a <see cref="PropertyDeclaration"/>.</summary>
        /// <param name="property">The <see cref="PropertyDeclaration"/> to visit.</param>
        protected internal sealed override void VisitProperty(PropertyDeclaration property)
        {
            _WriteHtmlDocumentBeginning(property);

            WritePropertyDeclaration(property);
            HasDefaultSection = true;

            _WriteHtmlDocumentEnding(property);
        }

        /// <summary>Writes the HTML documentation section for the provided <paramref name="property"/>.</summary>
        /// <param name="property">The <see cref="PropertyDeclaration"/> for which to write the documentation.</param>
        protected virtual void WritePropertyDeclaration(PropertyDeclaration property)
        {
            WriteDeclarationSectionBeginning(property);

            WriteNavigation(property);
            WritePageHeading($"{property.GetSimpleNameReference()} Property", property.AccessModifier);

            WriteSummary(property.Summary);
            TextWriter.Write("<p>");
            WriteSafeHtml("The type of this property is ");
            WriteConstructedTypeReference(property.Type);
            WriteSafeHtml(".");
            TextWriter.Write("</p>");

            TextWriter.Write("<p>");
            if (property.Getter != null && property.Setter != null)
            {
                WriteSafeHtml("This property has a ");
                WriteAccessModifier(property.Getter.AccessModifier);
                WriteSafeHtml(" getter and a ");
                WriteAccessModifier(property.Setter.AccessModifier);
                WriteSafeHtml(" setter.");
            }
            else if (property.Getter != null)
            {
                WriteSafeHtml("This property has a ");
                WriteAccessModifier(property.Getter.AccessModifier);
                WriteSafeHtml(" getter.");
            }
            else if (property.Setter != null)
            {
                WriteSafeHtml("This property has a ");
                WriteAccessModifier(property.Setter.AccessModifier);
                WriteSafeHtml(" setter.");
            }
            TextWriter.Write("</p>");

            if (property.IsAbstract)
            {
                TextWriter.Write("<p>");
                WriteSafeHtml("This property is abstract.");
                TextWriter.Write("</p>");
            }
            if (property.IsVirtual)
            {
                TextWriter.Write("<p>");
                WriteSafeHtml("This property is virtual.");
                TextWriter.Write("</p>");
            }
            if (property.IsOverride)
            {
                TextWriter.Write("<p>");
                WriteSafeHtml("This property is an override.");
                TextWriter.Write("</p>");
            }
            if (property.IsSealed)
            {
                TextWriter.Write("<p>");
                WriteSafeHtml("This property is sealed.");
                TextWriter.Write("</p>");
            }
            if (property.IsShadowing)
            {
                TextWriter.Write("<p>");
                WriteSafeHtml("This property is hiding an inherited member with the same name.");
                TextWriter.Write("</p>");
            }
            if (property.IsStatic)
            {
                TextWriter.Write("<p>");
                WriteSafeHtml("This property is static.");
                TextWriter.Write("</p>");
            }
            WriteValue(property.Value);

            WriteExceptions(property.Exceptions);
            WriteExamples(property.Examples);
            WriteRemarks(property.Remarks);
            WriteRelatedMembers(property.RelatedMembers);

            WriteDeclarationSectionEnding(property);
        }

        /// <summary>Visits a <see cref="MethodDeclaration"/>.</summary>
        /// <param name="method">The <see cref="MethodDeclaration"/> to visit.</param>
        protected internal sealed override void VisitMethod(MethodDeclaration method)
        {
            _WriteHtmlDocumentBeginning(method);

            WriteMethodDeclaration(method);
            HasDefaultSection = true;

            _WriteHtmlDocumentEnding(method);
        }

        /// <summary>Writes the HTML documentation section for the provided <paramref name="method"/>.</summary>
        /// <param name="method">The <see cref="MethodDeclaration"/> for which to write the documentation.</param>
        protected virtual void WriteMethodDeclaration(MethodDeclaration method)
        {
            WriteDeclarationSectionBeginning(method);

            WriteNavigation(method);
            WritePageHeading($"{method.GetSimpleNameReference()} Method", method.AccessModifier);

            WriteSummary(method.Summary);

            TextWriter.Write("<p>");
            WriteSafeHtml("This method is ");
            WriteAccessModifier(method.AccessModifier);
            WriteSafeHtml(".");
            TextWriter.Write("</p>");
            if (method.IsAbstract)
            {
                TextWriter.Write("<p>");
                WriteSafeHtml("This method is abstract.");
                TextWriter.Write("</p>");
            }
            if (method.IsVirtual)
            {
                TextWriter.Write("<p>");
                WriteSafeHtml("This method is virtual.");
                TextWriter.Write("</p>");
            }
            if (method.IsOverride)
            {
                TextWriter.Write("<p>");
                WriteSafeHtml("This method is an override.");
                TextWriter.Write("</p>");
            }
            if (method.IsSealed)
            {
                TextWriter.Write("<p>");
                WriteSafeHtml("This method is sealed.");
                TextWriter.Write("</p>");
            }
            if (method.IsShadowing)
            {
                TextWriter.Write("<p>");
                WriteSafeHtml("This method is hiding an inherited member with the same name.");
                TextWriter.Write("</p>");
            }
            if (method.IsStatic)
            {
                TextWriter.Write("<p>");
                WriteSafeHtml("This method is static.");
                TextWriter.Write("</p>");
            }

            WriteGenericParameters(method.GenericParameters);
            WriteParameters(method.Parameters);

            WriteReturn(method.Return);
            WriteExceptions(method.Exceptions);
            WriteExamples(method.Examples);
            WriteRemarks(method.Remarks);
            WriteRelatedMembers(method.RelatedMembers);

            WriteDeclarationSectionEnding(method);
        }

        /// <summary>Writes a user-friendly name for the provided <paramref name="accessModifier"/>.</summary>
        /// <param name="accessModifier">The <see cref="AccessModifier"/> for which to write the user-friendly name.</param>
        protected virtual void WriteAccessModifier(AccessModifier accessModifier)
            => WriteSafeHtml(accessModifier switch
            {
                AccessModifier.Public => "public",
                AccessModifier.Assembly => "internal",
                AccessModifier.Family => "protected",
                AccessModifier.FamilyOrAssembly => "protected internal",
                AccessModifier.FamilyAndAssembly => "private protected",
                AccessModifier.Private => "private",
                _ => throw new NotImplementedException()
            });

        /// <summary>Writes an HTML table for the provided <paramref name="types"/>.</summary>
        /// <param name="types">The <see cref="TypeDeclaration"/>s to include in the table.</param>
        /// <param name="title">The caption of the table.</param>
        /// <param name="sectionId">The section id of the table.</param>
        /// <remarks>
        /// The table contains two columns, the first one is a hyperlink with the name of the <see cref="TypeDeclaration"/>,
        /// and the secodn column contains the first paragraph of the summary of the related <see cref="TypeDeclaration"/>.
        /// </remarks>
        /// <seealso cref="WriteFirstSummaryParagraph(SummaryDocumentationElement)"/>
        protected virtual void WriteTypesList(IEnumerable<TypeDeclaration> types, string title, string sectionId)
        {
            if (types.Any())
            {
                TextWriter.Write("<section data-sectionId=\"");
                WriteSafeHtml(sectionId);
                TextWriter.Write("\">");

                TextWriter.Write("<table>");

                TextWriter.Write("<caption>");
                WriteSafeHtml(title);
                TextWriter.Write("</caption>");

                TextWriter.Write("<thead>");
                TextWriter.Write("<tr>");
                TextWriter.Write("<th>");
                WriteSafeHtml("Name");
                TextWriter.Write("</th>");
                TextWriter.Write("<th>");
                WriteSafeHtml("Description");
                TextWriter.Write("</th>");
                TextWriter.Write("</tr>");
                TextWriter.Write("</thead>");

                TextWriter.Write("<tbody>");
                foreach (var type in types)
                {
                    TextWriter.Write("<tr>");
                    TextWriter.Write("<td>");
                    TextWriter.Write("<a href=\"#");
                    TextWriter.Write(Uri.EscapeDataString(type.GetFullNameReference()));
                    TextWriter.Write("\">");
                    WriteSafeHtml(type.GetSimpleNameReference());
                    TextWriter.Write("</a>");
                    TextWriter.Write("</td>");
                    TextWriter.Write("<td>");
                    WriteFirstSummaryParagraph(type.Summary);
                    TextWriter.Write("</td>");
                    TextWriter.Write("</tr>");
                }
                TextWriter.Write("</tbody>");

                TextWriter.Write("</table>");

                TextWriter.Write("</section>");
            }
        }

        /// <summary>Writes an HTML table for the provided <paramref name="members"/>.</summary>
        /// <param name="members">The <see cref="MemberDeclaration"/>s to include in the table.</param>
        /// <param name="title">The caption of the table.</param>
        /// <param name="sectionId">The section id of the table.</param>
        /// <remarks>
        /// The table contains three columns, the first one is a hyperlink with the name of the <see cref="MemberDeclaration"/>,
        /// the second one contains the access modifier user-friendly name,
        /// and the secodn column contains the first paragraph of the summary of the related <see cref="MemberDeclaration"/>.
        /// </remarks>
        /// <seealso cref="WriteFirstSummaryParagraph(SummaryDocumentationElement)"/>
        protected void WriteMembersList(IEnumerable<MemberDeclaration> members, string title, string sectionId)
            => WriteMembersList(members, title, sectionId, false);

        /// <summary>Writes an HTML table for the provided <paramref name="members"/>.</summary>
        /// <param name="members">The <see cref="MemberDeclaration"/>s to include in the table.</param>
        /// <param name="title">The caption of the table.</param>
        /// <param name="sectionId">The section id of the table.</param>
        /// <param name="hideAccessModifier">A flag indicating whether to include the access modifer column.</param>
        /// <remarks>
        /// <para>
        /// The table contains three columns, the first one is a hyperlink with the name of the <see cref="MemberDeclaration"/>,
        /// the second one contains the access modifier user-friendly name,
        /// and the secodn column contains the first paragraph of the summary of the related <see cref="MemberDeclaration"/>.
        /// </para>
        /// <para>
        /// If <paramref name="hideAccessModifier"/> is set to <c>true</c> then only the first and last columns are included.
        /// </para>
        /// </remarks>
        /// <seealso cref="WriteFirstSummaryParagraph(SummaryDocumentationElement)"/>
        protected virtual void WriteMembersList(IEnumerable<MemberDeclaration> members, string title, string sectionId, bool hideAccessModifier)
        {
            if (members.Any())
            {
                TextWriter.Write("<section data-sectionId=\"");
                WriteSafeHtml(sectionId);
                TextWriter.Write("\">");

                TextWriter.Write("<table>");

                TextWriter.Write("<caption>");
                WriteSafeHtml(title);
                TextWriter.Write("</caption>");

                TextWriter.Write("<thead>");
                TextWriter.Write("<tr>");
                TextWriter.Write("<th>");
                WriteSafeHtml("Name");
                TextWriter.Write("</th>");
                if (!hideAccessModifier)
                {
                    TextWriter.Write("<th>");
                    WriteSafeHtml("Access");
                    TextWriter.Write("</th>");
                }
                TextWriter.Write("<th>");
                WriteSafeHtml("Description");
                TextWriter.Write("</th>");
                TextWriter.Write("</tr>");
                TextWriter.Write("</thead>");

                TextWriter.Write("<tbody>");
                foreach (var member in members)
                {
                    TextWriter.Write("<tr>");
                    TextWriter.Write("<td>");
                    TextWriter.Write("<a href=\"#");
                    TextWriter.Write(Uri.EscapeDataString(member.GetFullNameReference()));
                    TextWriter.Write("\">");
                    WriteSafeHtml(member.GetSimpleNameReference());
                    TextWriter.Write("</a>");
                    TextWriter.Write("</td>");
                    if (!hideAccessModifier)
                    {
                        TextWriter.Write("<td><code>");
                        WriteAccessModifier(member.AccessModifier);
                        TextWriter.Write("</code></td>");
                    }
                    TextWriter.Write("<td>");
                    WriteFirstSummaryParagraph(member.Summary);
                    TextWriter.Write("</td>");
                    TextWriter.Write("</tr>");
                }
                TextWriter.Write("</tbody>");

                TextWriter.Write("</table>");

                TextWriter.Write("</section>");
            }
        }

        /// <summary>Writes an HTML list describing the provided <paramref name="genericParameters"/>.</summary>
        /// <param name="genericParameters">The <see cref="GenericMethodParameterData"/> to include in the list.</param>
        protected virtual void WriteGenericParameters(IEnumerable<GenericParameterData> genericParameters)
        {
            if (genericParameters.Any())
            {
                var htmlWriterDocumentationVisitor = CreateDocumentationVisitor();

                TextWriter.Write("<section data-sectionId=\"generic-parameters\">");
                TextWriter.Write("<h2>");
                WriteSafeHtml("Generic Parameters");
                TextWriter.Write("</h2>");

                TextWriter.Write("<ul>");
                foreach (var genericParameter in genericParameters)
                {
                    TextWriter.Write("<li>");
                    TextWriter.Write("<strong>");
                    WriteSafeHtml(genericParameter.Name);
                    TextWriter.Write("</strong>");
                    genericParameter.Description.Accept(htmlWriterDocumentationVisitor);

                    if (genericParameter.HasDefaultConstructorConstraint)
                    {
                        TextWriter.Write("<p>");
                        WriteSafeHtml("Type arguments are required to have a public parameterless constructor.");
                        TextWriter.Write("</p>");
                    }
                    if (genericParameter.HasReferenceTypeConstraint)
                    {
                        TextWriter.Write("<p>");
                        WriteSafeHtml("Type arguments are required to be reference types.");
                        TextWriter.Write("</p>");
                    }
                    if (genericParameter.HasNonNullableValueTypeConstraint)
                    {
                        TextWriter.Write("<p>");
                        WriteSafeHtml("Type arguments are required to be value types.");
                        TextWriter.Write("</p>");
                    }
                    if (genericParameter.HasUnmanagedTypeConstraint)
                    {
                        TextWriter.Write("<p>");
                        WriteSafeHtml("Type arguments are required to be unmanaged types.");
                        TextWriter.Write("</p>");
                    }
                    if (genericParameter.IsCovariant)
                    {
                        TextWriter.Write("<p>");
                        WriteSafeHtml("This parameter is covariant.");
                        TextWriter.Write("</p>");
                    }
                    if (genericParameter.IsContravariant)
                    {
                        TextWriter.Write("<p>");
                        WriteSafeHtml("This parameter is contravariant.");
                        TextWriter.Write("</p>");
                    }
                    if (genericParameter.TypeConstraints.Any())
                    {
                        TextWriter.Write("<p>");
                        WriteSafeHtml("Type arguments are required to be extend the following: ");
                        var isFirst = true;
                        foreach (var typeConstraint in genericParameter.TypeConstraints)
                        {
                            if (isFirst)
                                isFirst = false;
                            else
                                WriteSafeHtml(", ");
                            if (typeConstraint is GenericParameterReference genericParameterReference)
                            {
                                TextWriter.Write("<code>");
                                WriteSafeHtml(genericParameterReference.Name);
                                TextWriter.Write("</code>");
                            }
                            else
                            {
                                TextWriter.Write("<a href=#");
                                TextWriter.Write(MemberReferenceResolver.GetUrl(typeConstraint));
                                TextWriter.Write("\">");
                                WriteSafeHtml(typeConstraint.GetSimpleNameReference());
                                TextWriter.Write("</a>");
                            }
                        }
                        WriteSafeHtml(".");
                        TextWriter.Write("</p>");
                    }

                    TextWriter.Write("</li>");

                }
                TextWriter.Write("</ul>");
                TextWriter.Write("</section>");
            }
        }

        /// <summary>Writes an HTML navigation (similar to breadcrums) for the provided <paramref name="declarationNode"/>.</summary>
        /// <param name="declarationNode">The <see cref="DeclarationNode"/> for which to write the navigation.</param>
        /// <remarks>The navigation is generated from the assembly level towards the nested member.</remarks>
        /// <seealso cref="WriteNavigationItems(DeclarationNode)"/>
        /// <seealso cref="WriteNavigationItem(DeclarationNode)"/>
        /// <seealso cref="WriteNavigationActiveItem(DeclarationNode)"/>
        protected virtual void WriteNavigation(DeclarationNode declarationNode)
        {
            TextWriter.Write("<nav>");
            WriteNavigationItems(declarationNode);
            TextWriter.Write("</nav>");
        }

        /// <summary>Writes an HTML navigation items (similar to breadcrums) for the provided <paramref name="declarationNode"/>.</summary>
        /// <param name="declarationNode">The <see cref="DeclarationNode"/> for which to write the navigation.</param>
        /// <remarks>
        /// <para>The navigation is generated from the assembly level towards the nested member.</para>
        /// <para>This method is used by <see cref="WriteNavigation(DeclarationNode)"/> which wraps the navigation elements in a root element.</para>
        /// </remarks>
        /// <seealso cref="WriteNavigation(DeclarationNode)"/>
        /// <seealso cref="WriteNavigationItem(DeclarationNode)"/>
        /// <seealso cref="WriteNavigationActiveItem(DeclarationNode)"/>
        protected virtual void WriteNavigationItems(DeclarationNode declarationNode)
            => _WriteDeclarationItems(declarationNode, isLeaf: true);

        void _WriteDeclarationItems(DeclarationNode declarationNode, bool isLeaf = false)
        {
            switch (declarationNode)
            {
                case AssemblyDeclaration assembly:
                    if (isLeaf)
                        WriteNavigationActiveItem(assembly);
                    else
                        WriteNavigationItem(assembly);
                    break;

                case NamespaceDeclaration @namespace:
                    _WriteDeclarationItems(@namespace.Assembly);

                    if (isLeaf)
                        WriteNavigationActiveItem(@namespace);
                    else
                        WriteNavigationItem(@namespace);
                    break;

                case TypeDeclaration type:
                    _WriteDeclarationItems(type.Namespace);

                    if (type.DeclaringType != (TypeDeclaration)null)
                        _WriteDeclarationItems(type.DeclaringType);

                    if (isLeaf)
                        WriteNavigationActiveItem(type);
                    else
                        WriteNavigationItem(type);
                    break;

                case MemberDeclaration member:
                    _WriteDeclarationItems(member.DeclaringType);

                    if (isLeaf)
                        WriteNavigationActiveItem(member);
                    else
                        WriteNavigationItem(member);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Writes the navigation item of the provided <paramref name="declarationNode"/>. Typically this
        /// is a hyperlink towards the <see cref="DeclarationNode"/>.
        /// </summary>
        /// <param name="declarationNode">The <see cref="DeclarationNode"/> for which to generate the navigation item.</param>
        /// <seealso cref="WriteNavigation(DeclarationNode)"/>
        /// <seealso cref="WriteNavigationItems(DeclarationNode)"/>
        /// <seealso cref="WriteNavigationActiveItem(DeclarationNode)"/>
        protected virtual void WriteNavigationItem(DeclarationNode declarationNode)
        {
            TextWriter.Write("<a href=\"#");
            if (!(declarationNode is AssemblyDeclaration))
                TextWriter.Write(Uri.EscapeDataString(declarationNode.GetFullNameReference()));
            TextWriter.Write("\">");
            WriteSafeHtml(declarationNode.GetSimpleNameReference());
            TextWriter.Write("</a>");
        }

        /// <summary>
        /// Writes the active navigation item of the provided <paramref name="declarationNode"/>. Typically this
        /// is just the simple name of the <see cref="DeclarationNode"/>. The active item is the current item.
        /// </summary>
        /// <param name="declarationNode">The <see cref="DeclarationNode"/> for which to generate the active navigation item.</param>
        /// <seealso cref="WriteNavigation(DeclarationNode)"/>
        /// <seealso cref="WriteNavigationItems(DeclarationNode)"/>
        /// <seealso cref="WriteNavigationItem(DeclarationNode)"/>
        protected virtual void WriteNavigationActiveItem(DeclarationNode declarationNode)
            => WriteSafeHtml(declarationNode.GetSimpleNameReference());

        /// <summary>Writes the heading of a documentation section (page).</summary>
        /// <param name="title">The title of the page.</param>
        protected virtual void WritePageHeading(string title)
            => WritePageHeading(title, null);

        /// <summary>Writes the heading of a documentation section (page).</summary>
        /// <param name="title">The title of the page.</param>
        /// <param name="accessModifier">When provided, writes the user-friend name of the <see cref="AccessModifier"/> after the provided <paramref name="title"/>.</param>
        protected virtual void WritePageHeading(string title, AccessModifier? accessModifier)
        {
            TextWriter.Write("<h1>");
            WriteSafeHtml(title);
            if (accessModifier.HasValue && accessModifier.Value != AccessModifier.Public)
            {
                TextWriter.Write(" (<code>");
                WriteAccessModifier(accessModifier.Value);
                TextWriter.Write("</code>)");
            }
            TextWriter.Write("</h1>");
        }

        /// <summary>Writes the summary documentation of a <see cref="DeclarationNode"/>.</summary>
        /// <param name="summary">The <see cref="SummaryDocumentationElement"/> for which to write the HTML.</param>
        /// <seealso cref="CreateDocumentationVisitor"/>
        protected virtual void WriteSummary(SummaryDocumentationElement summary)
            => summary.Accept(CreateDocumentationVisitor());

        /// <summary>Writes the contents of the first paragraph of the provided <see cref="SummaryDocumentationElement"/>.</summary>
        /// <param name="summary">The <see cref="SummaryDocumentationElement"/> for which to write the contents of the first <see cref="ParagraphDocumentationElement"/>.</param>
        /// <seealso cref="CreateDocumentationVisitor"/>
        /// <seealso cref="WriteEnumMembersList(IEnumerable{ConstantDeclaration})"/>
        /// <seealso cref="WriteNamespacesList(IEnumerable{NamespaceDeclaration})"/>
        /// <seealso cref="WriteTypesList(IEnumerable{TypeDeclaration}, string, string)"/>
        /// <seealso cref="WriteMembersList(IEnumerable{MemberDeclaration}, string, string)"/>
        /// <seealso cref="WriteMembersList(IEnumerable{MemberDeclaration}, string, string, bool)"/>
        protected virtual void WriteFirstSummaryParagraph(SummaryDocumentationElement summary)
        {
            var documentationVisitor = CreateDocumentationVisitor();
            foreach (var element in summary.Content.OfType<ParagraphDocumentationElement>().Take(1).SelectMany(paragraph => paragraph.Content))
                element.Accept(documentationVisitor);
        }

        /// <summary>Writes the value documentation of a <see cref="DeclarationNode"/>.</summary>
        /// <param name="value">The <see cref="ValueDocumentationElement"/> for which to write the HTML.</param>
        /// <seealso cref="CreateDocumentationVisitor"/>
        protected virtual void WriteValue(ValueDocumentationElement value)
        {
            if (value.Content.Any())
            {
                var htmlWriterDocumentationVisitor = CreateDocumentationVisitor();
                TextWriter.Write("<section data-sectionId=\"value\">");

                TextWriter.Write("<h2>");
                WriteSafeHtml("Value");
                TextWriter.Write("</h2>");

                value.Accept(htmlWriterDocumentationVisitor);
                TextWriter.Write("</section>");
            }
        }

        /// <summary>Writes an HTML list describing the provided <paramref name="parameters"/>.</summary>
        /// <param name="parameters">The <see cref="ParameterData"/> to include in the list.</param>
        protected virtual void WriteParameters(IEnumerable<ParameterData> parameters)
        {
            if (parameters.Any())
            {
                var htmlWriterDocumentationVisitor = CreateDocumentationVisitor();

                TextWriter.Write("<section data-sectionId=\"parameters\">");
                TextWriter.Write("<h2>");
                WriteSafeHtml("Parameters");
                TextWriter.Write("</h2>");

                TextWriter.Write("<ul>");
                foreach (var parameter in parameters)
                {
                    TextWriter.Write("<li>");
                    TextWriter.Write("<strong>");
                    WriteSafeHtml(parameter.Name);
                    TextWriter.Write("</strong>");
                    WriteSafeHtml(": ");
                    WriteConstructedTypeReference(parameter.Type);

                    parameter.Description.Accept(htmlWriterDocumentationVisitor);

                    if (parameter.HasDefaultValue)
                    {
                        TextWriter.Write("<p>");
                        WriteSafeHtml("This parameter has ");
                        TextWriter.Write("<code>");
                        if (parameter.DefaultValue is null)
                            WriteSafeHtml("null");
                        else if (parameter.DefaultValue is string stringDefaultValue)
                        {
                            WriteSafeHtml("\"");
                            WriteSafeHtml(stringDefaultValue);
                            WriteSafeHtml("\"");
                        }
                        else
                            WriteSafeHtml(Convert.ToString(parameter.DefaultValue, CultureInfo.InvariantCulture));
                        TextWriter.Write("</code>");
                        WriteSafeHtml("as default value.");
                        TextWriter.Write("</p>");
                    }
                    if (parameter.IsInputOutputByReference)
                    {
                        TextWriter.Write("<p>");
                        WriteSafeHtml("This parameter is an input and output parameter passed by reference (");
                        TextWriter.Write("<code>");
                        WriteSafeHtml("ref");
                        TextWriter.Write("</code>");
                        WriteSafeHtml(").");
                        TextWriter.Write("</p>");
                    }
                    if (parameter.IsOutputByReference)
                    {
                        TextWriter.Write("<p>");
                        WriteSafeHtml("This parameter is an output parameter passed by reference (");
                        TextWriter.Write("<code>");
                        WriteSafeHtml("out");
                        TextWriter.Write("</code>");
                        WriteSafeHtml(").");
                        TextWriter.Write("</p>");
                    }

                    TextWriter.Write("</li>");
                }
                TextWriter.Write("</ul>");
                TextWriter.Write("</section>");
            }
        }

        /// <summary>Writes the return documentation of a <see cref="DeclarationNode"/>.</summary>
        /// <param name="returnData">The <see cref="MethodReturnData"/> for which to write the HTML.</param>
        /// <seealso cref="CreateDocumentationVisitor"/>
        protected virtual void WriteReturn(MethodReturnData returnData)
        {
            if (returnData.Type != typeof(void) || returnData.Description.Any())
            {
                var htmlWriterDocumentationVisitor = CreateDocumentationVisitor();
                TextWriter.Write("<section data-sectionId=\"return\">");

                TextWriter.Write("<h2>");
                WriteSafeHtml("Return: ");
                WriteConstructedTypeReference(returnData.Type);
                TextWriter.Write("</h2>");

                returnData.Description.Accept(htmlWriterDocumentationVisitor);
                TextWriter.Write("</section>");
            }
        }

        /// <summary>Writes the exceptions documentation of a <see cref="DeclarationNode"/>.</summary>
        /// <param name="exceptions">The <see cref="ExceptionDocumentationElement"/>s for which to write the HTML.</param>
        protected virtual void WriteExceptions(IEnumerable<ExceptionDocumentationElement> exceptions)
        {
            if (exceptions.Any())
            {
                var htmlWriterDocumentationVisitor = CreateDocumentationVisitor();
                TextWriter.Write("<section data-sectionId=\"exceptions\">");
                foreach (var exception in exceptions)
                    exception.Accept(htmlWriterDocumentationVisitor);
                TextWriter.Write("</section>");
            }
        }

        /// <summary>Writes the examples documentation of a <see cref="DeclarationNode"/>.</summary>
        /// <param name="examples">The <see cref="ExampleDocumentationElement"/>s for which to write the HTML.</param>
        protected virtual void WriteExamples(IEnumerable<ExampleDocumentationElement> examples)
        {
            if (examples.Any())
            {
                var htmlWriterDocumentationVisitor = CreateDocumentationVisitor();
                TextWriter.Write("<section data-sectionId=\"examples\">");
                foreach (var example in examples)
                    example.Accept(htmlWriterDocumentationVisitor);
                TextWriter.Write("</section>");
            }
        }

        /// <summary>Writes the remarks documentation of a <see cref="DeclarationNode"/>.</summary>
        /// <param name="remarks">The <see cref="RemarksDocumentationElement"/> for which to write the HTML.</param>
        /// <seealso cref="CreateDocumentationVisitor"/>
        protected virtual void WriteRemarks(RemarksDocumentationElement remarks)
            => remarks.Accept(CreateDocumentationVisitor());

        /// <summary>Writes an HTML list for the provided <paramref name="relatedMembers"/>.</summary>
        /// <param name="relatedMembers">The <see cref="MemberReferenceDocumentationElement"/>s for which to write the HTML list.</param>
        /// <seealso cref="CreateDocumentationVisitor"/>
        protected virtual void WriteRelatedMembers(IEnumerable<MemberReferenceDocumentationElement> relatedMembers)
        {
            if (relatedMembers.Any())
            {
                var htmlWriterDocumentationVisitor = CreateDocumentationVisitor();
                TextWriter.Write("<section data-sectionId=\"references\">");
                TextWriter.Write("<ul>");
                foreach (var relatedMember in relatedMembers)
                {
                    TextWriter.Write("<li>");
                    relatedMember.Accept(htmlWriterDocumentationVisitor);
                    TextWriter.Write("</li>");
                }
                TextWriter.Write("</ul>");
                TextWriter.Write("</section>");
            }
        }

        /// <summary/>
        protected virtual void WriteAttributes(IEnumerable<AttributeData> attributes, string sectionId, string title)
        {
            if (attributes.Any())
            {
                TextWriter.Write("<section data-sectionId=\"");
                WriteSafeHtml(sectionId);
                TextWriter.Write("\">");

                TextWriter.Write("<h2>");
                WriteSafeHtml(title);
                TextWriter.Write("</h2>");

                TextWriter.Write("<ul>");
                foreach (var attribute in attributes)
                {
                    TextWriter.Write("<li>");
                    WriteConstructedTypeReference(attribute.Type);
                    if (attribute.PositionalParameters.Any())
                    {
                        TextWriter.Write("<br>");
                        WriteSafeHtml("Positional Parameters");
                        TextWriter.Write("<ol>");
                        foreach (var positionalParameter in attribute.PositionalParameters)
                        {
                            TextWriter.Write("<li>");
                            TextWriter.Write("<strong>");
                            WriteSafeHtml(positionalParameter.Name);
                            TextWriter.Write("</strong>");
                            WriteSafeHtml(": ");
                            TextWriter.Write("<code>");
                            WriteValue(positionalParameter.Value);
                            TextWriter.Write("</code>");
                            TextWriter.Write("</li>");
                        }
                        TextWriter.Write("</ol>");
                    }
                    if (attribute.NamedParameters.Any())
                    {
                        if (!attribute.PositionalParameters.Any())
                            TextWriter.Write("<br>");

                        WriteSafeHtml("Named Parameters");
                        TextWriter.Write("<ul>");
                        foreach (var namedParameters in attribute.NamedParameters)
                        {
                            TextWriter.Write("<li>");
                            TextWriter.Write("<strong>");
                            WriteSafeHtml(namedParameters.Name);
                            TextWriter.Write("</strong>");
                            WriteSafeHtml(": ");
                            TextWriter.Write("<code>");
                            WriteValue(namedParameters.Value);
                            TextWriter.Write("</code>");
                            TextWriter.Write("</li>");
                        }
                        TextWriter.Write("</ul>");
                    }
                    TextWriter.Write("</li>");
                }
                TextWriter.Write("</ul>");

                TextWriter.Write("</section>");
            }
        }

        /// <summary>Writes a constructed type reference containing one or multiple hyperlinks.</summary>
        /// <param name="type">The <see cref="BaseTypeReference"/> for which to write the reference.</param>
        /// <remarks>
        /// Constructed type references can be quite complex. For instance, a constructed generic type
        /// will generate a set of hyperlinks, one being towards the generic type definition and then
        /// one hyperlink for each generic argument.
        /// </remarks>
        protected virtual void WriteConstructedTypeReference(BaseTypeReference type)
        {
            var memberReferenceVisitor = new HyperlinkWriterMemberReferenceVisitor(TextWriter, MemberReferenceResolver);
            type.Accept(memberReferenceVisitor);
        }

        /// <summary>Writes the provided <paramref name="value"/>.</summary>
        /// <param name="value">The value to write.</param>
        /// <remarks>
        /// This method is intended to write constant values declared through <c>enums</c>,
        /// constant fields or attribute parameters.
        /// </remarks>
        /// <seealso cref="WriteEnumMembersList(IEnumerable{ConstantDeclaration})"/>
        /// <seealso cref="WriteConstantDeclaration(ConstantDeclaration)"/>
        /// <seealso cref="WriteAttributes(IEnumerable{AttributeData}, string, string)"/>
        protected virtual void WriteValue(object value)
        {
            if (value is null)
                WriteSafeHtml("null");
            else if (value is string)
                WriteSafeHtml($"\"{value}\"");
            else if (value is char)
                WriteSafeHtml($"'{value}'");
            else if (value.GetType().IsEnum)
                WriteSafeHtml(string.Format(CultureInfo.InvariantCulture, "{0:D}", value));
            else
                WriteSafeHtml(Convert.ToString(value, CultureInfo.InvariantCulture));
        }

        /// <summary>Writes the provided <paramref name="value"/> as a safe HTML string.</summary>
        /// <param name="value">The text to write.</param>
        /// <remarks>
        /// If the provied <paramref name="value"/> contains HTML reserved characters, they
        /// are escaped.
        /// </remarks>
        protected virtual void WriteSafeHtml(string value)
        {
            var htmlSafeValue = value;
            if (value.Any(@char => @char == '<' || @char == '>' || @char == '&' || @char == '\'' || @char == '"' || (char.IsControl(@char) && !char.IsWhiteSpace(@char))))
                htmlSafeValue = value
                    .Aggregate(
                        new StringBuilder(),
                        (stringBuilder, @char) =>
                        {
                            switch (@char)
                            {
                                case '<':
                                    return stringBuilder.Append("&lt;");

                                case '>':
                                    return stringBuilder.Append("&gt;");

                                case '&':
                                    return stringBuilder.Append("&amp;");

                                case '"':
                                    return stringBuilder.Append("&quot;");

                                default:
                                    if (@char == '\'' || (char.IsControl(@char) && !char.IsWhiteSpace(@char)))
                                        return stringBuilder.Append("&#x").Append(((short)@char).ToString("x2")).Append(';');
                                    else
                                        return stringBuilder.Append(@char);
                            }
                        }
                    )
                    .ToString();

            TextWriter.Write(htmlSafeValue);
        }

        /// <summary>Writes the HTML section begining of a <see cref="DeclarationNode"/>.</summary>
        /// <param name="declarationNode">The <see cref="DeclarationNode"/> for which to write the HTML section beginning.</param>
        /// <seealso cref="WriteDeclarationSectionEnding(DeclarationNode)"/>
        protected virtual void WriteDeclarationSectionBeginning(DeclarationNode declarationNode)
        {
            TextWriter.Write("<section id=\"");
            TextWriter.Write(declarationNode is AssemblyDeclaration ? "$default" : Uri.EscapeDataString(declarationNode.GetFullNameReference()));
            TextWriter.Write("\" data-title=\"");
            WriteSafeHtml(GetPageTitle(declarationNode));
            TextWriter.Write("\"");

            if (!HasDefaultSection)
                TextWriter.Write(" data-default=\"true\" style=\"display: initial\"");
            else
                TextWriter.Write(" data-default=\"false\" style=\"display: none\"");

            WriteOtherSectionAttributes(declarationNode);

            TextWriter.Write(">");
        }

        /// <summary>Writes other attributes for a declaration section.</summary>
        /// <param name="declarationNode">The <see cref="DeclarationNode"/> for which to write the HTML section beginning.</param>
        protected virtual void WriteOtherSectionAttributes(DeclarationNode declarationNode)
        {
        }

        /// <summary>Writes the HTML section ending of a <see cref="DeclarationNode"/>.</summary>
        /// <param name="declarationNode">The <see cref="DeclarationNode"/> for which to write the HTML section ending.</param>
        /// <seealso cref="WriteDeclarationSectionBeginning(DeclarationNode)"/>
        protected virtual void WriteDeclarationSectionEnding(DeclarationNode declarationNode)
            => TextWriter.Write("</section>");

        /// <summary>Gets the page/section title for the provided <paramref name="declarationNode"/>.</summary>
        /// <param name="declarationNode">The <see cref="DeclarationNode"/> for which to get the title.</param>
        /// <returns>Returns the page/section title for the provided <paramref name="declarationNode"/>.</returns>
        protected virtual string GetPageTitle(DeclarationNode declarationNode)
        {
            var visitor = new HtmlPageTitleDeclarationNodeVisitor();
            declarationNode.Accept(visitor);
            return visitor.TitleStringBuilder.ToString();
        }

        private void _WriteHtmlDocumentBeginning(DeclarationNode declarationNode)
        {
            if (_declarationDepth == 0)
            {
                if (_isHtmlInitialized)
                    throw new InvalidOperationException("The HTML document has already been started, please use a different instance.");

                WriteHtmlBeginning(declarationNode);
                _isHtmlInitialized = true;
            }
        }

        /// <summary>Writes the HTML document begining of a <see cref="DeclarationNode"/>.</summary>
        /// <param name="declarationNode">The first <see cref="DeclarationNode"/> that is visited for which the HTML document is generated.</param>
        /// <seealso cref="WriteHtmlEnding(DeclarationNode)"/>
        protected virtual void WriteHtmlBeginning(DeclarationNode declarationNode)
        {
            TextWriter.WriteLine("<!DOCTYPE html>");
            TextWriter.Write("<html lang=\"en-US\"");
            WriteOtherHtmlAttributes(declarationNode);
            TextWriter.Write(">");
            TextWriter.Write("<head>");
            TextWriter.Write("<meta charset=\"UTF-8\">");
            TextWriter.Write("<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">");
            TextWriter.Write("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            TextWriter.Write("<title>");
            WriteSafeHtml(GetPageTitle(declarationNode));
            TextWriter.Write("</title>");
            WriteOtherHtmlHeadTags(declarationNode);
            TextWriter.Write("</head>");
            TextWriter.Write("<body");
            WriteOtherBodyAttributes(declarationNode);
            TextWriter.Write(">");
        }

        /// <summary>Writes other attributes for the <c>html</c> element.</summary>
        /// <param name="declarationNode">The first <see cref="DeclarationNode"/> that is visited for which the HTML document is generated.</param>
        protected virtual void WriteOtherHtmlAttributes(DeclarationNode declarationNode)
        {
        }

        /// <summary>Writes other HTML head elements.</summary>
        /// <param name="declarationNode">The first <see cref="DeclarationNode"/> that is visited for which the HTML document is generated.</param>
        protected virtual void WriteOtherHtmlHeadTags(DeclarationNode declarationNode)
        {
        }

        /// <summary>Writes other attributes for the <c>body</c> element.</summary>
        /// <param name="declarationNode">The first <see cref="DeclarationNode"/> that is visited for which the HTML document is generated.</param>
        protected virtual void WriteOtherBodyAttributes(DeclarationNode declarationNode)
        {
        }

        private void _WriteHtmlDocumentEnding(DeclarationNode declarationNode)
        {
            if (_declarationDepth == 0)
            {
                if (!_isHtmlInitialized)
                    throw new InvalidOperationException("The HTML document has not been started.");

                WriteHtmlEnding(declarationNode);
            }
        }

        /// <summary>Writes the HTML document ending of a <see cref="DeclarationNode"/>.</summary>
        /// <param name="declarationNode">The first <see cref="DeclarationNode"/> that is visited for which the HTML document is generated.</param>
        /// <seealso cref="WriteHtmlBeginning(DeclarationNode)"/>
        protected virtual void WriteHtmlEnding(DeclarationNode declarationNode)
        {
            WriteNavigationJavaScript(declarationNode);
            WriteOtherHtmlBodyTags(declarationNode);

            TextWriter.Write("</body>");
            TextWriter.Write("</html>");
        }

        /// <summary>Writes the basic navigation JavaScript.</summary>
        /// <param name="declarationNode">The first <see cref="DeclarationNode"/> that is visited for which the HTML document is generated.</param>
        protected virtual void WriteNavigationJavaScript(DeclarationNode declarationNode)
        {
            TextWriter.Write("<script>");
            TextWriter.Write("window.addEventListener(\"hashchange\", function (hashChangeEvent) { ");
            TextWriter.Write("switchView(hashChangeEvent.oldURL.split(\"#\", 2)[1], hashChangeEvent.newURL.split(\"#\", 2)[1]);");
            TextWriter.Write("});");

            TextWriter.Write("function switchView(from, to) {");
            TextWriter.Write("hide(from || \"\");");
            TextWriter.Write("show(to || \"\");");

            TextWriter.Write("function show(elementId) {");
            TextWriter.Write("const element = document.getElementById(elementId) || document.getElementById(\"$default\") || document.querySelector(\"section[data-default='true']\");");
            TextWriter.Write("element.style.display = \"initial\";");
            TextWriter.Write("document.title = element.dataset.title");
            TextWriter.Write("}");

            TextWriter.Write("function hide(elementId) {");
            TextWriter.Write("const element = document.getElementById(elementId) || document.getElementById(\"$default\") || document.querySelector(\"section[data-default='true']\");");
            TextWriter.Write("element.style.display = \"none\";");
            TextWriter.Write("}");

            TextWriter.Write("}");

            TextWriter.Write("switchView((document.getElementById(\"$default\") || document.querySelector(\"section[data-default='true']\")).id, window.location.hash.split(\"#\")[1]);");
            TextWriter.Write("</script>");
        }

        /// <summary>Writes other HTML body elements.</summary>
        /// <param name="declarationNode">The first <see cref="DeclarationNode"/> that is visited for which the HTML document is generated.</param>
        protected virtual void WriteOtherHtmlBodyTags(DeclarationNode declarationNode)
        {
        }
    }
}