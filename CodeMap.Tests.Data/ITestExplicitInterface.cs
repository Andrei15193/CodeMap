using System;

namespace CodeMap.Tests.Data
{
    /// <summary/>
    public interface ITestExplicitInterface
    {
        /// <summary/>
        event EventHandler TestEvent;

        /// <summary/>
        int TestProperty
        {
            get;
            set;
        }

        /// <summary/>
        void TestMethod();
    }
}