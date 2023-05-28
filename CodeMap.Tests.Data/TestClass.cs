using System;
using System.Collections.Generic;

namespace CodeMap.Tests.Data
{
    /// <summary test="attribute">
    /// A test class having everything that can be declared on a class.
    /// <list>
    /// <item>Generic parameters</item>
    /// <item>Fields</item>
    /// <item>Constants</item>
    /// <item>Events</item>
    /// <item>Properties</item>
    /// <item>Indexer</item>
    /// <item>Methods</item>
    /// <item>Shadowed Fields</item>
    /// <item>Shadowed Events</item>
    /// <item>Shadowed Properties</item>
    /// <item>Shadowed Methods</item>
    /// <item>Attributes both on the type itself as well as on each type of member.</item>
    /// <item>Nested types.</item>
    /// <item>Inheritance scenarios (overrides, virtuals, sealed overrides).</item>
    /// </list>
    /// </summary>
    /// <typeparam name="TParam">A generic parameter, used for different things.</typeparam>
    /// <example>
    /// This is an example, where you usually have an use case with some sample code to illustrate how the defined type is useful.
    /// </example>
    /// <remarks>
    /// You can also include some remarks.
    /// </remarks>
    /// <seealso cref="TestClass{TParam}.TestMethod(int, string)"/>
    /// <seealso cref="TestClass{TParam}.TestMethod1(int)"/>
    /// <seealso cref="TestClass{TParam}.TestMethod2(byte[])"/>
    /// <seealso cref="TestClass{TParam}.TestMethod3(char[][])"/>
    /// <seealso cref="TestClass{TParam}.TestMethod4(double[,])"/>
    [Test("class test 1", Value2 = "class test 2", Value3 = "class test 3")]
    public unsafe class TestClass<TParam> : TestBaseClass, ITestExtendedBaseInterface
    {
        /// <summary>A nested enum.</summary>
        public enum NestedTestEnum
        {
        }

        /// <summary>A nested delegate.</summary>
        public delegate void NestedTestDelegate();

        /// <summary>A nested interface.</summary>
        public interface INestedTestInterface
        {
        }

        /// <summary>A nested class.</summary>
        /// <typeparam name="TParam2">A generic parameter used for testing nested generic types.</typeparam>
        /// <typeparam name="TParam3">A second generic parameter used for testing nested generic types.</typeparam>
        public class NestedTestClass<TParam2, TParam3>
        {
        }

        /// <summary>A nested record.</summary>
        public record NestedTestRecord
        {
        }

        /// <summary>A nested struct.</summary>
        public struct NestedTestStruct
        {
        }

        /// <summary>A test constant, with attributes.</summary>
        /// <example>
        /// This is an example, where you usually have an use case with some sample code to illustrate how the defined type is useful.
        /// </example>
        /// <remarks>
        /// You can also include some remarks.
        /// </remarks>
        /// <seealso cref="TestClass{TParam}.TestMethod(int, string)"/>
        /// <seealso cref="TestClass{TParam}.TestMethod1(int)"/>
        /// <seealso cref="TestClass{TParam}.TestMethod2(byte[])"/>
        /// <seealso cref="TestClass{TParam}.TestMethod3(char[][])"/>
        /// <seealso cref="TestClass{TParam}.TestMethod4(double[,])"/>
        [Test("class constant test 1", Value2 = "class constant test 2", Value3 = "class constant test 3")]
        private const double TestConstant = 1;

        /// <summary>A constant that shadows <see cref="TestBaseClass.ClassShadowedTestConstant"/>.</summary>
        public new const float ClassShadowedTestConstant = 2;

        /// <summary>A test field, with attributes.</summary>
        /// <example>
        /// This is an example, where you usually have an use case with some sample code to illustrate how the defined type is useful.
        /// </example>
        /// <remarks>
        /// You can also include some remarks.
        /// </remarks>
        /// <seealso cref="TestClass{TParam}.TestMethod(int, string)"/>
        /// <seealso cref="TestClass{TParam}.TestMethod1(int)"/>
        /// <seealso cref="TestClass{TParam}.TestMethod2(byte[])"/>
        /// <seealso cref="TestClass{TParam}.TestMethod3(char[][])"/>
        /// <seealso cref="TestClass{TParam}.TestMethod4(double[,])"/>
        [Test("class field test 1", Value2 = "class field test 2", Value3 = "class field test 3")]
        private protected byte TestField;

