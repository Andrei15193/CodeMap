using System;

namespace CodeMap.Tests.Data
{
    /// <summary test="attribute">
    /// A test struct having everything that can be declared on a struct.
    /// <list>
    /// <item>Generic parameters</item>
    /// <item>Fields</item>
    /// <item>Constants</item>
    /// <item>Events</item>
    /// <item>Properties</item>
    /// <item>Indexer</item>
    /// <item>Methods</item>
    /// <item>Shadowed Methods</item>
    /// <item>Attributes both on the type itself as well as on each type of member.</item>
    /// <item>Nested types.</item>
    /// <item>Inheritance scenarios (overrides and virtuals).</item>
    /// </list>
    /// </summary>
    /// <typeparam name="TParam">A generic parameter, used for different things.</typeparam>
    /// <example>
    /// This is an example, where you usually have an use case with some sample code to illustrate how the defined type is useful.
    /// </example>
    /// <remarks>
    /// You can also include some remarks.
    /// </remarks>
    /// <seealso cref="TestStruct{TParam}.TestStruct(int)"/>
    /// <seealso cref="TestStruct{TParam}.TestMethod(int)"/>
    [Test("struct test 1", Value2 = "struct test 2", Value3 = "struct test 3")]
    public unsafe struct TestStruct<TParam> : ITestExtendedBaseInterface
    {
        /// <summary>A nested enum.</summary>
        private enum NestedTestEnum
        {
        }

        /// <summary>A nested delegate.</summary>
        private delegate void NestedTestDelegate();

        /// <summary>A nested interface.</summary>
        private interface INestedTestInterface
        {
        }

        /// <summary>A nested class.</summary>
        private class NestedTestClass
        {
        }

        /// <summary>A nested struct.</summary>
        private struct NestedTestStruct
        {
        }

        /// <summary>A constant, with attributes.</summary>
        [Test("struct constant test 1", Value2 = "struct constant test 2", Value3 = "struct constant test 3")]
        private const double TestConstant = 1;

        /// <summary>A field, with attributes.</summary>
        [Test("struct field test 1", Value2 = "struct field test 2", Value3 = "struct field test 3")]
        private byte TestField;

        /// <summary>A read-only field.</summary>
        private readonly char ReadonlyTestField;

        /// <summary>A static field.</summary>
        private static string StaticTestField;

        /// <summary>A constructor, with attributes.</summary>
        /// <param name="param">A constructor parameter, with attributes.</param>
        [Test("struct constructor test 1", Value2 = "struct constructor test 2", Value3 = "struct constructor test 3")]
        public TestStruct([Test("struct constructor parameter test 1", Value2 = "struct constructor parameter test 2", Value3 = "struct constructor parameter test 3")] int param)
        {
            TestField = default;
            ReadonlyTestField = default;
            TestProperty = default;
        }

        /// <summary>A test event, with attributes.</summary>
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

        /// <summary>A static event.</summary>
        public static event EventHandler StaticTestEvent;

        /// <summary>A property, with attributes.</summary>
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

        /// <summary>An indexer, with attributes.</summary>
        /// <param name="param">An indexer parameter, with attributes.</param>
        /// <returns>Returns a number, a number that is at the given <paramref name="param"/> index.</returns>
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

        /// <summary>An overriden method.</summary>
        /// <returns>Returns the string representation of the current instance, or so we think.</returns>
        public override string ToString()
            => base.ToString();

        /// <summary>A method shadowing <see cref="object.GetHashCode"/>.</summary>
        /// <returns>Returns a hash code for the current instance, useful for hash-table backed collections (e.g. <see cref="System.Collections.Generic.Dictionary{TKey, TValue}"/>.</returns>
        public new int GetHashCode()
            => (TestField == default).GetHashCode() ^ (ReadonlyTestField == default).GetHashCode();

        /// <summary>A method, with attributes.</summary>
        /// <param name="param">A method parameter, with attributes.</param>
        [Test("struct method test 1", Value2 = "struct method test 2", Value3 = "struct method test 3")]
        [return: Test("struct method return test 1", Value2 = "struct method return test 2", Value3 = "struct method return test 3")]
        public void TestMethod([Test("struct method parameter test 1", Value2 = "struct method parameter test 2", Value3 = "struct method parameter test 3")] int param)
        {
        }

        /// <summary>An explicitly implemented event.</summary>
        event EventHandler ITestBaseInterface.InterfaceShadowedTestEvent { add { } remove { } }

        /// <summary>An explicitly implemented property.</summary>
        int ITestBaseInterface.InterfaceShadowedTestProperty
        {
            get => default;
            set { }
        }

        /// <summary>An explicitly implemented method.</summary>
        void ITestBaseInterface.BaseTestMethod()
        {
        }

        /// <summary>An explicitly implemented method.</summary>
        /// <returns>Returns the most magical of the numbers, only the lucky one.</returns>
        int ITestBaseInterface.InterfaceShadowedTestMethod()
            => default;
    }
}