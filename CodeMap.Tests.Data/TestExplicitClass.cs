using System;

namespace CodeMap.Tests.Data
{
    /// <summary>A class that explicitly implements <see cref="ITestExplicitInterface"/>.</summary>
    public class TestExplicitClass : ITestExplicitInterface
    {
        /// <summary>An explicitly implemented event.</summary>
        event EventHandler ITestExplicitInterface.TestEvent { add { } remove { } }

        /// <summary>An explicitly implemented property.</summary>
        int ITestExplicitInterface.TestProperty { get; set; }

        /// <summary>An explicitly implemented method.</summary>
        void ITestExplicitInterface.TestMethod()
        {
        }
    }
}