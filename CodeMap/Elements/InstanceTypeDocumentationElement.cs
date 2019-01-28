using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.Elements
{
    /// <summary>Represents a reference to a concrete type.</summary>
    public class InstanceTypeDocumentationElement : TypeReferenceDocumentationElement
    {
        internal InstanceTypeDocumentationElement()
        {
        }

        /// <summary>The type name.</summary>
        public string Name { get; internal set; }

        /// <summary>The type namespace.</summary>
        public string Namespace { get; internal set; }

        /// <summary>The type generic arguments.</summary>
        public IReadOnlyList<TypeReferenceDocumentationElement> GenericArguments { get; internal set; }

        /// <summary>The declaring type, if any.</summary>
        public InstanceTypeDocumentationElement DeclaringType { get; internal set; }

        /// <summary>The declaring assembly.</summary>
        public AssemblyReferenceDocumentationElement Assembly { get; internal set; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree asynchronously.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        public override Task AcceptAsync(DocumentationVisitor visitor, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>Determines whether the current <see cref="InstanceTypeDocumentationElement"/> is equal to the provided <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="InstanceTypeDocumentationElement"/> references the provided <paramref name="type"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(Type type)
        {
            if (type == null || type.IsPointer || type.IsArray || type.IsByRef || type.IsGenericParameter || (type.IsGenericType && !type.IsConstructedGenericType))
                return false;

            var backTickIndex = type.Name.LastIndexOf('`');
            return
                string.Equals(Name, (backTickIndex >= 0 ? type.Name.Substring(0, backTickIndex) : type.Name), StringComparison.OrdinalIgnoreCase)
                && string.Equals(Namespace, type.Namespace, StringComparison.OrdinalIgnoreCase)
                && DeclaringType == type.DeclaringType?.MakeGenericType(
                    type
                        .GetGenericArguments()
                        .Take(type.DeclaringType.GetGenericArguments().Length)
                        .ToArray()
                )
                && GenericArguments.Count == type.GetGenericArguments().Length
                && Assembly == type.Assembly.GetName();
        }
    }
}