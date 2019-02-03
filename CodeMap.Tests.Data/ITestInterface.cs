using System;
using System.Collections.Generic;

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
        int this[
            [Test("interface indexer parameter test 1", Value2 = "interface indexer parameter test 2", Value3 = "interface indexer parameter test 3")]
            int param1,
            byte[] param2,
            char[][] param3,
            double[,] param4,
            TestClass<int>.NestedTestClass<byte[], IEnumerable<string>> param5,
            TestClass<int>.NestedTestClass<byte[], IEnumerable<string>>[] param6,
            dynamic param7,
            int* param8,
            byte*[] param9,
            void* param10,
            void** param11,
            void**[] param12,
            TParam1 param13,
            string param14 = "test"]
        {
            [Test("interface indexer getter test 1", Value2 = "interface indexer getter test 2", Value3 = "interface indexer getter test 3")]
            [return: Test("interface indexer getter return test 1", Value2 = "interface indexer getter return test 2", Value3 = "interface indexer getter return test 3")]
            get;
            [Test("interface indexer setter test 1", Value2 = "interface indexer setter test 2", Value3 = "interface indexer setter test 3")]
            [return: Test("interface indexer setter return test 1", Value2 = "interface indexer setter return test 2", Value3 = "interface indexer setter return test 3")]
            set;
        }

        /// <summary/>
        new void InterfaceShadowedTestMethod();

        /// <summary/>
        [Test("interface method test 1", Value2 = "interface method test 2", Value3 = "interface method test 3")]
        [return: Test("interface method return test 1", Value2 = "interface method return test 2", Value3 = "interface method return test 3")]
        void TestMethod<TMethodParam1>(
            [Test("interface method parameter test 1", Value2 = "interface method parameter test 2", Value3 = "interface method parameter test 3")]
            int param1,
            byte[] param2,
            char[][] param3,
            double[,] param4,
            ref int param5,
            ref byte[] param6,
            ref char[][] param7,
            ref double[,] param8,
            out int param9,
            out byte[] param10,
            out char[][] param11,
            out double[,] param12,
            TestClass<int>.NestedTestClass<byte[], IEnumerable<string>> param13,
            TestClass<int>.NestedTestClass<byte[], IEnumerable<string>>[] param14,
            ref TestClass<int>.NestedTestClass<byte[], IEnumerable<string>> param15,
            out TestClass<int>.NestedTestClass<byte[], IEnumerable<string>> param16,
            ref TestClass<int>.NestedTestClass<byte[], IEnumerable<string>>[] param17,
            out TestClass<int>.NestedTestClass<byte[], IEnumerable<string>>[] param18,
            dynamic param19,
            ref dynamic param20,
            out dynamic param21,
            TParam1 param22,
            ref TParam1 param23,
            out TParam1 param24,
            int* param25,
            byte*[] param26,
            ref char* param27,
            out double* param28,
            ref decimal*[] param29,
            out short*[] param30,
            void* param31,
            void** param32,
            ref void** param33,
            out void** param34,
            void**[] param35,
            ref void**[] param36,
            out void**[] param37,
            TMethodParam1 param38,
            ref TMethodParam1 param39,
            out TMethodParam1 param40,
            string param41 = "test");
    }
}