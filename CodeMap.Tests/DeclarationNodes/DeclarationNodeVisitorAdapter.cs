using CodeMap.DeclarationNodes;

namespace CodeMap.Tests.DeclarationNodes
{
    public class DeclarationNodeVisitorAdapter : DeclarationNodeVisitor
    {
        private readonly IDeclarationNodeVisitor _visitor;

        public DeclarationNodeVisitorAdapter(IDeclarationNodeVisitor visitor)
        {
            _visitor = visitor;
        }

        protected override void VisitAssembly(AssemblyDeclaration assembly)
            => _visitor.VisitAssembly(assembly);

        protected override void VisitNamespace(NamespaceDeclaration @namespace)
            => _visitor.VisitNamespace(@namespace);

        protected override void VisitEnum(EnumDeclaration @enum)
            => _visitor.VisitEnum(@enum);

        protected override void VisitDelegate(DelegateDeclaration @delegate)
            => _visitor.VisitDelegate(@delegate);

        protected override void VisitInterface(InterfaceDeclaration @interface)
            => _visitor.VisitInterface(@interface);

        protected override void VisitClass(ClassDeclaration @class)
            => _visitor.VisitClass(@class);

        protected override void VisitStruct(StructDeclaration @struct)
            => _visitor.VisitStruct(@struct);

        protected override void VisitConstant(ConstantDeclaration constant)
            => _visitor.VisitConstant(constant);

        protected override void VisitField(FieldDeclaration field)
            => _visitor.VisitField(field);

        protected override void VisitConstructor(ConstructorDeclaration constructor)
            => _visitor.VisitConstructor(constructor);

        protected override void VisitEvent(EventDeclaration @event)
            => _visitor.VisitEvent(@event);

        protected override void VisitProperty(PropertyDeclaration property)
            => _visitor.VisitProperty(property);

        protected override void VisitMethod(MethodDeclaration method)
            => _visitor.VisitMethod(method);
    }
}