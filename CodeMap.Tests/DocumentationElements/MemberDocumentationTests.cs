﻿using System;
using CodeMap.DocumentationElements;
using Xunit;

namespace CodeMap.Tests.DocumentationElements
{
    public class MemberDocumentationTests
    {
        [Fact]
        public void NullListsInitializesWithEmptyOnes()
        {
            var memberDocumentation = new MemberDocumentation("canonical name", null, null, null, null, null, null, null, null, null);

            Assert.Empty(memberDocumentation.Examples);
            Assert.Empty(memberDocumentation.Exceptions);
            Assert.Empty(memberDocumentation.GenericParameters);
            Assert.Empty(memberDocumentation.Parameters);
            Assert.Empty(memberDocumentation.RelatedMembers);
            Assert.Empty(memberDocumentation.Returns);
        }

        [Fact]
        public void ConstructorThrowsExceptionForNullCanonicalName()
        {
            var exception = Assert.Throws<ArgumentNullException>("canonicalName", () => new MemberDocumentation(null, null, null, null, null, null, null, null, null, null));

            Assert.Equal(new ArgumentNullException("canonicalName").Message, exception.Message);
        }
    }
}