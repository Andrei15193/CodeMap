using CodeMap.ReferenceData;

namespace CodeMap.Html
{
    /// <summary>Helper methods for generating HTML pages.</summary>
    public static class MemberReferenceExtensions
    {
        /// <summary>Gets the simple name reference of the provided <paramref name="memberReference"/>.</summary>
        /// <param name="memberReference">The <see cref="MemberReference"/> for which to get the simple name reference.</param>
        /// <returns>Returns the simple name reference of the provided <paramref name="memberReference"/>.</returns>
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
        /// <seealso cref="DeclarationNodeExtensions.GetSimpleNameReference(DeclarationNodes.DeclarationNode)"/>
        public static string GetSimpleNameReference(this MemberReference memberReference)
        {
            var memberReferenceVisitor = new SimpleNameMemberReferenceVisitor();
            memberReference.Accept(memberReferenceVisitor);
            return memberReferenceVisitor.StringBuilder.ToString();
        }

        /// <summary>Gets the full name reference of the provided <paramref name="memberReference"/>.</summary>
        /// <param name="memberReference">The <see cref="MemberReference"/> for which to get the full name reference.</param>
        /// <returns>Returns the full name reference of the provided <paramref name="memberReference"/>.</returns>
        /// <remarks>
        /// <para>
        /// Unlike <see cref="GetSimpleNameReference(MemberReference)"/>, this gets the full name reference. This includes the
        /// namespace as well. Types do not have alias mapping, a reference to <c>int</c> will generate <c>System.Int32</c>.
        /// </para>
        /// <para>
        /// This method is particularly useful for IDs as it will generate unique outputs for each member of an <see cref="System.Reflection.Assembly"/>.
        /// </para>
        /// </remarks>
        /// <seealso cref="DeclarationNodeExtensions.GetFullNameReference(DeclarationNodes.DeclarationNode)"/>
        public static string GetFullNameReference(this MemberReference memberReference)
        {
            var memberReferenceVisitor = new FullNameMemberReferenceVisitor();
            memberReference.Accept(memberReferenceVisitor);
            return memberReferenceVisitor.StringBuilder.ToString();
        }
    }
}