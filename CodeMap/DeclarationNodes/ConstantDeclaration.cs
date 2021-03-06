﻿using CodeMap.ReferenceData;

namespace CodeMap.DeclarationNodes
{
    /// <summary>Represents a documented constant declared by a type.</summary>
    public class ConstantDeclaration : MemberDeclaration
    {
        internal ConstantDeclaration(ConstantReference constantReference)
            : base(constantReference)
        {
        }

        /// <summary>Indicates whether the constant hides a member from a base type.</summary>
        public bool IsShadowing { get; internal set; }

        /// <summary>The constant value.</summary>
        public object Value { get; internal set; }

        /// <summary>The constant type.</summary>
        public BaseTypeReference Type { get; internal set; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DeclarationNodeVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DeclarationNodeVisitor visitor)
            => visitor.VisitConstant(this);
    }
}