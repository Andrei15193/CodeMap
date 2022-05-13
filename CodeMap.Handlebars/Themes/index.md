---
title: Themes
layout: Themes
dropdown:
- Themes
---
Browse themes available with `CodeMap.Handlebars` for generating documentation websites for your .NET projects.

{% include subdirectory_browser.html button_label='View Theme Category' %}

All themes are made available through the package itself as embedded resources. This simplifies deployment and ensures that a theme is fully compatible with the package itself. This may get confusing when it comes to checking out themes and available files because both `CodeMap.Handlebars` and available themes have their own versions.

Themes are versioned to ensure backwards compatibility with the ones that were previously deployed and allow for newer features, or changes, to be made available for future deployments. If a theme is based on a 3rd party UI library, such as Bootstrap, MaterialUI or FluentUI then the theme version correlates with the one of 3rd party library. For instance, the Bootstrap 4.5.0 theme uses Bootstrap 4.5.0. This is in an effort to reduce confusion and make it easier to know what dependencies are being used.

Each release of `CodeMap.Handlebars` has a version of itself, as any other package, and with each release there may be other themes that are added or modified. To correctly identify the available themes, pick the `CodeMap.Handlebars` version from the dropdown that you are currently using. To get to the exact theme version that you are using, navigate to the respective theme version for the `CodeMap.Handlebars` version that you are currently using.

Themes are grouped into categories, each category aims to provide a number of features or have particularities about deployment. For instance, the GitHub Pages category is meant to generate documentation sites hosted on <a href="https://pages.github.com/">GitHub Pages</a>, while the Simple category is meant to generate HTML files that can be hosted anywhere, even browsed locally.

As mentioned above, themes are versioned themselves. However, files that are available for a specific theme can come from multiple levels. Globally available files are stored at the root of the Themes embedded directory, these are generally [Handlebars](https://handlebarsjs.com/) templates that are theme and theme category agnostic. A good example for this are the documentation [Handlebars partials](https://handlebarsjs.com/guide/partials.html) which are used to generate HTML for documentation written through the XML tags in C# source code.

The next level is the Theme Category level, these files are specific to the category containing them only. For instance, the GitHub Pages category provides a [Jekyll Include](https://jekyllrb.com/docs/includes/) for generating dropdown menus from pages, particularly useful for hosting multiple versions of a library. Whenever a new version is added a new item is added in the dropdown and all pages get automatically updated. Items matching a [SemVer](https://semver.org/) specification are sorted accordingly.

The 3rd level is the Theme itself, but not a specific theme version. At this level files can be made available for all versions of that specific theme. Once again, these can be helpers or common templates.

Finally, the 4th level is the Specific Theme Version level. Files at this level are available only to that specific theme version alone and is the ideal place to have the CSS and JavaScript files.

Multiple themes can be deployed at the same time and be used on different pages of the same site. In order to provide this, files are either named or placed in directories resembling both the theme and the version they belong to. For instance, the layout page for the Bootstrap 4.5.0 theme from GitHub Pages is named `bootstrap@4.5.0`, while includes are placed in in `Bootstrap/4.5.0/` ([Jekyll Includes](https://jekyllrb.com/docs/includes/) have restrictions on the file name, but not on the directory names). This will allow users to switch to a different version of Bootstrap while the already deployed pages will still use the previous version and be displayed the same way.

All dependencies are provided with the website, there are no external links to JavaScript, CSS, font, images and so on from the generated pages. Everything is meant to be hosted on the documentation website to remove dependencies to 3rd party hosting systems and ensure that the dependent files will always be available.

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