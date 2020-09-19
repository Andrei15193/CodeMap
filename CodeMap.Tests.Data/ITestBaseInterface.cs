using System;

namespace CodeMap.Tests.Data
{
    /// <summary>This is an interface that is inherited by <see cref="ITestExtendedBaseInterface"/> in order to test interface inheritance documentation.</summary>
    /// <seealso cref="ITestExtendedBaseInterface"/>
    public interface ITestBaseInterface
    {
        /// <summary>This is an event that is shadowed in <see cref="ITestInterface{TParam}"/>.</summary>
        /// <seealso cref="ITestInterface{TParam}.InterfaceShadowedTestEvent"/>
        event EventHandler InterfaceShadowedTestEvent;

        /// <summary>This is a property that is shadowed in <see cref="ITestInterface{TParam}"/>.</summary>
        /// <seealso cref="ITestInterface{TParam}.InterfaceShadowedTestProperty"/>
        int InterfaceShadowedTestProperty { get; set; }

        /// <summary>This is a base method that is inherited.</summary>
        void BaseTestMethod();

        /// <summary>This is a method that is shadowed in <see cref="ITestInterface{TParam}"/>.</summary>
        /// <returns>Returns a number, or at least that what we would like.</returns>
        /// <seealso cref="ITestInterface{TParam}.InterfaceShadowedTestMethod"/>
        int InterfaceShadowedTestMethod();
    }
}