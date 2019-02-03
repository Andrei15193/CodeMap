#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable IDE1006 // Naming Styles
using System;
using System.Collections.Generic;

namespace CodeMap.Tests.Data
{
    /// <summary/>
    [Test("class test 1", Value2 = "class test 2", Value3 = "class test 3")]
    public unsafe class TestClass<TParam1> : TestBaseClass, ITestExtendedBaseInterface
    {
        /// <summary/>
        public enum NestedTestEnum
        {
        }

        /// <summary/>
        public delegate void NestedTestDelegate();

        /// <summary/>
        public interface INestedTestInterface
        {
        }

        /// <summary/>
        public class NestedTestClass<TParam2, TParam3>
        {
        }

        /// <summary/>
        public struct NestedTestStruct
        {
        }

        /// <summary/>
        [Test("class constant test 1", Value2 = "class constant test 2", Value3 = "class constant test 3")]
        private const double TestConstant = 1;

        /// <summary/>
        [Test("class field test 1", Value2 = "class field test 2", Value3 = "class field test 3")]
        private byte TestField;

        /// <summary/>
        private readonly char ReadonlyTestField;

        /// <summary/>
        private static string StaticTestField;

        /// <summary/>
        protected new int ShadowedTestField;

        /// <summary/>
        static TestClass()
        {
        }

        /// <summary/>
        [Test("class method test 1", Value2 = "class method test 2", Value3 = "class method test 3")]
        public TestClass(
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
            TestClass<int>.NestedTestClass<byte[], IEnumerable<string>> param13,
            TestClass<int>.NestedTestClass<byte[], IEnumerable<string>>[] param14,
            ref TestClass<int>.NestedTestClass<byte[], IEnumerable<string>> param15,
            out TestClass<int>.NestedTestClass<byte[], IEnumerable<string>> param16,
            ref TestClass<int>.NestedTestClass<byte[], IEnumerable<string>>[] param17,
            out TestClass<int>.NestedTestClass<byte[], IEnumerable<string>>[] param18,
            dynamic param19,
            ref dynamic param20,
            out dynamic param21,
            int* param22,
            byte*[] param23,
            ref char* param24,
            out double* param25,
            ref decimal*[] param26,
            out short*[] param27,
            void* param28,
            void** param29,
            ref void** param30,
            out void** param31,
            void**[] param32,
            ref void**[] param33,
            out void**[] param34,
            TParam1 param35,
            ref TParam1 param36,
            out TParam1 param37,
            string param38 = "test")
        {
            param9 = default(int);
            param10 = default(byte[]);
            param11 = default(char[][]);
            param12 = default(double[,]);
            param16 = default(TestClass<int>.NestedTestClass<byte[], IEnumerable<string>>);
            param18 = default(TestClass<int>.NestedTestClass<byte[], IEnumerable<string>>[]);
            param21 = default(dynamic);
            param25 = default(double*);
            param27 = default(short*[]);
            param31 = default(void**);
            param34 = default(void**[]);
            param37 = default(TParam1);
        }

        /// <summary/>
        public new event EventHandler ClassShadowedTestEvent;

        /// <summary/>
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

        /// <summary/>
        public override byte AbstractTestProperty { get; set; }

        /// <summary/>
        public sealed override string VirtualTestProperty { get; set; }

        /// <summary/>
        public int InterfaceShadowedTestProperty { get; set; }

        /// <summary/>
        [Test("class indexer test 1", Value2 = "class indexer test 2", Value3 = "class indexer test 3")]
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

        /// <summary/>
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

        /// <summary/>
        public new void ClassShadowedTestMethod()
        {
        }

        /// <summary/>
        public override void AbstractTestMethod()
        {
        }

        /// <summary/>
        public sealed override void VirtualTestMethod()
        {
        }

        /// <summary/>
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

        /// <summary/>
        event EventHandler ITestBaseInterface.InterfaceShadowedTestEvent { add { } remove { } }

        /// <summary/>
        void ITestBaseInterface.InterfaceShadowedTestMethod()
        {
        }

        /// <summary/>
        void ITestBaseInterface.BaseTestMethod()
        {
        }
    }
}