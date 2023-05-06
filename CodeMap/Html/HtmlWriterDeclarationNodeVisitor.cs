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
    /// <summary/>
    public class HtmlWriterDeclarationNodeVisitor : DeclarationNodeVisitor
    {
        private bool _isHtmlInitialized = false;
        private int _declarationDepth = 0;

        /// <summary/>
        public HtmlWriterDeclarationNodeVisitor(TextWriter textWriter, IMemberReferenceResolver memberReferenceResolver)
        {
            TextWriter = textWriter;
            MemberReferenceResolver = memberReferenceResolver ?? throw new ArgumentNullException(nameof(memberReferenceResolver));
            HasDefaultSection = false;
        }

        /// <summary/>
        public TextWriter TextWriter { get; set; }

        /// <summary/>
        public IMemberReferenceResolver MemberReferenceResolver { get; }

        /// <summary/>
        protected bool HasDefaultSection { get; private set; }

        /// <summary/>
        protected virtual DocumentationVisitor CreateDocumentationVisitor()
            => new HtmlWriterDocumentationVisitor(TextWriter, MemberReferenceResolver);

        /// <summary/>
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

        /// <summary/>
        protected virtual void WriteAssemblyDeclaration(AssemblyDeclaration assembly)
        {
            WriteDeclarationSectionBeginning(assembly);

            WriteNavigation(assembly);
            WritePageHeading($"{assembly.Name}@{assembly.Version}");

            WriteSummary(assembly.Summary);
            WriteNamespacesList(assembly.Namespaces);

            WriteExamples(assembly.Examples);
            WriteRemarks(assembly.Remarks);
            WriteRelatedMembers(assembly.RelatedMembers);

            WriteAttributes(assembly.Attributes, "attributes", "Attributes");

            WriteDeclarationSectionEnding(assembly);
        }

        /// <summary/>
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

        /// <summary/>
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

        /// <summary/>
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

        /// <summary/>
        protected internal sealed override void VisitEnum(EnumDeclaration @enum)
        {
            _WriteHtmlDocumentBeginning(@enum);

            WriteEnumDeclaration(@enum);
            HasDefaultSection = true;

            _WriteHtmlDocumentEnding(@enum);
        }

        /// <summary/>
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

        /// <summary/>
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

        /// <summary/>
        protected internal sealed override void VisitDelegate(DelegateDeclaration @delegate)
        {
            _WriteHtmlDocumentBeginning(@delegate);

            WriteDelegateDeclaration(@delegate);
            HasDefaultSection = true;

            _WriteHtmlDocumentEnding(@delegate);
        }

        /// <summary/>
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

        /// <summary/>
        protected internal sealed override void VisitInterface(InterfaceDeclaration @interface)
        {
            _WriteHtmlDocumentBeginning(@interface);

            WriteInterfaceDeclaration(@interface);
            HasDefaultSection = true;

            foreach (var member in @interface.Members)
                member.Accept(this);

            _WriteHtmlDocumentEnding(@interface);
        }

        /// <summary/>
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

        /// <summary/>
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

        /// <summary/>
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

        /// <summary/>
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

        /// <summary/>
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

        /// <summary/>
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

        /// <summary/>
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

        /// <summary/>
        protected internal sealed override void VisitConstant(ConstantDeclaration constant)
        {
            _WriteHtmlDocumentBeginning(constant);

            WriteConstantDeclaration(constant);
            HasDefaultSection = true;

            _WriteHtmlDocumentEnding(constant);
        }

        /// <summary/>
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

        /// <summary/>
        protected internal sealed override void VisitField(FieldDeclaration field)
        {
            _WriteHtmlDocumentBeginning(field);

            WriteFieldDeclaration(field);
            HasDefaultSection = true;

            _WriteHtmlDocumentEnding(field);
        }

        /// <summary/>
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

        /// <summary/>
        protected internal sealed override void VisitConstructor(ConstructorDeclaration constructor)
        {
            _WriteHtmlDocumentBeginning(constructor);

            WriteConstructorDeclaration(constructor);
            HasDefaultSection = true;

            _WriteHtmlDocumentEnding(constructor);
        }

        /// <summary/>
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

        /// <summary/>
        protected internal sealed override void VisitEvent(EventDeclaration @event)
        {
            _WriteHtmlDocumentBeginning(@event);

            WriteEventDeclaration(@event);
            HasDefaultSection = true;

            _WriteHtmlDocumentEnding(@event);
        }

        /// <summary/>
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

        /// <summary/>
        protected internal sealed override void VisitProperty(PropertyDeclaration property)
        {
            _WriteHtmlDocumentBeginning(property);

            WritePropertyDeclaration(property);
            HasDefaultSection = true;

            _WriteHtmlDocumentEnding(property);
        }

        /// <summary/>
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

        /// <summary/>
        protected internal sealed override void VisitMethod(MethodDeclaration method)
        {
            _WriteHtmlDocumentBeginning(method);

            WriteMethodDeclaration(method);
            HasDefaultSection = true;

            _WriteHtmlDocumentEnding(method);
        }

        /// <summary/>
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

        /// <summary/>
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

        /// <summary/>
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

        /// <summary/>
        protected void WriteMembersList(IEnumerable<MemberDeclaration> members, string title, string sectionId)
            => WriteMembersList(members, title, sectionId, false);

        /// <summary/>
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

        /// <summary/>
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

        /// <summary/>
        protected virtual void WriteNavigation(DeclarationNode declarationNode)
        {
            TextWriter.Write("<nav>");
            WriteDeclarationItems(declarationNode);
            TextWriter.Write("</nav>");
        }

        /// <summary/>
        protected void WriteDeclarationItems(DeclarationNode declarationNode)
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

        /// <summary/>
        protected virtual void WriteNavigationItem(DeclarationNode declarationNode)
        {
            TextWriter.Write("<a href=\"#");
            if (!(declarationNode is AssemblyDeclaration))
                TextWriter.Write(Uri.EscapeDataString(declarationNode.GetFullNameReference()));
            TextWriter.Write("\">");
            WriteSafeHtml(declarationNode.GetSimpleNameReference());
            TextWriter.Write("</a>");
        }

        /// <summary/>
        protected virtual void WriteNavigationActiveItem(DeclarationNode declarationNode)
            => WriteSafeHtml(declarationNode.GetSimpleNameReference());

        /// <summary/>
        protected virtual void WritePageHeading(string title)
            => WritePageHeading(title, null);

        /// <summary/>
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

        /// <summary/>
        protected virtual void WriteSummary(SummaryDocumentationElement summary)
            => summary.Accept(CreateDocumentationVisitor());

        /// <summary/>
        protected virtual void WriteFirstSummaryParagraph(SummaryDocumentationElement summary)
        {
            var documentationVisitor = CreateDocumentationVisitor();
            foreach (var paragraphDocumentationElement in summary.Content.OfType<ParagraphDocumentationElement>().Take(1))
                foreach (var element in paragraphDocumentationElement.Content)
                    element.Accept(documentationVisitor);
        }

        /// <summary/>
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

        /// <summary/>
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

        /// <summary/>
        protected virtual void WriteReturn(MethodReturnData returnData)
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

        /// <summary/>
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

        /// <summary/>
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

        /// <summary/>
        protected virtual void WriteRemarks(RemarksDocumentationElement remarks)
            => remarks.Accept(CreateDocumentationVisitor());

        /// <summary/>
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

        /// <summary/>
        protected virtual void WriteConstructedTypeReference(BaseTypeReference type)
        {
            var memberReferenceVisitor = new HyperlinkWriterMemberReferenceVisitor(TextWriter, MemberReferenceResolver);
            type.Accept(memberReferenceVisitor);
        }

        /// <summary/>
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

        /// <summary/>
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

        /// <summary/>
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

        /// <summary/>
        protected virtual void WriteOtherSectionAttributes(DeclarationNode declarationNode)
        {
        }

        /// <summary/>
        protected virtual void WriteDeclarationSectionEnding(DeclarationNode declarationNode)
            => TextWriter.Write("</section>");

        /// <summary/>
        protected virtual string GetPageTitle(DeclarationNode declarationNode)
        {
            var visitor = new HtmlPageTitleDeclarationNodeVisitor();
            declarationNode.Accept(visitor);
            return visitor.TitleStringBuilder.ToString();
        }

        /// <summary/>
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

        /// <summary/>
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

        /// <summary/>
        protected virtual void WriteOtherHtmlAttributes(DeclarationNode declarationNode)
        {
        }

        /// <summary/>
        protected virtual void WriteOtherHtmlHeadTags(DeclarationNode declarationNode)
        {
        }

        /// <summary/>
        protected virtual void WriteOtherBodyAttributes(DeclarationNode declarationNode)
        {
        }

        /// <summary/>
        private void _WriteHtmlDocumentEnding(DeclarationNode declarationNode)
        {
            if (_declarationDepth == 0)
            {
                if (!_isHtmlInitialized)
                    throw new InvalidOperationException("The HTML document has not been started.");

                WriteHtmlEnding(declarationNode);
            }
        }

        /// <summary/>
        protected virtual void WriteHtmlEnding(DeclarationNode declarationNode)
        {
            WriteNavigationJavaScript(declarationNode);
            WriteOtherHtmlBodyTags(declarationNode);

            TextWriter.Write("</body>");
            TextWriter.Write("</html>");
        }

        /// <summary/>
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

        /// <summary/>
        protected virtual void WriteOtherHtmlBodyTags(DeclarationNode declarationNode)
        {
        }
    }
}