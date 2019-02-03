#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable IDE1006 // Naming Styles
using System;
using System.Collections.Generic;

namespace CodeMap.Tests.Data
{
    /// <summary/>
    [Test("struct test 1", Value2 = "struct test 2", Value3 = "struct test 3")]
    public unsafe struct TestStruct<TParam1>
    {
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
        public int this[
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
            [Test("struct indexer getter test 1", Value2 = "struct indexer getter test 2", Value3 = "struct indexer getter test 3")]
            [return: Test("struct indexer getter return test 1", Value2 = "struct indexer getter return test 2", Value3 = "struct indexer getter return test 3")]
            get
            {
                return default(int);
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
        public new bool Equals(object obj)
            => base.Equals(obj);

        /// <summary/>
        [Test("struct method test 1", Value2 = "struct method test 2", Value3 = "struct method test 3")]
        [return: Test("struct method return test 1", Value2 = "struct method return test 2", Value3 = "struct method return test 3")]
        public void TestMethod<TMethodParam1>(
            [Test("struct method parameter test 1", Value2 = "struct method parameter test 2", Value3 = "struct method parameter test 3")]
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
            string param41 = "test")
        {
            param9 = default(int);
            param10 = default(byte[]);
            param11 = default(char[][]);
            param12 = default(double[,]);
            param16 = default(TestClass<int>.NestedTestClass<byte[], IEnumerable<string>>);
            param18 = default(TestClass<int>.NestedTestClass<byte[], IEnumerable<string>>[]);
            param21 = default(dynamic);
            param24 = default(TParam1);
            param28 = default(double*);
            param30 = default(short*[]);
            param34 = default(void**);
            param37 = default(void**[]);
            param40 = default(TMethodParam1);
        }
    }
}