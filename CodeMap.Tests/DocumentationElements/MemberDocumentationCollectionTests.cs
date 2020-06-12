using CodeMap.DocumentationElements;
using System;
using System.Linq;
using Xunit;

namespace CodeMap.Tests.DocumentationElements
{
    public class MemberDocumentationCollectionTests
    {
        [Fact]
        public void TryFindBestMatchMemberDocumentation()
        {
            var canonicalName = "T:System.String";
            var memberDocumentation = new MemberDocumentation(canonicalName, null, null, null, null, null, null, null, null, null);
            var collection = new MemberDocumentationCollection(new[] { memberDocumentation });

            Assert.True(collection.TryFind(canonicalName, out var actualMemberDocumentation));

            Assert.Same(memberDocumentation, actualMemberDocumentation);
        }

        [Fact]
        public void TryFindMemberDocumentationWhenBestMatchDoesNotExist()
        {
            var canonicalName = "T:System.String".ToLowerInvariant();
            var memberDocumentation = new MemberDocumentation(canonicalName.ToUpperInvariant(), null, null, null, null, null, null, null, null, null);
            var collection = new MemberDocumentationCollection(new[] { memberDocumentation });

            Assert.True(collection.TryFind(canonicalName, out var actualMemberDocumentation));

            Assert.Same(memberDocumentation, actualMemberDocumentation);
        }

        [Fact]
        public void TryFindMemberReturnsFalseWhenMatchCannotBeFound()
        {
            var collection = new MemberDocumentationCollection(Enumerable.Empty<MemberDocumentation>());

            Assert.False(collection.TryFind("T:System.String", out var actualMemberDocumentation));

            Assert.Null(actualMemberDocumentation);
        }

        [Fact]
        public void ConstructorThrowsExceptionWhenInitializingWithNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new MemberDocumentationCollection(null));
            Assert.Equal(new ArgumentNullException("membersDocumentation").Message, exception.Message);
        }

        [Fact]
        public void ConstructorThrowsExceptionWhenCollectionContainsNull()
        {
            var exception = Assert.Throws<ArgumentException>(() => new MemberDocumentationCollection(new MemberDocumentation[] { null }));
            Assert.Equal(new ArgumentException("Cannot contain 'null' member documentation items.", "membersDocumentation").Message, exception.Message);
        }
    }
}