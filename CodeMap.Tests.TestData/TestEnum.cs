namespace CodeMap.Tests.TestData
{
    [Test("enum test 1", Value2 = "enum test 2", Value3 = "enum test 3")]
    internal enum TestEnum : byte
    {
        [Test("enum member test 1", Value2 = "enum member test 2", Value3 = "enum member test 3")]
        TestMember1,
        TestMember2,
        TestMember3,
    }
}