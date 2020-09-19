using System;

namespace CodeMap.Tests.Data
{
    /// <summary>
    /// A test interface having everything that can be declared on an interface.
    /// <list>
    /// <item>Generic parameters</item>
    /// <item>Events</item>
    /// <item>Properties</item>
    /// <item>Indexer</item>
    /// <item>Methods</item>
    /// <item>Shadowed Events</item>
    /// <item>Shadowed Properties</item>
    /// <item>Shadowed Methods</item>
    /// <item>Attributes both on the type itself as well as on each type of member.</item>
    /// </list>
    /// </summary>
    /// <typeparam name="TParam">A generic parameter, used for different things.</typeparam>
    [Test("interface test 1", Value2 = "interface test 2", Value3 = "interface test 3")]
    public interface ITestInterface<TParam> : ITestExtendedBaseInterface
    {
        /// <summary>An event, with attributes.</summary>
        [Test("interface event test 1", Value2 = "interface event test 2", Value3 = "interface event test 3")]
        event EventHandler<EventArgs> TestEvent;

        /// <summary>A shadowed event that is initially declared by <see cref="ITestBaseInterface.InterfaceShadowedTestEvent"/>.</summary>
        /// <remarks>
        /// Keep in mind, there will be two events with the same name declared by the implementing type, one of them will be explicitly
        /// implemented.
        /// </remarks>
        new event EventHandler InterfaceShadowedTestEvent;

        /// <summary>A property, with attributes.</summary>
        /// <value>This value is stored, not computed. How interfaces can demand such things, silly documentation.</value>
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

        /// <summary>A shadowed property that is initially declared by <see cref="ITestBaseInterface.InterfaceShadowedTestProperty"/>.</summary>
        /// <remarks>
        /// Keep in mind, there will be two properties with the same name declared by the implementing type, one of them will be explicitly
        /// implemented.
        /// </remarks>
        new int InterfaceShadowedTestProperty { get; set; }

        /// <summary>An indexer, with attributes.</summary>
        /// <param name="param">Just a parameter, should be an index of something.</param>
        /// <returns>Returns the value at the given <paramref name="param"/> index.</returns>
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

        /// <summary>A method, with attributes.</summary>
        /// <param name="param">A parameter that is required for some internal computation.</param>
        [Test("interface method test 1", Value2 = "interface method test 2", Value3 = "interface method test 3")]
        [return: Test("interface method return test 1", Value2 = "interface method return test 2", Value3 = "interface method return test 3")]
        void TestMethod([Test("interface method parameter test 1", Value2 = "interface method parameter test 2", Value3 = "interface method parameter test 3")] int param);

        /// <summary>A shadowed method that is initially declared by <see cref="ITestBaseInterface.InterfaceShadowedTestMethod"/>.</summary>
        /// <returns>Returns a value based on the state of the object, what could it be?</returns>
        /// <remarks>
        /// Keep in mind, there will be two methods with the same name declared by the implementing type, one of them will be explicitly
        /// implemented.
        /// </remarks>
        new int InterfaceShadowedTestMethod();
    }
}