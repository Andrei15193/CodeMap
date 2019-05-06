using CodeMap.ReferenceData;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap
{
    /// <summary>Represents a JSON member reference writer visitor for storing <see cref="MemberReference"/>s as JSON.</summary>
    public class JsonWriterMemberReferenceVisitor : MemberReferenceVisitor
    {
        private readonly JsonWriter _jsonWriter;

        /// <summary>Initializes a new instance of the <see cref="JsonWriterMemberReferenceVisitor"/> class.</summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write the visited <see cref="MemberReference"/> to.</param>
        public JsonWriterMemberReferenceVisitor(JsonWriter jsonWriter)
        {
            _jsonWriter = jsonWriter ?? throw new ArgumentNullException(nameof(jsonWriter));
        }

        /// <summary>Visits the given <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="TypeReference"/> to visit.</param>
        protected internal override void VisitType(TypeReference type)
        {
            _jsonWriter.WriteStartObject();

            if (type is VoidTypeReference)
                _jsonWriter.WriteProperty("kind", "specific/void");
            else if (type is DynamicTypeReference)
                _jsonWriter.WriteProperty("kind", "specific/dynamic");
            else
            {
                _jsonWriter.WriteProperty("kind", "specific");
                _jsonWriter.WriteProperty("name", type.Name);
                _jsonWriter.WriteProperty("namespace", type.Namespace);
                _jsonWriter.WriteProperty("declaringType", this, type.DeclaringType);
                _jsonWriter.WriteProperty("genericArguments", this, type.GenericArguments);
                _jsonWriter.WriteProperty("assembly", this, type.Assembly);
            }

            _jsonWriter.WriteEndObject();
        }

        /// <summary>Asynchronously visits the given <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="TypeReference"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override async Task VisitTypeAsync(TypeReference type, CancellationToken cancellationToken)
        {
            await _jsonWriter.WriteStartObjectAsync(cancellationToken).ConfigureAwait(false);

            if (type is VoidTypeReference)
                await _jsonWriter.WritePropertyAsync("kind", "specific/void", cancellationToken).ConfigureAwait(false);
            else if (type is DynamicTypeReference)
                await _jsonWriter.WritePropertyAsync("kind", "specific/dynamic", cancellationToken).ConfigureAwait(false);
            else
            {
                await _jsonWriter.WritePropertyAsync("kind", "specific", cancellationToken).ConfigureAwait(false);
                await _jsonWriter.WritePropertyAsync("name", type.Name, cancellationToken).ConfigureAwait(false);
                await _jsonWriter.WritePropertyAsync("namespace", type.Namespace, cancellationToken).ConfigureAwait(false);
                await _jsonWriter.WritePropertyAsync("declaringType", this, type.DeclaringType, cancellationToken).ConfigureAwait(false);
                await _jsonWriter.WritePropertyAsync("genericArguments", this, type.GenericArguments, cancellationToken).ConfigureAwait(false);
                await _jsonWriter.WritePropertyAsync("assembly", this, type.Assembly, cancellationToken).ConfigureAwait(false);
            }

            await _jsonWriter.WriteEndObjectAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>Visits the given <paramref name="array"/>.</summary>
        /// <param name="array">The <see cref="ArrayTypeReference"/> to visit.</param>
        protected internal override void VisitArray(ArrayTypeReference array)
        {
            _jsonWriter.WriteStartObject();
            _jsonWriter.WriteProperty("kind", "array");
            _jsonWriter.WriteProperty("rank", array.Rank);
            _jsonWriter.WriteProperty("itemType", this, array.ItemType);
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Asynchronously visits the given <paramref name="array"/>.</summary>
        /// <param name="array">The <see cref="ArrayTypeReference"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override async Task VisitArrayAsync(ArrayTypeReference array, CancellationToken cancellationToken)
        {
            await _jsonWriter.WriteStartObjectAsync(cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WritePropertyAsync("kind", "array", cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WritePropertyAsync("rank", array.Rank, cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WritePropertyAsync("itemType", this, array.ItemType, cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WriteEndObjectAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>Visits the given <paramref name="pointer"/>.</summary>
        /// <param name="pointer">The <see cref="PointerTypeReference"/> to visit.</param>
        protected internal override void VisitPointer(PointerTypeReference pointer)
        {
            _jsonWriter.WriteStartObject();
            _jsonWriter.WriteProperty("kind", "pointer");
            _jsonWriter.WriteProperty("referentType", this, pointer.ReferentType);
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Asynchronously visits the given <paramref name="pointer"/>.</summary>
        /// <param name="pointer">The <see cref="PointerTypeReference"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override async Task VisitPointerAsync(PointerTypeReference pointer, CancellationToken cancellationToken)
        {
            await _jsonWriter.WriteStartObjectAsync(cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WritePropertyAsync("kind", "pointer", cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WritePropertyAsync("referentType", this, pointer.ReferentType, cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WriteEndObjectAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>Visits the given <paramref name="byRef"/>.</summary>
        /// <param name="byRef">The <see cref="ByRefTypeReference"/> to visit.</param>
        protected internal override void VisitByRef(ByRefTypeReference byRef)
        {
            _jsonWriter.WriteStartObject();
            _jsonWriter.WriteProperty("kind", "byRef");
            _jsonWriter.WriteProperty("referentType", this, byRef.ReferentType);
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Asynchronously visits the given <paramref name="byRef"/>.</summary>
        /// <param name="byRef">The <see cref="ByRefTypeReference"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override async Task VisitByRefAsync(ByRefTypeReference byRef, CancellationToken cancellationToken)
        {
            await _jsonWriter.WriteStartObjectAsync(cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WritePropertyAsync("kind", "byRef", cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WritePropertyAsync("referentType", this, byRef.ReferentType, cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WriteEndObjectAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>Visits the given <paramref name="genericTypeParameter"/>.</summary>
        /// <param name="genericTypeParameter">The <see cref="GenericTypeParameterReference"/> to visit.</param>
        protected internal override void VisitGenericTypeParameter(GenericTypeParameterReference genericTypeParameter)
        {
            _jsonWriter.WriteStartObject();
            _jsonWriter.WriteProperty("kind", "genericTypeParameter");
            _jsonWriter.WriteProperty("name", genericTypeParameter.Name);
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Asynchronously visits the given <paramref name="genericTypeParameter"/>.</summary>
        /// <param name="genericTypeParameter">The <see cref="GenericTypeParameterReference"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override async Task VisitGenericTypeParameterAsync(GenericTypeParameterReference genericTypeParameter, CancellationToken cancellationToken)
        {
            await _jsonWriter.WriteStartObjectAsync(cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WritePropertyAsync("kind", "genericTypeParameter", cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WritePropertyAsync("name", genericTypeParameter.Name, cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WriteEndObjectAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>Visits the given <paramref name="genericMethodParameter"/>.</summary>
        /// <param name="genericMethodParameter">The <see cref="GenericMethodParameterReference"/> to visit.</param>
        protected internal override void VisitGenericMethodParameter(GenericMethodParameterReference genericMethodParameter)
        {
            _jsonWriter.WriteStartObject();
            _jsonWriter.WriteProperty("kind", "genericMethodParameter");
            _jsonWriter.WriteProperty("name", genericMethodParameter.Name);
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Asynchronously visits the given <paramref name="genericMethodParameter"/>.</summary>
        /// <param name="genericMethodParameter">The <see cref="GenericMethodParameterReference"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override async Task VisitGenericMethodParameterAsync(GenericMethodParameterReference genericMethodParameter, CancellationToken cancellationToken)
        {
            await _jsonWriter.WriteStartObjectAsync(cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WritePropertyAsync("kind", "genericMethodParameter", cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WritePropertyAsync("name", genericMethodParameter.Name, cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WriteEndObjectAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>Visits the given <paramref name="constant"/>.</summary>
        /// <param name="constant">The <see cref="ConstantReference"/> to visit.</param>
        protected internal override void VisitConstant(ConstantReference constant)
        {
            _jsonWriter.WriteStartObject();
            _jsonWriter.WriteProperty("kind", "constant");
            _jsonWriter.WriteProperty("name", constant.Name);
            _jsonWriter.WriteProperty("value", constant.Value);
            _jsonWriter.WriteProperty("declaringType", this, constant.DeclaringType);
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Asynchronously visits the given <paramref name="constant"/>.</summary>
        /// <param name="constant">The <see cref="ConstantReference"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override async Task VisitConstantAsync(ConstantReference constant, CancellationToken cancellationToken)
        {
            await _jsonWriter.WriteStartObjectAsync(cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WritePropertyAsync("kind", "constant", cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WritePropertyAsync("name", constant.Name, cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WritePropertyAsync("value", constant.Value, cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WritePropertyAsync("declaringType", this, constant.DeclaringType, cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WriteEndObjectAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>Visits the given <paramref name="field"/>.</summary>
        /// <param name="field">The <see cref="FieldReference"/> to visit.</param>
        protected internal override void VisitField(FieldReference field)
        {
            _jsonWriter.WriteStartObject();
            _jsonWriter.WriteProperty("kind", "field");
            _jsonWriter.WriteProperty("name", field.Name);
            _jsonWriter.WriteProperty("declaringType", this, field.DeclaringType);
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Asynchronously visits the given <paramref name="field"/>.</summary>
        /// <param name="field">The <see cref="FieldReference"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override async Task VisitFieldAsync(FieldReference field, CancellationToken cancellationToken)
        {
            await _jsonWriter.WriteStartObjectAsync(cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WritePropertyAsync("kind", "field", cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WritePropertyAsync("name", field.Name, cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WritePropertyAsync("declaringType", this, field.DeclaringType, cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WriteEndObjectAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>Visits the given <paramref name="constructor"/>.</summary>
        /// <param name="constructor">The <see cref="ConstructorReference"/> to visit.</param>
        protected internal override void VisitConstructor(ConstructorReference constructor)
        {
            _jsonWriter.WriteStartObject();
            _jsonWriter.WriteProperty("kind", "constructor");
            _jsonWriter.WriteProperty("declaringType", this, constructor.DeclaringType);
            _jsonWriter.WriteProperty("parameterTypes", this, constructor.ParameterTypes);
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Asynchronously visits the given <paramref name="constructor"/>.</summary>
        /// <param name="constructor">The <see cref="ConstructorReference"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override async Task VisitConstructorAsync(ConstructorReference constructor, CancellationToken cancellationToken)
        {
            await _jsonWriter.WriteStartObjectAsync(cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WritePropertyAsync("kind", "constructor", cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WritePropertyAsync("declaringType", this, constructor.DeclaringType, cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WritePropertyAsync("parameterTypes", this, constructor.ParameterTypes, cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WriteEndObjectAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>Visits the given <paramref name="event"/>.</summary>
        /// <param name="event">The <see cref="EventReference"/> to visit.</param>
        protected internal override void VisitEvent(EventReference @event)
        {
            _jsonWriter.WriteStartObject();
            _jsonWriter.WriteProperty("kind", "event");
            _jsonWriter.WriteProperty("name", @event.Name);
            _jsonWriter.WriteProperty("declaringType", this, @event.DeclaringType);
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Asynchronously visits the given <paramref name="event"/>.</summary>
        /// <param name="event">The <see cref="EventReference"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override async Task VisitEventAsync(EventReference @event, CancellationToken cancellationToken)
        {
            await _jsonWriter.WriteStartObjectAsync(cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WritePropertyAsync("kind", "event", cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WritePropertyAsync("name", @event.Name, cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WritePropertyAsync("declaringType", this, @event.DeclaringType, cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WriteEndObjectAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>Visits the given <paramref name="property"/>.</summary>
        /// <param name="property">The <see cref="PropertyReference"/> to visit.</param>
        protected internal override void VisitProperty(PropertyReference property)
        {
            _jsonWriter.WriteStartObject();
            _jsonWriter.WriteProperty("kind", "property");
            _jsonWriter.WriteProperty("name", property.Name);
            _jsonWriter.WriteProperty("declaringType", this, property.DeclaringType);
            _jsonWriter.WriteProperty("parameterTypes", this, property.ParameterTypes);
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Asynchronously visits the given <paramref name="property"/>.</summary>
        /// <param name="property">The <see cref="PropertyReference"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override async Task VisitPropertyAsync(PropertyReference property, CancellationToken cancellationToken)
        {
            await _jsonWriter.WriteStartObjectAsync(cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WritePropertyAsync("kind", "property", cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WritePropertyAsync("name", property.Name, cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WritePropertyAsync("declaringType", this, property.DeclaringType, cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WritePropertyAsync("parameterTypes", this, property.ParameterTypes, cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WriteEndObjectAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>Visits the given <paramref name="method"/>.</summary>
        /// <param name="method">The <see cref="MethodReference"/> to visit.</param>
        protected internal override void VisitMethod(MethodReference method)
        {
            _jsonWriter.WriteStartObject();
            _jsonWriter.WriteProperty("kind", "method");
            _jsonWriter.WriteProperty("name", method.Name);
            _jsonWriter.WriteProperty("declaringType", this, method.DeclaringType);
            _jsonWriter.WriteProperty("genericArguments", this, method.GenericArguments);
            _jsonWriter.WriteProperty("parameterTypes", this, method.ParameterTypes);
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Asynchronously visits the given <paramref name="method"/>.</summary>
        /// <param name="method">The <see cref="MethodReference"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override async Task VisitMethodAsync(MethodReference method, CancellationToken cancellationToken)
        {
            await _jsonWriter.WriteStartObjectAsync(cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WritePropertyAsync("kind", "method", cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WritePropertyAsync("name", method.Name, cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WritePropertyAsync("declaringType", this, method.DeclaringType, cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WritePropertyAsync("genericArguments", this, method.GenericArguments, cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WritePropertyAsync("parameterTypes", this, method.ParameterTypes, cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WriteEndObjectAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>Visits the given <paramref name="assembly"/>.</summary>
        /// <param name="assembly">The <see cref="AssemblyReference"/> to visit.</param>
        protected internal override void VisitAssembly(AssemblyReference assembly)
        {
            _jsonWriter.WriteStartObject();
            _jsonWriter.WriteProperty("name", assembly.Name);
            _jsonWriter.WriteProperty("version", assembly.Version.ToString());
            _jsonWriter.WriteProperty("culture", assembly.Culture);
            _jsonWriter.WriteProperty("publicKeyToken", assembly.PublicKeyToken);
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Asynchronously visits the given <paramref name="assembly"/>.</summary>
        /// <param name="assembly">The <see cref="AssemblyReference"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override async Task VisitAssemblyAsync(AssemblyReference assembly, CancellationToken cancellationToken)
        {
            await _jsonWriter.WriteStartObjectAsync(cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WritePropertyAsync("name", assembly.Name, cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WritePropertyAsync("version", assembly.Version.ToString(), cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WritePropertyAsync("culture", assembly.Culture, cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WritePropertyAsync("publicKeyToken", assembly.PublicKeyToken, cancellationToken).ConfigureAwait(false);
            await _jsonWriter.WriteEndObjectAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}