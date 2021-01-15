using System;
using System.IO;
using System.Linq;
using CodeMap.DeclarationNodes;

namespace CodeMap.Handlebars.Helpers
{
    /// <summary>A helper used to get the access modifier textual description for a <see cref="DeclarationNode"/>. Most useful for properties where the access modifier can be changed for one of the accessors.</summary>
    /// <example>
    /// The following template will generate a paragraph containing the access modifier of the given parameter.
    /// <code language="html">
    /// &lt;p&gt;{{MemberAccessModifier declaration}}&lt;/p&gt;
    /// </code>
    /// If the current context exposes a <c>declaration</c> property that is a public property with a private setter <see cref="PropertyDeclaration"/>, the output will be as follows:
    /// <code language="html">
    /// &lt;p&gt;public get; private set&lt;/p&gt;
    /// </code>
    /// </example>
    public class MemberAccessModifier : IHandlebarsHelper
    {
        /// <summary>Gets the name of the helper.</summary>
        /// <value>The value of this property is <c>MemberAccessModifier</c>. It is a constant.</value>
        public string Name
            => nameof(MemberAccessModifier);

        /// <summary>Writes the access modifier to the provided <paramref name="writer"/> for the first parameter or current context.</summary>
        /// <param name="writer">The <see cref="TextWriter"/> to write the result to.</param>
        /// <param name="context">The context in which this helper is called.</param>
        /// <param name="parameters">The parameter with which this helper has been called.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the first parameter is not a <see cref="DeclarationNode"/> or when not provided and the given <paramref name="context"/> is not a <see cref="DeclarationNode"/>.
        /// </exception>
        public void Apply(TextWriter writer, object context, params object[] parameters)
        {
            var declarationNode = parameters.DefaultIfEmpty(context).First() as DeclarationNode;
            if (declarationNode is null)
                throw new ArgumentException("Expected a " + nameof(DeclarationNode) + " provided as the first parameter or context.");

            switch (declarationNode)
            {
                case PropertyDeclaration propertyDeclaration:
                    if (propertyDeclaration.Getter is object && propertyDeclaration.Setter is object)
                        if (propertyDeclaration.Getter.AccessModifier == propertyDeclaration.Setter.AccessModifier)
                        {
                            writer.Write(GetAccessModifierLabel(propertyDeclaration.AccessModifier));
                            writer.Write(" get, set");
                        }
                        else
                        {
                            writer.Write(GetAccessModifierLabel(propertyDeclaration.Getter.AccessModifier));
                            writer.Write(" get; ");
                            writer.Write(GetAccessModifierLabel(propertyDeclaration.Setter.AccessModifier));
                            writer.Write(" set");
                        }
                    else if (propertyDeclaration.Getter is object)
                    {
                        writer.Write(GetAccessModifierLabel(propertyDeclaration.Getter.AccessModifier));
                        writer.Write(" get");
                    }
                    else
                    {
                        writer.Write(GetAccessModifierLabel(propertyDeclaration.Setter.AccessModifier));
                        writer.Write(" set");
                    }
                    break;

                case MemberDeclaration memberDeclaration:
                    writer.Write(GetAccessModifierLabel(memberDeclaration.AccessModifier));
                    break;

                case TypeDeclaration typeDeclaration:
                    writer.Write(GetAccessModifierLabel(typeDeclaration.AccessModifier));
                    break;

                default:
                    throw new ArgumentException($"Unhandled declaration node type: '{declarationNode.GetType().Name}'");
            }
        }

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