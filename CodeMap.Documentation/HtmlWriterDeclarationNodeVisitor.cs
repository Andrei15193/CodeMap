using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CodeMap.DeclarationNodes;
using CodeMap.DocumentationElements;
using CodeMap.Html;
using CodeMap.ReferenceData;

namespace CodeMap.Documentation
{
    public class HtmlWriterDeclarationNodeVisitor : DeclarationNodeVisitor
    {
        private bool _isHtmlInitialized = false;
        private int _declarationDepth = 0;

        public HtmlWriterDeclarationNodeVisitor(TextWriter textWriter, IMemberReferenceResolver memberReferenceResolver)
        {
            TextWriter = textWriter;
            MemberReferenceResolver = memberReferenceResolver ?? throw new ArgumentNullException(nameof(memberReferenceResolver));
            HasDefaultSection = false;
        }

        public TextWriter TextWriter { get; set; }

        public IMemberReferenceResolver MemberReferenceResolver { get; }

        protected bool HasDefaultSection { get; private set; }

        protected sealed override void VisitAssembly(AssemblyDeclaration assembly)
        {
            _WriteHtmlDocumentBeginning($"{assembly.Name}@{assembly.Version} - Home");

            WriteAssemblyDeclaration(assembly);

            _declarationDepth++;
            foreach (var @namespace in assembly.Namespaces)
                @namespace.Accept(this);
            _declarationDepth--;

            _WriteHtmlDocumentEnding();
        }

        protected virtual void WriteAssemblyDeclaration(AssemblyDeclaration assembly)
        {
            WriteDeclarationSectionBeginning(assembly, $"{assembly.Name}@{assembly.Version} - Home");
            HasDefaultSection = true;

            WriteNavigation(assembly);
            WritePageHeading($"{assembly.Name}@{assembly.Version}");

            WriteSummary(assembly.Summary);
            WriteNamespacesList(assembly.Namespaces);

            WriteExamples(assembly.Examples);
            WriteRemarks(assembly.Remarks);
            WriteRelatedMembers(assembly.RelatedMembers);

            WriteDeclarationSectionEnding();
        }

