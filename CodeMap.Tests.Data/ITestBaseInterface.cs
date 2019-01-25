namespace CodeMap.Tests.Data
{
    internal interface ITestBaseInterface
    {
        int InterfaceShadowedTestProperty { get; set; }

        void BaseTestMethod();

        void InterfaceShadowedTestMethod();
    }
}