using System;

namespace CodeMap.Tests.Data
{
    /// <summary/>
    [Test("struct test 1", Value2 = "struct test 2", Value3 = "struct test 3")]
    public unsafe struct TestStruct<TParam> : ITestExtendedBaseInterface
    {
        /// <summary/>
        private enum NestedTestEnum
        {
        }

        /// <summary/>
        private delegate void NestedTestDelegate();

        /// <summary/>
        private interface INestedTestInterface
        {
        }

        /// <summary/>
        private class NestedTestClass
        {
        }

        /// <summary/>
        private struct NestedTestStruct
        {
        }

        /// <summary/>
        [Test("struct constant test 1", Value2 = "struct constant test 2", Value3 = "struct constant test 3")]
        private const double TestConstant = 1;

        /// <summary/>
        [Test("struct field test 1", Value2 = "struct field test 2", Value3 = "struct field test 3")]
        private byte TestField;

        /// <summary/>
        private readonly char ReadonlyTestField;

        /// <summary/>
        private static string StaticTestField;

        /// <summary/>
        [Test("struct constructor test 1", Value2 = "struct constructor test 2", Value3 = "struct constructor test 3")]
        public TestStruct([Test("struct constructor parameter test 1", Value2 = "struct constructor parameter test 2", Value3 = "struct constructor parameter test 3")] int param)
        {
            TestField = default;
            ReadonlyTestField = default;
            TestProperty = default;
        }

        /// <summary/>
        [Test("struct event test 1", Value2 = "struct event test 2", Value3 = "struct event test 3")]
        public event EventHandler<EventArgs> TestEvent
        {
            [Test("struct event adder test 1", Value2 = "struct event adder test 2", Value3 = "struct event adder test 3")]
            [return: Test("struct event adder return test 1", Value2 = "struct event adder return test 2", Value3 = "struct event adder return test 3")]
            add
            {
            }
            [Test("struct event remover test 1", Value2 = "struct event remover test 2", Value3 = "struct event remover test 3")]
            [return: Test("struct event remover return test 1", Value2 = "struct event remover return test 2", Value3 = "struct event remover return test 3")]
            remove
            {
            }
        }

        /// <summary/>
        public static event EventHandler StaticTestEvent;

        /// <summary/>
        [Test("struct property test 1", Value2 = "struct property test 2", Value3 = "struct property test 3")]
        public byte TestProperty
        {
            [Test("struct property getter test 1", Value2 = "struct property getter test 2", Value3 = "struct property getter test 3")]
            [return: Test("struct property getter return test 1", Value2 = "struct property getter return test 2", Value3 = "struct property getter return test 3")]
            get;
            [Test("struct property setter test 1", Value2 = "struct property setter test 2", Value3 = "struct property setter test 3")]
            [return: Test("struct property setter return test 1", Value2 = "struct property setter return test 2", Value3 = "struct property setter return test 3")]
            set;
        }

        /// <summary/>
        [Test("struct indexer test 1", Value2 = "struct indexer test 2", Value3 = "struct indexer test 3")]
        public int this[[Test("struct indexer parameter test 1", Value2 = "struct indexer parameter test 2", Value3 = "struct indexer parameter test 3")] int param]
        {
            [Test("struct indexer getter test 1", Value2 = "struct indexer getter test 2", Value3 = "struct indexer getter test 3")]
            [return: Test("struct indexer getter return test 1", Value2 = "struct indexer getter return test 2", Value3 = "struct indexer getter return test 3")]
            get
            {
                return default;
            }
            [Test("struct indexer setter test 1", Value2 = "struct indexer setter test 2", Value3 = "struct indexer setter test 3")]
            [return: Test("struct indexer setter return test 1", Value2 = "struct indexer setter return test 2", Value3 = "struct indexer setter return test 3")]
            set
            {
            }
        }

        /// <summary/>
        public override string ToString()
            => base.ToString();

        /// <summary/>
        public new int GetHashCode()
            => (TestField == default).GetHashCode() ^ (ReadonlyTestField == default).GetHashCode();

        /// <summary/>
        [Test("struct method test 1", Value2 = "struct method test 2", Value3 = "struct method test 3")]
        [return: Test("struct method return test 1", Value2 = "struct method return test 2", Value3 = "struct method return test 3")]
        public void TestMethod([Test("struct method parameter test 1", Value2 = "struct method parameter test 2", Value3 = "struct method parameter test 3")] int param)
        {
        }

        event EventHandler ITestBaseInterface.InterfaceShadowedTestEvent { add { } remove { } }

        int ITestBaseInterface.InterfaceShadowedTestProperty
        {
            get => default;
            set { }
        }

        void ITestBaseInterface.BaseTestMethod()
        {
        }

        int ITestBaseInterface.InterfaceShadowedTestMethod()
            => default;
    }
}