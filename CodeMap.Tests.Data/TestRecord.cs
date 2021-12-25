using System;

namespace CodeMap.Tests.Data
{
    /// <summary>
    /// A test record having everything that can be declared on a record.
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
    /// <seealso cref="TestRecord{TParam}.TestMethod(int, string)"/>
    [Test("record test 1", Value2 = "record test 2", Value3 = "record test 3")]
    public record TestRecord<TParam> : TestBaseRecord, ITestExtendedBaseInterface
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
        public class NestedTestClass
        {
        }

        /// <summary>A nested struct.</summary>
        public struct NestedTestStruct
        {
        }

        /// <summary>A nested record.</summary>
        public record NestedTestRecord
        {
        }

        /// <summary>A test constant, with attributes.</summary>
        /// <example>
        /// This is an example, where you usually have an use case with some sample code to illustrate how the defined type is useful.
        /// </example>
        /// <remarks>
        /// You can also include some remarks.
        /// </remarks>
        /// <seealso cref="TestRecord{TParam}.TestMethod(int, string)"/>
        [Test("record constant test 1", Value2 = "record constant test 2", Value3 = "record constant test 3")]
        private const double TestConstant = 1;

        /// <summary>A constant that shadows <see cref="TestBaseRecord.RecordShadowedTestConstant"/>.</summary>
        public new const float RecordShadowedTestConstant = 2;

        /// <summary>A test field, with attributes.</summary>
        /// <example>
        /// This is an example, where you usually have an use case with some sample code to illustrate how the defined type is useful.
        /// </example>
        /// <remarks>
        /// You can also include some remarks.
        /// </remarks>
        /// <seealso cref="TestRecord{TParam}.TestMethod(int, string)"/>
        [Test("record field test 1", Value2 = "record field test 2", Value3 = "record field test 3")]
        private protected byte TestField;

        /// <summary>A read-only field.</summary>
        private readonly char ReadonlyTestField;

        /// <summary>A static field.</summary>
        private static string StaticTestField;

        /// <summary>A field that shadows <see cref="TestBaseRecord.ShadowedTestField"/>.</summary>
        protected new int ShadowedTestField;

        /// <summary>A static constructor, some static values intialization documentation can be added here.</summary>
        static TestRecord()
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
        /// <seealso cref="TestRecord{TParam}.TestMethod(int, string)"/>
        [Test("record constructor test 1", Value2 = "record constructor test 2", Value3 = "record constructor test 3")]
        public TestRecord([Test("record constructor parameter test 1", Value2 = "record constructor parameter test 2", Value3 = "record constructor parameter test 3")] int param)
            : base(param)
        {
        }

        /// <summary>An event shadowing <see cref="TestBaseRecord.RecordShadowedTestEvent"/>.</summary>
        public new event EventHandler RecordShadowedTestEvent;

        /// <summary>An implemented abstract event.</summary>
        /// <seealso cref="TestBaseRecord.AbstractTestEvent"/>
        public override event EventHandler AbstractTestEvent;

        /// <summary>A sealed overridden event.</summary>
        /// <see cref="TestBaseRecord.VirtualTestEvent"/>
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
        /// <seealso cref="TestRecord{TParam}.TestMethod(int, string)"/>
        [Test("record event test 1", Value2 = "record event test 2", Value3 = "record event test 3")]
        public event EventHandler<EventArgs> TestEvent
        {
            [Test("record event adder test 1", Value2 = "record event adder test 2", Value3 = "record event adder test 3")]
            [return: Test("record event adder return test 1", Value2 = "record event adder return test 2", Value3 = "record event adder return test 3")]
            add
            {
            }
            [Test("record event remover test 1", Value2 = "record event remover test 2", Value3 = "record event remover test 3")]
            [return: Test("record event remover return test 1", Value2 = "record event remover return test 2", Value3 = "record event remover return test 3")]
            remove
            {
            }
        }

        /// <summary>An implemented abstract property.</summary>
        public override byte AbstractTestProperty { get; set; }

        /// <summary>A sealed overridden property.</summary>
        /// <seealso cref="TestBaseRecord.VirtualTestProperty"/>
        public sealed override string VirtualTestProperty { get; set; }

        /// <summary>A property shadowing <see cref="TestBaseRecord.RecordShadowedTestProperty"/>.</summary>
        public new int RecordShadowedTestProperty { get; set; }

        /// <summary>An implemented property declared by <see cref="ITestBaseInterface"/>.</summary>
        public int InterfaceShadowedTestProperty { get; set; }

        /// <summary>An indexer, with attributes.</summary>
        /// <param name="param">Just a parameter, should be an index of something.</param>
        /// <returns>Returns the value at the given <paramref name="param"/> index.</returns>
        [Test("record indexer test 1", Value2 = "record indexer test 2", Value3 = "record indexer test 3")]
        public int this[[Test("record indexer parameter test 1", Value2 = "record indexer parameter test 2", Value3 = "record indexer parameter test 3")] int param]
        {
            [Test("record indexer getter test 1", Value2 = "record indexer getter test 2", Value3 = "record indexer getter test 3")]
            [return: Test("record indexer getter return test 1", Value2 = "record indexer getter return test 2", Value3 = "record indexer getter return test 3")]
            get => default;
            [Test("record indexer setter test 1", Value2 = "record indexer setter test 2", Value3 = "record indexer setter test 3")]
            [return: Test("record indexer setter return test 1", Value2 = "record indexer setter return test 2", Value3 = "record indexer setter return test 3")]
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
        /// <seealso cref="TestRecord{TParam}.TestMethod(int, string)"/>
        [Test("record property test 1", Value2 = "record property test 2", Value3 = "record property test 3")]
        public byte TestProperty
        {
            [Test("record property getter test 1", Value2 = "record property getter test 2", Value3 = "record property getter test 3")]
            [return: Test("record property getter return test 1", Value2 = "record property getter return test 2", Value3 = "record property getter return test 3")]
            get;
            [Test("record property setter test 1", Value2 = "record property setter test 2", Value3 = "record property setter test 3")]
            [return: Test("record property setter return test 1", Value2 = "record property setter return test 2", Value3 = "record property setter return test 3")]
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
            [Test("record property setter test 1", Value2 = "record property setter test 2", Value3 = "record property setter test 3")]
            [return: Test("record property setter return test 1", Value2 = "record property setter return test 2", Value3 = "record property setter return test 3")]
            init;
        }

        /// <summary>A read-only property, with private setter.</summary>
        public string TestPublicReadPrivateWriteProperty { get; private set; }

        /// <summary>A method shadowing <see cref="TestBaseRecord.RecordShadowedTestMethod"/>.</summary>
        /// <returns>Returns a value, a number to be more precise.</returns>
        public new int RecordShadowedTestMethod()
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
        /// <seealso cref="TestRecord{TParam}.TestMethod(int, string)"/>
        [Test("record method test 1", Value2 = "record method test 2", Value3 = "record method test 3")]
        [return: Test("record method return test 1", Value2 = "record method return test 2", Value3 = "record method return test 3")]
        public int TestMethod([Test("record method parameter test 1", Value2 = "record method parameter test 2", Value3 = "record method parameter test 3")] int param1, string param2)
            => default;

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