---
title: Themes
layout: Themes
is_dropdown_landing_page: true
dropdown:
- Themes
---
Browse themes available with `CodeMap.Handlebars` for generating documentation websites for your .NET projects.

Themes are grouped into categories, each category aims to provide a number of features or have a specific mode of deployment. For instance, the GitHub Pages category provides themes that are aimed to generate websites that are hosted on <a href="https://pages.github.com/">GitHub Pages</a> taking advantage of <a href="https://jekyllrb.com/">Jekyll</a> to provide different features, such as hosting documentation for multiple versions of a library allowing users to browse older documentations when they are not using the latest one yet.

{% include subdirectory_browser.html button_label='View Theme Category' %}

### Remarks

All related files for themes are stored as embedded resources in `CodeMap.Handlebars` making them easily available for anyone using this library. Themes are stored using the following strucutre.

```
Themes/
|-- Theme Category/
|   |-- Theme/
|   |   |-- Version/
|   |   |   |   # Contains files that can be copied as-is to the output directory
|   |   |   |-- Static/
|   |   |   |   # Contains Handlebars templates for generating pages for different
|   |   |   |   # assembly members, such as classes, structs, namespaces and so on.
|   |   |   |-- Templates/
|   |   |   |   # Contains Handlebars templates that are made available as partials
|   |   |   |   # which can be used in base templates.
|   |   |   |-- Partials/
|   |   |
|   |   |-- Static/
|   |   |-- Templates/
|   |   |-- Partials/
|   |
|   |-- Static/
|   |-- Templates/
|   |-- Partials/
|
|-- Static/
|-- Templates/
|-- Partials/
```

There are three dedicated subdirectories which can be specified at any level, `Static`, `Templates`, and `Partials`. These directories contain embedded resources used by a theme, each directory is passed down and can be overridden at any level either partially or completely, the resources are matched by their name in case-insensitive manner. `Static` files at the `Themes`, or global, level are available for all themes. `Static` files at the `Theme Category` level are available only for that category, files at the `Theme` level are available only for that theme and finally we have the version specific files. If there is a file with the same name at both the `Themes`, or global, level and `Theme Category` level, the latter has precedence.

This allows for partials and other helpers to be defined at a more global level and make them available accordingly without having to copy and paste them everywhere. This helps with maintenance, if there are issues with an update, the unmodified file can be copied to a more specific level without introducing breaking changes.

File matching mostly applies to `Templates` and `Partials` where the inner hierachy of directories is flatten and only then the files are matched. Having two template files with the same name in different subdirectories, such as `Templates/Class.hbs` and `Templates/Types/Class.hbs` then they are considered to be the same and only one will be picked, there is no guarantee which one of the two.

`Static` files, on the other hand, can contain files with the same name in different directories as these are files intended to simply be copied to the output directory without any processing. This is a perfect location for images and other helpers.

Customizing themes is done rather easy, embedded resources are loaded using the [EmbeddedResourceBrowser](https://andrei15193.github.io/EmbeddedResourceBrowser/) merge method. The default assemblies from which resources are loaded are `CodeMap.Handlebars` and then the calling assembly, usually a console application that configures `CodeMap` and generates the documentation. To override or add embedded resources that follow the same directory structure as presented above.