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

            _jsonWriter.WritePropertyName("examples");
            _jsonWriter.WriteStartArray();
            foreach (var example in assembly.Examples)
                example.Accept(this);
            _jsonWriter.WriteEndArray();

            _jsonWriter.WritePropertyName("relatedMembers");
            _jsonWriter.WriteStartArray();
            foreach (var relatedMember in assembly.RelatedMembers)
                relatedMember.Accept(this);
            _jsonWriter.WriteEndArray();

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

            await _jsonWriter.WritePropertyNameAsync("examples", cancellationToken);
            await _jsonWriter.WriteStartArrayAsync(cancellationToken);
            foreach (var example in assembly.Examples)
                await example.AcceptAsync(this, cancellationToken);
            await _jsonWriter.WriteEndArrayAsync(cancellationToken);

            await _jsonWriter.WritePropertyNameAsync("relatedMembers", cancellationToken);
            await _jsonWriter.WriteStartArrayAsync(cancellationToken);
            foreach (var relatedMember in assembly.RelatedMembers)
                await relatedMember.AcceptAsync(this, cancellationToken);
            await _jsonWriter.WriteEndArrayAsync(cancellationToken);

            await _jsonWriter.WritePropertyNameAsync("definitions", cancellationToken);
            await _jsonWriter.WriteStartObjectAsync(cancellationToken);
        }

        /// <summary>Visits a <see cref="NamespaceDocumentationElement"/>.</summary>
        /// <param name="namespace">The <see cref="NamespaceDocumentationElement"/> to visit.</param>
        protected internal override void VisitNamespace(NamespaceDocumentationElement @namespace)
        {
        }

        /// <summary>Visits a <see cref="NamespaceDocumentationElement"/>.</summary>
        /// <param name="namespace">The <see cref="NamespaceDocumentationElement"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitNamespaceAsync(NamespaceDocumentationElement @namespace, CancellationToken cancellationToken)
        {
            try
            {
                VisitNamespace(@namespace);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits an <see cref="EnumDocumentationElement"/>.</summary>
        /// <param name="enum">The <see cref="EnumDocumentationElement"/> to visit.</param>
        protected internal override void VisitEnum(EnumDocumentationElement @enum)
        {
        }

        /// <summary>Visits an <see cref="EnumDocumentationElement"/>.</summary>
        /// <param name="enum">The <see cref="EnumDocumentationElement"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitEnumAsync(EnumDocumentationElement @enum, CancellationToken cancellationToken)
        {
            try
            {
                VisitEnum(@enum);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits a <see cref="DelegateDocumentationElement"/>.</summary>
        /// <param name="delegate">The <see cref="DelegateDocumentationElement"/> to visit.</param>
        protected internal override void VisitDelegate(DelegateDocumentationElement @delegate)
        {
        }

        /// <summary>Visits a <see cref="DelegateDocumentationElement"/>.</summary>
        /// <param name="delegate">The <see cref="DelegateDocumentationElement"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitDelegateAsync(DelegateDocumentationElement @delegate, CancellationToken cancellationToken)
        {
            try
            {
                VisitDelegate(@delegate);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits an <see cref="InterfaceDocumentationElement"/>.</summary>
        /// <param name="interface">The <see cref="InterfaceDocumentationElement"/> to visit.</param>
        protected internal override void VisitInterface(InterfaceDocumentationElement @interface)
        {
        }

        /// <summary>Visits an <see cref="InterfaceDocumentationElement"/>.</summary>
        /// <param name="interface">The <see cref="InterfaceDocumentationElement"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitInterfaceAsync(InterfaceDocumentationElement @interface, CancellationToken cancellationToken)
        {
            try
            {
                VisitInterface(@interface);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits a <see cref="ClassDocumentationElement"/>.</summary>
        /// <param name="class">The <see cref="ClassDocumentationElement"/> to visit.</param>
        protected internal override void VisitClass(ClassDocumentationElement @class)
        {
        }

        /// <summary>Visits a <see cref="ClassDocumentationElement"/>.</summary>
        /// <param name="class">The <see cref="ClassDocumentationElement"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitClassAsync(ClassDocumentationElement @class, CancellationToken cancellationToken)
        {
            try
            {
                VisitClass(@class);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits a <see cref="StructDocumentationElement"/>.</summary>
        /// <param name="struct">The <see cref="StructDocumentationElement"/> to visit.</param>
        protected internal override void VisitStruct(StructDocumentationElement @struct)
        {
        }

        /// <summary>Visits a <see cref="StructDocumentationElement"/>.</summary>
        /// <param name="struct">The <see cref="StructDocumentationElement"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitStructAsync(StructDocumentationElement @struct, CancellationToken cancellationToken)
        {
            try
            {
                VisitStruct(@struct);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits a <see cref="ConstantDocumentationElement"/>.</summary>
        /// <param name="constant">The <see cref="ConstantDocumentationElement"/> to visit.</param>
        protected internal override void VisitConstant(ConstantDocumentationElement constant)
        {
        }

        /// <summary>Visits a <see cref="ConstantDocumentationElement"/>.</summary>
        /// <param name="constant">The <see cref="ConstantDocumentationElement"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitConstantAsync(ConstantDocumentationElement constant, CancellationToken cancellationToken)
        {
            try
            {
                VisitConstant(constant);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits a <see cref="FieldDocumentationElement"/>.</summary>
        /// <param name="field">The <see cref="FieldDocumentationElement"/> to visit.</param>
        protected internal override void VisitField(FieldDocumentationElement field)
        {
        }

        /// <summary>Visits a <see cref="FieldDocumentationElement"/>.</summary>
        /// <param name="field">The <see cref="FieldDocumentationElement"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitFieldAsync(FieldDocumentationElement field, CancellationToken cancellationToken)
        {
            try
            {
                VisitField(field);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits a <see cref="ConstructorDocumentationElement"/>.</summary>
        /// <param name="constructor">The <see cref="ConstructorDocumentationElement"/> to visit.</param>
        protected internal override void VisitConstructor(ConstructorDocumentationElement constructor)
        {
        }

        /// <summary>Visits a <see cref="ConstructorDocumentationElement"/>.</summary>
        /// <param name="constructor">The <see cref="ConstructorDocumentationElement"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitConstructorAsync(ConstructorDocumentationElement constructor, CancellationToken cancellationToken)
        {
            try
            {
                VisitConstructor(constructor);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits a <see cref="EventDocumentationElement"/>.</summary>
        /// <param name="event">The <see cref="EventDocumentationElement"/> to visit.</param>
        protected internal override void VisitEvent(EventDocumentationElement @event)
        {
        }

        /// <summary>Visits a <see cref="EventDocumentationElement"/>.</summary>
        /// <param name="event">The <see cref="EventDocumentationElement"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitEventAsync(EventDocumentationElement @event, CancellationToken cancellationToken)
        {
            try
            {
                VisitEvent(@event);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits a <see cref="PropertyDocumentationElement"/>.</summary>
        /// <param name="property">The <see cref="PropertyDocumentationElement"/> to visit.</param>
        protected internal override void VisitProperty(PropertyDocumentationElement property)
        {
        }

        /// <summary>Visits a <see cref="PropertyDocumentationElement"/>.</summary>
        /// <param name="property">The <see cref="PropertyDocumentationElement"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitPropertyAsync(PropertyDocumentationElement property, CancellationToken cancellationToken)
        {
            try
            {
                VisitProperty(property);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits a <see cref="MethodDocumentationElement"/>.</summary>
        /// <param name="method">The <see cref="MethodDocumentationElement"/> to visit.</param>
        protected internal override void VisitMethod(MethodDocumentationElement method)
        {
        }

        /// <summary>Visits a <see cref="MethodDocumentationElement"/>.</summary>
        /// <param name="method">The <see cref="MethodDocumentationElement"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitMethodAsync(MethodDocumentationElement method, CancellationToken cancellationToken)
        {
            try
            {
                VisitMethod(method);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the beginning of a summary element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the summary element.</param>
        protected internal override void VisitSummaryBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _jsonWriter.WritePropertyName("summary");
            _jsonWriter.WriteStartObject();

            _WriteXmlAttributes(xmlAttributes);

            _jsonWriter.WritePropertyName("content");
            _jsonWriter.WriteStartArray();
        }

        /// <summary>Visits the beginning of a summary element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the summary element.</param>
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

        /// <summary>Visits the beginning of a returns element.</summary>
        /// <param name="returnType">The return type of the method.</param>
        protected internal override void VisitReturnsBeginning(TypeReferenceData returnType)
        {
        }

        /// <summary>Visits the beginning of a returns element.</summary>
        /// <param name="returnType">The return type of the method.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitReturnsBeginningAsync(TypeReferenceData returnType, CancellationToken cancellationToken)
        {
            try
            {
                VisitReturnsBeginning(returnType);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the ending of a returns element.</summary>
        protected internal override void VisitReturnsEnding()
        {
        }

        /// <summary>Visits the ending of a returns element.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitReturnsEndingAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitReturnsEnding();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the beginning of a remarks element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the remarks element.</param>
        protected internal override void VisitRemarksBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _jsonWriter.WritePropertyName("remarks");
            _jsonWriter.WriteStartObject();

            _WriteXmlAttributes(xmlAttributes);

            _jsonWriter.WritePropertyName("content");
            _jsonWriter.WriteStartArray();
        }

        /// <summary>Visits the beginning of a remarks element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the remarks element.</param>
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
        protected internal override void VisitExampleBeginning()
        {
        }

        /// <summary>Visits the beginning of an example element.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitExampleBeginningAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitExampleBeginning();
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
        protected internal override void VisitValueBeginning()
        {
        }

        /// <summary>Visits the beginning of a value element.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitValueBeginningAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitValueBeginning();
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
        protected internal override void VisitParagraphBeginning()
        {
        }

        /// <summary>Visits the beginning of a paragraph element.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitParagraphBeginningAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitParagraphBeginning();
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
        protected internal override void VisitCodeBlock(string code)
        {
        }

        /// <summary>Visits a code block element.</summary>
        /// <param name="code">The text inside the code block.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitCodeBlockAsync(string code, CancellationToken cancellationToken)
        {
            try
            {
                VisitCodeBlock(code);
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
        protected internal override void VisitUnorderedListBeginning()
        {
        }

        /// <summary>Visits the beginning of an unordered list element.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitUnorderedListBeginningAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitUnorderedListBeginning();
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
        protected internal override void VisitOrderedListBeginning()
        {
        }

        /// <summary>Visits the beginning of an ordered list element.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitOrderedListBeginningAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitOrderedListBeginning();
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
        protected internal override void VisitListItemBeginning()
        {
        }

        /// <summary>Visits the beginning of a list item element.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitListItemBeginningAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitListItemBeginning();
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
        protected internal override void VisitDefinitionListBeginning()
        {
        }

        /// <summary>Visits the beginning of a definition list element.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitDefinitionListBeginningAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitDefinitionListBeginning();
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
        protected internal override void VisitDefinitionListTitleBeginning()
        {
        }

        /// <summary>Visits the beginning of a definition list title.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitDefinitionListTitleBeginningAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitDefinitionListTitleBeginning();
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
        protected internal override void VisitDefinitionListItemBeginning()
        {
        }

        /// <summary>Visits the beginning of a definition list item.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitDefinitionListItemBeginningAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitDefinitionListItemBeginning();
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
        protected internal override void VisitDefinitionTermBeginning()
        {
        }

        /// <summary>Visits the beginning of a definition list term.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitDefinitionTermBeginningAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitDefinitionTermBeginning();
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
        protected internal override void VisitDefinitionTermDescriptionBeginning()
        {
        }

        /// <summary>Visits the beginning of a definition list term description.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitDefinitionTermDescriptionBeginningAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitDefinitionTermDescriptionBeginning();
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
        protected internal override void VisitTableBeginning()
        {
        }

        /// <summary>Visits the beginning of a table.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitTableBeginningAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitTableBeginning();
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
        protected internal override void VisitTableColumnBeginning()
        {
        }

        /// <summary>Visits the beginning of a table column.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitTableColumnBeginningAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitTableColumnBeginning();
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
        protected internal override void VisitTableRowBeginning()
        {
        }

        /// <summary>Visits the beginning of a table row.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitTableRowBeginningAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitTableRowBeginning();
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
        protected internal override void VisitTableCellBeginning()
        {
        }

        /// <summary>Visits the beginning of a table cell.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitTableCellBeginningAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitTableCellBeginning();
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
        protected internal override void VisitInlineReference(string canonicalName)
        {
            throw new InvalidOperationException($"Could not find member from '{canonicalName}' canonical name. Override VisitInlineReference(string) or VisitInlineReferenceAsync(string, CancellationToken) to ignore this error.");
        }

        /// <summary>Visits an inline member reference.</summary>
        /// <param name="canonicalName">The canonical name of the referred member.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitInlineReferenceAsync(string canonicalName, CancellationToken cancellationToken)
        {
            try
            {
                VisitInlineReference(canonicalName);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits an inline member reference.</summary>
        /// <param name="referredMember">The referred member.</param>
        protected internal override void VisitInlineReference(MemberInfo referredMember)
        {
        }

        /// <summary>Visits an inline member reference.</summary>
        /// <param name="referredMember">The referred member.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitInlineReferenceAsync(MemberInfo referredMember, CancellationToken cancellationToken)
        {
            try
            {
                VisitInlineReference(referredMember);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits an inline code snippet.</summary>
        /// <param name="code">The text inside the inline code.</param>
        protected internal override void VisitInlineCode(string code)
        {
        }

        /// <summary>Visits an inline code snippet.</summary>
        /// <param name="code">The text inside the inline code.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitInlineCodeAsync(string code, CancellationToken cancellationToken)
        {
            try
            {
                VisitInlineCode(code);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits an inline parameter reference.</summary>
        /// <param name="parameterName">The name of the referred parameter.</param>
        protected internal override void VisitParameterReference(string parameterName)
        {
        }

        /// <summary>Visits an inline parameter reference.</summary>
        /// <param name="parameterName">The name of the referred parameter.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitParameterReferenceAsync(string parameterName, CancellationToken cancellationToken)
        {
            try
            {
                VisitParameterReference(parameterName);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits an inline generic parameter reference.</summary>
        /// <param name="genericParameterName">The name of the referred generic parameter.</param>
        protected internal override void VisitGenericParameterReference(string genericParameterName)
        {
        }

        /// <summary>Visits an inline generic parameter reference.</summary>
        /// <param name="genericParameterName">The name of the referred generic parameter.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitGenericParameterReferenceAsync(string genericParameterName, CancellationToken cancellationToken)
        {
            try
            {
                VisitGenericParameterReference(genericParameterName);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
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

        private void _WriteAttributes(IEnumerable<AttributeData> attributes)
        {
            _jsonWriter.WritePropertyName("attributes");
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
            if (parameter.Value == null)
                _jsonWriter.WriteNull();
            else
            {
                var valueType = parameter.Value.GetType();
                if (valueType.IsEnum || (Nullable.GetUnderlyingType(valueType)?.IsEnum ?? false))
                    _jsonWriter.WriteValue(parameter.Value.ToString());
                else
                    _jsonWriter.WriteValue(parameter.Value);
            }

            _jsonWriter.WritePropertyName("type");
            _WriteTypeReference(parameter.Type);

            _jsonWriter.WriteEndObject();
        }

        private async Task _WriteAttributeParameterAsync(AttributeParameterData parameter, CancellationToken cancellationToken)
        {
            await _jsonWriter.WriteStartObjectAsync(cancellationToken);

            await _jsonWriter.WritePropertyNameAsync("name", cancellationToken);
            await _jsonWriter.WriteValueAsync(parameter.Name, cancellationToken);

            await _jsonWriter.WritePropertyNameAsync("value", cancellationToken);
            if (parameter.Value == null)
                await _jsonWriter.WriteNullAsync(cancellationToken);
            else
            {
                var valueType = parameter.Value.GetType();
                if (valueType.IsEnum || (Nullable.GetUnderlyingType(valueType)?.IsEnum ?? false))
                    await _jsonWriter.WriteValueAsync(parameter.Value.ToString(), cancellationToken);
                else
                    await _jsonWriter.WriteValueAsync(parameter.Value, cancellationToken);
            }

            await _jsonWriter.WritePropertyNameAsync("type", cancellationToken);
            await _WriteTypeReferenceAsync(parameter.Type, cancellationToken);

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
    }
}