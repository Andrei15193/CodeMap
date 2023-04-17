using System;
using System.IO;
using CodeMap.DeclarationNodes;

namespace CodeMap.Documentation
{
    public abstract class FileWriterDeclarationNodeVisitor : DeclarationNodeVisitor
    {
        public static FileWriterDeclarationNodeVisitor Create(Func<DeclarationNode, TextWriter> fileWriterStreamFactory, Func<DeclarationNode, TextWriter, DeclarationNodeVisitor> declarationWriterNodeVisitorFactory)
            => new CallbackFileWriterDeclarationNodeVisitor(
                fileWriterStreamFactory ?? throw new ArgumentNullException(nameof(fileWriterStreamFactory)),
                declarationWriterNodeVisitorFactory ?? throw new ArgumentNullException(nameof(declarationWriterNodeVisitorFactory))
            );

        protected abstract TextWriter GetFileWriterStreamFor(DeclarationNode declarationNode);

        protected abstract DeclarationNodeVisitor GetDeclarationWriterNodeVisitor(DeclarationNode declarationNode, TextWriter textWriter);

        protected override void VisitAssembly(AssemblyDeclaration assembly)
        {
            using var declarationTextWriter = GetFileWriterStreamFor(assembly);
            var declarationWriterVisitor = GetDeclarationWriterNodeVisitor(assembly, declarationTextWriter);
            assembly.Accept(declarationWriterVisitor);

            foreach (var @namespace in assembly.Namespaces)
                @namespace.Accept(this);
        }

        protected override void VisitNamespace(NamespaceDeclaration @namespace)
        {
            using var declarationTextWriter = GetFileWriterStreamFor(@namespace);
            var declarationWriterVisitor = GetDeclarationWriterNodeVisitor(@namespace, declarationTextWriter);
            @namespace.Accept(declarationWriterVisitor);

            foreach (var type in @namespace.DeclaredTypes)
                type.Accept(this);
        }

        protected override void VisitEnum(EnumDeclaration @enum)
        {
            using var declarationTextWriter = GetFileWriterStreamFor(@enum);
            var declarationWriterVisitor = GetDeclarationWriterNodeVisitor(@enum, declarationTextWriter);
            @enum.Accept(declarationWriterVisitor);
        }

        protected override void VisitDelegate(DelegateDeclaration @delegate)
        {
            using var declarationTextWriter = GetFileWriterStreamFor(@delegate);
            var declarationWriterVisitor = GetDeclarationWriterNodeVisitor(@delegate, declarationTextWriter);
            @delegate.Accept(declarationWriterVisitor);
        }

        protected override void VisitInterface(InterfaceDeclaration @interface)
        {
            using var declarationTextWriter = GetFileWriterStreamFor(@interface);
            var declarationWriterVisitor = GetDeclarationWriterNodeVisitor(@interface, declarationTextWriter);
            @interface.Accept(declarationWriterVisitor);

            foreach (var member in @interface.Members)
                member.Accept(this);
        }

        protected override void VisitClass(ClassDeclaration @class)
        {
            using var declarationTextWriter = GetFileWriterStreamFor(@class);
            var declarationWriterVisitor = GetDeclarationWriterNodeVisitor(@class, declarationTextWriter);
            @class.Accept(declarationWriterVisitor);

            foreach (var member in @class.Members)
                member.Accept(this);
        }

        protected override void VisitRecord(RecordDeclaration record)
        {
            using var declarationTextWriter = GetFileWriterStreamFor(record);
            var declarationWriterVisitor = GetDeclarationWriterNodeVisitor(record, declarationTextWriter);
            record.Accept(declarationWriterVisitor);

            foreach (var member in record.Members)
                member.Accept(this);
        }

        protected override void VisitStruct(StructDeclaration @struct)
        {
            using var declarationTextWriter = GetFileWriterStreamFor(@struct);
            var declarationWriterVisitor = GetDeclarationWriterNodeVisitor(@struct, declarationTextWriter);
            @struct.Accept(declarationWriterVisitor);

            foreach (var member in @struct.Members)
                member.Accept(this);
        }

        protected override void VisitConstant(ConstantDeclaration constant)
        {
            using var declarationTextWriter = GetFileWriterStreamFor(constant);
            var declarationWriterVisitor = GetDeclarationWriterNodeVisitor(constant, declarationTextWriter);
            constant.Accept(declarationWriterVisitor);
        }

        protected override void VisitField(FieldDeclaration field)
        {
            using var declarationTextWriter = GetFileWriterStreamFor(field);
            var declarationWriterVisitor = GetDeclarationWriterNodeVisitor(field, declarationTextWriter);
            field.Accept(declarationWriterVisitor);
        }

        protected override void VisitConstructor(ConstructorDeclaration constructor)
        {
            using var declarationTextWriter = GetFileWriterStreamFor(constructor);
            var declarationWriterVisitor = GetDeclarationWriterNodeVisitor(constructor, declarationTextWriter);
            constructor.Accept(declarationWriterVisitor);
        }

        protected override void VisitEvent(EventDeclaration @event)
        {
            using var declarationTextWriter = GetFileWriterStreamFor(@event);
            var declarationWriterVisitor = GetDeclarationWriterNodeVisitor(@event, declarationTextWriter);
            @event.Accept(declarationWriterVisitor);
        }

        protected override void VisitProperty(PropertyDeclaration property)
        {
            using var declarationTextWriter = GetFileWriterStreamFor(property);
            var declarationWriterVisitor = GetDeclarationWriterNodeVisitor(property, declarationTextWriter);
            property.Accept(declarationWriterVisitor);
        }

        protected override void VisitMethod(MethodDeclaration method)
        {
            using var declarationTextWriter = GetFileWriterStreamFor(method);
            var declarationWriterVisitor = GetDeclarationWriterNodeVisitor(method, declarationTextWriter);
            method.Accept(declarationWriterVisitor);
        }

        private sealed class CallbackFileWriterDeclarationNodeVisitor : FileWriterDeclarationNodeVisitor
        {
            private readonly Func<DeclarationNode, TextWriter> _fileWriterStreamFactory;
            private readonly Func<DeclarationNode, TextWriter, DeclarationNodeVisitor> _declarationWriterNodeVisitorFactory;

            public CallbackFileWriterDeclarationNodeVisitor(Func<DeclarationNode, TextWriter> fileWriterStreamFactory, Func<DeclarationNode, TextWriter, DeclarationNodeVisitor> declarationWriterNodeVisitorFactory)
            {
                _fileWriterStreamFactory = fileWriterStreamFactory;
                _declarationWriterNodeVisitorFactory = declarationWriterNodeVisitorFactory;
            }

            protected override TextWriter GetFileWriterStreamFor(DeclarationNode declarationNode)
                => _fileWriterStreamFactory(declarationNode);

            protected override DeclarationNodeVisitor GetDeclarationWriterNodeVisitor(DeclarationNode declarationNode, TextWriter textWriter)
                => _declarationWriterNodeVisitorFactory(declarationNode, textWriter);
        }
    }
}