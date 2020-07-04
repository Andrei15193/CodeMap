﻿using Moq;
using System;
using System.Linq.Expressions;

namespace CodeMap.Tests.DocumentationElements
{
    internal class InvocationCheck
    {
        internal InvocationCheck(Expression<Action<IDocumentationVisitor>> method, Times invocationTimes)
        {
            Method = method;
            InvocationTimes = invocationTimes;
        }

        internal Expression<Action<IDocumentationVisitor>> Method { get; }

        internal Times InvocationTimes { get; }
    }
}