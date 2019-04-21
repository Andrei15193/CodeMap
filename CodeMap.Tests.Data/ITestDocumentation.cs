namespace CodeMap.Tests.Data
{
    /// <summary/>
    public interface ITestDocumentation
    {
        /// <summary/>
        int Member { get; }

        /// <summary>
        /// plain text
        /// <paramref name="parameter" test="paramref"/>
        /// <typeparamref name="GenericParameter" test="typeparamref"/>
        /// <see cref="Member" test="see"/>
        /// <c test="c">some code</c>
        ///
        /// <list type="table" test="table">
        ///     <listheader test="listheader" test2="old term">
        ///         <term test2="term">
        ///             plain text
        ///             <paramref name="parameter" test="paramref"/>
        ///             <typeparamref name="GenericParameter" test="typeparamref"/>
        ///             <see cref="Member" test="see"/>
        ///             <c test="c">some code</c>
        ///         </term>
        ///     </listheader>
        ///     <item test="item" test2="old description">
        ///         <description test2="description">
        ///             plain text
        ///             <paramref name="parameter" test="paramref"/>
        ///             <typeparamref name="GenericParameter" test="typeparamref"/>
        ///             <see cref="Member" test="see"/>
        ///             <c test="c">some code</c>
        ///         </description>
        ///         <description test2="description">
        ///             plain text
        ///             <paramref name="parameter" test="paramref"/>
        ///             <typeparamref name="GenericParameter" test="typeparamref"/>
        ///             <see cref="Member" test="see"/>
        ///             <c test="c">some code</c>
        ///         </description>
        ///     </item>
        ///     <item test="item" test2="old description"/>
        /// </list>
        ///
        /// <code test="code">
        ///     some code in a block
        /// </code>
        ///
        /// <list type="bullet" test="unordered list">
        ///     <item test="item" test2="description">
        ///         plain text
        ///         <paramref name="parameter" test="paramref"/>
        ///         <typeparamref name="GenericParameter" test="typeparamref"/>
        ///         <see cref="Member" test="see"/>
        ///         <c test="c">some code</c>
        ///     </item>
        ///     <item test="item" test2="old description">
        ///         <description test2="description">
        ///             plain text
        ///             <paramref name="parameter" test="paramref"/>
        ///             <typeparamref name="GenericParameter" test="typeparamref"/>
        ///             <see cref="Member" test="see"/>
        ///             <c test="c">some code</c>
        ///         </description>
        ///     </item>
        ///     <item test="item" test2="description"/>
        /// </list>
        ///
        /// <list type="number" test="ordered list">
        ///     <item test="item" test2="description">
        ///         plain text
        ///         <paramref name="parameter" test="paramref"/>
        ///         <typeparamref name="GenericParameter" test="typeparamref"/>
        ///         <see cref="Member" test="see"/>
        ///         <c test="c">some code</c>
        ///     </item>
        ///     <item test="item" test2="old description">
        ///         <description test2="description">
        ///             plain text
        ///             <paramref name="parameter" test="paramref"/>
        ///             <typeparamref name="GenericParameter" test="typeparamref"/>
        ///             <see cref="Member" test="see"/>
        ///             <c test="c">some code</c>
        ///         </description>
        ///     </item>
        ///     <item test="item" test2="description"/>
        /// </list>
        ///
        /// plain text
        /// <paramref name="parameter" test="paramref"/>
        /// <typeparamref name="GenericParameter" test="typeparamref"/>
        /// <see cref="Member" test="see"/>
        /// <c test="c">some code</c>
        ///
        /// <list test="definition list">
        ///     <listheader test="listheader">
        ///         plain text
        ///         <paramref name="parameter" test="paramref"/>
        ///         <typeparamref name="GenericParameter" test="typeparamref"/>
        ///         <see cref="Member" test="see"/>
        ///         <c test="c">some code</c>
        ///     </listheader>
        ///     <item test="item" test2="old description">
        ///         <term test="term">
        ///             plain text
        ///             <paramref name="parameter" test="paramref"/>
        ///             <typeparamref name="GenericParameter" test="typeparamref"/>
        ///             <see cref="Member" test="see"/>
        ///             <c test="c">some code</c>
        ///         </term>
        ///         <description test2="description">
        ///             plain text
        ///             <paramref name="parameter" test="paramref"/>
        ///             <typeparamref name="GenericParameter" test="typeparamref"/>
        ///             <see cref="Member" test="see"/>
        ///             <c test="c">some code</c>
        ///         </description>
        ///     </item>
        ///     <item test="item" test2="old description"></item>
        /// </list>
        /// </summary>
        void TestMethod<GenericParameter>(int parameter);
    }
}