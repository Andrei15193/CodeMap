using CodeMap.DeclarationNodes;
using CodeMap.DocumentationElements;
using CodeMap.ReferenceData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeMap.Json
{
    /// <summary>Represents a JSON writer documentation visitor for storing the documentation of an Assembly as JSON.</summary>
    public class JsonWriterDeclarationNodeVisitor : DeclarationNodeVisitor
    {
        private readonly JsonWriter _jsonWriter;
        private readonly JsonWriterDocumentationVisitor _jsonWriterDocumentationVisitor;
        private readonly JsonWriterMemberReferenceVisitor _memberReferenceWriter;

        /// <summary>Initializes a new instance of the <see cref="JsonWriterDeclarationNodeVisitor"/> class.</summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write the documentation to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="jsonWriter"/> is <c>null</c>.</exception>
        public JsonWriterDeclarationNodeVisitor(JsonWriter jsonWriter)
        {
            _jsonWriter = jsonWriter ?? throw new ArgumentNullException(nameof(jsonWriter));
            _jsonWriterDocumentationVisitor = new JsonWriterDocumentationVisitor(_jsonWriter);
            _memberReferenceWriter = new JsonWriterMemberReferenceVisitor(_jsonWriter);
        }

        /// <summary>Disposes the current instance.</summary>
        public void Dispose()
            => Dispose(true);

        /// <summary>Disposes the current instance.</summary>
        /// <param name="disposing">Specifies whether this method was called from the <see cref="Dispose()"/> method.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                ((IDisposable)_jsonWriter).Dispose();
        }

        /// <summary>Visits an <see cref="AssemblyDeclaration"/>.</summary>
        /// <param name="assembly">The <see cref="AssemblyDeclaration"/> to visit.</param>
        protected internal override void VisitAssembly(AssemblyDeclaration assembly)
        {
            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("name");
            _jsonWriter.WriteValue(assembly.Name);
            _jsonWriter.WritePropertyName("version");
            _jsonWriter.WriteValue(assembly.Version.ToString());
            _jsonWriter.WritePropertyName("culture");
            _jsonWriter.WriteValue(assembly.Culture);
            _jsonWriter.WritePropertyName("publicKeyToken");
            _jsonWriter.WriteValue(assembly.PublicKeyToken);

            _WriteAttributes(assembly.Attributes);
            _WriteDependencies(assembly.Dependencies);

            assembly.Summary.Accept(_jsonWriterDocumentationVisitor);
            assembly.Remarks.Accept(_jsonWriterDocumentationVisitor);
            _WriteExamples(assembly.Examples);
            _WriteRelatedMembers(assembly.RelatedMembers);

            _jsonWriter.WritePropertyName("definitions");
            _jsonWriter.WriteStartObject();

            foreach (var @namespace in assembly.Namespaces)
                @namespace.Accept(this);

            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits a <see cref="NamespaceDeclaration"/>.</summary>
        /// <param name="namespace">The <see cref="NamespaceDeclaration"/> to visit.</param>
        protected internal override void VisitNamespace(NamespaceDeclaration @namespace)
        {
            _WriteNamespaceDefinition(@namespace);
            foreach (var type in @namespace.DeclaredTypes)
                type.Accept(this);
        }

        private void _WriteNamespaceDefinition(NamespaceDeclaration @namespace)
        {
            _jsonWriter.WritePropertyName(@namespace.Name);

            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("kind");
            _jsonWriter.WriteValue("namespace");
            _jsonWriter.WritePropertyName("name");
            _jsonWriter.WriteValue(@namespace.Name);

            @namespace.Summary.Accept(_jsonWriterDocumentationVisitor);
            @namespace.Remarks.Accept(_jsonWriterDocumentationVisitor);
            _WriteExamples(@namespace.Examples);
            _WriteRelatedMembers(@namespace.RelatedMembers);

            _WriteEnumReferences(@namespace.Enums);
            _WriteDelegateReferences(@namespace.Delegates);
            _WriteInterfaceReferences(@namespace.Interfaces);
            _WriteClassReferences(@namespace.Classes);
            _WriteStructReferences(@namespace.Structs);

            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits an <see cref="EnumDeclaration"/>.</summary>
        /// <param name="enum">The <see cref="EnumDeclaration"/> to visit.</param>
        protected internal override void VisitEnum(EnumDeclaration @enum)
        {
            _WriteEnumDefinition(@enum);
            foreach (var member in @enum.Members)
                member.Accept(this);
        }

        private void _WriteEnumDefinition(EnumDeclaration @enum)
        {
            _jsonWriter.WritePropertyName(_GetIdFor(@enum));

            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("kind");
            _jsonWriter.WriteValue("enum");
            _jsonWriter.WritePropertyName("name");
            _jsonWriter.WriteValue(@enum.Name);
            _jsonWriter.WritePropertyName("namespace");
            _jsonWriter.WriteValue(@enum.Namespace.Name);
            _WriteAccessModifier(@enum.AccessModifier);
            _jsonWriter.WritePropertyName("underlyingType");
            @enum.UnderlyingType.Accept(_memberReferenceWriter);

            _WriteDeclaringTypeReference(@enum.DeclaringType);
            _WriteAttributes(@enum.Attributes);

            @enum.Summary.Accept(_jsonWriterDocumentationVisitor);
            @enum.Remarks.Accept(_jsonWriterDocumentationVisitor);
            _WriteExamples(@enum.Examples);
            _WriteRelatedMembers(@enum.RelatedMembers);

            _jsonWriter.WritePropertyName("members");
            _jsonWriter.WriteStartArray();
            foreach (var member in @enum.Members)
                _jsonWriter.WriteValue(_GetIdFor(member));
            _jsonWriter.WriteEndArray();

            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits a <see cref="DelegateDeclaration"/>.</summary>
        /// <param name="delegate">The <see cref="DelegateDeclaration"/> to visit.</param>
        protected internal override void VisitDelegate(DelegateDeclaration @delegate)
            => _WriteDelegateDefinition(@delegate);

        private void _WriteDelegateDefinition(DelegateDeclaration @delegate)
        {
            _jsonWriter.WritePropertyName(_GetIdFor(@delegate));

            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("kind");
            _jsonWriter.WriteValue("delegate");
            _jsonWriter.WritePropertyName("name");
            _jsonWriter.WriteValue(@delegate.Name);
            _jsonWriter.WritePropertyName("namespace");
            _jsonWriter.WriteValue(@delegate.Namespace.Name);
            _WriteAccessModifier(@delegate.AccessModifier);

            _WriteDeclaringTypeReference(@delegate.DeclaringType);
            _WriteAttributes(@delegate.Attributes);

            @delegate.Summary.Accept(_jsonWriterDocumentationVisitor);
            _WriteExceptions(@delegate.Exceptions);
            @delegate.Remarks.Accept(_jsonWriterDocumentationVisitor);
            _WriteExamples(@delegate.Examples);
            _WriteRelatedMembers(@delegate.RelatedMembers);

            _WriteGenericParameters(@delegate.GenericParameters);
            _WriteParameters(@delegate.Parameters);
            _WriteReturn(@delegate.Return);

            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits an <see cref="InterfaceDeclaration"/>.</summary>
        /// <param name="interface">The <see cref="InterfaceDeclaration"/> to visit.</param>
        protected internal override void VisitInterface(InterfaceDeclaration @interface)
        {
            _WriteInterfaceDefinition(@interface);
            foreach (var member in @interface.Members)
                member.Accept(this);
        }

        private void _WriteInterfaceDefinition(InterfaceDeclaration @interface)
        {
            _jsonWriter.WritePropertyName(_GetIdFor(@interface));

            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("kind");
            _jsonWriter.WriteValue("interface");
            _jsonWriter.WritePropertyName("name");
            _jsonWriter.WriteValue(@interface.Name);
            _jsonWriter.WritePropertyName("namespace");
            _jsonWriter.WriteValue(@interface.Namespace.Name);
            _WriteAccessModifier(@interface.AccessModifier);

            _WriteDeclaringTypeReference(@interface.DeclaringType);
            _WriteAttributes(@interface.Attributes);

            @interface.Summary.Accept(_jsonWriterDocumentationVisitor);
            @interface.Remarks.Accept(_jsonWriterDocumentationVisitor);
            _WriteExamples(@interface.Examples);
            _WriteRelatedMembers(@interface.RelatedMembers);

            _WriteGenericParameters(@interface.GenericParameters);

            _jsonWriter.WritePropertyName("baseInterfaces");
            _jsonWriter.WriteStartArray();
            foreach (var baseInterface in @interface.BaseInterfaces)
                baseInterface.Accept(_memberReferenceWriter);
            _jsonWriter.WriteEndArray();

            _WriteEventReferences(@interface.Events);
            _WritePropertyReferences(@interface.Properties);
            _WriteMethodReferences(@interface.Methods);

            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits a <see cref="ClassDeclaration"/>.</summary>
        /// <param name="class">The <see cref="ClassDeclaration"/> to visit.</param>
        protected internal override void VisitClass(ClassDeclaration @class)
        {
            _WriteClassDefinition(@class);
            foreach (var member in @class.Members)
                member.Accept(this);
            foreach (var nestedType in @class.NestedTypes)
                nestedType.Accept(this);
        }

        private void _WriteClassDefinition(ClassDeclaration @class)
        {
            _jsonWriter.WritePropertyName(_GetIdFor(@class));

            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("kind");
            _jsonWriter.WriteValue("class");
            _jsonWriter.WritePropertyName("name");
            _jsonWriter.WriteValue(@class.Name);
            _jsonWriter.WritePropertyName("namespace");
            _jsonWriter.WriteValue(@class.Namespace.Name);
            _WriteAccessModifier(@class.AccessModifier);
            _WriteDeclaringTypeReference(@class.DeclaringType);
            _WriteAttributes(@class.Attributes);

            @class.Summary.Accept(_jsonWriterDocumentationVisitor);
            @class.Remarks.Accept(_jsonWriterDocumentationVisitor);
            _WriteExamples(@class.Examples);
            _WriteRelatedMembers(@class.RelatedMembers);

            _WriteGenericParameters(@class.GenericParameters);

            _jsonWriter.WritePropertyName("baseClass");
            @class.BaseClass.Accept(_memberReferenceWriter);
            _WriteImplementedInterfacesReferences(@class.ImplementedInterfaces);

            _jsonWriter.WritePropertyName("isAbstract");
            _jsonWriter.WriteValue(@class.IsAbstract);
            _jsonWriter.WritePropertyName("isSealed");
            _jsonWriter.WriteValue(@class.IsSealed);
            _jsonWriter.WritePropertyName("isStatic");
            _jsonWriter.WriteValue(@class.IsStatic);

            _WriteConstantReferences(@class.Constants);
            _WriteFieldReferences(@class.Fields);
            _WriteConstructorReferences(@class.Constructors);
            _WriteEventReferences(@class.Events);
            _WritePropertyReferences(@class.Properties);
            _WriteMethodReferences(@class.Methods);

            _WriteNestedEnumReferences(@class.NestedEnums);
            _WriteNestedDelegateReferences(@class.NestedDelegates);
            _WriteNestedInterfaceReferences(@class.NestedInterfaces);
            _WriteNestedClasseReferences(@class.NestedClasses);
            _WriteNestedStructReferences(@class.NestedStructs);

            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits a <see cref="StructDeclaration"/>.</summary>
        /// <param name="struct">The <see cref="StructDeclaration"/> to visit.</param>
        protected internal override void VisitStruct(StructDeclaration @struct)
        {
            _WriteStructDefinition(@struct);
            foreach (var member in @struct.Members)
                member.Accept(this);
            foreach (var nestedType in @struct.NestedTypes)
                nestedType.Accept(this);
        }

        private void _WriteStructDefinition(StructDeclaration @struct)
        {
            _jsonWriter.WritePropertyName(_GetIdFor(@struct));

            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("kind");
            _jsonWriter.WriteValue("struct");
            _jsonWriter.WritePropertyName("name");
            _jsonWriter.WriteValue(@struct.Name);
            _jsonWriter.WritePropertyName("namespace");
            _jsonWriter.WriteValue(@struct.Namespace.Name);
            _WriteAccessModifier(@struct.AccessModifier);
            _WriteDeclaringTypeReference(@struct.DeclaringType);
            _WriteAttributes(@struct.Attributes);

            @struct.Summary.Accept(_jsonWriterDocumentationVisitor);
            @struct.Remarks.Accept(_jsonWriterDocumentationVisitor);
            _WriteExamples(@struct.Examples);
            _WriteRelatedMembers(@struct.RelatedMembers);

            _WriteGenericParameters(@struct.GenericParameters);

            _WriteImplementedInterfacesReferences(@struct.ImplementedInterfaces);

            _WriteConstantReferences(@struct.Constants);
            _WriteFieldReferences(@struct.Fields);
            _WriteConstructorReferences(@struct.Constructors);
            _WriteEventReferences(@struct.Events);
            _WritePropertyReferences(@struct.Properties);
            _WriteMethodReferences(@struct.Methods);

            _WriteNestedEnumReferences(@struct.NestedEnums);
            _WriteNestedDelegateReferences(@struct.NestedDelegates);
            _WriteNestedInterfaceReferences(@struct.NestedInterfaces);
            _WriteNestedClasseReferences(@struct.NestedClasses);
            _WriteNestedStructReferences(@struct.NestedStructs);

            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits a <see cref="ConstantDeclaration"/>.</summary>
        /// <param name="constant">The <see cref="ConstantDeclaration"/> to visit.</param>
        protected internal override void VisitConstant(ConstantDeclaration constant)
        {
            _jsonWriter.WritePropertyName(_GetIdFor(constant));

            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("kind");
            _jsonWriter.WriteValue("constant");
            _jsonWriter.WritePropertyName("name");
            _jsonWriter.WriteValue(constant.Name);
            _WriteAccessModifier(constant.AccessModifier);
            _jsonWriter.WritePropertyName("value");
            _jsonWriter.WriteValue(constant.Value);
            _jsonWriter.WritePropertyName("type");
            constant.Type.Accept(_memberReferenceWriter);
            _jsonWriter.WritePropertyName("isShadowing");
            _jsonWriter.WriteValue(constant.IsShadowing);
            _WriteDeclaringTypeReference(constant.DeclaringType);
            _WriteAttributes(constant.Attributes);

            constant.Summary.Accept(_jsonWriterDocumentationVisitor);
            constant.Remarks.Accept(_jsonWriterDocumentationVisitor);
            _WriteExamples(constant.Examples);
            _WriteRelatedMembers(constant.RelatedMembers);

            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits a <see cref="FieldDeclaration"/>.</summary>
        /// <param name="field">The <see cref="FieldDeclaration"/> to visit.</param>
        protected internal override void VisitField(FieldDeclaration field)
        {
            _jsonWriter.WritePropertyName(_GetIdFor(field));

            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("kind");
            _jsonWriter.WriteValue("constant");
            _jsonWriter.WritePropertyName("name");
            _jsonWriter.WriteValue(field.Name);
            _WriteAccessModifier(field.AccessModifier);
            _jsonWriter.WritePropertyName("type");
            field.Type.Accept(_memberReferenceWriter);

            _jsonWriter.WritePropertyName("isShadowing");
            _jsonWriter.WriteValue(field.IsShadowing);
            _jsonWriter.WritePropertyName("isStatic");
            _jsonWriter.WriteValue(field.IsStatic);
            _jsonWriter.WritePropertyName("isReadOnly");
            _jsonWriter.WriteValue(field.IsReadOnly);

            _WriteDeclaringTypeReference(field.DeclaringType);
            _WriteAttributes(field.Attributes);

            field.Summary.Accept(_jsonWriterDocumentationVisitor);
            field.Remarks.Accept(_jsonWriterDocumentationVisitor);
            _WriteExamples(field.Examples);
            _WriteRelatedMembers(field.RelatedMembers);

            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits a <see cref="ConstructorDeclaration"/>.</summary>
        /// <param name="constructor">The <see cref="ConstructorDeclaration"/> to visit.</param>
        protected internal override void VisitConstructor(ConstructorDeclaration constructor)
        {
            _jsonWriter.WritePropertyName(_GetIdFor(constructor));

            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("kind");
            _jsonWriter.WriteValue("constructor");
            _jsonWriter.WritePropertyName("name");
            _jsonWriter.WriteValue(constructor.Name);
            _WriteAccessModifier(constructor.AccessModifier);

            _WriteDeclaringTypeReference(constructor.DeclaringType);
            _WriteAttributes(constructor.Attributes);

            _WriteParameters(constructor.Parameters);

            constructor.Summary.Accept(_jsonWriterDocumentationVisitor);
            constructor.Remarks.Accept(_jsonWriterDocumentationVisitor);
            _WriteExceptions(constructor.Exceptions);
            _WriteExamples(constructor.Examples);
            _WriteRelatedMembers(constructor.RelatedMembers);

            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits a <see cref="EventDeclaration"/>.</summary>
        /// <param name="event">The <see cref="EventDeclaration"/> to visit.</param>
        protected internal override void VisitEvent(EventDeclaration @event)
        {
            _jsonWriter.WritePropertyName(_GetIdFor(@event));

            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("kind");
            _jsonWriter.WriteValue("event");
            _jsonWriter.WritePropertyName("name");
            _jsonWriter.WriteValue(@event.Name);
            _WriteAccessModifier(@event.AccessModifier);
            _jsonWriter.WritePropertyName("type");
            @event.Type.Accept(_memberReferenceWriter);

            _WriteDeclaringTypeReference(@event.DeclaringType);
            _WriteAttributes(@event.Attributes);

            _jsonWriter.WritePropertyName("isShadowing");
            _jsonWriter.WriteValue(@event.IsShadowing);
            _jsonWriter.WritePropertyName("isStatic");
            _jsonWriter.WriteValue(@event.IsStatic);
            _jsonWriter.WritePropertyName("isAbstract");
            _jsonWriter.WriteValue(@event.IsAbstract);
            _jsonWriter.WritePropertyName("isVirtual");
            _jsonWriter.WriteValue(@event.IsVirtual);
            _jsonWriter.WritePropertyName("isOverride");
            _jsonWriter.WriteValue(@event.IsOverride);
            _jsonWriter.WritePropertyName("isSealed");
            _jsonWriter.WriteValue(@event.IsSealed);

            _jsonWriter.WritePropertyName("adder");
            _WriteEventAccessorData(@event.Adder);

            _jsonWriter.WritePropertyName("remover");
            _WriteEventAccessorData(@event.Remover);

            @event.Summary.Accept(_jsonWriterDocumentationVisitor);
            @event.Remarks.Accept(_jsonWriterDocumentationVisitor);
            _WriteExceptions(@event.Exceptions);
            _WriteExamples(@event.Examples);
            _WriteRelatedMembers(@event.RelatedMembers);

            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits a <see cref="PropertyDeclaration"/>.</summary>
        /// <param name="property">The <see cref="PropertyDeclaration"/> to visit.</param>
        protected internal override void VisitProperty(PropertyDeclaration property)
        {
            _jsonWriter.WritePropertyName(_GetIdFor(property));

            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("kind");
            _jsonWriter.WriteValue("property");
            _jsonWriter.WritePropertyName("name");
            _jsonWriter.WriteValue(property.Name);
            _WriteAccessModifier(property.AccessModifier);
            _jsonWriter.WritePropertyName("type");
            property.Type.Accept(_memberReferenceWriter);

            _WriteDeclaringTypeReference(property.DeclaringType);
            _WriteAttributes(property.Attributes);

            _jsonWriter.WritePropertyName("isShadowing");
            _jsonWriter.WriteValue(property.IsShadowing);
            _jsonWriter.WritePropertyName("isStatic");
            _jsonWriter.WriteValue(property.IsStatic);
            _jsonWriter.WritePropertyName("isAbstract");
            _jsonWriter.WriteValue(property.IsAbstract);
            _jsonWriter.WritePropertyName("isVirtual");
            _jsonWriter.WriteValue(property.IsVirtual);
            _jsonWriter.WritePropertyName("isOverride");
            _jsonWriter.WriteValue(property.IsOverride);
            _jsonWriter.WritePropertyName("isSealed");
            _jsonWriter.WriteValue(property.IsSealed);

            _WriteParameters(property.Parameters);

            if (property.Getter != null)
            {
                _jsonWriter.WritePropertyName("getter");
                _WritePropertyAccessorData(property.Getter);
            }

            if (property.Setter != null)
            {
                _jsonWriter.WritePropertyName("setter");
                _WritePropertyAccessorData(property.Setter);
            }

            property.Summary.Accept(_jsonWriterDocumentationVisitor);
            property.Remarks.Accept(_jsonWriterDocumentationVisitor);
            _WriteExceptions(property.Exceptions);
            _WriteExamples(property.Examples);
            _WriteRelatedMembers(property.RelatedMembers);
            property.Value.Accept(_jsonWriterDocumentationVisitor);

            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits a <see cref="MethodDeclaration"/>.</summary>
        /// <param name="method">The <see cref="MethodDeclaration"/> to visit.</param>
        protected internal override void VisitMethod(MethodDeclaration method)
        {
            _jsonWriter.WritePropertyName(_GetIdFor(method));

            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("kind");
            _jsonWriter.WriteValue("method");
            _jsonWriter.WritePropertyName("name");
            _jsonWriter.WriteValue(method.Name);
            _WriteAccessModifier(method.AccessModifier);

            _WriteDeclaringTypeReference(method.DeclaringType);
            _WriteAttributes(method.Attributes);

            _jsonWriter.WritePropertyName("isShadowing");
            _jsonWriter.WriteValue(method.IsShadowing);
            _jsonWriter.WritePropertyName("isStatic");
            _jsonWriter.WriteValue(method.IsStatic);
            _jsonWriter.WritePropertyName("isAbstract");
            _jsonWriter.WriteValue(method.IsAbstract);
            _jsonWriter.WritePropertyName("isVirtual");
            _jsonWriter.WriteValue(method.IsVirtual);
            _jsonWriter.WritePropertyName("isOverride");
            _jsonWriter.WriteValue(method.IsOverride);
            _jsonWriter.WritePropertyName("isSealed");
            _jsonWriter.WriteValue(method.IsSealed);

            _WriteGenericParameters(method.GenericParameters);
            _WriteParameters(method.Parameters);
            _WriteReturn(method.Return);

            method.Summary.Accept(_jsonWriterDocumentationVisitor);
            method.Remarks.Accept(_jsonWriterDocumentationVisitor);
            _WriteExceptions(method.Exceptions);
            _WriteExamples(method.Examples);
            _WriteRelatedMembers(method.RelatedMembers);

            _jsonWriter.WriteEndObject();
        }

        private void _WriteDependencies(IEnumerable<AssemblyReference> dependencies)
        {
            _jsonWriter.WritePropertyName("dependencies");
            _jsonWriter.WriteStartArray();
            foreach (var dependency in dependencies)
                dependency.Accept(_memberReferenceWriter);
            _jsonWriter.WriteEndArray();
        }

        private void _WriteEventAccessorData(EventAccessorData accessorData)
        {
            _jsonWriter.WriteStartObject();
            _WriteAttributes(accessorData.Attributes);
            _WriteReturnAttributes(accessorData.ReturnAttributes);
            _jsonWriter.WriteEndObject();
        }

        private void _WritePropertyAccessorData(PropertyAccessorData accessorData)
        {
            _jsonWriter.WriteStartObject();
            _WriteAccessModifier(accessorData.AccessModifier);
            _WriteAttributes(accessorData.Attributes);
            _WriteReturnAttributes(accessorData.ReturnAttributes);
            _jsonWriter.WriteEndObject();
        }

        private void _WriteImplementedInterfacesReferences(IEnumerable<TypeReference> implementedInterfaces)
        {
            _jsonWriter.WritePropertyName("implementedInterfaces");
            _jsonWriter.WriteStartArray();
            foreach (var implementedInterface in implementedInterfaces)
                implementedInterface.Accept(_memberReferenceWriter);
            _jsonWriter.WriteEndArray();
        }

        private void _WriteDeclaringTypeReference(TypeDeclaration declaringType)
        {
            _jsonWriter.WritePropertyName("declaringType");
            if (declaringType != null)
                _jsonWriter.WriteValue(_GetIdFor(declaringType));
            else
                _jsonWriter.WriteNull();
        }

        private void _WriteConstantReferences(IEnumerable<ConstantDeclaration> constants)
        {
            _jsonWriter.WritePropertyName("constants");
            _jsonWriter.WriteStartArray();
            foreach (var constant in constants)
                _jsonWriter.WriteValue(_GetIdFor(constant));
            _jsonWriter.WriteEndArray();
        }

        private void _WriteFieldReferences(IEnumerable<FieldDeclaration> fields)
        {
            _jsonWriter.WritePropertyName("fields");
            _jsonWriter.WriteStartArray();
            foreach (var field in fields)
                _jsonWriter.WriteValue(_GetIdFor(field));
            _jsonWriter.WriteEndArray();
        }

        private void _WriteConstructorReferences(IEnumerable<ConstructorDeclaration> constructors)
        {
            _jsonWriter.WritePropertyName("constructors");
            _jsonWriter.WriteStartArray();
            foreach (var constructor in constructors)
                _jsonWriter.WriteValue(_GetIdFor(constructor));
            _jsonWriter.WriteEndArray();
        }

        private void _WriteEventReferences(IEnumerable<EventDeclaration> events)
        {
            _jsonWriter.WritePropertyName("events");
            _jsonWriter.WriteStartArray();
            foreach (var @event in events)
                _jsonWriter.WriteValue(_GetIdFor(@event));
            _jsonWriter.WriteEndArray();
        }

        private void _WritePropertyReferences(IEnumerable<PropertyDeclaration> properties)
        {
            _jsonWriter.WritePropertyName("properties");
            _jsonWriter.WriteStartArray();
            foreach (var property in properties)
                _jsonWriter.WriteValue(_GetIdFor(property));
            _jsonWriter.WriteEndArray();
        }

        private void _WriteMethodReferences(IEnumerable<MethodDeclaration> methods)
        {
            _jsonWriter.WritePropertyName("methods");
            _jsonWriter.WriteStartArray();
            foreach (var method in methods)
                _jsonWriter.WriteValue(_GetIdFor(method));
            _jsonWriter.WriteEndArray();
        }

        private void _WriteNestedEnumReferences(IEnumerable<EnumDeclaration> nestedEnums)
        {
            _jsonWriter.WritePropertyName("nestedEnums");
            _jsonWriter.WriteStartArray();
            foreach (var nestedEnum in nestedEnums)
                _jsonWriter.WriteValue(_GetIdFor(nestedEnum));
            _jsonWriter.WriteEndArray();
        }

        private void _WriteNestedDelegateReferences(IEnumerable<DelegateDeclaration> nestedDelegates)
        {
            _jsonWriter.WritePropertyName("nestedDelegates");
            _jsonWriter.WriteStartArray();
            foreach (var nestedDelegate in nestedDelegates)
                _jsonWriter.WriteValue(_GetIdFor(nestedDelegate));
            _jsonWriter.WriteEndArray();
        }

        private void _WriteNestedInterfaceReferences(IEnumerable<InterfaceDeclaration> nestedInterfaces)
        {
            _jsonWriter.WritePropertyName("nestedInterfaces");
            _jsonWriter.WriteStartArray();
            foreach (var nestedInterface in nestedInterfaces)
                _jsonWriter.WriteValue(_GetIdFor(nestedInterface));
            _jsonWriter.WriteEndArray();
        }

        private void _WriteNestedClasseReferences(IEnumerable<ClassDeclaration> nestedClasses)
        {
            _jsonWriter.WritePropertyName("nestedClasses");
            _jsonWriter.WriteStartArray();
            foreach (var nestedClass in nestedClasses)
                _jsonWriter.WriteValue(_GetIdFor(nestedClass));
            _jsonWriter.WriteEndArray();
        }

        private void _WriteNestedStructReferences(IEnumerable<StructDeclaration> nestedStructs)
        {
            _jsonWriter.WritePropertyName("nestedStructs");
            _jsonWriter.WriteStartArray();
            foreach (var nestedStruct in nestedStructs)
                _jsonWriter.WriteValue(_GetIdFor(nestedStruct));
            _jsonWriter.WriteEndArray();
        }

        private void _WriteAccessModifier(AccessModifier accessModifier)
        {
            _jsonWriter.WritePropertyName("accessModifier");
            switch (accessModifier)
            {
                case AccessModifier.Public:
                    _jsonWriter.WriteValue("public");
                    break;

                case AccessModifier.Assembly:
                    _jsonWriter.WriteValue("assembly");
                    break;

                case AccessModifier.Family:
                    _jsonWriter.WriteValue("family");
                    break;

                case AccessModifier.FamilyOrAssembly:
                    _jsonWriter.WriteValue("familyOrAssembly");
                    break;

                case AccessModifier.FamilyAndAssembly:
                    _jsonWriter.WriteValue("familyAndAssembly");
                    break;

                case AccessModifier.Private:
                default:
                    _jsonWriter.WriteValue("private");
                    break;
            }
        }

        private void _WriteAttributes(IEnumerable<AttributeData> attributes)
        {
            _jsonWriter.WritePropertyName("attributes");
            _WriteAttributesArray(attributes);
        }

        private void _WriteReturnAttributes(IEnumerable<AttributeData> attributes)
        {
            _jsonWriter.WritePropertyName("returnAttributes");
            _WriteAttributesArray(attributes);
        }

        private void _WriteAttributesArray(IEnumerable<AttributeData> attributes)
        {
            _jsonWriter.WriteStartArray();
            foreach (var attribute in attributes)
            {
                _jsonWriter.WriteStartObject();

                _jsonWriter.WritePropertyName("type");
                attribute.Type.Accept(_memberReferenceWriter);

                _jsonWriter.WritePropertyName("positionalParameters");
                _jsonWriter.WriteStartArray();
                foreach (var positionalParameter in attribute.PositionalParameters)
                    _WriteAttributeParameter(positionalParameter);
                _jsonWriter.WriteEndArray();

                _jsonWriter.WritePropertyName("namedParameters");
                _jsonWriter.WriteStartArray();
                foreach (var namedParameter in attribute.NamedParameters)
                    _WriteAttributeParameter(namedParameter);
                _jsonWriter.WriteEndArray();

                _jsonWriter.WriteEndObject();
            }
            _jsonWriter.WriteEndArray();
        }

        private void _WriteAttributeParameter(AttributeParameterData parameter)
        {
            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("name");
            _jsonWriter.WriteValue(parameter.Name);

            _jsonWriter.WritePropertyName("value");
            WriteValue(parameter.Value);

            _jsonWriter.WritePropertyName("type");
            parameter.Type.Accept(_memberReferenceWriter);

            _jsonWriter.WriteEndObject();

            void WriteValue(object value)
            {
                if (value == null)
                    _jsonWriter.WriteNull();
                else if (value is Array array)
                {
                    _jsonWriter.WriteStartArray();
                    foreach (var item in array)
                        WriteValue(item);
                    _jsonWriter.WriteEndArray();
                }
                else if (value is TypeReference typeReference)
                    typeReference.Accept(_memberReferenceWriter);
                else
                {
                    var valueType = value.GetType();
                    if (valueType.IsEnum || (Nullable.GetUnderlyingType(valueType)?.IsEnum ?? false))
                        _jsonWriter.WriteValue(value.ToString());
                    else
                        _jsonWriter.WriteValue(value);
                }
            }
        }

        private void _WriteGenericParameters(IEnumerable<GenericParameterData> genericParameters)
        {
            _jsonWriter.WritePropertyName("genericParameters");
            _jsonWriter.WriteStartArray();
            foreach (var genericParameter in genericParameters)
            {
                _jsonWriter.WriteStartObject();

                _jsonWriter.WritePropertyName("name");
                _jsonWriter.WriteValue(genericParameter.Name);
                _jsonWriter.WritePropertyName("isCovariant");
                _jsonWriter.WriteValue(genericParameter.IsCovariant);
                _jsonWriter.WritePropertyName("isContravariant");
                _jsonWriter.WriteValue(genericParameter.IsContravariant);
                _jsonWriter.WritePropertyName("hasReferenceTypeConstraint");
                _jsonWriter.WriteValue(genericParameter.HasReferenceTypeConstraint);
                _jsonWriter.WritePropertyName("hasNonNullableValueTypeConstraint");
                _jsonWriter.WriteValue(genericParameter.HasNonNullableValueTypeConstraint);
                _jsonWriter.WritePropertyName("hasDefaultConstructorConstraint");
                _jsonWriter.WriteValue(genericParameter.HasDefaultConstructorConstraint);
                _jsonWriter.WritePropertyName("typeConstraints");
                _jsonWriter.WriteStartArray();
                foreach (var typeConstraint in genericParameter.TypeConstraints)
                    typeConstraint.Accept(_memberReferenceWriter);
                _jsonWriter.WriteEndArray();
                _WriteDescription(genericParameter.Description);

                _jsonWriter.WriteEndObject();
            }
            _jsonWriter.WriteEndArray();
        }

        private void _WriteParameters(IEnumerable<ParameterData> parameters)
        {
            _jsonWriter.WritePropertyName("parameters");
            _jsonWriter.WriteStartArray();
            foreach (var parameter in parameters)
            {
                _jsonWriter.WriteStartObject();

                _jsonWriter.WritePropertyName("name");
                _jsonWriter.WriteValue(parameter.Name);
                _jsonWriter.WritePropertyName("type");
                parameter.Type.Accept(_memberReferenceWriter);

                _jsonWriter.WritePropertyName("isInputByReference");
                _jsonWriter.WriteValue(parameter.IsInputByReference);
                _jsonWriter.WritePropertyName("isInputOutputByReference");
                _jsonWriter.WriteValue(parameter.IsInputOutputByReference);
                _jsonWriter.WritePropertyName("isOutputByReference");
                _jsonWriter.WriteValue(parameter.IsOutputByReference);

                if (parameter.HasDefaultValue)
                {
                    _jsonWriter.WritePropertyName("defaultValue");
                    _jsonWriter.WriteValue(parameter.DefaultValue);
                }
                _WriteAttributes(parameter.Attributes);
                _WriteDescription(parameter.Description);

                _jsonWriter.WriteEndObject();
            }
            _jsonWriter.WriteEndArray();
        }

        private void _WriteReturn(MethodReturnData @return)
        {
            _jsonWriter.WritePropertyName("return");
            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("type");
            @return.Type.Accept(_memberReferenceWriter);
            _WriteAttributes(@return.Attributes);
            _WriteDescription(@return.Description);

            _jsonWriter.WriteEndObject();
        }

        private void _WriteEnumReferences(IEnumerable<EnumDeclaration> enums)
        {
            _jsonWriter.WritePropertyName("enums");
            _jsonWriter.WriteStartArray();
            foreach (var @enum in enums)
                _jsonWriter.WriteValue(_GetIdFor(@enum));
            _jsonWriter.WriteEndArray();
        }

        private void _WriteDelegateReferences(IEnumerable<DelegateDeclaration> delegates)
        {
            _jsonWriter.WritePropertyName("delegates");
            _jsonWriter.WriteStartArray();
            foreach (var @delegate in delegates)
                _jsonWriter.WriteValue(_GetIdFor(@delegate));
            _jsonWriter.WriteEndArray();
        }

        private void _WriteInterfaceReferences(IEnumerable<InterfaceDeclaration> interfaces)
        {
            _jsonWriter.WritePropertyName("interfaces");
            _jsonWriter.WriteStartArray();
            foreach (var @interface in interfaces)
                _jsonWriter.WriteValue(_GetIdFor(@interface));
            _jsonWriter.WriteEndArray();
        }

        private void _WriteClassReferences(IEnumerable<ClassDeclaration> classes)
        {
            _jsonWriter.WritePropertyName("classes");
            _jsonWriter.WriteStartArray();
            foreach (var @class in classes)
                _jsonWriter.WriteValue(_GetIdFor(@class));
            _jsonWriter.WriteEndArray();
        }

        private void _WriteStructReferences(IEnumerable<StructDeclaration> structs)
        {
            _jsonWriter.WritePropertyName("structs");
            _jsonWriter.WriteStartArray();
            foreach (var @struct in structs)
                _jsonWriter.WriteValue(_GetIdFor(@struct));
            _jsonWriter.WriteEndArray();
        }

        private static string _GetIdFor(EnumDeclaration @enum)
            => _GetIdBuilderFor(@enum).ToString();

        private static string _GetIdFor(DelegateDeclaration @delegate)
            => _GetIdBuilderFor(@delegate).ToString();

        private static string _GetIdFor(InterfaceDeclaration @interface)
            => _GetIdBuilderFor(@interface).ToString();

        private static string _GetIdFor(ClassDeclaration @class)
            => _GetIdBuilderFor(@class).ToString();

        private static string _GetIdFor(StructDeclaration @struct)
            => _GetIdBuilderFor(@struct).ToString();

        private static string _GetIdFor(ConstantDeclaration constant)
            => _GetIdBuilderFor(constant.DeclaringType).Append('.').Append(constant.Name).ToString();

        private static string _GetIdFor(FieldDeclaration field)
            => _GetIdBuilderFor(field.DeclaringType).Append('.').Append(field.Name).ToString();

        private static string _GetIdFor(EventDeclaration @event)
            => _GetIdBuilderFor(@event.DeclaringType).Append('.').Append(@event.Name).ToString();

        private static string _GetIdFor(PropertyDeclaration property)
        {
            var builder = _GetIdBuilderFor(property.DeclaringType).Append('.').Append(property.Name);
            if (property.Parameters.Count > 0)
            {
                builder.Append('(');
                var isFirst = true;
                foreach (var parameter in property.Parameters)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        builder.Append(',');
                    builder.Append(_GetTypeReferenceId(parameter.Type));
                }
                builder.Append(')');
            }
            return builder.ToString();
        }

        private static string _GetIdFor(ConstructorDeclaration constructor)
        {
            var builder = _GetIdBuilderFor(constructor.DeclaringType).Append('.').Append(constructor.DeclaringType.Name);

            if (constructor.Parameters.Count > 0)
            {
                builder.Append('(');
                var isFirst = true;
                foreach (var parameter in constructor.Parameters)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        builder.Append(',');
                    builder.Append(_GetTypeReferenceId(parameter.Type));
                }
                builder.Append(')');
            }
            return builder.ToString();
        }

        private static string _GetIdFor(MethodDeclaration method)
        {
            var builder = _GetIdBuilderFor(method.DeclaringType).Append('.').Append(method.Name);

            if (method.GenericParameters.Count > 0)
                builder.Append("``").Append(method.GenericParameters.Count);

            if (method.Parameters.Count > 0)
            {
                builder.Append('(');
                var isFirst = true;
                foreach (var parameter in method.Parameters)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        builder.Append(',');
                    builder.Append(_GetTypeReferenceId(parameter.Type));
                }
                builder.Append(')');
            }
            return builder.ToString();
        }

        private static string _GetIdFor(TypeDeclaration type)
        {
            switch (type)
            {
                case EnumDeclaration @enum:
                    return _GetIdFor(@enum);

                case DelegateDeclaration @delegate:
                    return _GetIdFor(@delegate);

                case InterfaceDeclaration @interface:
                    return _GetIdFor(@interface);

                case ClassDeclaration @class:
                    return _GetIdFor(@class);

                case StructDeclaration @struct:
                    return _GetIdFor(@struct);

                default:
                    return _GetBaseIdBuilder(type).ToString();
            }
        }

        private static StringBuilder _GetIdBuilderFor(EnumDeclaration @enum)
            => _GetBaseIdBuilder(@enum);

        private static StringBuilder _GetIdBuilderFor(DelegateDeclaration @delegate)
        {
            var idBuilder = _GetBaseIdBuilder(@delegate);
            if (@delegate.GenericParameters.Count > 0)
                idBuilder.Append('`').Append(@delegate.GenericParameters.Count);
            return idBuilder;
        }

        private static StringBuilder _GetIdBuilderFor(InterfaceDeclaration @interface)
        {
            var idBuilder = _GetBaseIdBuilder(@interface);
            if (@interface.GenericParameters.Count > 0)
                idBuilder.Append('`').Append(@interface.GenericParameters.Count);
            return idBuilder;
        }

        private static StringBuilder _GetIdBuilderFor(ClassDeclaration @class)
        {
            var idBuilder = _GetBaseIdBuilder(@class);
            if (@class.GenericParameters.Count > 0)
                idBuilder.Append('`').Append(@class.GenericParameters.Count);
            return idBuilder;
        }

        private static StringBuilder _GetIdBuilderFor(StructDeclaration @struct)
        {
            var idBuilder = _GetBaseIdBuilder(@struct);
            if (@struct.GenericParameters.Count > 0)
                idBuilder.Append('`').Append(@struct.GenericParameters.Count);
            return idBuilder;
        }

        private static StringBuilder _GetIdBuilderFor(TypeDeclaration type)
        {
            switch (type)
            {
                case EnumDeclaration @enum:
                    return _GetIdBuilderFor(@enum);

                case DelegateDeclaration @delegate:
                    return _GetIdBuilderFor(@delegate);

                case InterfaceDeclaration @interface:
                    return _GetIdBuilderFor(@interface);

                case ClassDeclaration @class:
                    return _GetIdBuilderFor(@class);

                case StructDeclaration @struct:
                    return _GetIdBuilderFor(@struct);

                default:
                    return _GetBaseIdBuilder(type);
            }
        }

        private static StringBuilder _GetBaseIdBuilder(TypeDeclaration type)
        {
            var idBuilder = new StringBuilder();

            if (!(type.Namespace is GlobalNamespaceDeclaration))
                idBuilder.Append(type.Namespace.Name);
            var nestingChain = new Stack<TypeDeclaration>();
            nestingChain.Push(type);
            while (type.DeclaringType != null)
            {
                nestingChain.Push(type.DeclaringType);
                type = type.DeclaringType;
            }
            do
            {
                if (idBuilder.Length > 0)
                    idBuilder.Append('.');
                idBuilder.Append(nestingChain.Pop().Name);
            } while (nestingChain.Count > 0);

            return idBuilder;
        }

        private static string _GetTypeReferenceId(BaseTypeReference typeReference)
            => _AppendTypeReferenceId(new StringBuilder(), typeReference).ToString();

        private static StringBuilder _AppendTypeReferenceId(StringBuilder idBuilder, BaseTypeReference typeReference)
        {
            switch (typeReference)
            {
                case VoidTypeReference voidType:
                    return idBuilder.Append("System.Void");

                case DynamicTypeReference dynamicType:
                    return idBuilder.Append("System.Object");

                case TypeReference type:
                    if (type.DeclaringType == null)
                    {
                        if (!string.IsNullOrWhiteSpace(type.Namespace))
                            idBuilder.Append(type.Namespace).Append('.');
                    }
                    else
                        _AppendTypeReferenceId(idBuilder, type.DeclaringType).Append('.');

                    idBuilder.Append(type.Name);
                    if (type.GenericArguments.Count > 0)
                    {
                        idBuilder.Append('<');
                        var isFirst = true;
                        foreach (var genericArgument in type.GenericArguments)
                        {
                            if (isFirst)
                                isFirst = false;
                            else
                                idBuilder.Append(',');
                            _AppendTypeReferenceId(idBuilder, genericArgument);
                        }
                        idBuilder.Append('>');
                    }
                    return idBuilder;

                case PointerTypeReference pointerType:
                    return _AppendTypeReferenceId(idBuilder, pointerType.ReferentType).Append('*');

                case GenericTypeParameterReference genericTypeParameter:
                    return idBuilder.Append('`').Append(
                        (genericTypeParameter.DeclaringType.DeclaringType?.GenericArguments.Count ?? 0)
                        + genericTypeParameter.DeclaringType.GenericArguments.TakeWhile(g => g != genericTypeParameter).Count()
                    );

                case GenericMethodParameterReference genericMethodParameter:
                    return idBuilder.Append("``").Append(
                        genericMethodParameter.DeclaringMethod.GenericArguments.TakeWhile(g => g != genericMethodParameter).Count()
                    );

                case ArrayTypeReference arrayType:
                    return _AppendTypeReferenceId(idBuilder, arrayType.ItemType)
                        .Append('[')
                        .Append(new string(',', arrayType.Rank - 1))
                        .Append(']');

                default:
                    return idBuilder;
            }
        }

        private void _WriteDescription(BlockDescriptionDocumentationElement blockDescription)
        {
            _jsonWriter.WritePropertyName("description");
            _jsonWriter.WriteStartObject();

            _WriteXmlAttributes(blockDescription.XmlAttributes);

            _jsonWriter.WritePropertyName("content");
            _jsonWriter.WriteStartArray();
            blockDescription.Accept(_jsonWriterDocumentationVisitor);
            _jsonWriter.WriteEndArray();

            _jsonWriter.WriteEndObject();
        }

        private void _WriteXmlAttributes(IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _jsonWriter.WritePropertyName("xmlAttributes");
            _jsonWriter.WriteStartObject();
            foreach (var xmlAttribute in xmlAttributes)
            {
                _jsonWriter.WritePropertyName(xmlAttribute.Key);
                _jsonWriter.WriteValue(xmlAttribute.Value);
            }
            _jsonWriter.WriteEndObject();
        }

        private void _WriteRelatedMembers(IEnumerable<MemberReferenceDocumentationElement> relatedMembers)
        {
            _jsonWriter.WritePropertyName("relatedMembers");
            _jsonWriter.WriteStartArray();
            foreach (var relatedMember in relatedMembers)
                relatedMember.Accept(_jsonWriterDocumentationVisitor);
            _jsonWriter.WriteEndArray();
        }

        private void _WriteExamples(IEnumerable<ExampleDocumentationElement> examples)
        {
            _jsonWriter.WritePropertyName("examples");
            _jsonWriter.WriteStartArray();
            foreach (var example in examples)
                example.Accept(_jsonWriterDocumentationVisitor);
            _jsonWriter.WriteEndArray();
        }

        private void _WriteExceptions(IEnumerable<ExceptionData> exceptions)
        {
            _jsonWriter.WritePropertyName("exceptions");
            _jsonWriter.WriteStartArray();
            foreach (var exception in exceptions)
            {
                _jsonWriter.WriteStartObject();

                _jsonWriter.WritePropertyName("type");
                exception.Type.Accept(_memberReferenceWriter);

                _WriteXmlAttributes(exception.Description.XmlAttributes);

                _jsonWriter.WritePropertyName("content");
                _jsonWriter.WriteStartArray();
                exception.Description.Accept(_jsonWriterDocumentationVisitor);
                _jsonWriter.WriteEndArray();

                _jsonWriter.WriteEndObject();
            }
            _jsonWriter.WriteEndArray();
        }
    }
}