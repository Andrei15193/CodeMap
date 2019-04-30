using CodeMap.ReferenceData;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CodeMap.Tests
{
    public interface ITestMemberReferenceVisitor
    {
        void VisitTypeReference(TypeReference typeReference);
    }

    public class MemberReferenceVisitorAdapter : MemberReferenceVisitor
    {
        private readonly ITestMemberReferenceVisitor _memberReferenceVisitor;

        public MemberReferenceVisitorAdapter(ITestMemberReferenceVisitor memberReferenceVisitor)
        {
            _memberReferenceVisitor = memberReferenceVisitor;
        }

        protected override void VisitTypeReference(TypeReference typeReference)
            => _memberReferenceVisitor.VisitTypeReference(typeReference);
    }

    public class MemberReferenceFactoryTests
    {
        private MemberReferenceFactory _Factory { get; }

        private Mock<ITestMemberReferenceVisitor> _VisitorMock { get; }

        private MemberReferenceVisitor _Visitor { get; }

        public MemberReferenceFactoryTests()
        {
            _Factory = new MemberReferenceFactory();
            _VisitorMock = new Mock<ITestMemberReferenceVisitor>();
            _Visitor = new MemberReferenceVisitorAdapter(_VisitorMock.Object);
        }

        [Fact]
        public async Task CreateFromSimpleType_ReturnsSpecificTypeReference()
        {
            TypeReference typeReference = null;
            _VisitorMock
                .Setup(visitor => visitor.VisitTypeReference(It.IsNotNull<TypeReference>()))
                .Callback((TypeReference actualTypeReference) => typeReference = actualTypeReference);

            var memberReference = _Factory.Create(typeof(int));
            await memberReference.AcceptAsync(_Visitor);

            Assert.Equal("Int32", typeReference.Name);
            Assert.Equal("System", typeReference.Namespace);
            Assert.Empty(typeReference.GenericArguments);
            Assert.Null(typeReference.DeclaringType);
            Assert.True(typeof(int).Assembly == typeReference.Assembly);
        }

        [Fact]
        public void CreateFromAssembly_ReturnsAssemblyReference()
        {
            var assemblyReference = _Factory.Create(typeof(GlobalTestClass).Assembly);

            Assert.Equal("CodeMap.Tests.Data", assemblyReference.Name);
            Assert.Equal(new Version(1, 2, 3, 4), assemblyReference.Version);
            Assert.Empty(assemblyReference.Culture);
            Assert.Empty(assemblyReference.PublicKeyToken);
        }

        [Fact]
        public void CreateFromAssemblyName_ReturnsAssemblyReference()
        {
            var assemblyReference = _Factory.Create(typeof(GlobalTestClass).Assembly.GetName());

            Assert.Equal("CodeMap.Tests.Data", assemblyReference.Name);
            Assert.Equal(new Version(1, 2, 3, 4), assemblyReference.Version);
            Assert.Empty(assemblyReference.Culture);
            Assert.Empty(assemblyReference.PublicKeyToken);
        }

        [Fact]
        public void CreateFromNullMemberInfo_ThrowsExceptions()
        {
            var exception = Assert.Throws<ArgumentNullException>("memberInfo", () => _Factory.Create(memberInfo: null));
            Assert.Equal(new ArgumentNullException("memberInfo").Message, exception.Message);
        }

        [Fact]
        public void CreateFromNullAssembly_ThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("assembly", () => _Factory.Create(assembly: null));
            Assert.Equal(new ArgumentNullException("assembly").Message, exception.Message);
        }

        [Fact]
        public void CreateFromNullAssemblyName_ThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("assemblyName", () => _Factory.Create(assemblyName: null));
            Assert.Equal(new ArgumentNullException("assemblyName").Message, exception.Message);
        }
    }
}