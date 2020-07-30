namespace CodeMap.Tests.Data
{
    /// <summary/>
    [Test("delegate test 1", Value2 = "delegate test 2", Value3 = "delegate test 3")]
    [return: Test("delegate return test 1", Value2 = "delegate return test 2", Value3 = "delegate return test 3")]
    public delegate void TestDelegate<TParam>([Test("delegate parameter test 1", Value2 = "delegate parameter test 2", Value3 = "delegate parameter test 3")] int param);
}