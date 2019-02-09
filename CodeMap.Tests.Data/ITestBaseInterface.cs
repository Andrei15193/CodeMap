using System;

namespace CodeMap.Tests.Data
{
    /// <summary/>
    public interface ITestBaseInterface
    {
        /// <summary/>
        event EventHandler InterfaceShadowedTestEvent;

        /// <summary/>
        int InterfaceShadowedTestProperty { get; set; }

        /// <summary/>
        void BaseTestMethod();

        /// <summary/>
        int InterfaceShadowedTestMethod();
    }
}