using System;

namespace CodeMap.Tests.TestData
{
    internal interface ITestGenericParameter<out TParam1, in TParam2, TParam3, TParam4, TParam5, TParam6>
        where TParam2 : class
        where TParam3 : struct
        where TParam4 : new()
        where TParam5 : TParam1
        where TParam6 : TParam1, IComparable<TParam1>
    {
    }
}