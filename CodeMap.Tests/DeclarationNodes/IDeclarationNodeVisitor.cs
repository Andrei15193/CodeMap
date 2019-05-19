using CodeMap.DeclarationNodes;

namespace CodeMap.Tests.DeclarationNodes
{
    public interface IDeclarationNodeVisitor
    {
        void VisitAssembly(AssemblyDeclaration assembly);

        void VisitNamespace(NamespaceDeclaration @namespace);

        void VisitEnum(EnumDeclaration @enum);

        void VisitDelegate(DelegateDeclaration @delegate);

        void VisitInterface(InterfaceDeclaration @interface);

        void VisitClass(ClassDeclaration @class);

        void VisitStruct(StructDeclaration @struct);

        void VisitConstant(ConstantDeclaration constant);

        void VisitField(FieldDeclaration field);

        void VisitConstructor(ConstructorDeclaration constructor);

        void VisitEvent(EventDeclaration @event);

        void VisitProperty(PropertyDeclaration property);

        void VisitMethod(MethodDeclaration method);
    }
}