        /// <summary>A read-only field.</summary>
        private readonly char ReadonlyTestField;

        /// <summary>A static field.</summary>
        private static string StaticTestField;

        /// <summary>A field that shadows <see cref="TestBaseClass.ShadowedTestField"/>.</summary>
        protected new int ShadowedTestField;

        /// <summary>A static constructor, some static values intialization documentation can be added here.</summary>
        static TestClass()
        {
        }

        /// <summary>A constructor, with attributes.</summary>
        /// <param name="param">A constructor parameter, with attributes.</param>
        /// <example>
        /// This is an example, where you usually have an use case with some sample code to illustrate how the defined type is useful.
        /// </example>
        /// <remarks>
        /// You can also include some remarks.
        /// </remarks>
        /// <exception cref="ArgumentException">You can also include exceptions.</exception>
        /// <seealso cref="TestClass{TParam}.TestMethod(int, string)"/>
        /// <seealso cref="TestClass{TParam}.TestMethod1(int)"/>
        /// <seealso cref="TestClass{TParam}.TestMethod2(byte[])"/>
        /// <seealso cref="TestClass{TParam}.TestMethod3(char[][])"/>
        /// <seealso cref="TestClass{TParam}.TestMethod4(double[,])"/>
        [Test("class constructor test 1", Value2 = "class constructor test 2", Value3 = "class constructor test 3")]
        public TestClass([Test("class constructor parameter test 1", Value2 = "class constructor parameter test 2", Value3 = "class constructor parameter test 3")] int param)
        {
        }

        /// <summary>An event shadowing <see cref="TestBaseClass.ClassShadowedTestEvent"/>.</summary>
        public new event EventHandler ClassShadowedTestEvent;

        /// <summary>An implemented abstract event.</summary>
        /// <seealso cref="TestBaseClass.AbstractTestEvent"/>
        public override event EventHandler AbstractTestEvent;

        /// <summary>A sealed overridden event.</summary>
        /// <see cref="TestBaseClass.VirtualTestEvent"/>
        public sealed override event EventHandler VirtualTestEvent;

        /// <summary>A static event.</summary>
        public static event EventHandler StaticTestEvent;

        /// <summary>An event, with attributes.</summary>
        /// <example>
        /// This is an example, where you usually have an use case with some sample code to illustrate how the defined type is useful.
        /// </example>
        /// <remarks>
        /// You can also include some remarks.
        /// </remarks>
        /// <exception cref="ArgumentException">You can also include exceptions.</exception>
        /// <seealso cref="TestClass{TParam}.TestMethod(int, string)"/>
        /// <seealso cref="TestClass{TParam}.TestMethod1(int)"/>
        /// <seealso cref="TestClass{TParam}.TestMethod2(byte[])"/>
        /// <seealso cref="TestClass{TParam}.TestMethod3(char[][])"/>
        /// <seealso cref="TestClass{TParam}.TestMethod4(double[,])"/>
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

        /// <summary>An implemented abstract property.</summary>
        public override byte AbstractTestProperty { get; set; }

        /// <summary>A sealed overridden property.</summary>
        /// <seealso cref="TestBaseClass.VirtualTestProperty"/>
        public sealed override string VirtualTestProperty { get; set; }

        /// <summary>A property shadowing <see cref="TestBaseClass.ClassShadowedTestProperty"/>.</summary>
        public new int ClassShadowedTestProperty { get; set; }

        /// <summary>An implemented property declared by <see cref="ITestBaseInterface"/>.</summary>
        public int InterfaceShadowedTestProperty { get; set; }

        /// <summary>An indexer, with attributes.</summary>
        /// <param name="param">Just a parameter, should be an index of something.</param>
        /// <returns>Returns the value at the given <paramref name="param"/> index.</returns>
        [Test("class indexer test 1", Value2 = "class indexer test 2", Value3 = "class indexer test 3")]
        public int this[[Test("class indexer parameter test 1", Value2 = "class indexer parameter test 2", Value3 = "class indexer parameter test 3")] int param]
        {
            [Test("class indexer getter test 1", Value2 = "class indexer getter test 2", Value3 = "class indexer getter test 3")]
            [return: Test("class indexer getter return test 1", Value2 = "class indexer getter return test 2", Value3 = "class indexer getter return test 3")]
            get => default;
            [Test("class indexer setter test 1", Value2 = "class indexer setter test 2", Value3 = "class indexer setter test 3")]
            [return: Test("class indexer setter return test 1", Value2 = "class indexer setter return test 2", Value3 = "class indexer setter return test 3")]
            set { }
        }

