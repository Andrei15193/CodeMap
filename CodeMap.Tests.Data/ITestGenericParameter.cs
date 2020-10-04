using System;

namespace CodeMap.Tests.Data
{
    /// <summary>This is an interface with a number of generic parameters, used as an example for documenting generic parameters and their constraints.</summary>
    /// <typeparam name="TParam1">No constraints, can be any type, is covariant.</typeparam>
    /// <typeparam name="TParam2">Must be a reference type, is contravariant.</typeparam>
    /// <typeparam name="TParam3">Must be a value type (nullables do not count).</typeparam>
    /// <typeparam name="TParam4">Must have a public parameterless constructor.</typeparam>
    /// <typeparam name="TParam5">Must be of same type as <typeparamref name="TParam1"/> or a subtype of <typeparamref name="TParam1"/>.</typeparam>
    /// <typeparam name="TParam6">Must be of same type as <typeparamref name="TParam1"/> or a subtype of <typeparamref name="TParam1"/> and implement <see cref="IComparable{T}"/> or be <see cref="IComparable{T}"/>.</typeparam>
    /// <example>
    /// For <typeparamref name="TParam6"/> the following configuration is valid, <see cref="int"/> implements <see cref="IComparable{T}"/>.
    /// <code language="c#">
    /// ITestGenericParameter&lt;int, object, int, object, int, int&gt;
    /// </code>
    /// </example>
    public interface ITestGenericParameter<out TParam1, in TParam2, TParam3, TParam4, TParam5, TParam6>
        where TParam2 : class
        where TParam3 : struct
        where TParam4 : new()
        where TParam5 : TParam1
        where TParam6 : TParam1, IComparable<TParam1>
    {
    }
}