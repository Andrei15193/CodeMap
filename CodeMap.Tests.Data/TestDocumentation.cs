namespace CodeMap.Tests.Data
{
    /// <summary>
    /// A class providing a sample for everything that can be done with the documentation tags. Every inline markup such as
    /// lists, inline code snippets, code blocks, references and so on.
    /// </summary>
    public class TestDocumentation
    {
        /// <summary>A member to reference.</summary>
        public int Member { get; }

        /// <summary>
        /// <para>
        /// Plain text followed by a parameter reference: <paramref name="parameter" test="paramref"/> and then
        /// by a generic parameter reference: <typeparamref name="GenericParameter" test="typeparamref"/> and then
        /// by a member reference: <see cref="Member" test="see"/>.
        /// </para>
        /// <para>
        /// In the second paragraph we have an inline code snippet: <c test="c">some code</c>. And following up with a table.
        /// </para>
        ///
        /// <list type="table" test="table">
        ///     <listheader test="listheader" test2="old term">
        ///         <term test2="term">The first column</term>
        ///     </listheader>
        ///     <item test="item" test2="old description">
        ///         <description test2="description">The first column in the first row.</description>
        ///         <description test2="description">The second column (auto generated) in the first row</description>
        ///     </item>
        /// </list>
        ///
        /// <code test="code">
        ///     This is a code block, we can use Pygments for syntax highlighting.
        /// </code>
        /// 
        /// <code language="C#">
        ///     public class LikeSo
        ///     {
        ///         public void MyMethodThatDoesStuff()
        ///         {
        ///         }
        ///     }
        /// </code>
        ///
        /// <para>Following up with a bullet (unordered) list.</para>
        /// 
        /// <list type="bullet" test="unordered list">
        ///     <item test="item" test2="description">The first item.</item>
        ///     <item test="item" test2="old description">
        ///         <description test2="description">The second item.</description>
        ///     </item>
        /// </list>
        /// 
        /// <para>Following up with a numbered (ordered) list.</para>
        ///
        /// <list type="number" test="ordered list">
        ///     <item test="item" test2="description">The first item.</item>
        ///     <item test="item" test2="old description">
        ///         <description test2="description">The second item.</description>
        ///     </item>
        /// </list>
        ///
        /// <para>Finally, a definition list.</para>
        ///
        /// <list test="definition list">
        ///     <listheader test="listheader">This is the definition list header.</listheader>
        ///     <item test="item" test2="old description">
        ///         <term test="term">The first term.</term>
        ///         <description test2="description">The description of the first term.</description>
        ///     </item>
        ///     <item test="item" test2="old description">
        ///         <term test="term">The second term.</term>
        ///         <description test2="description">The description of the second term.</description>
        ///     </item>
        /// </list>
        /// </summary>
        /// <typeparam name="GenericParameter">We can have inline markup elements basically anywhere. Like <c>here, some inline code</c>, and make a <see cref="Member"/> reference.</typeparam>
        /// <param name="parameter">Same here, <c>inline code</c>. You can use this stuff anywhere, it's really powerful.</param>
        public void TestMethod<GenericParameter>(int parameter)
        {
        }
    }
}