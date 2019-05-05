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

            _jsonWriter.WriteProperty("kind", "specific");
            _jsonWriter.WriteProperty("name", type.Name);
            _jsonWriter.WriteProperty("namespace", type.Namespace);
            _jsonWriter.WritePropertyIfNotNull(
                "declaringType",
                type.DeclaringType,
                declaringType => declaringType.Accept(this)
            );
            _jsonWriter.WritePropertyCollection(
                "genericArguments",
                type.GenericArguments,
                genericArgument => genericArgument.Accept(this)
            );
            _WriteAssemblyReference(type.Assembly);

            _jsonWriter.WriteEndObject();
        }

        /// <summary>Asynchronously visits the given <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="TypeReference"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitTypeAsync(TypeReference type, CancellationToken cancellationToken)
        {
            try
            {
                VisitType(type);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the given <paramref name="array"/>.</summary>
        /// <param name="array">The <see cref="ArrayTypeReference"/> to visit.</param>
        protected internal override void VisitArray(ArrayTypeReference array)
        {
        }

        /// <summary>Asynchronously visits the given <paramref name="array"/>.</summary>
        /// <param name="array">The <see cref="ArrayTypeReference"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitArrayAsync(ArrayTypeReference array, CancellationToken cancellationToken)
        {
            try
            {
                VisitArray(array);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the given <paramref name="pointer"/>.</summary>
        /// <param name="pointer">The <see cref="PointerTypeReference"/> to visit.</param>
        protected internal override void VisitPointer(PointerTypeReference pointer)
        {
        }

        /// <summary>Asynchronously visits the given <paramref name="pointer"/>.</summary>
        /// <param name="pointer">The <see cref="PointerTypeReference"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitPointerAsync(PointerTypeReference pointer, CancellationToken cancellationToken)
        {
            try
            {
                VisitPointer(pointer);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the given <paramref name="byRef"/>.</summary>
        /// <param name="byRef">The <see cref="ByRefTypeReference"/> to visit.</param>
        protected internal override void VisitByRef(ByRefTypeReference byRef)
        {
        }

        /// <summary>Asynchronously visits the given <paramref name="byRef"/>.</summary>
        /// <param name="byRef">The <see cref="ByRefTypeReference"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitByRefAsync(ByRefTypeReference byRef, CancellationToken cancellationToken)
        {
            try
            {
                VisitByRef(byRef);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the given <paramref name="genericTypeParameter"/>.</summary>
        /// <param name="genericTypeParameter">The <see cref="GenericTypeParameterReference"/> to visit.</param>
        protected internal override void VisitGenericTypeParameter(GenericTypeParameterReference genericTypeParameter)
        {
        }

        /// <summary>Asynchronously visits the given <paramref name="genericTypeParameter"/>.</summary>
        /// <param name="genericTypeParameter">The <see cref="GenericTypeParameterReference"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitGenericTypeParameterAsync(GenericTypeParameterReference genericTypeParameter, CancellationToken cancellationToken)
        {
            try
            {
                VisitGenericTypeParameter(genericTypeParameter);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the given <paramref name="genericMethodParameter"/>.</summary>
        /// <param name="genericMethodParameter">The <see cref="GenericMethodParameterReference"/> to visit.</param>
        protected internal override void VisitGenericMethodParameter(GenericMethodParameterReference genericMethodParameter)
        {
        }

        /// <summary>Asynchronously visits the given <paramref name="genericMethodParameter"/>.</summary>
        /// <param name="genericMethodParameter">The <see cref="GenericMethodParameterReference"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitGenericMethodParameterAsync(GenericMethodParameterReference genericMethodParameter, CancellationToken cancellationToken)
        {
            try
            {
                VisitGenericMethodParameter(genericMethodParameter);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the given <paramref name="constant"/>.</summary>
        /// <param name="constant">The <see cref="ConstantReference"/> to visit.</param>
        protected internal override void VisitConstant(ConstantReference constant)
        {
        }

        /// <summary>Asynchronously visits the given <paramref name="constant"/>.</summary>
        /// <param name="constant">The <see cref="ConstantReference"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitConstantAsync(ConstantReference constant, CancellationToken cancellationToken)
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

        /// <summary>Visits the given <paramref name="field"/>.</summary>
        /// <param name="field">The <see cref="FieldReference"/> to visit.</param>
        protected internal override void VisitField(FieldReference field)
        {
        }

        /// <summary>Asynchronously visits the given <paramref name="field"/>.</summary>
        /// <param name="field">The <see cref="FieldReference"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitFieldAsync(FieldReference field, CancellationToken cancellationToken)
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

        /// <summary>Visits the given <paramref name="constructor"/>.</summary>
        /// <param name="constructor">The <see cref="ConstructorReference"/> to visit.</param>
        protected internal override void VisitConstructor(ConstructorReference constructor)
        {
        }

        /// <summary>Asynchronously visits the given <paramref name="constructor"/>.</summary>
        /// <param name="constructor">The <see cref="ConstructorReference"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitConstructorAsync(ConstructorReference constructor, CancellationToken cancellationToken)
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

        /// <summary>Visits the given <paramref name="event"/>.</summary>
        /// <param name="event">The <see cref="EventReference"/> to visit.</param>
        protected internal override void VisitEvent(EventReference @event)
        {
        }

        /// <summary>Asynchronously visits the given <paramref name="event"/>.</summary>
        /// <param name="event">The <see cref="EventReference"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitEventAsync(EventReference @event, CancellationToken cancellationToken)
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

        /// <summary>Visits the given <paramref name="property"/>.</summary>
        /// <param name="property">The <see cref="PropertyReference"/> to visit.</param>
        protected internal override void VisitProperty(PropertyReference property)
        {
        }

        /// <summary>Asynchronously visits the given <paramref name="property"/>.</summary>
        /// <param name="property">The <see cref="PropertyReference"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitPropertyAsync(PropertyReference property, CancellationToken cancellationToken)
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

        /// <summary>Visits the given <paramref name="method"/>.</summary>
        /// <param name="method">The <see cref="MethodReference"/> to visit.</param>
        protected internal override void VisitMethod(MethodReference method)
        {
        }

        /// <summary>Asynchronously visits the given <paramref name="method"/>.</summary>
        /// <param name="method">The <see cref="MethodReference"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal override Task VisitMethodAsync(MethodReference method, CancellationToken cancellationToken)
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

        private void _WriteAssemblyReference(AssemblyReference assembly)
        {
            _jsonWriter.WritePropertyName("assembly");
            _jsonWriter.WriteStartObject();
            _jsonWriter.WriteProperty("name", assembly.Name);
            _jsonWriter.WriteProperty("version", assembly.Version.ToString());
            _jsonWriter.WriteProperty("culture", assembly.Culture);
            _jsonWriter.WriteProperty("publicKeyToken", assembly.PublicKeyToken);
            _jsonWriter.WriteEndObject();
        }
    }
}