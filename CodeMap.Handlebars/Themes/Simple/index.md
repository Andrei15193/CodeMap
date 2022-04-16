---
title: Simple Themes
layout: Bootstrap@4.5.0
card_relative_url: card.png
excerpt: 'Contains a number of simple themes that generate a static website for only one version of a library. Themes in this category provide an easy way of using CodeMap with minimal configuration making it easy to test the library.'
dropdown:
- Themes
---

<nav aria-label="breadcrumb">
    <ol class="breadcrumb">
        {% for page in site.pages %}
            {% if page.path == 'Themes/index.md' %}
                <li class="breadcrumb-item" aria-current="page">
                    <a href="{{ page.url }}">{{ page.title }}</a>
                </li>
            {% endif %}
        {% endfor %}
        <li class="breadcrumb-item active" aria-current="page">
            {{ page.title }}
        </li>
    </ol>
</nav>

Simple category
---------------