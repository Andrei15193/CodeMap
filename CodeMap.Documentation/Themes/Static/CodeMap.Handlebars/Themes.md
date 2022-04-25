---
title: Themes
layout: Bootstrap@4.5.0
is_dropdown_landing_page: true
permalink: 'CodeMap.Handlebars/Themes/'
dropdown:
- Themes
---
{% assign default_pre_release_names = 'alpha,beta,rc' | split: ',' %}
{% assign pre_release_names = site.data.pre_release_names | default: default_pre_release_names %}

{% assign directory_path = page.path | split: '/' %}
{% assign directory_path = directory_path | reverse | slice: 0, directory_path.size | reverse %}

{% assign codemap_handlebars_path = directory_path | reverse | slice: 1, directory_path.size | reverse | join: '/' %}

{% assign versioned_themes_index_pages_info = '' | split: ',' | where: 'an', 'array: [tuple versioned_themes_index_page_info: [page, codemap_handlebars_version: string, major: number, minor: number, path: number, pre_release_name_index: number, pre_release_number: number]]' %}
{% assign other_themes_index_pages_info = '' | split: ',' | where: 'an', 'array: [tuple other_themes_index_page_info: [page, codemap_handlebars_version: string]]' %}
{% for themes_version_index_page in site.pages %}
    {% if themes_version_index_page.name == 'index.md' %}
        {% assign themes_version_index_page_directory_path = themes_version_index_page.path | split: '/' %}
        {% assign themes_version_index_page_directory_path = themes_version_index_page_directory_path | reverse | slice: 1, themes_version_index_page_directory_path.size | reverse %}

        {% assign codemap_handlebars_path_other = themes_version_index_page_directory_path | reverse | slice: 2, themes_version_index_page_directory_path.size | reverse | join: '/' %}
        {% if codemap_handlebars_path == codemap_handlebars_path_other %}
            {% assign codemap_handlebars_version = themes_version_index_page_directory_path | reverse | slice: 1 | first %}

            {% assign is_valid_version = false %}
            {% assign version_parts = codemap_handlebars_version | split: '.' %}
            {% if version_parts.size == 3 %}
                {% assign major = version_parts[0] | plus: 0 %}
                {% assign minor = version_parts[1] | plus: 0 %}
                {% assign patch_with_pre_release = version_parts[2] | split: '-' %}

                {% if patch_with_pre_release.size <= 2 %}
                    {% assign patch = patch_with_pre_release[0] | plus: 0 %}
                    {% assign pre_release = patch_with_pre_release[1] %}

                    {% unless patch_with_pre_release.size == 2 %}
                        {% assign pre_release_name_index = pre_release_names.size %}
                        {% assign pre_release_number = -1 %}
                    {% else %}
                        {% assign pre_release_name_index = '' | plus: '0' %}
                        {% assign pre_release_number = '' | plus: '0' %}

                        {% for pre_release_name in pre_release_names %}
                            {% if pre_release == pre_release_name %}
                                {% assign pre_release_name_index = forloop.index0 %}
                                {% assign pre_release_number = -1 %}
                            {% elsif pre_release contains pre_release_name %}
                                {% assign pre_release_name_index = forloop.index0 %}
                                {% assign pre_release_number = pre_release | slice: pre_release_name.size | plus: 0 %}
                            {% endif %}
                        {% endfor %}
                    {% endunless %}

                    {%
                        assign versioned_themes_index_page_info = '' | split: ',' | where: 'a', 'versioned_themes_index_page_info'
                            | push: themes_version_index_page
                            | push: codemap_handlebars_version
                            | push: major
                            | push: minor
                            | push: patch
                            | push: pre_release_name_index
                            | push: pre_release_number
                    %}
                    {% assign is_valid_version = true %}
                    {% for version_part in versioned_themes_index_page_info offset: 2 %}
                        {% unless version_part <= 0 or version_part >= 0 %}
                            {% assign is_valid_version = false %}
                            {% break %}
                        {% endunless %}
                    {% endfor %}

                    {% if is_valid_version %}
                        {% assign versioned_themes_index_pages_info = versioned_themes_index_pages_info | push: versioned_themes_index_page_info %}
                    {% endif %}
                {% endif %}
            {% endif %}

            {% unless is_valid_version %}
                {%
                    assign other_themes_index_page_info = '' | split: ',' | where: 'a', 'other_themes_index_page_info'
                        | push: themes_version_index_page
                        | push: codemap_handlebars_version
                %}
                {% assign other_themes_index_pages_info = other_themes_index_pages_info | push: other_themes_index_page_info %}
            {% endunless %}
        {% endif %}
    {% endif %}
{% endfor %}

