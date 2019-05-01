using System;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.ReferenceData
{
    /// <summary>Represents a visitor for <see cref="MemberReference"/> instances.</summary>
    public abstract class MemberReferenceVisitor
    {
        /// <summary>Visits the given <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="TypeReference"/> to visit.</param>
        protected internal abstract void VisitType(TypeReference type);

        /// <summary>Asynchronously visits the given <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="TypeReference"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitTypeAsync(TypeReference type, CancellationToken cancellationToken)
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
        protected internal abstract void VisitArray(ArrayTypeReference array);

        /// <summary>Asynchronously visits the given <paramref name="array"/>.</summary>
        /// <param name="array">The <see cref="ArrayTypeReference"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitArrayAsync(ArrayTypeReference array, CancellationToken cancellationToken)
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
        protected internal abstract void VisitPointer(PointerTypeReference pointer);

        /// <summary>Asynchronously visits the given <paramref name="pointer"/>.</summary>
        /// <param name="pointer">The <see cref="PointerTypeReference"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitPointerAsync(PointerTypeReference pointer, CancellationToken cancellationToken)
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
        protected internal abstract void VisitByRef(ByRefTypeReference byRef);

        /// <summary>Asynchronously visits the given <paramref name="byRef"/>.</summary>
        /// <param name="byRef">The <see cref="ByRefTypeReference"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitByRefAsync(ByRefTypeReference byRef, CancellationToken cancellationToken)
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
        protected internal abstract void VisitGenericTypeParameter(GenericTypeParameterReference genericTypeParameter);

        /// <summary>Asynchronously visits the given <paramref name="genericTypeParameter"/>.</summary>
        /// <param name="genericTypeParameter">The <see cref="GenericTypeParameterReference"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitGenericTypeParameterAsync(GenericTypeParameterReference genericTypeParameter, CancellationToken cancellationToken)
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

        /// <summary>Visits the given <paramref name="constant"/>.</summary>
        /// <param name="constant">The <see cref="ConstantReference"/> to visit.</param>
        protected internal abstract void VisitConstant(ConstantReference constant);

        /// <summary>Asynchronously visits the given <paramref name="constant"/>.</summary>
        /// <param name="constant">The <see cref="ConstantReference"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitConstantAsync(ConstantReference constant, CancellationToken cancellationToken)
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
        protected internal abstract void VisitField(FieldReference field);

        /// <summary>Asynchronously visits the given <paramref name="field"/>.</summary>
        /// <param name="field">The <see cref="FieldReference"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitFieldAsync(FieldReference field, CancellationToken cancellationToken)
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
        protected internal abstract void VisitConstructor(ConstructorReference constructor);

        /// <summary>Asynchronously visits the given <paramref name="constructor"/>.</summary>
        /// <param name="constructor">The <see cref="ConstructorReference"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitConstructorAsync(ConstructorReference constructor, CancellationToken cancellationToken)
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
        protected internal abstract void VisitEvent(EventReference @event);

        /// <summary>Asynchronously visits the given <paramref name="event"/>.</summary>
        /// <param name="event">The <see cref="EventReference"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitEventAsync(EventReference @event, CancellationToken cancellationToken)
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
    }
}