---
title: Home
---

Home
====

CodeMap is a library for generating complete documentation out of compiled source code.

The C# compiler comes with the option to output documentation declared on members to a separate XML file, this can be enabled in Visual Studio
per project. Generally, the documentation file has the same name as the assembly name, but instead of the `.dll` extension it is `.xml` in order
to associate the two. This reference is also made in the generated XML file itself, however this library uses the name matching convention by default.

The aim is to generate a graph of objects that represent the assembly and the associated documentation in a way that is closest to the
source code itself. For instance, when you declare a method, it is implicitly [sealed](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/sealed)
unless you use the [virtual](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/virtual) keyword (for classes and structs).
The library will not set the `IsSealed` flag to `true` for non-virtual methods, it will only do so when a method is
[overriden](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/override) and [sealed](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/sealed).

The aim in this regard is to provide a model from which the declaration inside one assembly can be recreated just by using the flags directly.
If the `IsSealed` flag is set to `true`, then that keyword was used. This applies to all flags.

A second area that this library covers is the associated XML documentation by representing elements in a specific model that can be easily traversed.
This model covers the XML documentation elements described in this article [XML Documentation Comments (C# Programming Guide)](https://learn.microsoft.com/dotnet/csharp/programming-guide/xmldoc/xml-documentation-comments).
To check the full list of supported elements and any additions see the CodeMap documentation page, remarks section.

The third, and final area, that this library covers is member references. Both when declaring types and when writing documentation we reference
other members (e.g.: method parameter types reference other types, the [see](https://learn.microsoft.com/dotnet/csharp/programming-guide/xmldoc/see)
XML element is used to reference a member). The aim is to provide enough information to be able to generate a hyperlink to the referenced member
(even if it is part of .NET, like core types such as `string` and `int`).

Each area is represented by a separate model, member declarations for an assembly are represented by DeclarationNodes, documentation that is
associated to a declaration is represented by DocumentationElements and, finally, memeber references are represented by ReferenceData.
Each of these models have their own namespace and one references the other unidirectionally. Declaration Nodes depend on Documentation Elements and
Reference Data; Documentation Elements depends on Reference Data. There are no other dependencies between these models.

Each of these models come with their own visitors (see (visitor pattern)[https://en.wikipedia.org/wiki/Visitor_pattern] for more information)
to ease traversing these object graphs. The included HTML tooling exposes concrete visitors for generating documentation.

For more information check the documentation pages. For a demo, the documentation for this site is generated using this library. You can
have almost anything related to what you would expect from a documentation page. From a simple summary at the top to complex examples containing
highlighed code snippets (see [PygmentSharp](https://github.com/akatakritos/PygmentSharp")).


For suggestions and issues, create GitHub issues. Looking forward to hearing from you.