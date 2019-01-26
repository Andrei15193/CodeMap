namespace CodeMap.Tests.Data
{
    /// <summary/>
    public interface ITestBaseInterface
    {
        /// <summary/>
        int InterfaceShadowedTestProperty { get; set; }

        /// <summary/>
        void BaseTestMethod();

        /// <summary/>
        void InterfaceShadowedTestMethod();
    }
}