{% for offset in (1..versioned_themes_index_pages_info.size) %}
    {% assign max_versioned_themes_index_page_info = versioned_themes_index_pages_info | slice: forloop.index0 | first %}
    {% assign versioned_themes_index_pages_info_not_sorted = '' | split: ',' | where: 'an', 'array: [versioned_themes_index_page_info]' %}

    {% for versioned_themes_index_page_info in versioned_themes_index_pages_info offset: offset %}
        {% assign is_current_greater = false %}

        {% for version_part_index in (2..max_versioned_themes_index_page_info.size) %}
            {% if versioned_themes_index_page_info[version_part_index] > max_versioned_themes_index_page_info[version_part_index] %}
                {% assign is_current_greater = true %}
                {% break %}
            {% elsif max_versioned_themes_index_page_info[version_part_index] > versioned_themes_index_page_info[version_part_index] %}
                {% break %}
            {% endif %}
        {% endfor %}

        {% if is_current_greater %}
            {% assign versioned_themes_index_pages_info_not_sorted = versioned_themes_index_pages_info_not_sorted | push: max_versioned_themes_index_page_info %}
            {% assign max_versioned_themes_index_page_info = versioned_themes_index_page_info %}
        {% else %}
            {% assign versioned_themes_index_pages_info_not_sorted = versioned_themes_index_pages_info_not_sorted | push: versioned_themes_index_page_info %}
        {% endif %}
    {% endfor %}

    {% assign versioned_themes_index_pages_info = versioned_themes_index_pages_info | slice: 0, forloop.index0 | push: max_versioned_themes_index_page_info %}
    {% for versioned_themes_index_page_info_not_sorted in versioned_themes_index_pages_info_not_sorted %}
        {% assign versioned_themes_index_pages_info = versioned_themes_index_pages_info | push: versioned_themes_index_page_info_not_sorted %}
    {% endfor %}
{% endfor %}

{% for offset in (1..other_themes_index_pages_info.size) %}
    {% assign min_other_themes_index_page_info = other_themes_index_pages_info | slice: forloop.index0 | first %}
    {% assign other_themes_index_pages_info_not_sorted = '' | split: ',' | where: 'an', 'array: [other_themes_index_page_info]' %}

    {% for other_themes_index_page_info in other_themes_index_pages_info offset: offset %}
        {% assign min_other_themes_index_page_version = min_other_themes_index_page_info[1] %}
        {% assign other_themes_index_page_version = other_themes_index_page_info[1] %}

        {% if other_themes_index_page_version < min_other_themes_index_page_version %}
            {% assign other_themes_index_pages_info_not_sorted = other_themes_index_pages_info_not_sorted | push: min_other_themes_index_page_info %}
            {% assign min_other_themes_index_page_info = other_themes_index_page_info %}
        {% else %}
            {% assign other_themes_index_pages_info_not_sorted = other_themes_index_pages_info_not_sorted | push: other_themes_index_page_info %}
        {% endif %}
    {% endfor %}

    {% assign other_themes_index_pages_info = other_themes_index_pages_info | slice: 0, forloop.index0 | push: min_other_themes_index_page_info %}
    {% for other_themes_index_page_info_not_sorted in other_themes_index_pages_info_not_sorted %}
        {% assign other_themes_index_pages_info = other_themes_index_pages_info | push: other_themes_index_page_info_not_sorted %}
    {% endfor %}
{% endfor %}

