namespace CodeMap.Tests.TestData
{
    internal interface ITestBaseInterface
    {
        int InterfaceShadowedTestProperty { get; set; }

        void BaseTestMethod();

        void InterfaceShadowedTestMethod();
    }
}