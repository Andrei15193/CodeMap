using CodeMap.DeclarationNodes;

namespace CodeMap.Documentation
{
    public static class DeclarationNodeVisitorExtensions
    {
        public static string GetSimpleNameReference(this DeclarationNode declarationNode)
        {
            var declarationNodeVisitor = new SimpleNameDeclarationNodeVisitor();
            declarationNode.Accept(declarationNodeVisitor);
            return declarationNodeVisitor.StringBuilder.ToString();
        }

        public static string GetFullNameReference(this DeclarationNode declarationNode)
        {
            var declarationNodeVisitor = new FullNameDeclarationNodeVisitor();
            declarationNode.Accept(declarationNodeVisitor);
            return declarationNodeVisitor.StringBuilder.ToString();
        }
    }
}