using System;
using System.Linq;
using CodeMap.DeclarationNodes;
using HandlebarsDotNet;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.PathStructure;

namespace CodeMap.Handlebars.Helpers
{
    /// <summary>A helper used to get the access modifier textual description for a <see cref="DeclarationNode"/>. Most useful for properties where the access modifier can be changed for one of the accessors.</summary>
    /// <example>
    /// The following template will generate a paragraph containing the access modifier of the given argument.
    /// <code language="html">
    /// &lt;p&gt;{{MemberAccessModifier declaration}}&lt;/p&gt;
    /// </code>
    /// If the current context exposes a <c>declaration</c> property that is a public property with a private setter <see cref="PropertyDeclaration"/>, the output will be as follows:
    /// <code language="html">
    /// &lt;p&gt;public get; private set&lt;/p&gt;
    /// </code>
    /// </example>
    public class MemberAccessModifier : IHelperDescriptor<HelperOptions>
    {
        /// <summary>Gets the name of the helper.</summary>
        /// <value>The value of this property is <c>MemberAccessModifier</c>.</value>
        public PathInfo Name
            => nameof(MemberAccessModifier);

        /// <summary>Gets the access modifier name for the first argument or current context.</summary>
        /// <param name="options">The helper options.</param>
        /// <param name="context">The context in which this helper is called.</param>
        /// <param name="arguments">The arguments with which this helper has been called.</param>
        /// <returns>Returns the access modifier name for the first argument or current context.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the first argument is not a <see cref="DeclarationNode"/> or when not provided and the given <paramref name="context"/> is not a <see cref="DeclarationNode"/>.
        /// </exception>
        public object Invoke(in HelperOptions options, in Context context, in Arguments arguments)
        {
            var declarationNode = arguments.DefaultIfEmpty(context.Value).ElementAtOrDefault(0) as DeclarationNode ?? throw new ArgumentException("Expected a " + nameof(DeclarationNode) + " provided as the first argument or context.");

            switch (declarationNode)
            {
                case PropertyDeclaration propertyDeclaration:
                    if (propertyDeclaration.Getter is object && propertyDeclaration.Setter is object)
                        if (propertyDeclaration.Getter.AccessModifier == propertyDeclaration.Setter.AccessModifier)
                            return $"{GetAccessModifierLabel(propertyDeclaration.AccessModifier)} get, set";
                        else
                            return $"{GetAccessModifierLabel(propertyDeclaration.Getter.AccessModifier)} get; {GetAccessModifierLabel(propertyDeclaration.Setter.AccessModifier)} set;";
                    else if (propertyDeclaration.Getter is object)
                        return $"{GetAccessModifierLabel(propertyDeclaration.Getter.AccessModifier)} get";
                    else
                        return $"{GetAccessModifierLabel(propertyDeclaration.Setter.AccessModifier)} set";

                case MemberDeclaration memberDeclaration:
                    return GetAccessModifierLabel(memberDeclaration.AccessModifier);

                case TypeDeclaration typeDeclaration:
                    return GetAccessModifierLabel(typeDeclaration.AccessModifier);

                default:
                    throw new ArgumentException($"Unhandled declaration node type: '{declarationNode.GetType().Name}'.");
            }
        }

        /// <summary>Writes the access modifier name for the first argument or current context to the provided <paramref name="output"/>.</summary>
        /// <param name="output">The <see cref="EncodedTextWriter"/> to write the result to.</param>
        /// <param name="options">The helper options.</param>
        /// <param name="context">The context in which this helper is called.</param>
        /// <param name="arguments">The arguments with which this helper has been called.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the first argument is not a <see cref="DeclarationNode"/> or when not provided and the given <paramref name="context"/> is not a <see cref="DeclarationNode"/>.
        /// </exception>
        public void Invoke(in EncodedTextWriter output, in HelperOptions options, in Context context, in Arguments arguments)
            => output.WriteSafeString(Invoke(options, context, arguments));

        /// <summary>Gets the display label for the provided <paramref name="accessModifier"/>.</summary>
        /// <param name="accessModifier">The <see cref="AccessModifier"/> for which to get the display value.</param>
        /// <returns>Returns the display label of the provided <paramref name="accessModifier"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the provided <paramref name="accessModifier"/> is not handled.</exception>
        protected virtual string GetAccessModifierLabel(AccessModifier accessModifier)
            => accessModifier switch
            {
                AccessModifier.Private => "private",
                AccessModifier.FamilyAndAssembly => "private",
                AccessModifier.Assembly => "internal",
                AccessModifier.Family => "protected",
                AccessModifier.FamilyOrAssembly => "protected",
                AccessModifier.Public => "public",
                _ => throw new ArgumentException($"Unhandled access modifier: {accessModifier}.")
            };
    }
}