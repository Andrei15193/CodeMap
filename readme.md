![.NET Core](https://github.com/Andrei15193/CodeMap/workflows/.NET%20Core/badge.svg?branch=master&event=push)

Visual Studio enables developers to write comprehensive documentation inside
their code using XML in three slashed comments,
see [XML Documentation Comments (C# Programming Guide)](https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/xml-documentation-comments) for
more details about supported tags.

When we build a project that has been configured to generate XML documentation
files (simply specify the output path where resulting files should be saved)
Visual Studio will look for this file and display the respective documentation
in IntelliSense.

This is great and all library owners should be writing documentation like this
in their code, however there aren't that many tools out there that allow us to
transform the XML generated documentation into something else, say JSON or
custom HTML, to display the exact same documentation on the project's website.

Having a single source for documentation is the best way to ensure consistency,
what you see in code is what you see on the project's website. There are no
discrepancies. Writing all documentation in code may upset developers, but
after all they know best what the code they are writing is all about.

CodeMap aims to bring a tool into the developers hands that allows them to
generate any output format they wish from a given assembly and associated
XML documentation.

This is done following the visitor design pattern, an assembly is being visited
and all elements that make up an assembly get their turn. When you want to
generate a specific output (say Markdown or Creole) you simply implement your
own visitor.

There are some elements that do not have XML documentation correspondents, such
as assemblies, modules and namespaces. You cannot write documentation for
neither in code and have it generated in the resulting XML document.

This tool does not work directly on the XML file but rather on a representation
of the file (XDocument), this allows for the document to be loaded, updated if
necessary and only then processed. This enables the tool to support additions
to the standard XML document format allowing for extra documentation to be
manually added. If you want to add specific documentation to a namespace, you
can, but you have to do it manually which means you follow the XML document
structure as well.

This project exposes an XML Schema (.xsd) that is used for validating an XML
document containing documentation to ensure that it is being correctly
processed.

Exceptions
----------

Some elements are ignored when processing the XML document because they either
are complex to use or there is little support for them.

* [include](https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/include)
is being ignored because its usage is rather complex and can be misleading.
The documentation is being kept separate from the source code which can easily
lead to discrepancies when changes to code occur.

* [permission](https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/permission)
is being ignored because its usage is more of an edge case and code permissions
are not supported in .NET Core making this element obsolete in this case.

Interpretations
---------------

There are XML elements defined for various sections and limited markup support.
The main sections for documentation are the following:

* [summary](https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/summary)
* [typeparam](https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/typeparam) (available only for types and methods)
* [param](https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/param) (available only for delegates, constructors, methods, properties with parameters)
* [returns](https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/returns) (available only for delegates and methods with result)
* [exception](https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/exception) (available only for delegates, constructors, methods and properties)
* [remarks](https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/remarks)
* [example](https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/example)
* [value](https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/value) (available only for properties)
* [seealso](https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/seealso)

All of the above sections can contain documentation made out of blocks, text
and limited markup. [seealso](https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/seealso) does
not contain any content and are interpreted as references.

The content blocks are made using the following tags:

* [para](https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/para)
* [code](https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/code)
* [list](https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/list)

It is not mandatory to specify the [para](https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/para) element
because it is inferred. It is mandatory to do so only when you want to
distinguish two paragraphs that are one after the other, but if there is plain
text that has a [code](https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/code)
or a [list](https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/list) element
then the text before the respective tag is considered a paragraph and the text
afterwards is considered a separate paragraph.

For instance, the following [returns](https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/returns) section
contains three paragraphs, a code block and a list.

```xml
<remarks>
This is my first paragraph.
<code>
This is a code block
</code>
This is my second paragraph.
<list>
    <item>Item 1</item>
    <item>Item 2</item>
    <item>Item 3</item>
</list>
This is my third paragraph.
</remarks>
```

To distinguish two paragraphs one after the other you need to use the [para](https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/para) tag.

```xml
<remarks>
<para>
    This is my first paragraph.
</para>
<para>
    This is my second paragraph.
</para>
</remarks>
```

Each content block can contain plain text and limited markup using the following tags:

* [c](https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/code-inline)
* [see](https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/see)
* [paramref](https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/paramref)
* [typeparamref](https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/typeparamref)

Some XML elements lack a proper documentation and are not properly interpreted
by Visual Studio.

[List](https://docs.microsoft.com/dotnet/csharp/programming-guide/xmldoc/list) is
one of these elements, regardless of how you define the list element, Visual
Studio will display the contents as plain text. This is understandable, there
should not be such complex markup in the summary of a method or parameter
description.

One the other hand, the lack of documentation for this element and the lack of
proper interpretation allows for more flexibility when it comes to parsing
lists. This tool parses lists as follows:

### Simple Lists

This corresponds to list type `bullet` and `number` that have no `listheader`
element.

To define a list simply use the `list` element, specify a type (the default is
`bullet`) and then define each item using an `item` element. Optionally include
a `description` subelement (this is more for compliance with the documentation,
having a `description` element or not makes no difference).

#### Examples

Bullet (unordered) list:

```xml
<list type="bullet">
    <item>this is an item</item>
    <item>
        <description>this is an item using description, it is the same as the first one</description>
    </item>
    <item>the description element is optional, use at your discretion</item>
</list>
```

Number (ordered) list:

```xml
<list type="number">
    <item>first item</item>
    <item>second item</item>
    <item>third item</item>
</list>
```

### Definition Lists

This is where it can get a bit confusing. The definition list is inferred by
the given list type (`bullet` or `number`) or lack of this attribute.
Definition lists cannot be ordered or unordered, they define a list of terms.
This makes the type attribute rather useless in this case, it can be safely
omitted since the default is `bullet` and alligns with the interpretation.

To create a definition list it must either have a title or one of its items
to contain the `term` element. The title is optional, however if the `list`
element is empty it will not be inferred as a definition list!

Optionally, a definition list can have a title which is defined using the
`listheader` element. The title can be written directly in the element or
can be wrapped in a `term` element.

#### Examples

Simple definition list:

```xml
<list>
    <item>
        <term>Exception</term>
        <description>Describes an abnormal behaviour</description>
    </item>
    <item>
        <term>Logging</term>
        <description>Something we should be doing</description>
    </item>
    <item>
        <term>HTTP</term>
        <description>HyperText Transfer Protocol, today's standard for communication</description>
    </item>
</list>
```

Definition list with title:

```xml
<list>
    <listheader>Music Genres</listheader>
    <item>
        <term>Pop</term>
        <description>Very popular (hence the name) genre, mostly singing nonsence.</description>
    </item>
    <item>
        <term>Rock</term>
        <description>Experiences with depth that are very musical with some grotesque exceptions that seem more like yelling than singing.</description>
    </item>
    <item>
        <term>EDM</term>
        <description>Electronic Dance Music, less singing mostly used for background noise or to help get into a specific mood.</description>
    </item>
</list>
```

Same definition list, we can wrap the title in a `term` element to conform with
the documentation:

```xml
<list>
    <listheader>
        <term>Music Genres</term>
    </listheader>
    <item>
        <term>Pop</term>
        <description>Very popular (hence the name) genre, mostly singing nonsence.</description>
    </item>
    <item>
        <term>Rock</term>
        <description>Experiences with depth that are very musical with some grotesque exceptions that seem more like yelling than singing.</description>
    </item>
    <item>
        <term>EDM</term>
        <description>Electronic Dance Music, less singing mostly used for background noise or to help get into a specific mood.</description>
    </item>
</list>
```

A definition list without a term for its second item:

```xml
<list>
    <item>
        <term>LOL</term>
        <description>Usually meaning laughing out loud, can be confused with League of Legends, a very popular video game.</description>
    </item>
    <item>
        <description>For some reason I forgot to mention the term I am currently describing, Infer this!</description>
    </item>
</list>
```

### Tables

This is where things will get a bit interesting. Tables are useful in a lot of
scenarios, however there is no explicit syntax for them.

To define a table specify the `type` of a `list` to `table`. To define the
header rows (if any) use the `listheader` element, to define each column
header specify multiple `term` elements.

To define each row use the `item` element and for each column use a
`description` element.

The number of columns is determined by the maximum number of `term` or
`description` elements found in an enclosing element (`listheader` or `item`).
If there are missing values for a column header or row then they will be filled
with blank.

#### Examples

Simple table:

```xml
<list type="table">
    <listheader>
        <term>Column 1</term>
        <term>Column 2</term>
    </listheader>
    <item>
        <description>Row 1, Column 1</description>
        <description>Row 1, Column 2</description>
    </item>
    <item>
        <description>Row 2, Column 1</description>
        <description>Row 2, Column 2</description>
    </item>
    <item>
        <description>Row 3, Column 1</description>
        <description>Row 3, Column 2</description>
    </item>
</list>
```

Table without heading:

```xml
<list type="table">
    <item>
        <description>Row 1, Column 1</description>
        <description>Row 1, Column 2</description>
    </item>
    <item>
        <description>Row 2, Column 1</description>
        <description>Row 2, Column 2</description>
    </item>
    <item>
        <description>Row 3, Column 1</description>
        <description>Row 3, Column 2</description>
    </item>
</list>
```

Table with missing values for last column:

```xml
<list type="table">
    <listheader>
        <term>Column 1</term>
    </listheader>
    <item>
        <description>Row 1, Column 1</description>
    </item>
    <item>
        <description>Row 2, Column 1</description>
        <description>Row 2, Column 2</description>
    </item>
    <item>
        <description>Row 3, Column 1</description>
    </item>
</list>
```

Additions
---------

There are no additions at this moment.