        protected virtual void WriteNamespacesList(IEnumerable<NamespaceDeclaration> namespaces)
        {
            if (namespaces.Any())
            {
                TextWriter.Write("<section id=\"namespaces\">");

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
                    WriteSafeHtml(@namespace.GetFullNameReference());
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

        protected sealed override void VisitNamespace(NamespaceDeclaration @namespace)
        {
            _WriteHtmlDocumentBeginning($"{@namespace.Assembly.Name}@{@namespace.Assembly.Version} - {@namespace.Name} Namespace");

            WriteNamespaceDeclaration(@namespace);

            _declarationDepth++;
            foreach (var type in @namespace.DeclaredTypes)
                type.Accept(this);
            _declarationDepth--;

            _WriteHtmlDocumentEnding();
        }

        protected virtual void WriteNamespaceDeclaration(NamespaceDeclaration @namespace)
        {
            WriteDeclarationSectionBeginning(@namespace, $"{@namespace.Assembly.Name}@{@namespace.Assembly.Version} - {@namespace.Name} Namespace");
            HasDefaultSection = true;

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

            WriteDeclarationSectionEnding();
        }

        protected sealed override void VisitEnum(EnumDeclaration @enum)
        {
            _WriteHtmlDocumentBeginning($"{@enum.Assembly.Name}@{@enum.Assembly.Version} - {@enum.GetSimpleNameReference()} Enum");

            WriteEnumDeclaration(@enum);

            _WriteHtmlDocumentEnding();
        }

        protected virtual void WriteEnumDeclaration(EnumDeclaration @enum)
        {
            WriteDeclarationSectionBeginning(@enum, $"{@enum.Assembly.Name}@{@enum.Assembly.Version} - {@enum.GetSimpleNameReference()} Enum");
            HasDefaultSection = true;

            WriteNavigation(@enum);
            WritePageHeading($"{@enum.GetSimpleNameReference()} Enum", @enum.AccessModifier);

            WriteSummary(@enum.Summary);
            if (@enum.Members.Any())
            {
                TextWriter.Write("<section id=\"members\">");

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
                foreach (var member in @enum.Members)
                {
                    TextWriter.Write("<tr>");
                    TextWriter.Write("<td><code>");
                    WriteSafeHtml(member.GetSimpleNameReference());
                    TextWriter.Write("</code></td>");
                    TextWriter.Write("<td><code>");
                    WriteConstantValue(member);
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

            WriteExamples(@enum.Examples);
            WriteRemarks(@enum.Remarks);
            WriteRelatedMembers(@enum.RelatedMembers);

            WriteDeclarationSectionEnding();
        }

        protected sealed override void VisitDelegate(DelegateDeclaration @delegate)
        {
            _WriteHtmlDocumentBeginning($"{@delegate.Assembly.Name}@{@delegate.Assembly.Version} - {@delegate.GetSimpleNameReference()} Delegate");

            WriteDelegateDeclaration(@delegate);

            _WriteHtmlDocumentEnding();
        }

        protected void WriteDelegateDeclaration(DelegateDeclaration @delegate)
        {
            WriteDeclarationSectionBeginning(@delegate, $"{@delegate.Assembly.Name}@{@delegate.Assembly.Version} - {@delegate.GetSimpleNameReference()} Delegate");
            HasDefaultSection = true;

            WriteNavigation(@delegate);
            WritePageHeading($"{@delegate.GetSimpleNameReference()} Delegate", @delegate.AccessModifier);

            WriteSummary(@delegate.Summary);

            WriteGenericParameters(@delegate.GenericParameters);
            WriteParameters(@delegate.Parameters);

            WriteReturn(@delegate.Return);
            WriteExceptions(@delegate.Exceptions);
            WriteExamples(@delegate.Examples);
            WriteRemarks(@delegate.Remarks);
            WriteRelatedMembers(@delegate.RelatedMembers);

            WriteDeclarationSectionEnding();
        }

        protected sealed override void VisitInterface(InterfaceDeclaration @interface)
        {
            _WriteHtmlDocumentBeginning($"{@interface.Assembly.Name}@{@interface.Assembly.Version} - {@interface.GetSimpleNameReference()} Interface");

            WriteInterfaceDeclaration(@interface);

            foreach (var member in @interface.Members)
                member.Accept(this);

            _WriteHtmlDocumentEnding();
        }

        protected virtual void WriteInterfaceDeclaration(InterfaceDeclaration @interface)
        {
            WriteDeclarationSectionBeginning(@interface, $"{@interface.Assembly.Name}@{@interface.Assembly.Version} - {@interface.GetSimpleNameReference()} Interface");
            HasDefaultSection = true;

            WriteNavigation(@interface);
            WritePageHeading($"{@interface.GetSimpleNameReference()} Interface", @interface.AccessModifier);

            WriteSummary(@interface.Summary);

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

            WriteDeclarationSectionEnding();
        }

        protected sealed override void VisitClass(ClassDeclaration @class)
        {
            _WriteHtmlDocumentBeginning($"{@class.Assembly.Name}@{@class.Assembly.Version} - {@class.GetSimpleNameReference()} Class");

            WriteClassDeclaration(@class);

            foreach (var type in @class.NestedTypes)
                type.Accept(this);
            foreach (var member in @class.Members)
                member.Accept(this);

            _WriteHtmlDocumentEnding();
        }

        protected virtual void WriteClassDeclaration(ClassDeclaration @class)
        {
            WriteDeclarationSectionBeginning(@class, $"{@class.Assembly.Name}@{@class.Assembly.Version} - {@class.GetSimpleNameReference()} Class");
            HasDefaultSection = true;

            WriteNavigation(@class);
            WritePageHeading($"{@class.GetSimpleNameReference()} Class", @class.AccessModifier);

            WriteSummary(@class.Summary);

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

            WriteDeclarationSectionEnding();
        }

        protected sealed override void VisitRecord(RecordDeclaration record)
        {
            _WriteHtmlDocumentBeginning($"{@record.Assembly.Name}@{@record.Assembly.Version} - {@record.GetSimpleNameReference()} Record");

            WriteRecordDeclaration(record);

            foreach (var type in record.NestedTypes)
                type.Accept(this);
            foreach (var member in record.Members)
                member.Accept(this);

            _WriteHtmlDocumentEnding();
        }

        protected virtual void WriteRecordDeclaration(RecordDeclaration record)
        {
            WriteDeclarationSectionBeginning(record, $"{@record.Assembly.Name}@{@record.Assembly.Version} - {@record.GetSimpleNameReference()} Record");
            HasDefaultSection = true;

            WriteNavigation(@record);
            WritePageHeading($"{@record.GetSimpleNameReference()} Record", @record.AccessModifier);

            WriteSummary(@record.Summary);

            if (@record.BaseRecord != typeof(object))
            {
                TextWriter.Write("<p>");
                WriteSafeHtml("This class extends ");
                WriteConstructedTypeReference(@record.BaseRecord);
                WriteSafeHtml(".");
                TextWriter.Write("</p>");
            }
            if (@record.ImplementedInterfaces.Any())
            {
                TextWriter.Write("<p>");
                WriteSafeHtml("This class implements the following interfaces: ");
                var isFrist = true;
                foreach (var imlementedInterface in @record.ImplementedInterfaces)
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

            WriteGenericParameters(@record.GenericParameters);
            WriteMembersList(@record.Constants, "Constants", "constants");
            WriteMembersList(@record.Fields, "Fields", "fields");
            WriteMembersList(@record.Constructors, "Constructors", "constructors");
            WriteMembersList(@record.Events, "Events", "events");
            WriteMembersList(@record.Properties, "Properties", "properties");
            WriteMembersList(@record.Methods, "Methods", "methods");

            WriteExamples(@record.Examples);
            WriteRemarks(@record.Remarks);
            WriteRelatedMembers(@record.RelatedMembers);

            WriteDeclarationSectionEnding();
        }

        protected sealed override void VisitStruct(StructDeclaration @struct)
        {
            _WriteHtmlDocumentBeginning($"{@struct.Assembly.Name}@{@struct.Assembly.Version} - {@struct.GetSimpleNameReference()} Struct");

            WriteStructDeclaration(@struct);

            foreach (var type in @struct.NestedTypes)
                type.Accept(this);
            foreach (var member in @struct.Members)
                member.Accept(this);

            _WriteHtmlDocumentEnding();
        }

        protected virtual void WriteStructDeclaration(StructDeclaration @struct)
        {
            WriteDeclarationSectionBeginning(@struct, $"{@struct.Assembly.Name}@{@struct.Assembly.Version} - {@struct.GetSimpleNameReference()} Struct");
            HasDefaultSection = true;

            WriteNavigation(@struct);
            WritePageHeading($"{@struct.GetSimpleNameReference()} Struct", @struct.AccessModifier);

            WriteSummary(@struct.Summary);

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

            WriteDeclarationSectionEnding();
        }

        protected sealed override void VisitConstant(ConstantDeclaration constant)
        {
            _WriteHtmlDocumentBeginning($"{constant.DeclaringType.Assembly.Name}@{constant.DeclaringType.Assembly.Version} - {constant.DeclaringType.GetSimpleNameReference()} Constant");

            WriteConstantDeclaration(constant);

            _WriteHtmlDocumentEnding();
        }

        protected virtual void WriteConstantDeclaration(ConstantDeclaration constant)
        {
            WriteDeclarationSectionBeginning(constant, $"{constant.DeclaringType.Assembly.Name}@{constant.DeclaringType.Assembly.Version} - {constant.DeclaringType.GetSimpleNameReference()} Constant");
            HasDefaultSection = true;

            WriteNavigation(constant);
            WritePageHeading($"{constant.GetSimpleNameReference()} Constant", constant.AccessModifier);

            WriteSummary(constant.Summary);
            TextWriter.Write("<p>");
            WriteSafeHtml("The value of this constant is ");
            TextWriter.Write("<code>");
            WriteConstantValue(constant);
            TextWriter.Write("</code>");
            WriteSafeHtml(".");
            TextWriter.Write("</p>");

            WriteExamples(constant.Examples);
            WriteRemarks(constant.Remarks);
            WriteRelatedMembers(constant.RelatedMembers);

            WriteDeclarationSectionEnding();
        }

        protected sealed override void VisitField(FieldDeclaration field)
        {
            _WriteHtmlDocumentBeginning($"{field.DeclaringType.Assembly.Name}@{field.DeclaringType.Assembly.Version} - {field.DeclaringType.GetSimpleNameReference()} Field");

            WriteFieldDeclaration(field);

            _WriteHtmlDocumentEnding();
        }

        protected virtual void WriteFieldDeclaration(FieldDeclaration field)
        {
            WriteDeclarationSectionBeginning(field, $"{field.DeclaringType.Assembly.Name}@{field.DeclaringType.Assembly.Version} - {field.DeclaringType.GetSimpleNameReference()} Field");
            HasDefaultSection = true;

            WriteNavigation(field);
            WritePageHeading($"{field.GetSimpleNameReference()} Field", field.AccessModifier);

            WriteSummary(field.Summary);

            WriteExamples(field.Examples);
            WriteRemarks(field.Remarks);
            WriteRelatedMembers(field.RelatedMembers);

            WriteDeclarationSectionEnding();
        }

        protected sealed override void VisitConstructor(ConstructorDeclaration constructor)
        {
            _WriteHtmlDocumentBeginning($"{constructor.DeclaringType.Assembly.Name}@{constructor.DeclaringType.Assembly.Version} - {constructor.GetSimpleNameReference()} Constructor");

            WriteConstructorDeclaration(constructor);

            _WriteHtmlDocumentEnding();
        }

        private void WriteConstructorDeclaration(ConstructorDeclaration constructor)
        {
            WriteDeclarationSectionBeginning(constructor, $"{constructor.DeclaringType.Assembly.Name}@{constructor.DeclaringType.Assembly.Version} - {constructor.GetSimpleNameReference()} Constructor");
            HasDefaultSection = true;

            WriteNavigation(constructor);
            WritePageHeading($"{constructor.GetSimpleNameReference()} Delegate", constructor.AccessModifier);

            WriteSummary(constructor.Summary);

            WriteParameters(constructor.Parameters);

            WriteExceptions(constructor.Exceptions);
            WriteExamples(constructor.Examples);
            WriteRemarks(constructor.Remarks);
            WriteRelatedMembers(constructor.RelatedMembers);

            WriteDeclarationSectionEnding();
        }

        protected sealed override void VisitEvent(EventDeclaration @event)
        {
            _WriteHtmlDocumentBeginning($"{@event.DeclaringType.Assembly.Name}@{@event.DeclaringType.Assembly.Version} - {@event.DeclaringType.GetSimpleNameReference()} Event");

            WriteEventDeclaration(@event);

            _WriteHtmlDocumentEnding();
        }

        protected virtual void WriteEventDeclaration(EventDeclaration @event)
        {
            WriteDeclarationSectionBeginning(@event, $"{@event.DeclaringType.Assembly.Name}@{@event.DeclaringType.Assembly.Version} - {@event.DeclaringType.GetSimpleNameReference()} Event");
            HasDefaultSection = true;

            WriteNavigation(@event);
            WritePageHeading($"{@event.GetSimpleNameReference()} Event", @event.AccessModifier);

            WriteSummary(@event.Summary);
            TextWriter.Write("<p>");
            WriteSafeHtml("The type of this event is ");
            WriteConstructedTypeReference(@event.Type);
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

            WriteDeclarationSectionEnding();
        }

        protected sealed override void VisitProperty(PropertyDeclaration property)
        {
            _WriteHtmlDocumentBeginning($"{property.DeclaringType.Assembly.Name}@{property.DeclaringType.Assembly.Version} - {property.DeclaringType.GetSimpleNameReference()} Property");

            WritePropertyDeclaration(property);

            _WriteHtmlDocumentEnding();
        }

        protected virtual void WritePropertyDeclaration(PropertyDeclaration property)
        {
            WriteDeclarationSectionBeginning(property, $"{property.DeclaringType.Assembly.Name}@{property.DeclaringType.Assembly.Version} - {property.DeclaringType.GetSimpleNameReference()} Property");
            HasDefaultSection = true;

            WriteNavigation(property);
            WritePageHeading($"{property.GetSimpleNameReference()} Property", property.AccessModifier);

            WriteSummary(property.Summary);
            TextWriter.Write("<p>");
            WriteSafeHtml("The type of this property is ");
            WriteConstructedTypeReference(property.Type);
            WriteSafeHtml(".");
            TextWriter.Write("</p>");

            TextWriter.Write("<p>");
            if (property.Getter is not null && property.Setter is not null)
            {
                WriteSafeHtml("This property has a ");
                WriteAccessModifier(property.Getter.AccessModifier);
                WriteSafeHtml(" getter and a ");
                WriteAccessModifier(property.Setter.AccessModifier);
                WriteSafeHtml(" setter.");
            }
            else if (property.Getter is not null)
            {
                WriteSafeHtml("This property has a ");
                WriteAccessModifier(property.Getter.AccessModifier);
                WriteSafeHtml(" getter.");
            }
            else if (property.Setter is not null)
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

            WriteDeclarationSectionEnding();
        }

        protected sealed override void VisitMethod(MethodDeclaration method)
        {
            _WriteHtmlDocumentBeginning($"{method.DeclaringType.Assembly.Name}@{method.DeclaringType.Assembly.Version} - {method.DeclaringType.GetSimpleNameReference()} Method");

            WriteMethodDeclaration(method);

            _WriteHtmlDocumentEnding();
        }

        protected virtual void WriteMethodDeclaration(MethodDeclaration method)
        {
            WriteDeclarationSectionBeginning(method, $"{method.DeclaringType.Assembly.Name}@{method.DeclaringType.Assembly.Version} - {method.DeclaringType.GetSimpleNameReference()} Method");
            HasDefaultSection = true;

            WriteNavigation(method);
            WritePageHeading($"{method.GetSimpleNameReference()} Method", method.AccessModifier);

            WriteSummary(method.Summary);

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

            WriteDeclarationSectionEnding();
        }

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

        protected virtual void WriteTypesList(IEnumerable<TypeDeclaration> types, string title, string sectionId)
        {
            if (types.Any())
            {
                TextWriter.Write("<section id=\"");
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
                    WriteSafeHtml(type.GetFullNameReference());
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

        protected virtual void WriteMembersList(IEnumerable<MemberDeclaration> members, string title, string sectionId)
            => WriteMembersList(members, title, sectionId, false);

        protected virtual void WriteMembersList(IEnumerable<MemberDeclaration> members, string title, string sectionId, bool hideAccessModifier)
        {
            if (members.Any())
            {
                TextWriter.Write("<section id=\"");
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
                    WriteSafeHtml(member.GetFullNameReference());
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

        protected virtual void WriteGenericParameters(IEnumerable<GenericParameterData> genericParameters)
        {
            if (genericParameters.Any())
            {
                var htmlWriterDocumentationVisitor = new HtmlWriterDocumentationVisitor(TextWriter, MemberReferenceResolver);

                TextWriter.Write("<section id=\"generic-parameters\">");
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
                                WriteSafeHtml(MemberReferenceResolver.GetUrl(typeConstraint));
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

        protected virtual void WriteNavigation(DeclarationNode declarationNode)
        {
            TextWriter.Write("<nav>");
            WriteDeclarationReference(declarationNode, isLeaf: true);
            TextWriter.Write("</nav>");

            void WriteDeclarationReference(DeclarationNode declarationNode, bool isLeaf = false)
            {
                switch (declarationNode)
                {
                    case AssemblyDeclaration assembly:
                        if (isLeaf)
                            WriteNavigationActiveReference(assembly);
                        else
                            WriteNavigationReference(assembly);
                        break;

                    case NamespaceDeclaration @namespace:
                        WriteDeclarationReference(@namespace.Assembly);
                        TextWriter.Write(" / ");

                        if (isLeaf)
                            WriteNavigationActiveReference(@namespace);
                        else
                            WriteNavigationReference(@namespace);
                        break;

                    case TypeDeclaration type:
                        WriteDeclarationReference(type.Namespace);
                        TextWriter.Write(" / ");

                        if (type.DeclaringType is not null)
                        {
                            WriteDeclarationReference(type.DeclaringType);
                            TextWriter.Write(" / ");
                        }

                        if (isLeaf)
                            WriteNavigationActiveReference(type);
                        else
                            WriteNavigationReference(type);
                        break;

                    case MemberDeclaration member:
                        WriteDeclarationReference(member.DeclaringType);
                        TextWriter.Write(" / ");

                        if (isLeaf)
                            WriteNavigationActiveReference(member);
                        else
                            WriteNavigationReference(member);
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        protected virtual void WriteNavigationReference(DeclarationNode declarationNode)
        {
            TextWriter.Write("<a href=\"");
            if (declarationNode is not AssemblyDeclaration)
            {
                WriteSafeHtml("#");
                WriteSafeHtml(declarationNode.GetFullNameReference());
            }
            TextWriter.Write("\">");
            WriteSafeHtml(declarationNode.GetSimpleNameReference());
            TextWriter.Write("</a>");
        }

        protected virtual void WriteNavigationActiveReference(DeclarationNode declarationNode)
            => WriteSafeHtml(declarationNode.GetSimpleNameReference());

        protected virtual void WritePageHeading(string title)
            => WritePageHeading(title, null);

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

        protected virtual void WriteSummary(SummaryDocumentationElement summary)
            => summary.Accept(new HtmlWriterDocumentationVisitor(TextWriter, MemberReferenceResolver));

        protected virtual void WriteFirstSummaryParagraph(SummaryDocumentationElement summary)
        {
            var documentationVisitor = new HtmlWriterDocumentationVisitor(TextWriter, MemberReferenceResolver);
            foreach (var paragraphDocumentationElement in summary.Content.OfType<ParagraphDocumentationElement>().Take(1))
                foreach (var element in paragraphDocumentationElement.Content)
                    element.Accept(documentationVisitor);
        }

        protected virtual void WriteValue(ValueDocumentationElement value)
        {
            if (value.Content.Any())
            {
                var htmlWriterDocumentationVisitor = new HtmlWriterDocumentationVisitor(TextWriter, MemberReferenceResolver);
                TextWriter.Write("<section id=\"value\">");

                TextWriter.Write("<h2>");
                WriteSafeHtml("Value");
                TextWriter.Write("</h2>");

                value.Accept(htmlWriterDocumentationVisitor);
                TextWriter.Write("</section>");
            }
        }

        protected virtual void WriteParameters(IEnumerable<ParameterData> parameters)
        {
            if (parameters.Any())
            {
                var htmlWriterDocumentationVisitor = new HtmlWriterDocumentationVisitor(TextWriter, MemberReferenceResolver);

                TextWriter.Write("<section id=\"parameters\">");
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

        protected virtual void WriteReturn(MethodReturnData returnData)
        {
            var htmlWriterDocumentationVisitor = new HtmlWriterDocumentationVisitor(TextWriter, MemberReferenceResolver);
            TextWriter.Write("<section id=\"return\">");

            TextWriter.Write("<h2>");
            WriteSafeHtml("Return: ");
            WriteConstructedTypeReference(returnData.Type);
            TextWriter.Write("</h2>");

            returnData.Description.Accept(htmlWriterDocumentationVisitor);
            TextWriter.Write("</section>");
        }

        protected virtual void WriteExceptions(IEnumerable<ExceptionDocumentationElement> exceptions)
        {
            if (exceptions.Any())
            {
                var htmlWriterDocumentationVisitor = new HtmlWriterDocumentationVisitor(TextWriter, MemberReferenceResolver);
                TextWriter.Write("<section id=\"exceptions\">");
                foreach (var exception in exceptions)
                    exception.Accept(htmlWriterDocumentationVisitor);
                TextWriter.Write("</section>");
            }
        }

        protected virtual void WriteExamples(IEnumerable<ExampleDocumentationElement> examples)
        {
            if (examples.Any())
            {
                var htmlWriterDocumentationVisitor = new HtmlWriterDocumentationVisitor(TextWriter, MemberReferenceResolver);
                TextWriter.Write("<section id=\"examples\">");
                foreach (var example in examples)
                    example.Accept(htmlWriterDocumentationVisitor);
                TextWriter.Write("</section>");
            }
        }

        protected virtual void WriteRemarks(RemarksDocumentationElement remarks)
            => remarks.Accept(new HtmlWriterDocumentationVisitor(TextWriter, MemberReferenceResolver));

        protected virtual void WriteRelatedMembers(IEnumerable<MemberReferenceDocumentationElement> relatedMembers)
        {
            if (relatedMembers.Any())
            {
                var htmlWriterDocumentationVisitor = new HtmlWriterDocumentationVisitor(TextWriter, MemberReferenceResolver);
                TextWriter.Write("<section id=\"references\">");
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

        protected virtual void WriteConstructedTypeReference(BaseTypeReference type)
        {
            var memberReferenceVisitor = new HyperlinkWriterMemberReferenceVisitor(TextWriter, MemberReferenceResolver);
            type.Accept(memberReferenceVisitor);
        }

        protected virtual void WriteConstantValue(ConstantDeclaration constant)
        {
            if (constant.Value is null)
                WriteSafeHtml("null");
            else if (constant.Value is string)
                WriteSafeHtml($"\"{constant.Value}\"");
            else if (constant.Value is char)
                WriteSafeHtml($"'{constant.Value}'");
            else if (constant.Value.GetType().IsEnum)
                WriteSafeHtml(string.Format(CultureInfo.InvariantCulture, "{0:D}", constant.Value));
            else
                WriteSafeHtml(Convert.ToString(constant.Value, CultureInfo.InvariantCulture));
        }

        protected virtual void WriteSafeHtml(string value)
        {
            var htmlSafeValue = value;
            if (value.Any(@char => @char == '<' || @char == '>' || @char == '&' || @char == '\'' || @char == '"' || char.IsControl(@char)))
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
                                    return stringBuilder.Append("&quot");

                                default:
                                    if (@char == '\'' || char.IsControl(@char))
                                        return stringBuilder.Append("&#x").Append(((short)@char).ToString("x2")).Append(';');
                                    else
                                        return stringBuilder.Append(@char);
                            }
                        }
                    )
                    .ToString();

            TextWriter.Write(htmlSafeValue);
        }

        protected virtual void WriteDeclarationSectionBeginning(DeclarationNode declarationNode, string sectionTitle)
        {
            TextWriter.Write("<section id=\"");
            WriteSafeHtml(declarationNode.GetFullNameReference());
            TextWriter.Write("\" data-title=\"");
            WriteSafeHtml(sectionTitle);
            TextWriter.Write("\"");

            if (!HasDefaultSection)
                TextWriter.Write(" data-default=\"true\" style=\"display: initial\"");
            else
                TextWriter.Write(" data-default=\"false\" style=\"display: none\"");

            TextWriter.Write(">");
        }

        protected virtual void WriteDeclarationSectionEnding()
            => TextWriter.Write("</section>");

        private void _WriteHtmlDocumentBeginning(string title)
        {
            if (_declarationDepth == 0)
            {
                if (_isHtmlInitialized)
                    throw new InvalidOperationException("The HTML document has already been started, please use a different instance.");

                WriteHtmlBeginning(title);
                _isHtmlInitialized = true;
            }
        }

        protected virtual void WriteHtmlBeginning(string pageTitle)
        {
            TextWriter.WriteLine("<!DOCTYPE html>");
            TextWriter.Write("<html lang=\"en-US\">");
            TextWriter.Write("<head>");
            TextWriter.Write("<meta charset=\"UTF-8\">");
            TextWriter.Write("<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">");
            TextWriter.Write("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            TextWriter.Write("<title>");
            WriteSafeHtml(pageTitle);
            TextWriter.Write("</title>");
            TextWriter.Write("</head>");
            TextWriter.Write("<body>");
        }

        private void _WriteHtmlDocumentEnding()
        {
            if (_declarationDepth == 0)
            {
                if (!_isHtmlInitialized)
                    throw new InvalidOperationException("The HTML document has not been started.");

                WriteHtmlEnding();
            }
        }

        protected virtual void WriteHtmlEnding()
        {
            TextWriter.Write("<script>");
            TextWriter.Write("window.addEventListener(\"hashchange\", function (hashChangeEvent) { ");
            TextWriter.Write("switchView(hashChangeEvent.oldURL.split(\"#\", 2)[1], hashChangeEvent.newURL.split(\"#\", 2)[1]);");
            TextWriter.Write("});");

            TextWriter.Write("function switchView(from, to) {");
            TextWriter.Write("hide(from || \"\");");
            TextWriter.Write("show(to || \"\");");

            TextWriter.Write("function show(elementId) {");
            TextWriter.Write("const element = document.getElementById(elementId) || document.querySelector(\"section[data-default='true']\");");
            TextWriter.Write("element.style.display = \"initial\";");
            TextWriter.Write("}");

            TextWriter.Write("function hide(elementId) {");
            TextWriter.Write("const element = document.getElementById(elementId) || document.querySelector(\"section[data-default='true']\");");
            TextWriter.Write("element.style.display = \"none\";");
            TextWriter.Write("}");

            TextWriter.Write("}");

            TextWriter.Write("switchView(document.querySelector(\"section[data-default='true']\").id, window.location.hash.split(\"#\")[1]);");
            TextWriter.Write("</script>");

            TextWriter.Write("</body>");
            TextWriter.Write("</html>");
        }
    }
}