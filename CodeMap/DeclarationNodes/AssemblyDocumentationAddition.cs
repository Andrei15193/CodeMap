using System.Collections.Generic;
using CodeMap.DocumentationElements;

namespace CodeMap.DeclarationNodes
{
    /// <summary>Represents a documentation addition at the assembly level.</summary>
    /// <remarks>
    /// By default, all virtual methods return <c>null</c> which means that no documentation is
    /// being provided at that level (summary, remarks, examples and so on). This can be useful
    /// if the related documentation is generated based on some conditions, if this conditions
    /// are not met, simply return <c>null</c>.
    /// </remarks>
    /// <example title="Adding Assembly Documentation">
    /// <para>
    /// Unfortunately, there is no way to add documentation at the assembly level like we can
    /// add them at the type level or type member level. This class is specifically designed
    /// to address this issue by providing custom documentation for the assembly using the
    /// <see cref="DocumentationElement"/> hierarchy, the same structure that is used for type
    /// and type member documentation. The downside of this is that documentation elements are
    /// not directly created and need to be provided manually.
    /// </para>
    /// <para>
    /// Generally, we can assume that the summary of an assembly can be the assembly
    /// description itself. We can write an <see cref="AssemblyDocumentationAddition"/>
    /// implementation that will retrieve this information from the related attribute. The code
    /// bellow does exactly that and it is actually used to generate the documentation that
    /// you are reading.
    /// </para>
    /// <code language="c#">
    /// public class CodeMapAssemblyDocumentationAddition : AssemblyDocumentationAddition
    /// {
    ///     public override bool CanApply(AssemblyDeclaration assemblyDeclaration)
    ///         => assemblyDeclaration.Version.Major == 1 &amp;&amp; assemblyDeclaration.Version.Minor == 0;
    /// 
    ///     public override SummaryDocumentationElement GetSummary(AssemblyDeclaration assemblyDeclaration)
    ///         => DocumentationElement.Summary(
    ///             DocumentationElement.Paragraph(
    ///                 DocumentationElement.Text(
    ///                     assemblyDeclaration
    ///                         .Attributes
    ///                         .Single(attribute => attribute.Type == typeof(AssemblyDescriptionAttribute))
    ///                         .PositionalParameters
    ///                         .Single()
    ///                         .Value
    ///                         .ToString()
    ///                 )
    ///             )
    ///         );
    /// }
    /// </code>
    /// <para>
    /// Both <see cref="ReferenceData.MemberReference"/>s and <see cref="DeclarationNode"/>s
    /// can be used to compare directly with <see cref="System.Reflection.MemberInfo"/>s, this
    /// is why comparisons such as <c>attribute.Type == typeof(AssemblyDescriptionAttribute)</c>
    /// work. This was done to ease lookups when searching for particular types (especially
    /// when looking for attributes).
    /// </para>
    /// </example>
    public abstract class AssemblyDocumentationAddition
    {
        /// <summary>Initializes a new instance of the <see cref="AssemblyDocumentationAddition"/> class.</summary>
        protected AssemblyDocumentationAddition()
        {
        }

        /// <summary>
        /// A filtering predicate that indicates whether the current instance can be applied to the provided <paramref name="assembly"/>.
        /// </summary>
        /// <param name="assembly">The <see cref="AssemblyDeclaration"/> to check.</param>
        /// <returns>Returns <c>true</c> if the current addition is applicable; <c>false</c> otherwise.</returns>
        public abstract bool CanApply(AssemblyDeclaration assembly);

        /// <summary>Gets the summary addition for the provided <paramref name="assembly"/>.</summary>
        /// <param name="assembly">The <see cref="AssemblyDeclaration"/> for which to get the summary addition.</param>
        /// <returns>Returns a <see cref="SummaryDocumentationElement"/> for the provided <paramref name="assembly"/>.</returns>
        public virtual SummaryDocumentationElement GetSummary(AssemblyDeclaration assembly)
            => null;

        /// <summary>Gets the remarks addition for the provided <paramref name="assembly"/>.</summary>
        /// <param name="assembly">The <see cref="AssemblyDeclaration"/> for which to get the remarks addition.</param>
        /// <returns>Returns a <see cref="RemarksDocumentationElement"/> for the provided <paramref name="assembly"/>.</returns>
        public virtual RemarksDocumentationElement GetRemarks(AssemblyDeclaration assembly)
            => null;

        /// <summary>Gets the example additions for the provided <paramref name="assembly"/>.</summary>
        /// <param name="assembly">The <see cref="AssemblyDeclaration"/> for which to get the example additions.</param>
        /// <returns>Returns a collection of <see cref="ExampleDocumentationElement"/> for the provided <paramref name="assembly"/>.</returns>
        public virtual IEnumerable<ExampleDocumentationElement> GetExamples(AssemblyDeclaration assembly)
            => null;

        /// <summary>Gets the related members addition for the provided <paramref name="assembly"/>.</summary>
        /// <param name="assembly">The <see cref="AssemblyDeclaration"/> for which to get the related members addition.</param>
        /// <returns>Returns a collection of <see cref="MemberReferenceDocumentationElement"/> for the provided <paramref name="assembly"/>.</returns>
        public virtual IEnumerable<MemberReferenceDocumentationElement> GetRelatedMembers(AssemblyDeclaration assembly)
            => null;

        /// <summary>Gets the namespace additions for the provided <paramref name="assembly"/>.</summary>
        /// <param name="assembly">The <see cref="AssemblyDeclaration"/> for which to get the namespace additions.</param>
        /// <returns>Returns a collection of <see cref="NamespaceDocumentationAddition"/> for the provided <paramref name="assembly"/>.</returns>
        public virtual IEnumerable<NamespaceDocumentationAddition> GetNamespaceAdditions(AssemblyDeclaration assembly)
            => null;
    }
}