namespace CodeMap.DeclarationNodes
{
    /// <summary>Represents a visitor for traversing an assembly declaration tree.</summary>
    public abstract class DeclarationVisitor
    {
        /// <summary>Initializes a new instance of the <see cref="DeclarationVisitor"/> class.</summary>
        protected DeclarationVisitor()
        {
        }

        /// <summary>Visits an <see cref="AssemblyDeclaration"/>.</summary>
        /// <param name="assembly">The <see cref="AssemblyDeclaration"/> to visit.</param>
        protected internal abstract void VisitAssembly(AssemblyDeclaration assembly);

        /// <summary>Visits a <see cref="NamespaceDeclaration"/>.</summary>
        /// <param name="namespace">The <see cref="NamespaceDeclaration"/> to visit.</param>
        protected internal abstract void VisitNamespace(NamespaceDeclaration @namespace);

        /// <summary>Visits an <see cref="EnumDeclaration"/>.</summary>
        /// <param name="enum">The <see cref="EnumDeclaration"/> to visit.</param>
        protected internal abstract void VisitEnum(EnumDeclaration @enum);

        /// <summary>Visits a <see cref="DelegateDeclaration"/>.</summary>
        /// <param name="delegate">The <see cref="DelegateDeclaration"/> to visit.</param>
        protected internal abstract void VisitDelegate(DelegateDeclaration @delegate);

        /// <summary>Visits an <see cref="InterfaceDeclaration"/>.</summary>
        /// <param name="interface">The <see cref="InterfaceDeclaration"/> to visit.</param>
        protected internal abstract void VisitInterface(InterfaceDeclaration @interface);

        /// <summary>Visits a <see cref="ClassDeclaration"/>.</summary>
        /// <param name="class">The <see cref="ClassDeclaration"/> to visit.</param>
        protected internal abstract void VisitClass(ClassDeclaration @class);

        /// <summary>Visits a <see cref="StructDeclaration"/>.</summary>
        /// <param name="struct">The <see cref="StructDeclaration"/> to visit.</param>
        protected internal abstract void VisitStruct(StructDeclaration @struct);

        /// <summary>Visits a <see cref="ConstantDeclaration"/>.</summary>
        /// <param name="constant">The <see cref="ConstantDeclaration"/> to visit.</param>
        protected internal abstract void VisitConstant(ConstantDeclaration constant);

        /// <summary>Visits a <see cref="FieldDeclaration"/>.</summary>
        /// <param name="field">The <see cref="FieldDeclaration"/> to visit.</param>
        protected internal abstract void VisitField(FieldDeclaration field);

        /// <summary>Visits a <see cref="ConstructorDeclaration"/>.</summary>
        /// <param name="constructor">The <see cref="ConstructorDeclaration"/> to visit.</param>
        protected internal abstract void VisitConstructor(ConstructorDeclaration constructor);

        /// <summary>Visits a <see cref="EventDeclaration"/>.</summary>
        /// <param name="event">The <see cref="EventDeclaration"/> to visit.</param>
        protected internal abstract void VisitEvent(EventDeclaration @event);

        /// <summary>Visits a <see cref="PropertyDeclaration"/>.</summary>
        /// <param name="property">The <see cref="PropertyDeclaration"/> to visit.</param>
        protected internal abstract void VisitProperty(PropertyDeclaration property);

        /// <summary>Visits a <see cref="MethodDeclaration"/>.</summary>
        /// <param name="method">The <see cref="MethodDeclaration"/> to visit.</param>
        protected internal abstract void VisitMethod(MethodDeclaration method);
    }
}