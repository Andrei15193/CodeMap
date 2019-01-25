#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable IDE1006 // Naming Styles
using System;
using System.Collections.Generic;

namespace CodeMap.Tests.TestData
{
    [Test("class test 1", Value2 = "class test 2", Value3 = "class test 3")]
    internal unsafe class TestClass<TParam1> : TestBaseClass, ITestExtendedBaseInterface
    {
        public class NestedTestClass<TParam2, TParam3>
        {
        }

        [Test("class constant test 1", Value2 = "class constant test 2", Value3 = "class constant test 3")]
        private const double TestConstant = 1;

        [Test("class field test 1", Value2 = "class field test 2", Value3 = "class field test 3")]
        private byte TestField;

        private readonly char ReadonlyTestField;

        private static string StaticTestField;

        new protected int ShadowedTestField = 0;

        [Test("class event test 1", Value2 = "class event test 2", Value3 = "class event test 3")]
        public event EventHandler<EventArgs> TestEvent
        {
            [Test("class event adder test 1", Value2 = "class event adder test 2", Value3 = "class event adder test 3")]
            [return: Test("class event adder return test 1", Value2 = "class event adder return test 2", Value3 = "class event adder return test 3")]
            add
            {
            }
            [Test("class event remover test 1", Value2 = "class event remover test 2", Value3 = "class event remover test 3")]
            [return: Test("class event remover return test 1", Value2 = "class event remover return test 2", Value3 = "class event remover return test 3")]
            remove
            {
            }
        }

        public override byte AbstractTestProperty { get; set; }

        public sealed override string VirtualTestProperty { get; set; }

        public int InterfaceShadowedTestProperty { get; set; }

        [Test("class indexer test 1", Value2 = "class indexer test 2", Value3 = "class indexer test 3")]
        public int this[
            int param1,
            byte[] param2,
            char[][] param3,
            double[,] param4,
            TestClass<int>.NestedTestClass<byte, IEnumerable<string>> param5,
            TestClass<int>.NestedTestClass<byte, IEnumerable<string>>[] param6,
            dynamic param7,
            int* param8,
            byte*[] param9,
            void* param10,
            void** param11,
            void**[] param12,
            TParam1 param13,
            string param14 = "test"]
        {
            [Test("class indexer getter test 1", Value2 = "class indexer getter test 2", Value3 = "class indexer getter test 3")]
            [return: Test("class indexer getter return test 1", Value2 = "class indexer getter return test 2", Value3 = "class indexer getter return test 3")]
            get
            {
                return default(int);
            }
            [Test("class indexer setter test 1", Value2 = "class indexer setter test 2", Value3 = "class indexer setter test 3")]
            [return: Test("class indexer setter return test 1", Value2 = "class indexer setter return test 2", Value3 = "class indexer setter return test 3")]
            set
            {
            }
        }

        [Test("class property test 1", Value2 = "class property test 2", Value3 = "class property test 3")]
        public byte TestProperty
        {
            [Test("class property getter test 1", Value2 = "class property getter test 2", Value3 = "class property getter test 3")]
            [return: Test("class property getter return test 1", Value2 = "class property getter return test 2", Value3 = "class property getter return test 3")]
            get;
            [Test("class property setter test 1", Value2 = "class property setter test 2", Value3 = "class property setter test 3")]
            [return: Test("class property setter return test 1", Value2 = "class property setter return test 2", Value3 = "class property setter return test 3")]
            set;
        }

        new public void ClassShadowedTestMethod()
        {
        }

        public override void AbstractTestMethod()
        {
        }

        public sealed override void VirtualTestMethod()
        {
        }

        [Test("class method test 1", Value2 = "class method test 2", Value3 = "class method test 3")]
        [return: Test("class method return test 1", Value2 = "class method return test 2", Value3 = "class method return test 3")]
        public void TestMethod<TMethodParam1>(
            [Test("class method parameter test 1", Value2 = "class method parameter test 2", Value3 = "class method parameter test 3")]
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
            TestClass<int>.NestedTestClass<byte, IEnumerable<string>> param13,
            TestClass<int>.NestedTestClass<byte, IEnumerable<string>>[] param14,
            ref TestClass<int>.NestedTestClass<byte, IEnumerable<string>> param15,
            out TestClass<int>.NestedTestClass<byte, IEnumerable<string>> param16,
            ref TestClass<int>.NestedTestClass<byte, IEnumerable<string>>[] param17,
            out TestClass<int>.NestedTestClass<byte, IEnumerable<string>>[] param18,
            dynamic param19,
            ref dynamic param20,
            out dynamic param21,
            TMethodParam1 param22,
            ref TMethodParam1 param23,
            out TMethodParam1 pram24,
            int* param25,
            byte*[] param26,
            ref char* param27,
            out double* param29,
            ref decimal*[] param30,
            out short*[] param31,
            void* param32,
            void** param33,
            ref void** param34,
            out void** param35,
            void**[] param36,
            ref void**[] param37,
            out void**[] param38,
            TParam1 param39,
            ref TParam1 param40,
            out TParam1 param41,
            string param42 = "test")
        {
            param9 = default(int);
            param10 = default(byte[]);
            param11 = default(char[][]);
            param12 = default(double[,]);
            param16 = default(TestClass<int>.NestedTestClass<byte, IEnumerable<string>>);
            param18 = default(TestClass<int>.NestedTestClass<byte, IEnumerable<string>>[]);
            param21 = default(dynamic);
            pram24 = default(TMethodParam1);
            param29 = default(double*);
            param31 = default(short*[]);
            param35 = default(void**);
            param38 = default(void**[]);
            param41 = default(TParam1);
        }

        void ITestBaseInterface.InterfaceShadowedTestMethod()
        {
        }

        void ITestBaseInterface.BaseTestMethod()
        {
        }
    }
}