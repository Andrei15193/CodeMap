using System;

namespace CodeMap.Tests.Data
{
    /// <summary>This is an interface that is explicitly implemented, for documentation purposes.</summary>
    public interface ITestExplicitInterface
    {
        /// <summary>An event to implement explicitly.</summary>
        event EventHandler TestEvent;

        /// <summary>A property to implement explicitly.</summary>
        int TestProperty
        {
            get;
            set;
        }

        /// <summary>A method to implement explicitly.</summary>
        void TestMethod();
    }
}