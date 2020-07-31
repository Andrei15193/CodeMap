using System;

namespace CodeMap.Tests.Data
{
    /// <summary/>
    [Test("interface test 1", Value2 = "interface test 2", Value3 = "interface test 3")]
    public unsafe interface ITestInterface<TParam1> : ITestExtendedBaseInterface
    {
        /// <summary/>
        [Test("interface event test 1", Value2 = "interface event test 2", Value3 = "interface event test 3")]
        event EventHandler<EventArgs> TestEvent;

        /// <summary/>
        new event EventHandler InterfaceShadowedTestEvent;

        /// <summary/>
        [Test("interface property test 1", Value2 = "interface property test 2", Value3 = "interface property test 3")]
        byte TestProperty
        {
            [Test("interface property getter test 1", Value2 = "interface property getter test 2", Value3 = "interface property getter test 3")]
            [return: Test("interface property getter return test 1", Value2 = "interface property getter return test 2", Value3 = "interface property getter return test 3")]
            get;
            [Test("interface property setter test 1", Value2 = "interface property setter test 2", Value3 = "interface property setter test 3")]
            [return: Test("interface property setter return test 1", Value2 = "interface property setter return test 2", Value3 = "interface property setter return test 3")]
            set;
        }

        /// <summary/>
        new int InterfaceShadowedTestProperty { get; set; }

        /// <summary/>
        [Test("interface indexer test 1", Value2 = "interface indexer test 2", Value3 = "interface indexer test 3")]
        int this[[Test("interface indexer parameter test 1", Value2 = "interface indexer parameter test 2", Value3 = "interface indexer parameter test 3")] int param]
        {
            [Test("interface indexer getter test 1", Value2 = "interface indexer getter test 2", Value3 = "interface indexer getter test 3")]
            [return: Test("interface indexer getter return test 1", Value2 = "interface indexer getter return test 2", Value3 = "interface indexer getter return test 3")]
            get;
            [Test("interface indexer setter test 1", Value2 = "interface indexer setter test 2", Value3 = "interface indexer setter test 3")]
            [return: Test("interface indexer setter return test 1", Value2 = "interface indexer setter return test 2", Value3 = "interface indexer setter return test 3")]
            set;
        }

        /// <summary/>
        new int InterfaceShadowedTestMethod();

        /// <summary/>
        [Test("interface method test 1", Value2 = "interface method test 2", Value3 = "interface method test 3")]
        [return: Test("interface method return test 1", Value2 = "interface method return test 2", Value3 = "interface method return test 3")]
        void TestMethod([Test("interface method parameter test 1", Value2 = "interface method parameter test 2", Value3 = "interface method parameter test 3")] int param);
    }
}