        /// <summary>A property, with attributes.</summary>
        /// <example>
        /// This is an example, where you usually have an use case with some sample code to illustrate how the defined type is useful.
        /// </example>
        /// <remarks>
        /// You can also include some remarks.
        /// </remarks>
        /// <value>
        /// You can include details about the value of the property as well.
        /// </value>
        /// <exception cref="ArgumentException">You can also include exceptions.</exception>
        /// <seealso cref="TestClass{TParam}.TestMethod(int, string)"/>
        /// <seealso cref="TestClass{TParam}.TestMethod1(int)"/>
        /// <seealso cref="TestClass{TParam}.TestMethod2(byte[])"/>
        /// <seealso cref="TestClass{TParam}.TestMethod3(char[][])"/>
        /// <seealso cref="TestClass{TParam}.TestMethod4(double[,])"/>
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

        /// <summary>A read-only property.</summary>
        public string TestReadOnlyProperty { get; }

        /// <summary>A write-only property.</summary>
        public string TestWriteOnlyProperty { set { } }

        /// <summary>An init property.</summary>
        public string TestInitProperty
        {
            get;
            [Test("class property setter test 1", Value2 = "class property setter test 2", Value3 = "class property setter test 3")]
            [return: Test("class property setter return test 1", Value2 = "class property setter return test 2", Value3 = "class property setter return test 3")]
            init;
        }

        /// <summary>A read-only property, with private setter.</summary>
        public string TestPublicReadPrivateWriteProperty { get; private set; }

        /// <summary>A method shadowing <see cref="TestBaseClass.ClassShadowedTestMethod"/>.</summary>
        /// <returns>Returns a value, a number to be more precise.</returns>
        public new int ClassShadowedTestMethod()
            => default;

        /// <summary>An implemented abstract method.</summary>
        /// <returns>Returns a string value containing some useful information.</returns>
        public override string AbstractTestMethod()
            => default;

        /// <summary>A sealed overridden method.</summary>
        /// <returns>Returns a boolean value signifying something important.</returns>
        protected internal sealed override bool VirtualTestMethod()
            => default;

        /// <summary>A method, with attributes.</summary>
        /// <param name="param1">A parameter, with attributes.</param>
        /// <param name="param2">A second parameter, without attributes.</param>
        /// <returns>
        /// Returns a magic number, it may be the lucky one. Who knows?
        /// </returns>
        /// <example>
        /// This is an example, where you usually have an use case with some sample code to illustrate how the defined type is useful.
        /// </example>
        /// <remarks>
        /// You can also include some remarks.
        /// </remarks>
        /// <exception cref="ArgumentException">You can also include exceptions.</exception>
        /// <seealso cref="TestClass{TParam}.TestMethod1(int)"/>
        /// <seealso cref="TestClass{TParam}.TestMethod2(byte[])"/>
        /// <seealso cref="TestClass{TParam}.TestMethod3(char[][])"/>
        /// <seealso cref="TestClass{TParam}.TestMethod4(double[,])"/>
        [Test("class method test 1", Value2 = "class method test 2", Value3 = "class method test 3")]
        [return: Test("class method return test 1", Value2 = "class method return test 2", Value3 = "class method return test 3")]
        public int TestMethod([Test("class method parameter test 1", Value2 = "class method parameter test 2", Value3 = "class method parameter test 3")] int param1, string param2)
            => default;

        /// <summary>A method used to test type reference.</summary>
        /// <param name="param">A parameter used for sampling the type reference.</param>
        public void TestMethod1(int param)
        {
        }

        /// <summary>A method used to test type reference.</summary>
        /// <param name="param">A parameter used for sampling the type reference.</param>
        public void TestMethod2(byte[] param)
        {
        }

        /// <summary>A method used to test type reference.</summary>
        /// <param name="param">A parameter used for sampling the type reference.</param>
        public void TestMethod3(char[][] param)
        {
        }

