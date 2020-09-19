namespace CodeMap.Tests.Data
{
    /// <summary>A test enum having everything that can be declared on an enum.</summary>
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
    [Test("enum test 1", Value2 = "enum test 2", Value3 = "enum test 3")]
    public enum TestEnum : byte
    {
        /// <summary>An enum member, with attributes.</summary>
        [Test("enum member test 1", Value2 = "enum member test 2", Value3 = "enum member test 3")]
        TestMember1,
        /// <summary>A second enum member.</summary>
        TestMember2,
        /// <summary>A third enum member.</summary>
        TestMember3,
    }
}