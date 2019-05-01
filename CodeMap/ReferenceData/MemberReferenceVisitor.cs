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

        /// <summary>Visits the given <paramref name="arrayType"/>.</summary>
        /// <param name="arrayType">The <see cref="ArrayTypeReference"/> to visit.</param>
        protected internal abstract void VisitArrayType(ArrayTypeReference arrayType);

        /// <summary>Asynchronously visits the given <paramref name="arrayType"/>.</summary>
        /// <param name="arrayType">The <see cref="ArrayTypeReference"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitArrayTypeAsync(ArrayTypeReference arrayType, CancellationToken cancellationToken)
        {
            try
            {
                VisitArrayType(arrayType);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the given <paramref name="pointerType"/>.</summary>
        /// <param name="pointerType">The <see cref="PointerTypeReference"/> to visit.</param>
        protected internal abstract void VisitPointerType(PointerTypeReference pointerType);

        /// <summary>Asynchronously visits the given <paramref name="pointerType"/>.</summary>
        /// <param name="pointerType">The <see cref="PointerTypeReference"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitPointerTypeAsync(PointerTypeReference pointerType, CancellationToken cancellationToken)
        {
            try
            {
                VisitPointerType(pointerType);
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
    }
}