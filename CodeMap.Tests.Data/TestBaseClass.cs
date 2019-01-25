namespace CodeMap.Tests.Data
{
    internal abstract class TestBaseClass
    {
        protected int ShadowedTestField;

        public static int StaticTestProperty { get; set; }

        public char ClassShadowedTestProperty { get; set; }

        public abstract byte AbstractTestProperty { get; set; }

        public virtual string VirtualTestProperty { get; set; }

        public static void StaticTestMethod()
        {
        }

        public void ClassShadowedTestMethod()
        {
        }

        public abstract void AbstractTestMethod();

        public virtual void VirtualTestMethod()
        {
        }
    }
}