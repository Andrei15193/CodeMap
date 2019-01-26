namespace CodeMap.Tests.Data
{
    /// <summary/>
    [Test("enum test 1", Value2 = "enum test 2", Value3 = "enum test 3")]
    public enum TestEnum : byte
    {
        /// <summary/>
        [Test("enum member test 1", Value2 = "enum member test 2", Value3 = "enum member test 3")]
        TestMember1,
        /// <summary/>
        TestMember2,
        /// <summary/>
        TestMember3,
    }
}