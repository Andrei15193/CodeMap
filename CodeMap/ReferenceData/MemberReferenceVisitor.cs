namespace CodeMap.ReferenceData
{
    /// <summary>Represents a visitor for <see cref="MemberReference"/> instances.</summary>
    public abstract class MemberReferenceVisitor
    {
        /// <summary>Visits the given <paramref name="assembly"/>.</summary>
        /// <param name="assembly">The <see cref="AssemblyReference"/> to visit.</param>
        protected internal abstract void VisitAssembly(AssemblyReference assembly);

        /// <summary>Visits the given <paramref name="namespace"/>.</summary>
        /// <param name="namespace">The <see cref="NamespaceReference"/> to visit.</param>
        protected internal abstract void VisitNamespace(NamespaceReference @namespace);

        /// <summary>Visits the given <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="TypeReference"/> to visit.</param>
        protected internal abstract void VisitType(TypeReference type);

        /// <summary>Visits the given <paramref name="array"/>.</summary>
        /// <param name="array">The <see cref="ArrayTypeReference"/> to visit.</param>
        protected internal abstract void VisitArray(ArrayTypeReference array);

        /// <summary>Visits the given <paramref name="pointer"/>.</summary>
        /// <param name="pointer">The <see cref="PointerTypeReference"/> to visit.</param>
        protected internal abstract void VisitPointer(PointerTypeReference pointer);

        /// <summary>Visits the given <paramref name="byRef"/>.</summary>
        /// <param name="byRef">The <see cref="ByRefTypeReference"/> to visit.</param>
        protected internal abstract void VisitByRef(ByRefTypeReference byRef);

        /// <summary>Visits the given <paramref name="genericTypeParameter"/>.</summary>
        /// <param name="genericTypeParameter">The <see cref="GenericTypeParameterReference"/> to visit.</param>
        protected internal abstract void VisitGenericTypeParameter(GenericTypeParameterReference genericTypeParameter);

        /// <summary>Visits the given <paramref name="genericMethodParameter"/>.</summary>
        /// <param name="genericMethodParameter">The <see cref="GenericMethodParameterReference"/> to visit.</param>
        protected internal abstract void VisitGenericMethodParameter(GenericMethodParameterReference genericMethodParameter);

        /// <summary>Visits the given <paramref name="constant"/>.</summary>
        /// <param name="constant">The <see cref="ConstantReference"/> to visit.</param>
        protected internal abstract void VisitConstant(ConstantReference constant);

        /// <summary>Visits the given <paramref name="field"/>.</summary>
        /// <param name="field">The <see cref="FieldReference"/> to visit.</param>
        protected internal abstract void VisitField(FieldReference field);

        /// <summary>Visits the given <paramref name="constructor"/>.</summary>
        /// <param name="constructor">The <see cref="ConstructorReference"/> to visit.</param>
        protected internal abstract void VisitConstructor(ConstructorReference constructor);

        /// <summary>Visits the given <paramref name="event"/>.</summary>
        /// <param name="event">The <see cref="EventReference"/> to visit.</param>
        protected internal abstract void VisitEvent(EventReference @event);

        /// <summary>Visits the given <paramref name="property"/>.</summary>
        /// <param name="property">The <see cref="PropertyReference"/> to visit.</param>
        protected internal abstract void VisitProperty(PropertyReference property);

        /// <summary>Visits the given <paramref name="method"/>.</summary>
        /// <param name="method">The <see cref="MethodReference"/> to visit.</param>
        protected internal abstract void VisitMethod(MethodReference method);
    }
}