        /// <summary>A method used to test type reference.</summary>
        /// <param name="param">A parameter used for sampling the type reference.</param>
        public void TestMethod4(double[,] param)
        {
        }

        /// <summary>A method used to test type reference.</summary>
        /// <param name="param">A parameter used for sampling the type reference.</param>
        public void TestMethod5(ref int param)
        {
        }

        /// <summary>A method used to test type reference.</summary>
        /// <param name="param">A parameter used for sampling the type reference.</param>
        public void TestMethod6(ref byte[] param)
        {
        }

        /// <summary>A method used to test type reference.</summary>
        /// <param name="param">A parameter used for sampling the type reference.</param>
        public void TestMethod7(ref char[][] param)
        {
        }

        /// <summary>A method used to test type reference.</summary>
        /// <param name="param">A parameter used for sampling the type reference.</param>
        public void TestMethod8(ref double[,] param)
        {
        }

        /// <summary>A method used to test type reference.</summary>
        /// <param name="param">A parameter used for sampling the type reference.</param>
        public void TestMethod9(out int param)
        {
            param = default;
        }

        /// <summary>A method used to test type reference.</summary>
        /// <param name="param">A parameter used for sampling the type reference.</param>
        public void TestMethod10(out byte[] param)
        {
            param = default;
        }

        /// <summary>A method used to test type reference.</summary>
        /// <param name="param">A parameter used for sampling the type reference.</param>
        public void TestMethod11(out char[][] param)
        {
            param = default;
        }

        /// <summary>A method used to test type reference.</summary>
        /// <param name="param">A parameter used for sampling the type reference.</param>
        public void TestMethod12(out double[,] param)
        {
            param = default;
        }

        /// <summary>A method used to test type reference.</summary>
        /// <param name="param">A parameter used for sampling the type reference.</param>
        public void TestMethod13(TestClass<int>.NestedTestClass<byte[], IEnumerable<string>> param)
        {
        }

        /// <summary>A method used to test type reference.</summary>
        /// <param name="param">A parameter used for sampling the type reference.</param>
        public void TestMethod14(TestClass<int>.NestedTestClass<byte[], IEnumerable<string>>[] param)
        {
        }

        /// <summary>A method used to test type reference.</summary>
        /// <param name="param">A parameter used for sampling the type reference.</param>
        public void TestMethod15(ref TestClass<int>.NestedTestClass<byte[], IEnumerable<string>> param)
        {
        }

        /// <summary>A method used to test type reference.</summary>
        /// <param name="param">A parameter used for sampling the type reference.</param>
        public void TestMethod16(out TestClass<int>.NestedTestClass<byte[], IEnumerable<string>> param)
        {
            param = default;
        }

        /// <summary>A method used to test type reference.</summary>
        /// <param name="param">A parameter used for sampling the type reference.</param>
        public void TestMethod17(ref TestClass<int>.NestedTestClass<byte[], IEnumerable<string>>[] param)
        {
        }

        /// <summary>A method used to test type reference.</summary>
        /// <param name="param">A parameter used for sampling the type reference.</param>
        public void TestMethod18(out TestClass<int>.NestedTestClass<byte[], IEnumerable<string>>[] param)
        {
            param = default;
        }

        /// <summary>A method used to test type reference.</summary>
        /// <param name="param">A parameter used for sampling the type reference.</param>
        public void TestMethod19(dynamic param)
        {
        }

        /// <summary>A method used to test type reference.</summary>
        /// <param name="param">A parameter used for sampling the type reference.</param>
        public void TestMethod20(ref dynamic param)
        {
        }

        /// <summary>A method used to test type reference.</summary>
        /// <param name="param">A parameter used for sampling the type reference.</param>
        public void TestMethod21(out dynamic param)
        {
            param = default;
        }

        /// <summary>A method used to test type reference.</summary>
        /// <param name="param">A parameter used for sampling the type reference.</param>
        public void TestMethod22(TParam param)
        {
        }

        /// <summary>A method used to test type reference.</summary>
        /// <param name="param">A parameter used for sampling the type reference.</param>
        public void TestMethod23(ref TParam param)
        {
        }

        /// <summary>A method used to test type reference.</summary>
        /// <param name="param">A parameter used for sampling the type reference.</param>
        public void TestMethod24(out TParam param)
        {
            param = default;
        }

