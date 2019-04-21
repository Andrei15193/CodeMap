using CodeMap.Elements;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap
{
    /// <summary>Represents a JSON writer documentation visitor for storing the documentation of an Assembly as JSON.</summary>
    public class JsonWriterDocumentationVisitor : DocumentationVisitor, IDisposable
    {
        private readonly JsonWriter _jsonWriter;

        /// <summary>Initializes a new instance of the <see cref="JsonWriterDocumentationVisitor"/> class.</summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write the documentation to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="jsonWriter"/> is <c>null</c>.</exception>
        public JsonWriterDocumentationVisitor(JsonWriter jsonWriter)
        {
            _jsonWriter = jsonWriter ?? throw new ArgumentNullException(nameof(jsonWriter));
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

        /// <summary>Visits an <see cref="AssemblyDocumentationElement"/>.</summary>
        /// <param name="assembly">The <see cref="AssemblyDocumentationElement"/> to visit.</param>
        protected internal override void VisitAssembly(AssemblyDocumentationElement assembly)
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

            assembly.Summary.Accept(this);
            assembly.Remarks.Accept(this);
            _WriteExamples(assembly.Examples);
            _WriteRelatedMembers(assembly.RelatedMembers);

            _jsonWriter.WritePropertyName("definitions");
            _jsonWriter.WriteStartObject();
        }

        /// <summary>Visits an <see cref="AssemblyDocumentationElement"/>.</summary>
        /// <param name="assembly">The <see cref="AssemblyDocumentationElement"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override async Task VisitAssemblyAsync(AssemblyDocumentationElement assembly, CancellationToken cancellationToken)
        {
            await _jsonWriter.WriteStartObjectAsync(cancellationToken);

            await _jsonWriter.WritePropertyNameAsync("name", cancellationToken);
            await _jsonWriter.WriteValueAsync(assembly.Name, cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("version", cancellationToken);
            await _jsonWriter.WriteValueAsync(assembly.Version.ToString(), cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("culture", cancellationToken);
            await _jsonWriter.WriteValueAsync(assembly.Culture, cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("publicKeyToken", cancellationToken);
            await _jsonWriter.WriteValueAsync(assembly.PublicKeyToken, cancellationToken);

            _WriteAttributes(assembly.Attributes);
            _WriteDependencies(assembly.Dependencies);

            await assembly.Summary.AcceptAsync(this, cancellationToken);
            await assembly.Remarks.AcceptAsync(this, cancellationToken);
            await _WriteExamplesAsync(assembly.Examples, cancellationToken);
            await _WriteRelatedMembersAsync(assembly.RelatedMembers, cancellationToken);

            await _jsonWriter.WritePropertyNameAsync("definitions", cancellationToken);
            await _jsonWriter.WriteStartObjectAsync(cancellationToken);
        }

        /// <summary>Visits a <see cref="NamespaceDocumentationElement"/>.</summary>
        /// <param name="namespace">The <see cref="NamespaceDocumentationElement"/> to visit.</param>
        protected internal override void VisitNamespace(NamespaceDocumentationElement @namespace)
        {
            _jsonWriter.WritePropertyName(@namespace.Name);

            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("kind");
            _jsonWriter.WriteValue("namespace");
            _jsonWriter.WritePropertyName("name");
            _jsonWriter.WriteValue(@namespace.Name);

            @namespace.Summary.Accept(this);
            @namespace.Remarks.Accept(this);
            _WriteExamples(@namespace.Examples);
            _WriteRelatedMembers(@namespace.RelatedMembers);

            _WriteEnumReferences(@namespace.Enums);
            _WriteDelegateReferences(@namespace.Delegates);
            _WriteInterfaceReferences(@namespace.Interfaces);
            _WriteClassReferences(@namespace.Classes);
            _WriteStructReferences(@namespace.Structs);

            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits a <see cref="NamespaceDocumentationElement"/>.</summary>
        /// <param name="namespace">The <see cref="NamespaceDocumentationElement"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override async Task VisitNamespaceAsync(NamespaceDocumentationElement @namespace, CancellationToken cancellationToken)
        {
            await _jsonWriter.WritePropertyNameAsync(@namespace.Name, cancellationToken);

            await _jsonWriter.WriteStartObjectAsync(cancellationToken);

            await _jsonWriter.WritePropertyNameAsync("kind", cancellationToken);
            await _jsonWriter.WriteValueAsync("namespace", cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("name", cancellationToken);
            await _jsonWriter.WriteValueAsync(@namespace.Name, cancellationToken);

            await @namespace.Summary.AcceptAsync(this, cancellationToken);
            await @namespace.Remarks.AcceptAsync(this, cancellationToken);
            await _WriteExamplesAsync(@namespace.Examples, cancellationToken);
            await _WriteRelatedMembersAsync(@namespace.RelatedMembers, cancellationToken);

            await _WriteEnumReferencesAsync(@namespace.Enums, cancellationToken);
            await _WriteDelegateReferencesAsync(@namespace.Delegates, cancellationToken);
            await _WriteInterfaceReferencesAsync(@namespace.Interfaces, cancellationToken);
            await _WriteClassReferencesAsync(@namespace.Classes, cancellationToken);
            await _WriteStructReferencesAsync(@namespace.Structs, cancellationToken);

            await _jsonWriter.WriteEndObjectAsync(cancellationToken);
        }

        /// <summary>Visits an <see cref="EnumDocumentationElement"/>.</summary>
        /// <param name="enum">The <see cref="EnumDocumentationElement"/> to visit.</param>
        protected internal override void VisitEnum(EnumDocumentationElement @enum)
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
            _WriteTypeReference(@enum.UnderlyingType);

            _WriteDeclaringTypeReference(@enum.DeclaringType);
            _WriteAttributes(@enum.Attributes);

            @enum.Summary.Accept(this);
            @enum.Remarks.Accept(this);
            _WriteExamples(@enum.Examples);
            _WriteRelatedMembers(@enum.RelatedMembers);

            _jsonWriter.WritePropertyName("members");
            _jsonWriter.WriteStartArray();
            foreach (var member in @enum.Members)
                _jsonWriter.WriteValue(_GetIdFor(member));
            _jsonWriter.WriteEndArray();

            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits an <see cref="EnumDocumentationElement"/>.</summary>
        /// <param name="enum">The <see cref="EnumDocumentationElement"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override async Task VisitEnumAsync(EnumDocumentationElement @enum, CancellationToken cancellationToken)
        {
            await _jsonWriter.WritePropertyNameAsync(_GetIdFor(@enum), cancellationToken);

            await _jsonWriter.WriteStartObjectAsync(cancellationToken);

            await _jsonWriter.WritePropertyNameAsync("kind", cancellationToken);
            await _jsonWriter.WriteValueAsync("enum", cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("name", cancellationToken);
            await _jsonWriter.WriteValueAsync(@enum.Name, cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("namespace", cancellationToken);
            await _jsonWriter.WriteValueAsync(@enum.Namespace.Name, cancellationToken);
            await _WriteAccessModifierAsync(@enum.AccessModifier, cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("underlyingType", cancellationToken);
            await _WriteTypeReferenceAsync(@enum.UnderlyingType, cancellationToken);

            await _WriteDeclaringTypeReferenceAsync(@enum.DeclaringType, cancellationToken);
            await _WriteAttributesAsync(@enum.Attributes, cancellationToken);

            await @enum.Summary.AcceptAsync(this, cancellationToken);
            await @enum.Remarks.AcceptAsync(this, cancellationToken);
            await _WriteExamplesAsync(@enum.Examples, cancellationToken);
            await _WriteRelatedMembersAsync(@enum.RelatedMembers, cancellationToken);

            await _jsonWriter.WritePropertyNameAsync("members", cancellationToken);
            await _jsonWriter.WriteStartArrayAsync(cancellationToken);
            foreach (var member in @enum.Members)
                await _jsonWriter.WriteValueAsync(_GetIdFor(member), cancellationToken);
            await _jsonWriter.WriteEndArrayAsync(cancellationToken);

            await _jsonWriter.WriteEndObjectAsync(cancellationToken);
        }

        /// <summary>Visits a <see cref="DelegateDocumentationElement"/>.</summary>
        /// <param name="delegate">The <see cref="DelegateDocumentationElement"/> to visit.</param>
        protected internal override void VisitDelegate(DelegateDocumentationElement @delegate)
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

            @delegate.Summary.Accept(this);
            _WriteExceptions(@delegate.Exceptions);
            @delegate.Remarks.Accept(this);
            _WriteExamples(@delegate.Examples);
            _WriteRelatedMembers(@delegate.RelatedMembers);

            _WriteGenericParameters(@delegate.GenericParameters);
            _WriteParameters(@delegate.Parameters);
            _WriteReturn(@delegate.Return);

            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits a <see cref="DelegateDocumentationElement"/>.</summary>
        /// <param name="delegate">The <see cref="DelegateDocumentationElement"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override async Task VisitDelegateAsync(DelegateDocumentationElement @delegate, CancellationToken cancellationToken)
        {
            await _jsonWriter.WritePropertyNameAsync(_GetIdFor(@delegate), cancellationToken);

            await _jsonWriter.WriteStartObjectAsync(cancellationToken);

            await _jsonWriter.WritePropertyNameAsync("kind", cancellationToken);
            await _jsonWriter.WriteValueAsync("delegate", cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("name", cancellationToken);
            await _jsonWriter.WriteValueAsync(@delegate.Name, cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("namespace", cancellationToken);
            await _jsonWriter.WriteValueAsync(@delegate.Namespace.Name, cancellationToken);
            await _WriteAccessModifierAsync(@delegate.AccessModifier, cancellationToken);

            await _WriteDeclaringTypeReferenceAsync(@delegate.DeclaringType, cancellationToken);
            await _WriteAttributesAsync(@delegate.Attributes, cancellationToken);

            await @delegate.Summary.AcceptAsync(this, cancellationToken);
            await _WriteExceptionsAsync(@delegate.Exceptions, cancellationToken);
            await @delegate.Remarks.AcceptAsync(this, cancellationToken);
            await _WriteExamplesAsync(@delegate.Examples, cancellationToken);
            await _WriteRelatedMembersAsync(@delegate.RelatedMembers, cancellationToken);

            await _WriteGenericParametersAsync(@delegate.GenericParameters, cancellationToken);
            await _WriteParametersAsync(@delegate.Parameters, cancellationToken);
            await _WriteReturnAsync(@delegate.Return, cancellationToken);

            await _jsonWriter.WriteEndObjectAsync(cancellationToken);
        }

        /// <summary>Visits an <see cref="InterfaceDocumentationElement"/>.</summary>
        /// <param name="interface">The <see cref="InterfaceDocumentationElement"/> to visit.</param>
        protected internal override void VisitInterface(InterfaceDocumentationElement @interface)
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

            @interface.Summary.Accept(this);
            @interface.Remarks.Accept(this);
            _WriteExamples(@interface.Examples);
            _WriteRelatedMembers(@interface.RelatedMembers);

            _WriteGenericParameters(@interface.GenericParameters);

            _jsonWriter.WritePropertyName("baseInterfaces");
            _jsonWriter.WriteStartArray();
            foreach (var baseInterface in @interface.BaseInterfaces)
                _WriteTypeReference(baseInterface);
            _jsonWriter.WriteEndArray();

            _WriteEventReferences(@interface.Events);
            _WritePropertyReferences(@interface.Properties);
            _WriteMethodReferences(@interface.Methods);

            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits an <see cref="InterfaceDocumentationElement"/>.</summary>
        /// <param name="interface">The <see cref="InterfaceDocumentationElement"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override async Task VisitInterfaceAsync(InterfaceDocumentationElement @interface, CancellationToken cancellationToken)
        {
            await _jsonWriter.WritePropertyNameAsync(_GetIdFor(@interface), cancellationToken);

            await _jsonWriter.WriteStartObjectAsync(cancellationToken);

            await _jsonWriter.WritePropertyNameAsync("kind", cancellationToken);
            await _jsonWriter.WriteValueAsync("interface", cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("name", cancellationToken);
            await _jsonWriter.WriteValueAsync(@interface.Name, cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("namespace", cancellationToken);
            await _jsonWriter.WriteValueAsync(@interface.Namespace.Name, cancellationToken);
            await _WriteAccessModifierAsync(@interface.AccessModifier, cancellationToken);

            await _WriteDeclaringTypeReferenceAsync(@interface.DeclaringType, cancellationToken);
            await _WriteAttributesAsync(@interface.Attributes, cancellationToken);

            await @interface.Summary.AcceptAsync(this, cancellationToken);
            await @interface.Remarks.AcceptAsync(this, cancellationToken);
            await _WriteExamplesAsync(@interface.Examples, cancellationToken);
            await _WriteRelatedMembersAsync(@interface.RelatedMembers, cancellationToken);

            await _WriteGenericParametersAsync(@interface.GenericParameters, cancellationToken);

            await _jsonWriter.WritePropertyNameAsync("baseInterfaces", cancellationToken);
            await _jsonWriter.WriteStartArrayAsync(cancellationToken);
            foreach (var baseInterface in @interface.BaseInterfaces)
                await _WriteTypeReferenceAsync(baseInterface, cancellationToken);
            await _jsonWriter.WriteEndArrayAsync(cancellationToken);

            await _WriteEventReferencesAsync(@interface.Events, cancellationToken);
            await _WritePropertyReferencesAsync(@interface.Properties, cancellationToken);
            await _WriteMethodReferencesAsync(@interface.Methods, cancellationToken);

            await _jsonWriter.WriteEndObjectAsync(cancellationToken);
        }

        /// <summary>Visits a <see cref="ClassDocumentationElement"/>.</summary>
        /// <param name="class">The <see cref="ClassDocumentationElement"/> to visit.</param>
        protected internal override void VisitClass(ClassDocumentationElement @class)
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

            @class.Summary.Accept(this);
            @class.Remarks.Accept(this);
            _WriteExamples(@class.Examples);
            _WriteRelatedMembers(@class.RelatedMembers);

            _WriteGenericParameters(@class.GenericParameters);

            _jsonWriter.WritePropertyName("baseClass");
            _WriteTypeReference(@class.BaseClass);
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

        /// <summary>Visits a <see cref="ClassDocumentationElement"/>.</summary>
        /// <param name="class">The <see cref="ClassDocumentationElement"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override async Task VisitClassAsync(ClassDocumentationElement @class, CancellationToken cancellationToken)
        {
            await _jsonWriter.WritePropertyNameAsync(_GetIdFor(@class), cancellationToken);

            await _jsonWriter.WriteStartObjectAsync(cancellationToken);

            await _jsonWriter.WritePropertyNameAsync("kind", cancellationToken);
            await _jsonWriter.WriteValueAsync("class", cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("name", cancellationToken);
            await _jsonWriter.WriteValueAsync(@class.Name, cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("namespace", cancellationToken);
            await _jsonWriter.WriteValueAsync(@class.Namespace.Name, cancellationToken);
            await _WriteAccessModifierAsync(@class.AccessModifier, cancellationToken);

            await _WriteDeclaringTypeReferenceAsync(@class.DeclaringType, cancellationToken);
            await _WriteAttributesAsync(@class.Attributes, cancellationToken);

            await @class.Summary.AcceptAsync(this, cancellationToken);
            await @class.Remarks.AcceptAsync(this, cancellationToken);
            await _WriteExamplesAsync(@class.Examples, cancellationToken);
            await _WriteRelatedMembersAsync(@class.RelatedMembers, cancellationToken);

            await _WriteGenericParametersAsync(@class.GenericParameters, cancellationToken);

            await _jsonWriter.WritePropertyNameAsync("baseClass", cancellationToken);
            await _WriteTypeReferenceAsync(@class.BaseClass, cancellationToken);
            await _WriteImplementedInterfacesReferencesAsync(@class.ImplementedInterfaces, cancellationToken);

            await _jsonWriter.WritePropertyNameAsync("isAbstract", cancellationToken);
            await _jsonWriter.WriteValueAsync(@class.IsAbstract, cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("isSealed", cancellationToken);
            await _jsonWriter.WriteValueAsync(@class.IsSealed, cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("isStatic", cancellationToken);
            await _jsonWriter.WriteValueAsync(@class.IsStatic, cancellationToken);

            await _WriteConstantReferencesAsync(@class.Constants, cancellationToken);
            await _WriteFieldReferencesAsync(@class.Fields, cancellationToken);
            await _WriteConstructorReferencesAsync(@class.Constructors, cancellationToken);
            await _WriteEventReferencesAsync(@class.Events, cancellationToken);
            await _WritePropertyReferencesAsync(@class.Properties, cancellationToken);
            await _WriteMethodReferencesAsync(@class.Methods, cancellationToken);

            await _WriteNestedEnumReferencesAsync(@class.NestedEnums, cancellationToken);
            await _WriteNestedDelegateReferencesAsync(@class.NestedDelegates, cancellationToken);
            await _WriteNestedInterfaceReferencesAsync(@class.NestedInterfaces, cancellationToken);
            await _WriteNestedClasseReferencesAsync(@class.NestedClasses, cancellationToken);
            await _WriteNestedStructReferencesAsync(@class.NestedStructs, cancellationToken);

            await _jsonWriter.WriteEndObjectAsync(cancellationToken);
        }

        /// <summary>Visits a <see cref="StructDocumentationElement"/>.</summary>
        /// <param name="struct">The <see cref="StructDocumentationElement"/> to visit.</param>
        protected internal override void VisitStruct(StructDocumentationElement @struct)
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

            @struct.Summary.Accept(this);
            @struct.Remarks.Accept(this);
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

        /// <summary>Visits a <see cref="StructDocumentationElement"/>.</summary>
        /// <param name="struct">The <see cref="StructDocumentationElement"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override async Task VisitStructAsync(StructDocumentationElement @struct, CancellationToken cancellationToken)
        {
            await _jsonWriter.WritePropertyNameAsync(_GetIdFor(@struct), cancellationToken);

            await _jsonWriter.WriteStartObjectAsync(cancellationToken);

            await _jsonWriter.WritePropertyNameAsync("kind", cancellationToken);
            await _jsonWriter.WriteValueAsync("struct", cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("name", cancellationToken);
            await _jsonWriter.WriteValueAsync(@struct.Name, cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("namespace", cancellationToken);
            await _jsonWriter.WriteValueAsync(@struct.Namespace.Name, cancellationToken);
            await _WriteAccessModifierAsync(@struct.AccessModifier, cancellationToken);
            await _WriteDeclaringTypeReferenceAsync(@struct.DeclaringType, cancellationToken);
            await _WriteAttributesAsync(@struct.Attributes, cancellationToken);

            await @struct.Summary.AcceptAsync(this, cancellationToken);
            await @struct.Remarks.AcceptAsync(this, cancellationToken);
            await _WriteExamplesAsync(@struct.Examples, cancellationToken);
            await _WriteRelatedMembersAsync(@struct.RelatedMembers, cancellationToken);

            await _WriteGenericParametersAsync(@struct.GenericParameters, cancellationToken);

            await _WriteImplementedInterfacesReferencesAsync(@struct.ImplementedInterfaces, cancellationToken);

            await _WriteConstantReferencesAsync(@struct.Constants, cancellationToken);
            await _WriteFieldReferencesAsync(@struct.Fields, cancellationToken);
            await _WriteConstructorReferencesAsync(@struct.Constructors, cancellationToken);
            await _WriteEventReferencesAsync(@struct.Events, cancellationToken);
            await _WritePropertyReferencesAsync(@struct.Properties, cancellationToken);
            await _WriteMethodReferencesAsync(@struct.Methods, cancellationToken);

            await _WriteNestedEnumReferencesAsync(@struct.NestedEnums, cancellationToken);
            await _WriteNestedDelegateReferencesAsync(@struct.NestedDelegates, cancellationToken);
            await _WriteNestedInterfaceReferencesAsync(@struct.NestedInterfaces, cancellationToken);
            await _WriteNestedClasseReferencesAsync(@struct.NestedClasses, cancellationToken);
            await _WriteNestedStructReferencesAsync(@struct.NestedStructs, cancellationToken);

            await _jsonWriter.WriteEndObjectAsync(cancellationToken);
        }

        /// <summary>Visits a <see cref="ConstantDocumentationElement"/>.</summary>
        /// <param name="constant">The <see cref="ConstantDocumentationElement"/> to visit.</param>
        protected internal override void VisitConstant(ConstantDocumentationElement constant)
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
            _WriteTypeReference(constant.Type);
            _jsonWriter.WritePropertyName("isShadowing");
            _jsonWriter.WriteValue(constant.IsShadowing);
            _WriteDeclaringTypeReference(constant.DeclaringType);
            _WriteAttributes(constant.Attributes);

            constant.Summary.Accept(this);
            constant.Remarks.Accept(this);
            _WriteExamples(constant.Examples);
            _WriteRelatedMembers(constant.RelatedMembers);

            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits a <see cref="ConstantDocumentationElement"/>.</summary>
        /// <param name="constant">The <see cref="ConstantDocumentationElement"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override async Task VisitConstantAsync(ConstantDocumentationElement constant, CancellationToken cancellationToken)
        {
            await _jsonWriter.WritePropertyNameAsync(_GetIdFor(constant), cancellationToken);

            await _jsonWriter.WriteStartObjectAsync(cancellationToken);

            await _jsonWriter.WritePropertyNameAsync("kind", cancellationToken);
            await _jsonWriter.WriteValueAsync("constant", cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("name", cancellationToken);
            await _jsonWriter.WriteValueAsync(constant.Name, cancellationToken);
            await _WriteAccessModifierAsync(constant.AccessModifier, cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("value", cancellationToken);
            await _jsonWriter.WriteValueAsync(constant.Value, cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("type", cancellationToken);
            await _WriteTypeReferenceAsync(constant.Type, cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("isShadowing", cancellationToken);
            await _jsonWriter.WriteValueAsync(constant.IsShadowing, cancellationToken);

            await _WriteDeclaringTypeReferenceAsync(constant.DeclaringType, cancellationToken);
            await _WriteAttributesAsync(constant.Attributes, cancellationToken);

            await constant.Summary.AcceptAsync(this, cancellationToken);
            await constant.Remarks.AcceptAsync(this, cancellationToken);
            await _WriteExamplesAsync(constant.Examples, cancellationToken);
            await _WriteRelatedMembersAsync(constant.RelatedMembers, cancellationToken);

            await _jsonWriter.WriteEndObjectAsync(cancellationToken);
        }

        /// <summary>Visits a <see cref="FieldDocumentationElement"/>.</summary>
        /// <param name="field">The <see cref="FieldDocumentationElement"/> to visit.</param>
        protected internal override void VisitField(FieldDocumentationElement field)
        {
            _jsonWriter.WritePropertyName(_GetIdFor(field));

            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("kind");
            _jsonWriter.WriteValue("constant");
            _jsonWriter.WritePropertyName("name");
            _jsonWriter.WriteValue(field.Name);
            _WriteAccessModifier(field.AccessModifier);
            _jsonWriter.WritePropertyName("type");
            _WriteTypeReference(field.Type);

            _jsonWriter.WritePropertyName("isShadowing");
            _jsonWriter.WriteValue(field.IsShadowing);
            _jsonWriter.WritePropertyName("isStatic");
            _jsonWriter.WriteValue(field.IsStatic);
            _jsonWriter.WritePropertyName("isReadOnly");
            _jsonWriter.WriteValue(field.IsReadOnly);

            _WriteDeclaringTypeReference(field.DeclaringType);
            _WriteAttributes(field.Attributes);

            field.Summary.Accept(this);
            field.Remarks.Accept(this);
            _WriteExamples(field.Examples);
            _WriteRelatedMembers(field.RelatedMembers);

            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits a <see cref="FieldDocumentationElement"/>.</summary>
        /// <param name="field">The <see cref="FieldDocumentationElement"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override async Task VisitFieldAsync(FieldDocumentationElement field, CancellationToken cancellationToken)
        {
            await _jsonWriter.WritePropertyNameAsync(_GetIdFor(field), cancellationToken);

            await _jsonWriter.WriteStartObjectAsync(cancellationToken);

            await _jsonWriter.WritePropertyNameAsync("kind", cancellationToken);
            await _jsonWriter.WriteValueAsync("constant", cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("name", cancellationToken);
            await _jsonWriter.WriteValueAsync(field.Name, cancellationToken);
            await _WriteAccessModifierAsync(field.AccessModifier, cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("type", cancellationToken);
            await _WriteTypeReferenceAsync(field.Type, cancellationToken);

            await _jsonWriter.WritePropertyNameAsync("isShadowing", cancellationToken);
            await _jsonWriter.WriteValueAsync(field.IsShadowing, cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("isStatic", cancellationToken);
            await _jsonWriter.WriteValueAsync(field.IsStatic, cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("isReadOnly", cancellationToken);
            await _jsonWriter.WriteValueAsync(field.IsReadOnly, cancellationToken);

            await _WriteDeclaringTypeReferenceAsync(field.DeclaringType, cancellationToken);
            await _WriteAttributesAsync(field.Attributes, cancellationToken);

            await field.Summary.AcceptAsync(this, cancellationToken);
            await field.Remarks.AcceptAsync(this, cancellationToken);
            await _WriteExamplesAsync(field.Examples, cancellationToken);
            await _WriteRelatedMembersAsync(field.RelatedMembers, cancellationToken);

            await _jsonWriter.WriteEndObjectAsync(cancellationToken);
        }

        /// <summary>Visits a <see cref="ConstructorDocumentationElement"/>.</summary>
        /// <param name="constructor">The <see cref="ConstructorDocumentationElement"/> to visit.</param>
        protected internal override void VisitConstructor(ConstructorDocumentationElement constructor)
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

            constructor.Summary.Accept(this);
            constructor.Remarks.Accept(this);
            _WriteExceptions(constructor.Exceptions);
            _WriteExamples(constructor.Examples);
            _WriteRelatedMembers(constructor.RelatedMembers);

            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits a <see cref="ConstructorDocumentationElement"/>.</summary>
        /// <param name="constructor">The <see cref="ConstructorDocumentationElement"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override async Task VisitConstructorAsync(ConstructorDocumentationElement constructor, CancellationToken cancellationToken)
        {
            await _jsonWriter.WritePropertyNameAsync(_GetIdFor(constructor), cancellationToken);

            await _jsonWriter.WriteStartObjectAsync(cancellationToken);

            await _jsonWriter.WritePropertyNameAsync("kind", cancellationToken);
            await _jsonWriter.WriteValueAsync("constructor", cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("name", cancellationToken);
            await _jsonWriter.WriteValueAsync(constructor.Name, cancellationToken);
            await _WriteAccessModifierAsync(constructor.AccessModifier, cancellationToken);

            await _WriteDeclaringTypeReferenceAsync(constructor.DeclaringType, cancellationToken);
            await _WriteAttributesAsync(constructor.Attributes, cancellationToken);

            await _WriteParametersAsync(constructor.Parameters, cancellationToken);

            await constructor.Summary.AcceptAsync(this, cancellationToken);
            await constructor.Remarks.AcceptAsync(this, cancellationToken);
            await _WriteExceptionsAsync(constructor.Exceptions, cancellationToken);
            await _WriteExamplesAsync(constructor.Examples, cancellationToken);
            await _WriteRelatedMembersAsync(constructor.RelatedMembers, cancellationToken);

            await _jsonWriter.WriteEndObjectAsync(cancellationToken);
        }

        /// <summary>Visits a <see cref="EventDocumentationElement"/>.</summary>
        /// <param name="event">The <see cref="EventDocumentationElement"/> to visit.</param>
        protected internal override void VisitEvent(EventDocumentationElement @event)
        {
            _jsonWriter.WritePropertyName(_GetIdFor(@event));

            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("kind");
            _jsonWriter.WriteValue("event");
            _jsonWriter.WritePropertyName("name");
            _jsonWriter.WriteValue(@event.Name);
            _WriteAccessModifier(@event.AccessModifier);
            _jsonWriter.WritePropertyName("type");
            _WriteTypeReference(@event.Type);

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

            @event.Summary.Accept(this);
            @event.Remarks.Accept(this);
            _WriteExceptions(@event.Exceptions);
            _WriteExamples(@event.Examples);
            _WriteRelatedMembers(@event.RelatedMembers);

            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits a <see cref="EventDocumentationElement"/>.</summary>
        /// <param name="event">The <see cref="EventDocumentationElement"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override async Task VisitEventAsync(EventDocumentationElement @event, CancellationToken cancellationToken)
        {
            await _jsonWriter.WritePropertyNameAsync(_GetIdFor(@event), cancellationToken);

            await _jsonWriter.WriteStartObjectAsync(cancellationToken);

            await _jsonWriter.WritePropertyNameAsync("kind", cancellationToken);
            await _jsonWriter.WriteValueAsync("event", cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("name", cancellationToken);
            await _jsonWriter.WriteValueAsync(@event.Name, cancellationToken);
            await _WriteAccessModifierAsync(@event.AccessModifier, cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("type", cancellationToken);
            await _WriteTypeReferenceAsync(@event.Type, cancellationToken);

            await _WriteDeclaringTypeReferenceAsync(@event.DeclaringType, cancellationToken);
            await _WriteAttributesAsync(@event.Attributes, cancellationToken);

            await _jsonWriter.WritePropertyNameAsync("isShadowing", cancellationToken);
            await _jsonWriter.WriteValueAsync(@event.IsShadowing, cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("isStatic", cancellationToken);
            await _jsonWriter.WriteValueAsync(@event.IsStatic, cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("isAbstract", cancellationToken);
            await _jsonWriter.WriteValueAsync(@event.IsAbstract, cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("isVirtual", cancellationToken);
            await _jsonWriter.WriteValueAsync(@event.IsVirtual, cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("isOverride", cancellationToken);
            await _jsonWriter.WriteValueAsync(@event.IsOverride, cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("isSealed", cancellationToken);
            await _jsonWriter.WriteValueAsync(@event.IsSealed, cancellationToken);

            await _jsonWriter.WritePropertyNameAsync("adder", cancellationToken);
            await _WriteEventAccessorDataAsync(@event.Adder, cancellationToken);

            await _jsonWriter.WritePropertyNameAsync("remover", cancellationToken);
            await _WriteEventAccessorDataAsync(@event.Remover, cancellationToken);

            await @event.Summary.AcceptAsync(this, cancellationToken);
            await @event.Remarks.AcceptAsync(this, cancellationToken);
            await _WriteExceptionsAsync(@event.Exceptions, cancellationToken);
            await _WriteExamplesAsync(@event.Examples, cancellationToken);
            await _WriteRelatedMembersAsync(@event.RelatedMembers, cancellationToken);

            await _jsonWriter.WriteEndObjectAsync(cancellationToken);
        }

        /// <summary>Visits a <see cref="PropertyDocumentationElement"/>.</summary>
        /// <param name="property">The <see cref="PropertyDocumentationElement"/> to visit.</param>
        protected internal override void VisitProperty(PropertyDocumentationElement property)
        {
            _jsonWriter.WritePropertyName(_GetIdFor(property));

            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("kind");
            _jsonWriter.WriteValue("property");
            _jsonWriter.WritePropertyName("name");
            _jsonWriter.WriteValue(property.Name);
            _WriteAccessModifier(property.AccessModifier);
            _jsonWriter.WritePropertyName("type");
            _WriteTypeReference(property.Type);

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

            property.Summary.Accept(this);
            property.Remarks.Accept(this);
            _WriteExceptions(property.Exceptions);
            _WriteExamples(property.Examples);
            _WriteRelatedMembers(property.RelatedMembers);
            property.Value.Accept(this);

            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits a <see cref="PropertyDocumentationElement"/>.</summary>
        /// <param name="property">The <see cref="PropertyDocumentationElement"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override async Task VisitPropertyAsync(PropertyDocumentationElement property, CancellationToken cancellationToken)
        {
            await _jsonWriter.WritePropertyNameAsync(_GetIdFor(property), cancellationToken);

            await _jsonWriter.WriteStartObjectAsync(cancellationToken);

            await _jsonWriter.WritePropertyNameAsync("kind", cancellationToken);
            await _jsonWriter.WriteValueAsync("property", cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("name", cancellationToken);
            await _jsonWriter.WriteValueAsync(property.Name, cancellationToken);
            await _WriteAccessModifierAsync(property.AccessModifier, cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("type", cancellationToken);
            await _WriteTypeReferenceAsync(property.Type, cancellationToken);

            await _WriteDeclaringTypeReferenceAsync(property.DeclaringType, cancellationToken);
            await _WriteAttributesAsync(property.Attributes, cancellationToken);

            await _jsonWriter.WritePropertyNameAsync("isShadowing", cancellationToken);
            await _jsonWriter.WriteValueAsync(property.IsShadowing, cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("isStatic", cancellationToken);
            await _jsonWriter.WriteValueAsync(property.IsStatic, cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("isAbstract", cancellationToken);
            await _jsonWriter.WriteValueAsync(property.IsAbstract, cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("isVirtual", cancellationToken);
            await _jsonWriter.WriteValueAsync(property.IsVirtual, cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("isOverride", cancellationToken);
            await _jsonWriter.WriteValueAsync(property.IsOverride, cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("isSealed", cancellationToken);
            await _jsonWriter.WriteValueAsync(property.IsSealed, cancellationToken);

            await _WriteParametersAsync(property.Parameters, cancellationToken);

            if (property.Getter != null)
            {
                await _jsonWriter.WritePropertyNameAsync("getter", cancellationToken);
                await _WritePropertyAccessorDataAsync(property.Getter, cancellationToken);
            }

            if (property.Setter != null)
            {
                await _jsonWriter.WritePropertyNameAsync("setter", cancellationToken);
                await _WritePropertyAccessorDataAsync(property.Setter, cancellationToken);
            }

            await property.Summary.AcceptAsync(this, cancellationToken);
            await property.Remarks.AcceptAsync(this, cancellationToken);
            await _WriteExceptionsAsync(property.Exceptions, cancellationToken);
            await _WriteExamplesAsync(property.Examples, cancellationToken);
            await _WriteRelatedMembersAsync(property.RelatedMembers, cancellationToken);
            await property.Value.AcceptAsync(this, cancellationToken);

            await _jsonWriter.WriteEndObjectAsync(cancellationToken);
        }

        /// <summary>Visits a <see cref="MethodDocumentationElement"/>.</summary>
        /// <param name="method">The <see cref="MethodDocumentationElement"/> to visit.</param>
        protected internal override void VisitMethod(MethodDocumentationElement method)
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

            method.Summary.Accept(this);
            method.Remarks.Accept(this);
            _WriteExceptions(method.Exceptions);
            _WriteExamples(method.Examples);
            _WriteRelatedMembers(method.RelatedMembers);

            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits a <see cref="MethodDocumentationElement"/>.</summary>
        /// <param name="method">The <see cref="MethodDocumentationElement"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override async Task VisitMethodAsync(MethodDocumentationElement method, CancellationToken cancellationToken)
        {
            await _jsonWriter.WritePropertyNameAsync(_GetIdFor(method), cancellationToken);

            await _jsonWriter.WriteStartObjectAsync(cancellationToken);

            await _jsonWriter.WritePropertyNameAsync("kind", cancellationToken);
            await _jsonWriter.WriteValueAsync("method", cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("name", cancellationToken);
            await _jsonWriter.WriteValueAsync(method.Name, cancellationToken);
            await _WriteAccessModifierAsync(method.AccessModifier, cancellationToken);

            await _WriteDeclaringTypeReferenceAsync(method.DeclaringType, cancellationToken);
            await _WriteAttributesAsync(method.Attributes, cancellationToken);

            await _jsonWriter.WritePropertyNameAsync("isShadowing", cancellationToken);
            await _jsonWriter.WriteValueAsync(method.IsShadowing, cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("isStatic", cancellationToken);
            await _jsonWriter.WriteValueAsync(method.IsStatic, cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("isAbstract", cancellationToken);
            await _jsonWriter.WriteValueAsync(method.IsAbstract, cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("isVirtual", cancellationToken);
            await _jsonWriter.WriteValueAsync(method.IsVirtual, cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("isOverride", cancellationToken);
            await _jsonWriter.WriteValueAsync(method.IsOverride, cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("isSealed", cancellationToken);
            await _jsonWriter.WriteValueAsync(method.IsSealed, cancellationToken);

            await _WriteGenericParametersAsync(method.GenericParameters, cancellationToken);
            await _WriteParametersAsync(method.Parameters, cancellationToken);
            await _WriteReturnAsync(method.Return, cancellationToken);

            await method.Summary.AcceptAsync(this, cancellationToken);
            await method.Remarks.AcceptAsync(this, cancellationToken);
            await _WriteExceptionsAsync(method.Exceptions, cancellationToken);
            await _WriteExamplesAsync(method.Examples, cancellationToken);
            await _WriteRelatedMembersAsync(method.RelatedMembers, cancellationToken);

            await _jsonWriter.WriteEndObjectAsync(cancellationToken);
        }

        /// <summary>Visits the beginning of a summary element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>summary</c> element.</param>
        protected internal override void VisitSummaryBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _jsonWriter.WritePropertyName("summary");
            _jsonWriter.WriteStartObject();

            _WriteXmlAttributes(xmlAttributes);

            _jsonWriter.WritePropertyName("content");
            _jsonWriter.WriteStartArray();
        }

        /// <summary>Visits the beginning of a summary element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>summary</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override async Task VisitSummaryBeginningAsync(IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            await _jsonWriter.WritePropertyNameAsync("summary", cancellationToken);
            await _jsonWriter.WriteStartObjectAsync(cancellationToken);

            await _WriteXmlAttributesAsync(xmlAttributes, cancellationToken);

            await _jsonWriter.WritePropertyNameAsync("content", cancellationToken);
            await _jsonWriter.WriteStartArrayAsync(cancellationToken);
        }

        /// <summary>Visits the ending of a summary element.</summary>
        protected internal override void VisitSummaryEnding()
        {
            _jsonWriter.WriteEndArray();
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits the ending of a summary element.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override async Task VisitSummaryEndingAsync(CancellationToken cancellationToken)
        {
            await _jsonWriter.WriteEndArrayAsync(cancellationToken);
            await _jsonWriter.WriteEndObjectAsync(cancellationToken);
        }

        /// <summary>Visits the beginning of a remarks element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>remarks</c> element.</param>
        protected internal override void VisitRemarksBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _jsonWriter.WritePropertyName("remarks");
            _jsonWriter.WriteStartObject();

            _WriteXmlAttributes(xmlAttributes);

            _jsonWriter.WritePropertyName("content");
            _jsonWriter.WriteStartArray();
        }

        /// <summary>Visits the beginning of a remarks element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>remarks</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override async Task VisitRemarksBeginningAsync(IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            await _jsonWriter.WritePropertyNameAsync("remarks", cancellationToken);
            await _jsonWriter.WriteStartObjectAsync(cancellationToken);

            await _WriteXmlAttributesAsync(xmlAttributes, cancellationToken);

            await _jsonWriter.WritePropertyNameAsync("content", cancellationToken);
            await _jsonWriter.WriteStartArrayAsync(cancellationToken);
        }

        /// <summary>Visits the ending of a remarks element.</summary>
        protected internal override void VisitRemarksEnding()
        {
            _jsonWriter.WriteEndArray();
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits the ending of a remarks element.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override async Task VisitRemarksEndingAsync(CancellationToken cancellationToken)
        {
            await _jsonWriter.WriteEndArrayAsync(cancellationToken);
            await _jsonWriter.WriteEndObjectAsync(cancellationToken);
        }

        /// <summary>Visits the beginning of an example element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>example</c> element.</param>
        protected internal override void VisitExampleBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
        }


        /// <summary>Visits the beginning of an example element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>example</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitExampleBeginningAsync(IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            try
            {
                VisitExampleBeginning(xmlAttributes);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the ending of an example element.</summary>
        protected internal override void VisitExampleEnding()
        {
        }

        /// <summary>Visits the ending of an example element.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitExampleEndingAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitExampleEnding();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the beginning of a value element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>value</c> element.</param>
        protected internal override void VisitValueBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
        }

        /// <summary>Visits the beginning of a value element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>value</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitValueBeginningAsync(IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            try
            {
                VisitValueBeginning(xmlAttributes);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the ending of a value element.</summary>
        protected internal override void VisitValueEnding()
        {
        }

        /// <summary>Visits the ending of a value element.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitValueEndingAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitValueEnding();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the beginning of a paragraph element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>para</c> element.</param>
        protected internal override void VisitParagraphBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
        }

        /// <summary>Visits the beginning of a paragraph element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>para</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitParagraphBeginningAsync(IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            try
            {
                VisitParagraphBeginning(xmlAttributes);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the ending of a paragraph element.</summary>
        protected internal override void VisitParagraphEnding()
        {
        }

        /// <summary>Visits the ending of a paragraph element.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitParagraphEndingAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitParagraphEnding();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits a code block element.</summary>
        /// <param name="code">The text inside the code block.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>code</c> element.</param>
        protected internal override void VisitCodeBlock(string code, IReadOnlyDictionary<string, string> xmlAttributes)
        {
        }

        /// <summary>Visits a code block element.</summary>
        /// <param name="code">The text inside the code block.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>code</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitCodeBlockAsync(string code, IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            try
            {
                VisitCodeBlock(code, xmlAttributes);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the beginning of a related members list.</summary>
        protected internal override void VisitRelatedMembersListBeginning()
        {
        }

        /// <summary>Visits the beginning of a related members list.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitRelatedMembersListBeginningAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitRelatedMembersListBeginning();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the ending of a related members list.</summary>
        protected internal override void VisitRelatedMembersListEnding()
        {
        }

        /// <summary>Visits the ending of a related members list.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitRelatedMembersListEndingAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitRelatedMembersListEnding();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits a related member element.</summary>
        /// <param name="relatedMember">The related member reference.</param>
        protected internal override void VisitRelatedMember(MemberReferenceDocumentationElement relatedMember)
        {
            relatedMember.Accept(this);
        }

        /// <summary>Visits a related member element.</summary>
        /// <param name="relatedMember">The related member reference.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitRelatedMemberAsync(MemberReferenceDocumentationElement relatedMember, CancellationToken cancellationToken)
        {
            try
            {
                VisitRelatedMember(relatedMember);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the beginning of an unordered list element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>list</c> element.</param>
        protected internal override void VisitUnorderedListBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
        }

        /// <summary>Visits the beginning of an unordered list element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>list</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitUnorderedListBeginningAsync(IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            try
            {
                VisitUnorderedListBeginning(xmlAttributes);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the ending of an unordered list element.</summary>
        protected internal override void VisitUnorderedListEnding()
        {
        }

        /// <summary>Visits the ending of an unordered list element.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitUnorderedListEndingAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitUnorderedListEnding();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the beginning of an ordered list element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>list</c> element.</param>
        protected internal override void VisitOrderedListBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
        }

        /// <summary>Visits the beginning of an ordered list element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>list</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitOrderedListBeginningAsync(IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            try
            {
                VisitOrderedListBeginning(xmlAttributes);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the ending of an ordered list element.</summary>
        protected internal override void VisitOrderedListEnding()
        {
        }

        /// <summary>Visits the ending of an ordered list element.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitOrderedListEndingAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitOrderedListEnding();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the beginning of a list item element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>item</c> or <c>description</c> element.</param>
        protected internal override void VisitListItemBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
        }

        /// <summary>Visits the beginning of a list item element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>item</c> or <c>description</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitListItemBeginningAsync(IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            try
            {
                VisitListItemBeginning(xmlAttributes);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the ending of a list item element.</summary>
        protected internal override void VisitListItemEnding()
        {
        }

        /// <summary>Visits the ending of a list item element.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitListItemEndingAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitListItemEnding();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the beginning of a definition list element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>list</c> element.</param>
        protected internal override void VisitDefinitionListBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
        }

        /// <summary>Visits the beginning of a definition list element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>list</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitDefinitionListBeginningAsync(IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            try
            {
                VisitDefinitionListBeginning(xmlAttributes);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the ending of a definition list element.</summary>
        protected internal override void VisitDefinitionListEnding()
        {
        }

        /// <summary>Visits the ending of a definition list element.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitDefinitionListEndingAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitDefinitionListEnding();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the beginning of a definition list title.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>listheader</c> element.</param>
        protected internal override void VisitDefinitionListTitleBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
        }

        /// <summary>Visits the beginning of a definition list title.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>listheader</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitDefinitionListTitleBeginningAsync(IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            try
            {
                VisitDefinitionListTitleBeginning(xmlAttributes);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the ending of a definition list title.</summary>
        protected internal override void VisitDefinitionListTitleEnding()
        {
        }

        /// <summary>Visits the ending of a definition list title.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitDefinitionListTitleEndingAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitDefinitionListTitleEnding();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the beginning of a definition list item.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>item</c> element.</param>
        protected internal override void VisitDefinitionListItemBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
        }

        /// <summary>Visits the beginning of a definition list item.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>item</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitDefinitionListItemBeginningAsync(IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            try
            {
                VisitDefinitionListItemBeginning(xmlAttributes);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the ending of a definition list item.</summary>
        protected internal override void VisitDefinitionListItemEnding()
        {
        }

        /// <summary>Visits the ending of a definition list item.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitDefinitionListItemEndingAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitDefinitionListItemEnding();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the beginning of a definition list term.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>term</c> element.</param>
        protected internal override void VisitDefinitionTermBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
        }

        /// <summary>Visits the beginning of a definition list term.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>term</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitDefinitionTermBeginningAsync(IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            try
            {
                VisitDefinitionTermBeginning(xmlAttributes);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the ending of a definition list term.</summary>
        protected internal override void VisitDefinitionTermEnding()
        {
        }

        /// <summary>Visits the ending of a definition list term.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitDefinitionTermEndingAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitDefinitionTermEnding();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the beginning of a definition list term description.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>description</c> element.</param>
        protected internal override void VisitDefinitionTermDescriptionBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
        }

        /// <summary>Visits the beginning of a definition list term description.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>description</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitDefinitionTermDescriptionBeginningAsync(IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            try
            {
                VisitDefinitionTermDescriptionBeginning(xmlAttributes);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the ending of a definition list term description.</summary>
        protected internal override void VisitDefinitionTermDescriptionEnding()
        {
        }

        /// <summary>Visits the ending of a definition list term description.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitDefinitionTermDescriptionEndingAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitDefinitionTermDescriptionEnding();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the beginning of a table.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>list</c> element.</param>
        protected internal override void VisitTableBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
        }

        /// <summary>Visits the beginning of a table.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>list</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitTableBeginningAsync(IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            try
            {
                VisitTableBeginning(xmlAttributes);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the ending of a table.</summary>
        protected internal override void VisitTableEnding()
        {
        }

        /// <summary>Visits the ending of a table.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitTableEndingAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitTableEnding();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the beginning of a table heading.</summary>
        protected internal override void VisitTableHeadingBeginning()
        {
        }

        /// <summary>Visits the beginning of a table heading.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitTableHeadingBeginningAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitTableHeadingBeginning();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the ending of a table heading.</summary>
        protected internal override void VisitTableHeadingEnding()
        {
        }

        /// <summary>Visits the ending of a table heading.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitTableHeadingEndingAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitTableHeadingEnding();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the beginning of a table body.</summary>
        protected internal override void VisitTableBodyBeginning()
        {
        }

        /// <summary>Visits the beginning of a table body.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitTableBodyBeginningAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitTableBodyBeginning();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the ending of a table body.</summary>
        protected internal override void VisitTableBodyEnding()
        {
        }

        /// <summary>Visits the ending of a table body.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitTableBodyEndingAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitTableBodyEnding();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the beginning of a table column.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>term</c> element.</param>
        protected internal override void VisitTableColumnBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
        }

        /// <summary>Visits the beginning of a table column.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>term</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitTableColumnBeginningAsync(IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            try
            {
                VisitTableColumnBeginning(xmlAttributes);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the ending of a table column.</summary>
        protected internal override void VisitTableColumnEnding()
        {
        }

        /// <summary>Visits the ending of a table column.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitTableColumnEndingAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitTableColumnEnding();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the beginning of a table row.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>item</c> element.</param>
        protected internal override void VisitTableRowBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
        }

        /// <summary>Visits the beginning of a table row.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>item</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitTableRowBeginningAsync(IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            try
            {
                VisitTableRowBeginning(xmlAttributes);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the ending of a table row.</summary>
        protected internal override void VisitTableRowEnding()
        {
        }

        /// <summary>Visits the ending of a table row.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitTableRowEndingAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitTableRowEnding();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the beginning of a table cell.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>description</c> element.</param>
        protected internal override void VisitTableCellBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
        }

        /// <summary>Visits the beginning of a table cell.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>description</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitTableCellBeginningAsync(IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            try
            {
                VisitTableCellBeginning(xmlAttributes);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the ending of a table cell.</summary>
        protected internal override void VisitTableCellEnding()
        {
        }

        /// <summary>Visits the ending of a table cell.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitTableCellEndingAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitTableCellEnding();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits plain text.</summary>
        /// <param name="text">The plain text inside a block element.</param>
        protected internal override void VisitText(string text)
        {
        }

        /// <summary>Visits plain text.</summary>
        /// <param name="text">The plain text inside a block element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitTextAsync(string text, CancellationToken cancellationToken)
        {
            try
            {
                VisitText(text);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits an inline member reference.</summary>
        /// <param name="canonicalName">The canonical name of the referred member.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>see</c> or <c>seealso</c> element.</param>
        protected internal override void VisitInlineReference(string canonicalName, IReadOnlyDictionary<string, string> xmlAttributes)
        {
            throw new InvalidOperationException($"Could not find member from '{canonicalName}' canonical name. Override VisitInlineReference(string) or VisitInlineReferenceAsync(string, CancellationToken) to ignore this error.");
        }

        /// <summary>Visits an inline member reference.</summary>
        /// <param name="canonicalName">The canonical name of the referred member.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>see</c> or <c>seealso</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitInlineReferenceAsync(string canonicalName, IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            try
            {
                VisitInlineReference(canonicalName, xmlAttributes);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits an inline member reference.</summary>
        /// <param name="referredMember">The referred member.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>see</c> or <c>seealso</c> element.</param>
        protected internal override void VisitInlineReference(MemberInfo referredMember, IReadOnlyDictionary<string, string> xmlAttributes)
        {
        }

        /// <summary>Visits an inline member reference.</summary>
        /// <param name="referredMember">The referred member.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>see</c> or <c>seealso</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitInlineReferenceAsync(MemberInfo referredMember, IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            try
            {
                VisitInlineReference(referredMember, xmlAttributes);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits an inline code snippet.</summary>
        /// <param name="code">The text inside the inline code.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>c</c> element.</param>
        protected internal override void VisitInlineCode(string code, IReadOnlyDictionary<string, string> xmlAttributes)
        {
        }

        /// <summary>Visits an inline code snippet.</summary>
        /// <param name="code">The text inside the inline code.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>c</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitInlineCodeAsync(string code, IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            try
            {
                VisitInlineCode(code, xmlAttributes);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits an inline parameter reference.</summary>
        /// <param name="parameterName">The name of the referred parameter.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>paramref</c> element.</param>
        protected internal override void VisitParameterReference(string parameterName, IReadOnlyDictionary<string, string> xmlAttributes)
        {
        }

        /// <summary>Visits an inline parameter reference.</summary>
        /// <param name="parameterName">The name of the referred parameter.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>paramref</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitParameterReferenceAsync(string parameterName, IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            try
            {
                VisitParameterReference(parameterName, xmlAttributes);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits an inline generic parameter reference.</summary>
        /// <param name="genericParameterName">The name of the referred generic parameter.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>typeparamref</c> element.</param>
        protected internal override void VisitGenericParameterReference(string genericParameterName, IReadOnlyDictionary<string, string> xmlAttributes)
        {
        }

        /// <summary>Visits an inline generic parameter reference.</summary>
        /// <param name="genericParameterName">The name of the referred generic parameter.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>typeparamref</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitGenericParameterReferenceAsync(string genericParameterName, IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            try
            {
                VisitGenericParameterReference(genericParameterName, xmlAttributes);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        private void _WriteEventAccessorData(EventAccessorData accessorData)
        {
            _jsonWriter.WriteStartObject();
            _WriteAttributes(accessorData.Attributes);
            _WriteReturnAttributes(accessorData.ReturnAttributes);
            _jsonWriter.WriteEndObject();
        }

        private async Task _WriteEventAccessorDataAsync(EventAccessorData accessorData, CancellationToken cancellationToken)
        {
            await _jsonWriter.WriteStartObjectAsync(cancellationToken);
            await _WriteAttributesAsync(accessorData.Attributes, cancellationToken);
            await _WriteReturnAttributesAsync(accessorData.ReturnAttributes, cancellationToken);
            await _jsonWriter.WriteEndObjectAsync(cancellationToken);
        }

        private void _WritePropertyAccessorData(PropertyAccessorData accessorData)
        {
            _jsonWriter.WriteStartObject();
            _WriteAccessModifier(accessorData.AccessModifier);
            _WriteAttributes(accessorData.Attributes);
            _WriteReturnAttributes(accessorData.ReturnAttributes);
            _jsonWriter.WriteEndObject();
        }

        private async Task _WritePropertyAccessorDataAsync(PropertyAccessorData accessorData, CancellationToken cancellationToken)
        {
            await _jsonWriter.WriteStartObjectAsync(cancellationToken);
            await _WriteAccessModifierAsync(accessorData.AccessModifier, cancellationToken);
            await _WriteAttributesAsync(accessorData.Attributes, cancellationToken);
            await _WriteReturnAttributesAsync(accessorData.ReturnAttributes, cancellationToken);
            await _jsonWriter.WriteEndObjectAsync(cancellationToken);
        }

        private void _WriteImplementedInterfacesReferences(IEnumerable<TypeReferenceData> implementedInterfaces)
        {
            _jsonWriter.WritePropertyName("implementedInterfaces");
            _jsonWriter.WriteStartArray();
            foreach (var implementedInterface in implementedInterfaces)
                _WriteTypeReference(implementedInterface);
            _jsonWriter.WriteEndArray();
        }

        private async Task _WriteImplementedInterfacesReferencesAsync(IEnumerable<TypeReferenceData> implementedInterfaces, CancellationToken cancellationToken)
        {
            await _jsonWriter.WritePropertyNameAsync("implementedInterfaces", cancellationToken);
            await _jsonWriter.WriteStartArrayAsync(cancellationToken);
            foreach (var implementedInterface in implementedInterfaces)
                await _WriteTypeReferenceAsync(implementedInterface, cancellationToken);
            await _jsonWriter.WriteEndArrayAsync(cancellationToken);
        }

        private void _WriteDeclaringTypeReference(TypeDocumentationElement declaringType)
        {
            _jsonWriter.WritePropertyName("declaringType");
            if (declaringType != null)
                _jsonWriter.WriteValue(_GetIdFor(declaringType));
            else
                _jsonWriter.WriteNull();
        }

        private async Task _WriteDeclaringTypeReferenceAsync(TypeDocumentationElement declaringType, CancellationToken cancellationToken)
        {
            await _jsonWriter.WritePropertyNameAsync("declaringType", cancellationToken);
            if (declaringType != null)
                await _jsonWriter.WriteValueAsync(_GetIdFor(declaringType), cancellationToken);
            else
                await _jsonWriter.WriteNullAsync(cancellationToken);
        }

        private void _WriteConstantReferences(IEnumerable<ConstantDocumentationElement> constants)
        {
            _jsonWriter.WritePropertyName("constants");
            _jsonWriter.WriteStartArray();
            foreach (var constant in constants)
                _jsonWriter.WriteValue(_GetIdFor(constant));
            _jsonWriter.WriteEndArray();
        }

        private async Task _WriteConstantReferencesAsync(IEnumerable<ConstantDocumentationElement> constants, CancellationToken cancellationToken)
        {
            await _jsonWriter.WritePropertyNameAsync("constants", cancellationToken);
            await _jsonWriter.WriteStartArrayAsync(cancellationToken);
            foreach (var constant in constants)
                await _jsonWriter.WriteValueAsync(_GetIdFor(constant), cancellationToken);
            await _jsonWriter.WriteEndArrayAsync(cancellationToken);
        }

        private void _WriteFieldReferences(IEnumerable<FieldDocumentationElement> fields)
        {
            _jsonWriter.WritePropertyName("fields");
            _jsonWriter.WriteStartArray();
            foreach (var field in fields)
                _jsonWriter.WriteValue(_GetIdFor(field));
            _jsonWriter.WriteEndArray();
        }

        private async Task _WriteFieldReferencesAsync(IEnumerable<FieldDocumentationElement> fields, CancellationToken cancellationToken)
        {
            await _jsonWriter.WritePropertyNameAsync("fields", cancellationToken);
            await _jsonWriter.WriteStartArrayAsync(cancellationToken);
            foreach (var field in fields)
                await _jsonWriter.WriteValueAsync(_GetIdFor(field), cancellationToken);
            await _jsonWriter.WriteEndArrayAsync(cancellationToken);
        }

        private void _WriteConstructorReferences(IEnumerable<ConstructorDocumentationElement> constructors)
        {
            _jsonWriter.WritePropertyName("constructors");
            _jsonWriter.WriteStartArray();
            foreach (var constructor in constructors)
                _jsonWriter.WriteValue(_GetIdFor(constructor));
            _jsonWriter.WriteEndArray();
        }

        private async Task _WriteConstructorReferencesAsync(IEnumerable<ConstructorDocumentationElement> constructors, CancellationToken cancellationToken)
        {
            await _jsonWriter.WritePropertyNameAsync("constructors", cancellationToken);
            await _jsonWriter.WriteStartArrayAsync(cancellationToken);
            foreach (var constructor in constructors)
                await _jsonWriter.WriteValueAsync(_GetIdFor(constructor), cancellationToken);
            await _jsonWriter.WriteEndArrayAsync(cancellationToken);
        }

        private void _WriteEventReferences(IEnumerable<EventDocumentationElement> events)
        {
            _jsonWriter.WritePropertyName("events");
            _jsonWriter.WriteStartArray();
            foreach (var @event in events)
                _jsonWriter.WriteValue(_GetIdFor(@event));
            _jsonWriter.WriteEndArray();
        }

        private async Task _WriteEventReferencesAsync(IEnumerable<EventDocumentationElement> events, CancellationToken cancellationToken)
        {
            await _jsonWriter.WritePropertyNameAsync("events", cancellationToken);
            await _jsonWriter.WriteStartArrayAsync(cancellationToken);
            foreach (var @event in events)
                await _jsonWriter.WriteValueAsync(_GetIdFor(@event), cancellationToken);
            await _jsonWriter.WriteEndArrayAsync(cancellationToken);
        }

        private void _WritePropertyReferences(IEnumerable<PropertyDocumentationElement> properties)
        {
            _jsonWriter.WritePropertyName("properties");
            _jsonWriter.WriteStartArray();
            foreach (var property in properties)
                _jsonWriter.WriteValue(_GetIdFor(property));
            _jsonWriter.WriteEndArray();
        }

        private async Task _WritePropertyReferencesAsync(IEnumerable<PropertyDocumentationElement> properties, CancellationToken cancellationToken)
        {
            await _jsonWriter.WritePropertyNameAsync("properties", cancellationToken);
            await _jsonWriter.WriteStartArrayAsync(cancellationToken);
            foreach (var property in properties)
                await _jsonWriter.WriteValueAsync(_GetIdFor(property), cancellationToken);
            await _jsonWriter.WriteEndArrayAsync(cancellationToken);
        }

        private void _WriteMethodReferences(IEnumerable<MethodDocumentationElement> methods)
        {
            _jsonWriter.WritePropertyName("methods");
            _jsonWriter.WriteStartArray();
            foreach (var method in methods)
                _jsonWriter.WriteValue(_GetIdFor(method));
            _jsonWriter.WriteEndArray();
        }

        private async Task _WriteMethodReferencesAsync(IEnumerable<MethodDocumentationElement> methods, CancellationToken cancellationToken)
        {
            await _jsonWriter.WritePropertyNameAsync("methods", cancellationToken);
            await _jsonWriter.WriteStartArrayAsync(cancellationToken);
            foreach (var method in methods)
                await _jsonWriter.WriteValueAsync(_GetIdFor(method), cancellationToken);
            await _jsonWriter.WriteEndArrayAsync(cancellationToken);
        }

        private void _WriteNestedEnumReferences(IEnumerable<EnumDocumentationElement> nestedEnums)
        {
            _jsonWriter.WritePropertyName("nestedEnums");
            _jsonWriter.WriteStartArray();
            foreach (var nestedEnum in nestedEnums)
                _jsonWriter.WriteValue(_GetIdFor(nestedEnum));
            _jsonWriter.WriteEndArray();
        }

        private async Task _WriteNestedEnumReferencesAsync(IEnumerable<EnumDocumentationElement> nestedEnums, CancellationToken cancellationToken)
        {
            await _jsonWriter.WritePropertyNameAsync("nestedEnums", cancellationToken);
            await _jsonWriter.WriteStartArrayAsync(cancellationToken);
            foreach (var nestedEnum in nestedEnums)
                await _jsonWriter.WriteValueAsync(_GetIdFor(nestedEnum), cancellationToken);
            await _jsonWriter.WriteEndArrayAsync(cancellationToken);
        }

        private void _WriteNestedDelegateReferences(IEnumerable<DelegateDocumentationElement> nestedDelegates)
        {
            _jsonWriter.WritePropertyName("nestedDelegates");
            _jsonWriter.WriteStartArray();
            foreach (var nestedDelegate in nestedDelegates)
                _jsonWriter.WriteValue(_GetIdFor(nestedDelegate));
            _jsonWriter.WriteEndArray();
        }

        private async Task _WriteNestedDelegateReferencesAsync(IEnumerable<DelegateDocumentationElement> nestedDelegates, CancellationToken cancellationToken)
        {
            await _jsonWriter.WritePropertyNameAsync("nestedDelegates", cancellationToken);
            await _jsonWriter.WriteStartArrayAsync(cancellationToken);
            foreach (var nestedDelegate in nestedDelegates)
                await _jsonWriter.WriteValueAsync(_GetIdFor(nestedDelegate), cancellationToken);
            await _jsonWriter.WriteEndArrayAsync(cancellationToken);
        }

        private void _WriteNestedInterfaceReferences(IEnumerable<InterfaceDocumentationElement> nestedInterfaces)
        {
            _jsonWriter.WritePropertyName("nestedInterfaces");
            _jsonWriter.WriteStartArray();
            foreach (var nestedInterface in nestedInterfaces)
                _jsonWriter.WriteValue(_GetIdFor(nestedInterface));
            _jsonWriter.WriteEndArray();
        }

        private async Task _WriteNestedInterfaceReferencesAsync(IEnumerable<InterfaceDocumentationElement> nestedInterfaces, CancellationToken cancellationToken)
        {
            await _jsonWriter.WritePropertyNameAsync("nestedInterfaces", cancellationToken);
            await _jsonWriter.WriteStartArrayAsync(cancellationToken);
            foreach (var nestedInterface in nestedInterfaces)
                await _jsonWriter.WriteValueAsync(_GetIdFor(nestedInterface), cancellationToken);
            await _jsonWriter.WriteEndArrayAsync(cancellationToken);
        }

        private void _WriteNestedClasseReferences(IEnumerable<ClassDocumentationElement> nestedClasses)
        {
            _jsonWriter.WritePropertyName("nestedClasses");
            _jsonWriter.WriteStartArray();
            foreach (var nestedClass in nestedClasses)
                _jsonWriter.WriteValue(_GetIdFor(nestedClass));
            _jsonWriter.WriteEndArray();
        }

        private async Task _WriteNestedClasseReferencesAsync(IEnumerable<ClassDocumentationElement> nestedClasses, CancellationToken cancellationToken)
        {
            await _jsonWriter.WritePropertyNameAsync("nestedClasses", cancellationToken);
            await _jsonWriter.WriteStartArrayAsync(cancellationToken);
            foreach (var nestedClass in nestedClasses)
                await _jsonWriter.WriteValueAsync(_GetIdFor(nestedClass), cancellationToken);
            await _jsonWriter.WriteEndArrayAsync(cancellationToken);
        }

        private void _WriteNestedStructReferences(IEnumerable<StructDocumentationElement> nestedStructs)
        {
            _jsonWriter.WritePropertyName("nestedStructs");
            _jsonWriter.WriteStartArray();
            foreach (var nestedStruct in nestedStructs)
                _jsonWriter.WriteValue(_GetIdFor(nestedStruct));
            _jsonWriter.WriteEndArray();
        }

        private async Task _WriteNestedStructReferencesAsync(IEnumerable<StructDocumentationElement> nestedStructs, CancellationToken cancellationToken)
        {
            await _jsonWriter.WritePropertyNameAsync("nestedStructs", cancellationToken);
            await _jsonWriter.WriteStartArrayAsync(cancellationToken);
            foreach (var nestedStruct in nestedStructs)
                await _jsonWriter.WriteValueAsync(_GetIdFor(nestedStruct), cancellationToken);
            await _jsonWriter.WriteEndArrayAsync(cancellationToken);
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

        private async Task _WriteXmlAttributesAsync(IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            await _jsonWriter.WritePropertyNameAsync("xmlAttributes", cancellationToken);
            await _jsonWriter.WriteStartObjectAsync(cancellationToken);
            foreach (var xmlAttribute in xmlAttributes)
            {
                await _jsonWriter.WritePropertyNameAsync(xmlAttribute.Key, cancellationToken);
                await _jsonWriter.WriteValueAsync(xmlAttribute.Value, cancellationToken);
            }
            await _jsonWriter.WriteEndObjectAsync(cancellationToken);
        }

        private void _WriteTypeReference(TypeReferenceData typeReferenceData)
        {
            switch (typeReferenceData)
            {
                case VoidTypeData voidTypeData:
                    _jsonWriter.WriteStartObject();
                    _jsonWriter.WritePropertyName("kind");
                    _jsonWriter.WriteValue("void");
                    _jsonWriter.WriteEndObject();
                    break;

                case DynamicTypeData dynamicTypeData:
                    _jsonWriter.WriteStartObject();
                    _jsonWriter.WritePropertyName("kind");
                    _jsonWriter.WriteValue("dynamic");
                    _jsonWriter.WriteEndObject();
                    break;

                case TypeData typeData:
                    _jsonWriter.WriteStartObject();
                    _jsonWriter.WritePropertyName("kind");
                    _jsonWriter.WriteValue("specific");
                    _jsonWriter.WritePropertyName("name");
                    _jsonWriter.WriteValue(typeData.Name);
                    _jsonWriter.WritePropertyName("namespace");
                    _jsonWriter.WriteValue(typeData.Namespace);
                    _jsonWriter.WritePropertyName("declaringType");
                    if (typeData.DeclaringType != null)
                        _WriteTypeReference(typeData.DeclaringType);
                    else
                        _jsonWriter.WriteNull();
                    _jsonWriter.WritePropertyName("fullName");
                    _jsonWriter.WriteValue(_GetFullName(typeData));

                    _jsonWriter.WritePropertyName("assembly");
                    _WriteAssemblyReference(typeData.Assembly);

                    _jsonWriter.WritePropertyName("genericArguments");
                    _jsonWriter.WriteStartArray();
                    foreach (var genericArgument in typeData.GenericArguments)
                        _WriteTypeReference(genericArgument);
                    _jsonWriter.WriteEndArray();

                    _jsonWriter.WriteEndObject();
                    break;

                case PointerTypeData pointerTypeData:
                    _jsonWriter.WriteStartObject();
                    _jsonWriter.WritePropertyName("kind");
                    _jsonWriter.WriteValue("pointer");
                    _jsonWriter.WritePropertyName("referent");
                    _WriteTypeReference(pointerTypeData.ReferentType);
                    _jsonWriter.WriteEndObject();
                    break;

                case GenericParameterData genericParameterData:
                    _jsonWriter.WriteStartObject();
                    _jsonWriter.WritePropertyName("kind");
                    _jsonWriter.WriteValue("genericParameter");
                    _jsonWriter.WritePropertyName("name");
                    _jsonWriter.WriteValue(genericParameterData.Name);
                    _jsonWriter.WriteEndObject();
                    break;

                case ArrayTypeData arrayTypeData:
                    _jsonWriter.WriteStartObject();
                    _jsonWriter.WritePropertyName("kind");
                    _jsonWriter.WriteValue("array");
                    _jsonWriter.WritePropertyName("rank");
                    _jsonWriter.WriteValue(arrayTypeData.Rank);
                    _jsonWriter.WritePropertyName("item");
                    _WriteTypeReference(arrayTypeData.ItemType);
                    _jsonWriter.WriteEndObject();
                    break;
            }
        }

        private async Task _WriteTypeReferenceAsync(TypeReferenceData typeReferenceData, CancellationToken cancellationToken)
        {
            switch (typeReferenceData)
            {
                case VoidTypeData voidTypeData:
                    await _jsonWriter.WriteStartObjectAsync(cancellationToken);
                    await _jsonWriter.WritePropertyNameAsync("kind", cancellationToken);
                    await _jsonWriter.WriteValueAsync("void", cancellationToken);
                    await _jsonWriter.WriteEndObjectAsync(cancellationToken);
                    break;

                case DynamicTypeData dynamicTypeData:
                    await _jsonWriter.WriteStartObjectAsync(cancellationToken);
                    await _jsonWriter.WritePropertyNameAsync("kind", cancellationToken);
                    await _jsonWriter.WriteValueAsync("dynamic", cancellationToken);
                    await _jsonWriter.WriteEndObjectAsync(cancellationToken);
                    break;

                case TypeData typeData:
                    await _jsonWriter.WriteStartObjectAsync(cancellationToken);
                    await _jsonWriter.WritePropertyNameAsync("kind", cancellationToken);
                    await _jsonWriter.WriteValueAsync("specific", cancellationToken);
                    await _jsonWriter.WritePropertyNameAsync("name", cancellationToken);
                    await _jsonWriter.WriteValueAsync(typeData.Name, cancellationToken);
                    await _jsonWriter.WritePropertyNameAsync("namespace", cancellationToken);
                    await _jsonWriter.WriteValueAsync(typeData.Namespace, cancellationToken);
                    await _jsonWriter.WritePropertyNameAsync("declaringType", cancellationToken);
                    if (typeData.DeclaringType != null)
                        await _WriteTypeReferenceAsync(typeData.DeclaringType, cancellationToken);
                    else
                        await _jsonWriter.WriteNullAsync(cancellationToken);
                    await _jsonWriter.WritePropertyNameAsync("fullName", cancellationToken);
                    await _jsonWriter.WriteValueAsync(_GetFullName(typeData), cancellationToken);

                    await _jsonWriter.WritePropertyNameAsync("assembly", cancellationToken);
                    await _WriteAssemblyReferenceAsync(typeData.Assembly, cancellationToken);

                    await _jsonWriter.WritePropertyNameAsync("genericArguments", cancellationToken);
                    await _jsonWriter.WriteStartArrayAsync(cancellationToken);
                    foreach (var genericArgument in typeData.GenericArguments)
                        await _WriteTypeReferenceAsync(genericArgument, cancellationToken);
                    await _jsonWriter.WriteEndArrayAsync(cancellationToken);

                    await _jsonWriter.WriteEndObjectAsync(cancellationToken);
                    break;

                case PointerTypeData pointerTypeData:
                    await _jsonWriter.WriteStartObjectAsync(cancellationToken);
                    await _jsonWriter.WritePropertyNameAsync("kind", cancellationToken);
                    await _jsonWriter.WriteValueAsync("pointer", cancellationToken);
                    await _jsonWriter.WritePropertyNameAsync("referent", cancellationToken);
                    await _WriteTypeReferenceAsync(pointerTypeData.ReferentType, cancellationToken);
                    await _jsonWriter.WriteEndObjectAsync(cancellationToken);
                    break;

                case GenericParameterData genericParameterData:
                    await _jsonWriter.WriteStartObjectAsync(cancellationToken);
                    await _jsonWriter.WritePropertyNameAsync("kind", cancellationToken);
                    await _jsonWriter.WriteValueAsync("genericParameter", cancellationToken);
                    await _jsonWriter.WritePropertyNameAsync("name", cancellationToken);
                    await _jsonWriter.WriteValueAsync(genericParameterData.Name, cancellationToken);
                    await _jsonWriter.WriteEndObjectAsync(cancellationToken);
                    break;

                case ArrayTypeData arrayTypeData:
                    await _jsonWriter.WriteStartObjectAsync(cancellationToken);
                    await _jsonWriter.WritePropertyNameAsync("kind", cancellationToken);
                    await _jsonWriter.WriteValueAsync("array", cancellationToken);
                    await _jsonWriter.WritePropertyNameAsync("rank", cancellationToken);
                    await _jsonWriter.WriteValueAsync(arrayTypeData.Rank, cancellationToken);
                    await _jsonWriter.WritePropertyNameAsync("item", cancellationToken);
                    await _WriteTypeReferenceAsync(arrayTypeData.ItemType, cancellationToken);
                    await _jsonWriter.WriteEndObjectAsync(cancellationToken);
                    break;
            }
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

        private async Task _WriteAccessModifierAsync(AccessModifier accessModifier, CancellationToken cancellationToken)
        {
            await _jsonWriter.WritePropertyNameAsync("accessModifier", cancellationToken);
            switch (accessModifier)
            {
                case AccessModifier.Public:
                    await _jsonWriter.WriteValueAsync("public", cancellationToken);
                    break;

                case AccessModifier.Assembly:
                    await _jsonWriter.WriteValueAsync("assembly", cancellationToken);
                    break;

                case AccessModifier.Family:
                    await _jsonWriter.WriteValueAsync("family", cancellationToken);
                    break;

                case AccessModifier.FamilyOrAssembly:
                    await _jsonWriter.WriteValueAsync("familyOrAssembly", cancellationToken);
                    break;

                case AccessModifier.FamilyAndAssembly:
                    await _jsonWriter.WriteValueAsync("familyAndAssembly", cancellationToken);
                    break;

                case AccessModifier.Private:
                default:
                    await _jsonWriter.WriteValueAsync("private", cancellationToken);
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
                _WriteTypeReference(attribute.Type);

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

        private async Task _WriteAttributesAsync(IEnumerable<AttributeData> attributes, CancellationToken cancellationToken)
        {
            await _jsonWriter.WritePropertyNameAsync("attributes", cancellationToken);
            await _WriteAttributesArrayAsync(attributes, cancellationToken);
        }

        private async Task _WriteReturnAttributesAsync(IEnumerable<AttributeData> attributes, CancellationToken cancellationToken)
        {
            await _jsonWriter.WritePropertyNameAsync("returnAttributes", cancellationToken);
            await _WriteAttributesArrayAsync(attributes, cancellationToken);
        }

        private async Task _WriteAttributesArrayAsync(IEnumerable<AttributeData> attributes, CancellationToken cancellationToken)
        {
            await _jsonWriter.WriteStartArrayAsync(cancellationToken);
            foreach (var attribute in attributes)
            {
                await _jsonWriter.WriteStartObjectAsync(cancellationToken);

                await _jsonWriter.WritePropertyNameAsync("type", cancellationToken);
                await _WriteTypeReferenceAsync(attribute.Type, cancellationToken);

                await _jsonWriter.WritePropertyNameAsync("positionalParameters", cancellationToken);
                await _jsonWriter.WriteStartArrayAsync(cancellationToken);
                foreach (var positionalParameter in attribute.PositionalParameters)
                    await _WriteAttributeParameterAsync(positionalParameter, cancellationToken);
                await _jsonWriter.WriteEndArrayAsync(cancellationToken);

                await _jsonWriter.WritePropertyNameAsync("namedParameters", cancellationToken);
                await _jsonWriter.WriteStartArrayAsync(cancellationToken);
                foreach (var namedParameter in attribute.NamedParameters)
                    await _WriteAttributeParameterAsync(namedParameter, cancellationToken);
                await _jsonWriter.WriteEndArrayAsync(cancellationToken);

                await _jsonWriter.WriteEndObjectAsync(cancellationToken);
            }
            await _jsonWriter.WriteEndArrayAsync(cancellationToken);
        }

        private void _WriteAttributeParameter(AttributeParameterData parameter)
        {
            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("name");
            _jsonWriter.WriteValue(parameter.Name);

            _jsonWriter.WritePropertyName("value");
            WriteValue(parameter.Value);

            _jsonWriter.WritePropertyName("type");
            _WriteTypeReference(parameter.Type);

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
                else if (value is TypeReferenceData typeReferenceData)
                    _WriteTypeReference(typeReferenceData);
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

        private async Task _WriteAttributeParameterAsync(AttributeParameterData parameter, CancellationToken cancellationToken)
        {
            await _jsonWriter.WriteStartObjectAsync(cancellationToken);

            await _jsonWriter.WritePropertyNameAsync("name", cancellationToken);
            await _jsonWriter.WriteValueAsync(parameter.Name, cancellationToken);

            await _jsonWriter.WritePropertyNameAsync("value", cancellationToken);
            await WriteValueAsync(parameter.Value);

            await _jsonWriter.WritePropertyNameAsync("type", cancellationToken);
            await _WriteTypeReferenceAsync(parameter.Type, cancellationToken);

            await _jsonWriter.WriteEndObjectAsync(cancellationToken);

            async Task WriteValueAsync(object value)
            {
                if (value == null)
                    await _jsonWriter.WriteNullAsync(cancellationToken);
                else if (value is Array array)
                {
                    await _jsonWriter.WriteStartArrayAsync(cancellationToken);
                    foreach (var item in array)
                        await WriteValueAsync(item);
                    await _jsonWriter.WriteEndArrayAsync(cancellationToken);
                }
                else if (value is TypeReferenceData typeReferenceData)
                    await _WriteTypeReferenceAsync(typeReferenceData, cancellationToken);
                else
                {
                    var valueType = value.GetType();
                    if (valueType.IsEnum || (Nullable.GetUnderlyingType(valueType)?.IsEnum ?? false))
                        await _jsonWriter.WriteValueAsync(value.ToString(), cancellationToken);
                    else
                        await _jsonWriter.WriteValueAsync(value, cancellationToken);
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
                    _WriteTypeReference(typeConstraint);
                _jsonWriter.WriteEndArray();
                _WriteDescription(genericParameter.Description);

                _jsonWriter.WriteEndObject();
            }
            _jsonWriter.WriteEndArray();
        }

        private async Task _WriteGenericParametersAsync(IEnumerable<GenericParameterData> genericParameters, CancellationToken cancellationToken)
        {
            await _jsonWriter.WritePropertyNameAsync("genericParameters", cancellationToken);
            await _jsonWriter.WriteStartArrayAsync(cancellationToken);
            foreach (var genericParameter in genericParameters)
            {
                await _jsonWriter.WriteStartObjectAsync(cancellationToken);

                await _jsonWriter.WritePropertyNameAsync("name", cancellationToken);
                await _jsonWriter.WriteValueAsync(genericParameter.Name, cancellationToken);
                await _jsonWriter.WritePropertyNameAsync("isCovariant", cancellationToken);
                await _jsonWriter.WriteValueAsync(genericParameter.IsCovariant, cancellationToken);
                await _jsonWriter.WritePropertyNameAsync("isContravariant", cancellationToken);
                await _jsonWriter.WriteValueAsync(genericParameter.IsContravariant, cancellationToken);

                await _jsonWriter.WritePropertyNameAsync("hasReferenceTypeConstraint", cancellationToken);
                await _jsonWriter.WriteValueAsync(genericParameter.HasReferenceTypeConstraint, cancellationToken);
                await _jsonWriter.WritePropertyNameAsync("hasNonNullableValueTypeConstraint", cancellationToken);
                await _jsonWriter.WriteValueAsync(genericParameter.HasNonNullableValueTypeConstraint, cancellationToken);
                await _jsonWriter.WritePropertyNameAsync("hasDefaultConstructorConstraint", cancellationToken);
                await _jsonWriter.WriteValueAsync(genericParameter.HasDefaultConstructorConstraint, cancellationToken);

                await _jsonWriter.WritePropertyNameAsync("typeConstraints", cancellationToken);
                await _jsonWriter.WriteStartArrayAsync(cancellationToken);
                foreach (var typeConstraint in genericParameter.TypeConstraints)
                    await _WriteTypeReferenceAsync(typeConstraint, cancellationToken);
                await _jsonWriter.WriteEndArrayAsync(cancellationToken);

                await _WriteDescriptionAsync(genericParameter.Description, cancellationToken);
                await _jsonWriter.WriteEndObjectAsync(cancellationToken);
            }
            await _jsonWriter.WriteEndArrayAsync(cancellationToken);
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
                _WriteTypeReference(parameter.Type);

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

        private async Task _WriteParametersAsync(IEnumerable<ParameterData> parameters, CancellationToken cancellationToken)
        {
            await _jsonWriter.WritePropertyNameAsync("parameters", cancellationToken);
            await _jsonWriter.WriteStartArrayAsync(cancellationToken);
            foreach (var parameter in parameters)
            {
                await _jsonWriter.WriteStartObjectAsync(cancellationToken);

                await _jsonWriter.WritePropertyNameAsync("name", cancellationToken);
                await _jsonWriter.WriteValueAsync(parameter.Name, cancellationToken);
                await _jsonWriter.WritePropertyNameAsync("type", cancellationToken);
                await _WriteTypeReferenceAsync(parameter.Type, cancellationToken);

                await _jsonWriter.WritePropertyNameAsync("isInputByReference", cancellationToken);
                await _jsonWriter.WriteValueAsync(parameter.IsInputByReference, cancellationToken);
                await _jsonWriter.WritePropertyNameAsync("isInputOutputByReference", cancellationToken);
                await _jsonWriter.WriteValueAsync(parameter.IsInputOutputByReference, cancellationToken);
                await _jsonWriter.WritePropertyNameAsync("isOutputByReference", cancellationToken);
                await _jsonWriter.WriteValueAsync(parameter.IsOutputByReference, cancellationToken);

                if (parameter.HasDefaultValue)
                {
                    await _jsonWriter.WritePropertyNameAsync("defaultValue", cancellationToken);
                    await _jsonWriter.WriteValueAsync(parameter.DefaultValue, cancellationToken);
                }
                await _WriteAttributesAsync(parameter.Attributes, cancellationToken);
                await _WriteDescriptionAsync(parameter.Description, cancellationToken);

                await _jsonWriter.WriteEndObjectAsync(cancellationToken);
            }
            await _jsonWriter.WriteEndArrayAsync(cancellationToken);
        }

        private void _WriteReturn(ReturnsData @return)
        {
            _jsonWriter.WritePropertyName("return");
            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("type");
            _WriteTypeReference(@return.Type);
            _WriteAttributes(@return.Attributes);
            _WriteDescription(@return.Description);

            _jsonWriter.WriteEndObject();
        }

        private async Task _WriteReturnAsync(ReturnsData @return, CancellationToken cancellationToken)
        {
            await _jsonWriter.WritePropertyNameAsync("return", cancellationToken);
            await _jsonWriter.WriteStartObjectAsync(cancellationToken);

            await _jsonWriter.WritePropertyNameAsync("type", cancellationToken);
            await _WriteTypeReferenceAsync(@return.Type, cancellationToken);
            await _WriteAttributesAsync(@return.Attributes, cancellationToken);
            await _WriteDescriptionAsync(@return.Description, cancellationToken);

            await _jsonWriter.WriteEndObjectAsync(cancellationToken);
        }

        private void _WriteEnumReferences(IEnumerable<EnumDocumentationElement> enums)
        {
            _jsonWriter.WritePropertyName("enums");
            _jsonWriter.WriteStartArray();
            foreach (var @enum in enums)
                _jsonWriter.WriteValue(_GetIdFor(@enum));
            _jsonWriter.WriteEndArray();
        }

        private async Task _WriteEnumReferencesAsync(IEnumerable<EnumDocumentationElement> enums, CancellationToken cancellationToken)
        {
            await _jsonWriter.WritePropertyNameAsync("enums", cancellationToken);
            await _jsonWriter.WriteStartArrayAsync(cancellationToken);
            foreach (var @struct in enums)
                await _jsonWriter.WriteValueAsync(_GetIdFor(@struct), cancellationToken);
            await _jsonWriter.WriteEndArrayAsync(cancellationToken);
        }

        private void _WriteDelegateReferences(IEnumerable<DelegateDocumentationElement> delegates)
        {
            _jsonWriter.WritePropertyName("delegates");
            _jsonWriter.WriteStartArray();
            foreach (var @delegate in delegates)
                _jsonWriter.WriteValue(_GetIdFor(@delegate));
            _jsonWriter.WriteEndArray();
        }

        private async Task _WriteDelegateReferencesAsync(IEnumerable<DelegateDocumentationElement> delegates, CancellationToken cancellationToken)
        {
            await _jsonWriter.WritePropertyNameAsync("delegates", cancellationToken);
            await _jsonWriter.WriteStartArrayAsync(cancellationToken);
            foreach (var @delegate in delegates)
                await _jsonWriter.WriteValueAsync(_GetIdFor(@delegate), cancellationToken);
            await _jsonWriter.WriteEndArrayAsync(cancellationToken);
        }

        private void _WriteInterfaceReferences(IEnumerable<InterfaceDocumentationElement> interfaces)
        {
            _jsonWriter.WritePropertyName("interfaces");
            _jsonWriter.WriteStartArray();
            foreach (var @interface in interfaces)
                _jsonWriter.WriteValue(_GetIdFor(@interface));
            _jsonWriter.WriteEndArray();
        }

        private async Task _WriteInterfaceReferencesAsync(IEnumerable<InterfaceDocumentationElement> interfaces, CancellationToken cancellationToken)
        {
            await _jsonWriter.WritePropertyNameAsync("interfaces", cancellationToken);
            await _jsonWriter.WriteStartArrayAsync(cancellationToken);
            foreach (var @interface in interfaces)
                await _jsonWriter.WriteValueAsync(_GetIdFor(@interface), cancellationToken);
            await _jsonWriter.WriteEndArrayAsync(cancellationToken);
        }

        private void _WriteClassReferences(IEnumerable<ClassDocumentationElement> classes)
        {
            _jsonWriter.WritePropertyName("classes");
            _jsonWriter.WriteStartArray();
            foreach (var @class in classes)
                _jsonWriter.WriteValue(_GetIdFor(@class));
            _jsonWriter.WriteEndArray();
        }

        private async Task _WriteClassReferencesAsync(IEnumerable<ClassDocumentationElement> classes, CancellationToken cancellationToken)
        {
            await _jsonWriter.WritePropertyNameAsync("classes", cancellationToken);
            await _jsonWriter.WriteStartArrayAsync(cancellationToken);
            foreach (var @class in classes)
                await _jsonWriter.WriteValueAsync(_GetIdFor(@class), cancellationToken);
            await _jsonWriter.WriteEndArrayAsync(cancellationToken);
        }

        private void _WriteStructReferences(IEnumerable<StructDocumentationElement> structs)
        {
            _jsonWriter.WritePropertyName("structs");
            _jsonWriter.WriteStartArray();
            foreach (var @struct in structs)
                _jsonWriter.WriteValue(_GetIdFor(@struct));
            _jsonWriter.WriteEndArray();
        }

        private async Task _WriteStructReferencesAsync(IEnumerable<StructDocumentationElement> structs, CancellationToken cancellationToken)
        {
            await _jsonWriter.WritePropertyNameAsync("structs", cancellationToken);
            await _jsonWriter.WriteStartArrayAsync(cancellationToken);
            foreach (var @struct in structs)
                await _jsonWriter.WriteValueAsync(_GetIdFor(@struct), cancellationToken);
            await _jsonWriter.WriteEndArrayAsync(cancellationToken);
        }

        private void _WriteExceptions(IEnumerable<ExceptionData> exceptions)
        {
            _jsonWriter.WritePropertyName("exceptions");
            _jsonWriter.WriteStartArray();
            foreach (var exception in exceptions)
            {
                _jsonWriter.WriteStartObject();

                _jsonWriter.WritePropertyName("type");
                _WriteTypeReference(exception.Type);

                _WriteXmlAttributes(exception.Description.XmlAttributes);

                _jsonWriter.WritePropertyName("content");
                _jsonWriter.WriteStartArray();
                exception.Description.Accept(this);
                _jsonWriter.WriteEndArray();

                _jsonWriter.WriteEndObject();
            }
            _jsonWriter.WriteEndArray();
        }

        private async Task _WriteExceptionsAsync(IEnumerable<ExceptionData> exceptions, CancellationToken cancellationToken)
        {
            await _jsonWriter.WritePropertyNameAsync("exceptions", cancellationToken);
            await _jsonWriter.WriteStartArrayAsync(cancellationToken);
            foreach (var exception in exceptions)
            {
                await _jsonWriter.WriteStartObjectAsync(cancellationToken);

                await _jsonWriter.WritePropertyNameAsync("type", cancellationToken);
                await _WriteTypeReferenceAsync(exception.Type, cancellationToken);

                await _WriteXmlAttributesAsync(exception.Description.XmlAttributes, cancellationToken);

                await _jsonWriter.WritePropertyNameAsync("content", cancellationToken);
                await _jsonWriter.WriteStartArrayAsync(cancellationToken);
                await exception.Description.AcceptAsync(this, cancellationToken);
                await _jsonWriter.WriteEndArrayAsync(cancellationToken);

                await _jsonWriter.WriteEndObjectAsync(cancellationToken);
            }
            await _jsonWriter.WriteEndArrayAsync(cancellationToken);
        }

        private void _WriteExamples(IEnumerable<ExampleDocumentationElement> examples)
        {
            _jsonWriter.WritePropertyName("examples");
            _jsonWriter.WriteStartArray();
            foreach (var example in examples)
            {
                _jsonWriter.WriteStartObject();
                _WriteXmlAttributes(example.XmlAttributes);

                _jsonWriter.WritePropertyName("content");
                _jsonWriter.WriteStartArray();
                example.Accept(this);
                _jsonWriter.WriteEndArray();

                _jsonWriter.WriteEndObject();
            }
            _jsonWriter.WriteEndArray();
        }

        private async Task _WriteExamplesAsync(IEnumerable<ExampleDocumentationElement> examples, CancellationToken cancellationToken)
        {
            await _jsonWriter.WritePropertyNameAsync("examples", cancellationToken);
            await _jsonWriter.WriteStartArrayAsync(cancellationToken);
            foreach (var example in examples)
            {
                await _jsonWriter.WriteStartObjectAsync(cancellationToken);
                await _WriteXmlAttributesAsync(example.XmlAttributes, cancellationToken);

                await _jsonWriter.WritePropertyNameAsync("content", cancellationToken);
                await _jsonWriter.WriteStartArrayAsync(cancellationToken);
                await example.AcceptAsync(this, cancellationToken);
                await _jsonWriter.WriteEndArrayAsync(cancellationToken);

                await _jsonWriter.WriteEndObjectAsync(cancellationToken);
            }
            await _jsonWriter.WriteEndArrayAsync(cancellationToken);
        }

        private void _WriteRelatedMembers(IEnumerable<MemberReferenceDocumentationElement> relatedMembers)
        {
            _jsonWriter.WritePropertyName("relatedMembers");
            _jsonWriter.WriteStartArray();
            foreach (var relatedMember in relatedMembers)
                relatedMember.Accept(this);
            _jsonWriter.WriteEndArray();
        }

        private async Task _WriteRelatedMembersAsync(IEnumerable<MemberReferenceDocumentationElement> relatedMembers, CancellationToken cancellationToken)
        {
            await _jsonWriter.WritePropertyNameAsync("relatedMembers", cancellationToken);
            await _jsonWriter.WriteStartArrayAsync(cancellationToken);
            foreach (var relatedMember in relatedMembers)
                await relatedMember.AcceptAsync(this, cancellationToken);
            await _jsonWriter.WriteEndArrayAsync(cancellationToken);
        }

        private void _WriteDescription(BlockDescriptionDocumentationElement blockDescription)
        {
            _jsonWriter.WritePropertyName("description");
            _jsonWriter.WriteStartObject();

            _WriteXmlAttributes(blockDescription.XmlAttributes);

            _jsonWriter.WritePropertyName("content");
            _jsonWriter.WriteStartArray();
            blockDescription.Accept(this);
            _jsonWriter.WriteEndArray();

            _jsonWriter.WriteEndObject();
        }

        private async Task _WriteDescriptionAsync(BlockDescriptionDocumentationElement blockDescription, CancellationToken cancellationToken)
        {
            await _jsonWriter.WritePropertyNameAsync("description", cancellationToken);
            await _jsonWriter.WriteStartObjectAsync(cancellationToken);

            await _WriteXmlAttributesAsync(blockDescription.XmlAttributes, cancellationToken);

            await _jsonWriter.WritePropertyNameAsync("content", cancellationToken);
            await _jsonWriter.WriteStartArrayAsync(cancellationToken);
            await blockDescription.AcceptAsync(this, cancellationToken);
            await _jsonWriter.WriteEndArrayAsync(cancellationToken);

            await _jsonWriter.WriteEndObjectAsync(cancellationToken);
        }

        private void _WriteDependencies(IEnumerable<AssemblyReference> dependencies)
        {
            _jsonWriter.WritePropertyName("dependencies");
            _jsonWriter.WriteStartArray();
            foreach (var dependency in dependencies)
                _WriteAssemblyReference(dependency);
            _jsonWriter.WriteEndArray();
        }

        private async Task _WriteDependenciesAsync(IEnumerable<AssemblyReference> dependencies, CancellationToken cancellationToken)
        {
            await _jsonWriter.WritePropertyNameAsync("dependencies", cancellationToken);
            await _jsonWriter.WriteStartArrayAsync(cancellationToken);
            foreach (var dependency in dependencies)
                await _WriteAssemblyReferenceAsync(dependency, cancellationToken);
            await _jsonWriter.WriteEndArrayAsync(cancellationToken);
        }

        private void _WriteAssemblyReference(AssemblyReference assemblyReference)
        {
            _jsonWriter.WriteStartObject();
            _jsonWriter.WritePropertyName("name");
            _jsonWriter.WriteValue(assemblyReference.Name);
            _jsonWriter.WritePropertyName("version");
            _jsonWriter.WriteValue(assemblyReference.Version.ToString());
            _jsonWriter.WritePropertyName("culture");
            _jsonWriter.WriteValue(assemblyReference.Culture);
            _jsonWriter.WritePropertyName("publicKeyToken");
            _jsonWriter.WriteValue(assemblyReference.PublicKeyToken);
            _jsonWriter.WriteEndObject();
        }

        private async Task _WriteAssemblyReferenceAsync(AssemblyReference assemblyReference, CancellationToken cancellationToken)
        {
            await _jsonWriter.WriteStartObjectAsync(cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("name", cancellationToken);
            await _jsonWriter.WriteValueAsync(assemblyReference.Name, cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("version", cancellationToken);
            await _jsonWriter.WriteValueAsync(assemblyReference.Version.ToString(), cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("culture", cancellationToken);
            await _jsonWriter.WriteValueAsync(assemblyReference.Culture, cancellationToken);
            await _jsonWriter.WritePropertyNameAsync("publicKeyToken", cancellationToken);
            await _jsonWriter.WriteValueAsync(assemblyReference.PublicKeyToken, cancellationToken);
            await _jsonWriter.WriteEndObjectAsync(cancellationToken);
        }

        private static string _GetFullName(TypeData typeData)
        {
            var fullNameBuilder = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(typeData.Namespace))
                fullNameBuilder.Append(typeData.Namespace);
            var nestingChain = new Stack<TypeData>();
            nestingChain.Push(typeData);
            while (typeData.DeclaringType is TypeData declaringTypeData)
            {
                nestingChain.Push(declaringTypeData);
                typeData = declaringTypeData;
            }
            do
            {
                if (fullNameBuilder.Length > 0)
                    fullNameBuilder.Append('.');
                fullNameBuilder.Append(nestingChain.Pop().Name);
            } while (nestingChain.Count > 0);

            return fullNameBuilder.ToString();
        }

        private static string _GetIdFor(EnumDocumentationElement @enum)
            => _GetIdBuilderFor(@enum).ToString();

        private static string _GetIdFor(DelegateDocumentationElement @delegate)
            => _GetIdBuilderFor(@delegate).ToString();

        private static string _GetIdFor(InterfaceDocumentationElement @interface)
            => _GetIdBuilderFor(@interface).ToString();

        private static string _GetIdFor(ClassDocumentationElement @class)
            => _GetIdBuilderFor(@class).ToString();

        private static string _GetIdFor(StructDocumentationElement @struct)
            => _GetIdBuilderFor(@struct).ToString();

        private static string _GetIdFor(ConstantDocumentationElement constant)
            => _GetIdBuilderFor(constant.DeclaringType).Append('.').Append(constant.Name).ToString();

        private static string _GetIdFor(FieldDocumentationElement field)
            => _GetIdBuilderFor(field.DeclaringType).Append('.').Append(field.Name).ToString();

        private static string _GetIdFor(EventDocumentationElement @event)
            => _GetIdBuilderFor(@event.DeclaringType).Append('.').Append(@event.Name).ToString();

        private static string _GetIdFor(PropertyDocumentationElement property)
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

        private static string _GetIdFor(ConstructorDocumentationElement constructor)
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

        private static string _GetIdFor(MethodDocumentationElement method)
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

        private static string _GetIdFor(TypeDocumentationElement type)
        {
            switch (type)
            {
                case EnumDocumentationElement @enum:
                    return _GetIdFor(@enum);

                case DelegateDocumentationElement @delegate:
                    return _GetIdFor(@delegate);

                case InterfaceDocumentationElement @interface:
                    return _GetIdFor(@interface);

                case ClassDocumentationElement @class:
                    return _GetIdFor(@class);

                case StructDocumentationElement @struct:
                    return _GetIdFor(@struct);

                default:
                    return _GetBaseIdBuilder(type).ToString();
            }
        }

        private static StringBuilder _GetIdBuilderFor(EnumDocumentationElement @enum)
            => _GetBaseIdBuilder(@enum);

        private static StringBuilder _GetIdBuilderFor(DelegateDocumentationElement @delegate)
        {
            var idBuilder = _GetBaseIdBuilder(@delegate);
            if (@delegate.GenericParameters.Count > 0)
                idBuilder.Append('`').Append(@delegate.GenericParameters.Count);
            return idBuilder;
        }

        private static StringBuilder _GetIdBuilderFor(InterfaceDocumentationElement @interface)
        {
            var idBuilder = _GetBaseIdBuilder(@interface);
            if (@interface.GenericParameters.Count > 0)
                idBuilder.Append('`').Append(@interface.GenericParameters.Count);
            return idBuilder;
        }

        private static StringBuilder _GetIdBuilderFor(ClassDocumentationElement @class)
        {
            var idBuilder = _GetBaseIdBuilder(@class);
            if (@class.GenericParameters.Count > 0)
                idBuilder.Append('`').Append(@class.GenericParameters.Count);
            return idBuilder;
        }

        private static StringBuilder _GetIdBuilderFor(StructDocumentationElement @struct)
        {
            var idBuilder = _GetBaseIdBuilder(@struct);
            if (@struct.GenericParameters.Count > 0)
                idBuilder.Append('`').Append(@struct.GenericParameters.Count);
            return idBuilder;
        }

        private static StringBuilder _GetIdBuilderFor(TypeDocumentationElement type)
        {
            switch (type)
            {
                case EnumDocumentationElement @enum:
                    return _GetIdBuilderFor(@enum);

                case DelegateDocumentationElement @delegate:
                    return _GetIdBuilderFor(@delegate);

                case InterfaceDocumentationElement @interface:
                    return _GetIdBuilderFor(@interface);

                case ClassDocumentationElement @class:
                    return _GetIdBuilderFor(@class);

                case StructDocumentationElement @struct:
                    return _GetIdBuilderFor(@struct);

                default:
                    return _GetBaseIdBuilder(type);
            }
        }

        private static StringBuilder _GetBaseIdBuilder(TypeDocumentationElement type)
        {
            var idBuilder = new StringBuilder();

            if (!(type.Namespace is GlobalNamespaceDocumentationElement))
                idBuilder.Append(type.Namespace.Name);
            var nestingChain = new Stack<TypeDocumentationElement>();
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

        private static string _GetTypeReferenceId(TypeReferenceData typeReferenceData)
            => _AppendTypeReferenceId(new StringBuilder(), typeReferenceData).ToString();

        private static StringBuilder _AppendTypeReferenceId(StringBuilder idBuilder, TypeReferenceData typeReferenceData)
        {
            switch (typeReferenceData)
            {
                case VoidTypeData voidTypeData:
                    return idBuilder.Append("System.Void");

                case DynamicTypeData dynamicTypeData:
                    return idBuilder.Append("System.Object");

                case TypeData typeData:
                    if (typeData.DeclaringType == null)
                    {
                        if (!string.IsNullOrWhiteSpace(typeData.Namespace))
                            idBuilder.Append(typeData.Namespace).Append('.');
                    }
                    else
                        _AppendTypeReferenceId(idBuilder, typeData.DeclaringType).Append('.');

                    idBuilder.Append(typeData.Name);
                    if (typeData.GenericArguments.Count > 0)
                    {
                        idBuilder.Append('<');
                        var isFirst = true;
                        foreach (var genericArgument in typeData.GenericArguments)
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

                case PointerTypeData pointerTypeData:
                    return _AppendTypeReferenceId(idBuilder, pointerTypeData.ReferentType).Append('*');

                case TypeGenericParameterData typeGenericParameterData:
                    return idBuilder.Append('`').Append(_GetGenericParameterAbsolutePosition(typeGenericParameterData));

                case MethodGenericParameterTypeData methodGenericParameterData:
                    return idBuilder.Append("``").Append(methodGenericParameterData.Position);

                case ArrayTypeData arrayTypeData:
                    return _AppendTypeReferenceId(idBuilder, arrayTypeData.ItemType)
                        .Append('[')
                        .Append(new string(',', arrayTypeData.Rank - 1))
                        .Append(']');

                default:
                    return idBuilder;
            }
        }

        private static int _GetGenericParameterAbsolutePosition(TypeGenericParameterData typeGenericParameterData)
        {
            var genericParameterPosition = typeGenericParameterData.Position;
            var declaryingType = typeGenericParameterData.DeclaringType.DeclaringType;
            while (declaryingType != null)
            {
                switch (declaryingType)
                {
                    case ClassDocumentationElement @class:
                        genericParameterPosition += @class.GenericParameters.Count;
                        break;

                    case StructDocumentationElement @struct:
                        genericParameterPosition += @struct.GenericParameters.Count;
                        break;
                }
                declaryingType = declaryingType.DeclaringType;
            }
            return genericParameterPosition;
        }
    }
}