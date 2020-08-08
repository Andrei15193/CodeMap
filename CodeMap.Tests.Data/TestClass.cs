using System;
using System.Collections.Generic;

namespace CodeMap.Tests.Data
{
    /// <summary test="attribute"/>
    [Test("class test 1", Value2 = "class test 2", Value3 = "class test 3")]
    public unsafe class TestClass<TParam> : TestBaseClass, ITestExtendedBaseInterface
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
        private protected byte TestField;

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
        [Test("class constructor test 1", Value2 = "class constructor test 2", Value3 = "class constructor test 3")]
        public TestClass([Test("class constructor parameter test 1", Value2 = "class constructor parameter test 2", Value3 = "class constructor parameter test 3")] int param)
        {
        }

        /// <summary/>
        public new event EventHandler ClassShadowedTestEvent;

        /// <summary/>
        public override event EventHandler AbstractTestEvent;

        /// <summary/>
        public sealed override event EventHandler VirtualTestEvent;

        /// <summary/>
        public static event EventHandler StaticTestEvent;

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
        public new int ClassShadowedTestProperty { get; set; }

        /// <summary/>
        public int InterfaceShadowedTestProperty { get; set; }

        /// <summary/>
        [Test("class indexer test 1", Value2 = "class indexer test 2", Value3 = "class indexer test 3")]
        public int this[[Test("class indexer parameter test 1", Value2 = "class indexer parameter test 2", Value3 = "class indexer parameter test 3")] int param]
        {
            [Test("class indexer getter test 1", Value2 = "class indexer getter test 2", Value3 = "class indexer getter test 3")]
            [return: Test("class indexer getter return test 1", Value2 = "class indexer getter return test 2", Value3 = "class indexer getter return test 3")]
            get
            {
                return default;
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
        public string TestReadOnlyProperty { get; }

        /// <summary/>
        public string TestWriteOnlyProperty { set { } }

        /// <summary/>
        public string TestPublicReadPrivateWriteProperty { get; private set; }

        /// <summary/>
        public new int ClassShadowedTestMethod()
            => default;

        /// <summary/>
        public override string AbstractTestMethod()
            => default;

        /// <summary/>
        protected internal sealed override bool VirtualTestMethod()
            => default;

        /// <summary/>
        [Test("class method test 1", Value2 = "class method test 2", Value3 = "class method test 3")]
        [return: Test("class method return test 1", Value2 = "class method return test 2", Value3 = "class method return test 3")]
        public void TestMethod([Test("class method parameter test 1", Value2 = "class method parameter test 2", Value3 = "class method parameter test 3")] int param1, string param2)
        {
        }

        /// <summary/>
        public void TestMethod1(int param)
        {
        }

        /// <summary/>
        public void TestMethod2(byte[] param)
        {
        }

        /// <summary/>
        public void TestMethod3(char[][] param)
        {
        }

        /// <summary/>
        public void TestMethod4(double[,] param)
        {
        }

        /// <summary/>
        public void TestMethod5(ref int param)
        {
        }

        /// <summary/>
        public void TestMethod6(ref byte[] param)
        {
        }

        /// <summary/>
        public void TestMethod7(ref char[][] param)
        {
        }

        /// <summary/>
        public void TestMethod8(ref double[,] param)
        {
        }

        /// <summary/>
        public void TestMethod9(out int param)
        {
            param = default;
        }

        /// <summary/>
        public void TestMethod10(out byte[] param)
        {
            param = default;
        }

        /// <summary/>
        public void TestMethod11(out char[][] param)
        {
            param = default;
        }

        /// <summary/>
        public void TestMethod12(out double[,] param)
        {
            param = default;
        }

        /// <summary/>
        public void TestMethod13(TestClass<int>.NestedTestClass<byte[], IEnumerable<string>> param)
        {
        }

        /// <summary/>
        public void TestMethod14(TestClass<int>.NestedTestClass<byte[], IEnumerable<string>>[] param)
        {
        }

        /// <summary/>
        public void TestMethod15(ref TestClass<int>.NestedTestClass<byte[], IEnumerable<string>> param)
        {
        }

        /// <summary/>
        public void TestMethod16(out TestClass<int>.NestedTestClass<byte[], IEnumerable<string>> param)
        {
            param = default;
        }

        /// <summary/>
        public void TestMethod17(ref TestClass<int>.NestedTestClass<byte[], IEnumerable<string>>[] param)
        {
        }

        /// <summary/>
        public void TestMethod18(out TestClass<int>.NestedTestClass<byte[], IEnumerable<string>>[] param)
        {
            param = default;
        }

        /// <summary/>
        public void TestMethod19(dynamic param)
        {
        }

        /// <summary/>
        public void TestMethod20(ref dynamic param)
        {
        }

        /// <summary/>
        public void TestMethod21(out dynamic param)
        {
            param = default;
        }

        /// <summary/>
        public void TestMethod22(TParam param)
        {
        }

        /// <summary/>
        public void TestMethod23(ref TParam param)
        {
        }

        /// <summary/>
        public void TestMethod24(out TParam param)
        {
            param = default;
        }

        /// <summary/>
        public unsafe void TestMethod25(int* param)
        {
        }

        /// <summary/>
        public unsafe void TestMethod26(byte*[] param)
        {
        }

        /// <summary/>
        public unsafe void TestMethod27(ref char* param)
        {
        }

        /// <summary/>
        public unsafe void TestMethod28(out double* param)
        {
            param = default;
        }

        /// <summary/>
        public unsafe void TestMethod29(ref decimal*[] param)
        {
        }

        /// <summary/>
        public unsafe void TestMethod30(out short*[] param)
        {
            param = default;
        }

        /// <summary/>
        public unsafe void TestMethod31(void* param)
        {
        }

        /// <summary/>
        public unsafe void TestMethod32(void** param)
        {
        }

        /// <summary/>
        public unsafe void TestMethod33(ref void** param)
        {
        }

        /// <summary/>
        public unsafe void TestMethod34(out void** param)
        {
            param = default;
        }

        /// <summary/>
        public unsafe void TestMethod35(void**[] param)
        {
        }

        /// <summary/>
        public unsafe void TestMethod36(ref void**[] param)
        {
        }

        /// <summary/>
        public unsafe void TestMethod37(out void**[] param)
        {
            param = default;
        }

        /// <summary/>
        public void TestMethod38<TMethodParam>(TMethodParam param)
        {
        }

        /// <summary/>
        public void TestMethod39<TMethodParam>(ref TMethodParam param)
        {
        }

        /// <summary/>
        public void TestMethod40<TMethodParam>(out TMethodParam param)
        {
            param = default;
        }

        /// <summary/>
        public void TestMethod41(string param = "test")
        {
        }

        /// <summary/>
        event EventHandler ITestBaseInterface.InterfaceShadowedTestEvent { add { } remove { } }

        /// <summary/>
        int ITestBaseInterface.InterfaceShadowedTestMethod()
            => default;

        /// <summary/>
        void ITestBaseInterface.BaseTestMethod()
        {
        }
    }
}