        /// <summary>A method used to test type reference.</summary>
        /// <param name="param">A parameter used for sampling the type reference.</param>
        public unsafe void TestMethod25(int* param)
        {
        }

        /// <summary>A method used to test type reference.</summary>
        /// <param name="param">A parameter used for sampling the type reference.</param>
        public unsafe void TestMethod26(byte*[] param)
        {
        }

        /// <summary>A method used to test type reference.</summary>
        /// <param name="param">A parameter used for sampling the type reference.</param>
        public unsafe void TestMethod27(ref char* param)
        {
        }

        /// <summary>A method used to test type reference.</summary>
        /// <param name="param">A parameter used for sampling the type reference.</param>
        public unsafe void TestMethod28(out double* param)
        {
            param = default;
        }

        /// <summary>A method used to test type reference.</summary>
        /// <param name="param">A parameter used for sampling the type reference.</param>
        public unsafe void TestMethod29(ref decimal*[] param)
        {
        }

        /// <summary>A method used to test type reference.</summary>
        /// <param name="param">A parameter used for sampling the type reference.</param>
        public unsafe void TestMethod30(out short*[] param)
        {
            param = default;
        }

        /// <summary>A method used to test type reference.</summary>
        /// <param name="param">A parameter used for sampling the type reference.</param>
        public unsafe void TestMethod31(void* param)
        {
        }

        /// <summary>A method used to test type reference.</summary>
        /// <param name="param">A parameter used for sampling the type reference.</param>
        public unsafe void TestMethod32(void** param)
        {
        }

        /// <summary>A method used to test type reference.</summary>
        /// <param name="param">A parameter used for sampling the type reference.</param>
        public unsafe void TestMethod33(ref void** param)
        {
        }

        /// <summary>A method used to test type reference.</summary>
        /// <param name="param">A parameter used for sampling the type reference.</param>
        public unsafe void TestMethod34(out void** param)
        {
            param = default;
        }

        /// <summary>A method used to test type reference.</summary>
        /// <param name="param">A parameter used for sampling the type reference.</param>
        public unsafe void TestMethod35(void**[] param)
        {
        }

        /// <summary>A method used to test type reference.</summary>
        /// <param name="param">A parameter used for sampling the type reference.</param>
        public unsafe void TestMethod36(ref void**[] param)
        {
        }

        /// <summary>A method used to test type reference.</summary>
        /// <param name="param">A parameter used for sampling the type reference.</param>
        public unsafe void TestMethod37(out void**[] param)
        {
            param = default;
        }

        /// <summary>A method used to test type reference.</summary>
        /// <typeparam name="TMethodParam">A method generic parameter.</typeparam>
        /// <param name="param">A parameter used for sampling the type reference.</param>
        public void TestMethod38<TMethodParam>(TMethodParam param)
        {
        }

        /// <summary>A method used to test type reference.</summary>
        /// <typeparam name="TMethodParam">A method generic parameter.</typeparam>
        /// <param name="param">A parameter used for sampling the type reference.</param>
        public void TestMethod39<TMethodParam>(ref TMethodParam param)
        {
        }

        /// <summary>A method used to test type reference.</summary>
        /// <typeparam name="TMethodParam">A method generic parameter.</typeparam>
        /// <param name="param">A parameter used for sampling the type reference.</param>
        public void TestMethod40<TMethodParam>(out TMethodParam param)
        {
            param = default;
        }

        /// <summary>A method used to test default parameter values.</summary>
        /// <param name="param">A parameter with a default value.</param>
        public void TestMethod41(string param = "test")
        {
        }

        /// <summary>An explicitly implemented event declared by <see cref="ITestBaseInterface"/>.</summary>
        event EventHandler ITestBaseInterface.InterfaceShadowedTestEvent { add { } remove { } }

        /// <summary>An explicitly implemented method declared by <see cref="ITestBaseInterface"/>.</summary>
        /// <returns>Returns a number, an important number, something very useful.</returns>
        int ITestBaseInterface.InterfaceShadowedTestMethod()
            => default;

        /// <summary>An explicitly implemented method declared by <see cref="ITestBaseInterface"/>.</summary>
        void ITestBaseInterface.BaseTestMethod()
        {
        }
    }
}