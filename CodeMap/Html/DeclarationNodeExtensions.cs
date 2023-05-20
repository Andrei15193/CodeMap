using CodeMap.DeclarationNodes;

namespace CodeMap.Html
{
    /// <summary>Helper methods for generating HTML pages.</summary>
    public static class DeclarationNodeExtensions
    {
        /// <summary>Gets the simple name reference of the provided <paramref name="declarationNode"/>.</summary>
        /// <param name="declarationNode">The <see cref="DeclarationNode"/> for which to get the simple name reference.</param>
        /// <returns>Returns the simple name reference of the provided <paramref name="declarationNode"/>.</returns>
        /// <remarks>
        /// <para>
        /// A simple name reference consists of just the type name and generic parameters (if any), in case of member it is the
        /// declaring type followed by the member name, generic parameters (if any) and parameters (if any).
        /// </para>
        /// <para>
        /// The purpose is to get a simple name as one would normally write it in code, type aliases are covered as well.
        /// Instead of having <c>Int32</c> as a simple name reference this will retrieve <c>int</c> instead, just list one
        /// would write it in code.
        /// </para>
        /// </remarks>
        /// <seealso cref="MemberReferenceExtensions.GetSimpleNameReference(ReferenceData.MemberReference)"/>
        public static string GetSimpleNameReference(this DeclarationNode declarationNode)
        {
            var declarationNodeVisitor = new SimpleNameDeclarationNodeVisitor();
            declarationNode.Accept(declarationNodeVisitor);
            return declarationNodeVisitor.StringBuilder.ToString();
        }

        /// <summary>Gets the full name reference of the provided <paramref name="declarationNode"/>.</summary>
        /// <param name="declarationNode">The <see cref="DeclarationNode"/> for which to get the full name reference.</param>
        /// <returns>Returns the full name reference of the provided <paramref name="declarationNode"/>.</returns>
        /// <remarks>
        /// <para>
        /// Unlike <see cref="GetSimpleNameReference(DeclarationNode)"/>, this gets the full name reference. This includes the
        /// namespace as well. Types do not have alias mapping, a reference to <c>int</c> will generate <c>System.Int32</c>.
        /// </para>
        /// <para>
        /// This method is particularly useful for IDs as it will generate unique outputs for each member of an <see cref="System.Reflection.Assembly"/>.
        /// </para>
        /// </remarks>
        /// <seealso cref="MemberReferenceExtensions.GetFullNameReference(ReferenceData.MemberReference)"/>
        public static string GetFullNameReference(this DeclarationNode declarationNode)
        {
            var declarationNodeVisitor = new FullNameDeclarationNodeVisitor();
            declarationNode.Accept(declarationNodeVisitor);
            return declarationNodeVisitor.StringBuilder.ToString();
        }
    }
}