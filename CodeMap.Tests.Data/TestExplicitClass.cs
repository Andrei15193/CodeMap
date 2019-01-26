using System;

namespace CodeMap.Tests.Data
{
    /// <summary/>
    public class TestExplicitClass : ITestExplicitInterface
    {
        /// <summary/>
        event EventHandler ITestExplicitInterface.TestEvent { add { } remove { } }

        /// <summary/>
        int ITestExplicitInterface.TestProperty { get; set; }

        /// <summary/>
        void ITestExplicitInterface.TestMethod()
        {
        }
    }
}