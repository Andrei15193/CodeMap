---
title: Themes
layout: Bootstrap@4.5.0
is_dropdown_landing_page: true
dropdown:
- Themes
---

<nav aria-label="breadcrumb">
    <ol class="breadcrumb">
        <li class="breadcrumb-item active" aria-current="page">
            {{ page.title }}
        </li>
    </ol>
</nav>

CodeMap.Handlebars Themes
-------------------------

Browse themes available with `CodeMap.Handlebars` for generating documentation websites for your .NET libraries.

Each theme belongs to a category which aims to provide a number of features or target specific ways of deploying the website. Each theme can have multiple versions and at each level (global, category, theme and version) the theme files can be overridden and new files can be added allowing customization at any level, even rewriting a theme completely.

Following the same directory structure, new themes can be added in any other assembly which can be loaded when generating documentation websites allowing for any project to simply add themes and make the available for usage without these themes being included in the `CodeMap.Handlebars` NuGet package. All theme files are included in the `CodeMap.Handlebars` assembly as embedded resources.

<div class="container p-0">
    <div class="row row-cols-1 row-cols-sm-2 row-cols-md-3 no-gutters" style="margin-right: -1.5rem;">
        {% assign directory_path_size = page.path.size | minus: page.name.size %}
        {% assign directory_path = page.path | slice: 0, directory_path_size %}
        {% for theme_category_page in site.pages %}
            {% if theme_category_page.path.size > page.path.size %}
                {% assign theme_category_subdirectory_name_size = theme_category_page.path.size | minus: directory_path_size | minus: theme_category_page.name.size %}
                {% assign theme_category_subdirectory_name = theme_category_page.path | slice: directory_path_size, theme_category_subdirectory_name_size | split: '/' %}

                {% assign expected_theme_category_path = directory_path | append: theme_category_subdirectory_name[0] | append: '/' | append: theme_category_page.name %}
                {% assign theme_category_card_image_path = directory_path | append: theme_category_subdirectory_name[0] | append: '/' | append: theme_category_page.card_relative_url %}

                {% if theme_category_page.path == expected_theme_category_path %}
<div class="col pb-3 pr-4">
    <div class="card bg-light h-100">
        <div class="card-header">{{ theme_category_page.title }}</div>
        <img src="{{ theme_category_card_image_path | relative_url }}" alt="{{ theme_category_page.title }} card">
        <div class="card-body d-flex flex-column">
            <p class="card-text">{{ theme_category_page.excerpt }}</p>
            <div class="mt-auto text-right">
                <a href="{{ theme_category_page.url }}" class="btn btn-sm btn-primary" style="text-align: right">View Theme Category</a>
            </div>
        </div>
    </div>
</div>
                {% endif %}
            {% endif %}
        {% endfor %}
    </div>
</div>