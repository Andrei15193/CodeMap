using System;

namespace CodeMap.Tests.Data
{
    /// <summary>A test delegate having everything that can be declared by a delegate.</summary>
    /// <typeparam name="TParam">A generic parameter, for various things.</typeparam>
    /// <param name="param">A parameter, with attribute.</param>
    /// <returns>Returns a number, only a number. That's it, that's all you get.</returns>
    /// <example>
    /// This is an example, where you usually have an use case with some sample code to illustrate how the defined type is useful.
    /// </example>
    /// <remarks>
    /// You can also include some remarks.
    /// </remarks>
    /// <exception cref="ArgumentException">You can also include exceptions.</exception>
    [Test("delegate test 1", Value2 = "delegate test 2", Value3 = "delegate test 3")]
    [return: Test("delegate return test 1", Value2 = "delegate return test 2", Value3 = "delegate return test 3")]
    public delegate int TestDelegate<TParam>([Test("delegate parameter test 1", Value2 = "delegate parameter test 2", Value3 = "delegate parameter test 3")] int param);
}