{% assign latest_themes_index_page_info = nil %}
{% if versioned_themes_index_pages_info.size > 0 %}
    {% assign latest_themes_index_page_info = versioned_themes_index_pages_info | first %}
{% elsif other_themes_index_pages_info.size > 0 %}
    {% assign latest_themes_index_page_info = other_themes_index_pages_info | first %}
{% endif %}

{% unless latest_themes_index_page_info %}
<nav aria-label="breadcrumb">
    <ol class="breadcrumb">
        <li class="breadcrumb-item active" aria-current="page">
            <span title="CodeMap.Handlebars">{{ page.title }}</span>
        </li>
    </ol>
</nav>

<h2>{{ page.title }}</h2>

<p><code>CodeMap.Handlebars</code> does not have any published packages with themes, please check again later.</p>
{% else %}
{% assign latest_themes_index_page = latest_themes_index_page_info[0] %}
{% assign latest_codemap_handlebars_version = latest_themes_index_page_info[1] %}

{% assign latest_themes_index_page_directory_path = latest_themes_index_page.path | split: '/' %}
{% assign latest_themes_index_page_directory_path = latest_themes_index_page_directory_path | reverse | slice: 1, latest_themes_index_page_directory_path.size | reverse %}

<nav aria-label="breadcrumb">
    <ol class="breadcrumb">
        <li class="breadcrumb-item active" aria-current="page">
            <span title="CodeMap.Handlebars@{{ latest_codemap_handlebars_version }}">{{ latest_themes_index_page.title }}@{{ latest_codemap_handlebars_version }}</span>
        </li>
    </ol>
</nav>

<h2>
    {{ latest_themes_index_page.title }}
    <div class="btn-group">
        <button type="button" class="btn btn-sm btn-primary dropdown-toggle" data-toggle="dropdown" aria-expanded="false">
            CodeMap.Handlebars@{{ latest_codemap_handlebars_version }}
        </button>
        <div class="dropdown-menu">
            {% for versioned_themes_index_page_info in versioned_themes_index_pages_info %}
                {% assign themes_version_index_page = versioned_themes_index_page_info[0] %}
                {% assign codemap_handlebars_version = versioned_themes_index_page_info[1] %}
                <a class="dropdown-item{% if codemap_handlebars_version == latest_codemap_handlebars_version %} active{% endif %}" href="{{ themes_version_index_page.url | relative_url }}">CodeMap.Handlebars@{{ codemap_handlebars_version }}</a>
            {% endfor %}
            {% if versioned_themes_index_pages_info.size > 0 and other_themes_index_page_info.size > 0 %}
                <div class="dropdown-divider"></div>
            {% endif %}
            {% for other_themes_index_page_info in other_themes_index_pages_info %}
                {% assign themes_version_index_page = other_themes_index_page_info[0] %}
                {% assign codemap_handlebars_version = other_themes_index_page_info[1] %}
                <a class="dropdown-item{% if codemap_handlebars_version == latest_codemap_handlebars_version %} active{% endif %}" href="{{ themes_version_index_page.url | relative_url }}">CodeMap.Handlebars@{{ codemap_handlebars_version }}</a>
            {% endfor %}
        </div>
    </div>
</h2>

{% capture subdirectory_view_content %}
    {% include subdirectory_browser.html directory_path=latest_themes_index_page_directory_path button_label='View Theme Category' %}
{% endcapture %}

{% capture subdirectory_view_replace %}
{% raw %}
{% include subdirectory_browser.html button_label='View Theme Category' %}
{% endraw %}
{% endcapture %}

{{ latest_themes_index_page.content | replace: subdirectory_view_replace, subdirectory_view_content }}

{% include {{ latest_themes_index_page_directory_path | join: '/' | append: '/files.html' | replace: ' ', '-' }} %}

{% endunless %}