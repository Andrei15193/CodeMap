---
title: GitHub Pages Themes
layout: Bootstrap@4.5.0
card_relative_url: card.png
excerpt: 'This category contains a number of themes all of which generate static websites aimed to be deployed through <a href="https://pages.github.com/">GitHub Pages</a> taking advantage of <a href="https://jekyllrb.com/">Jekyll</a> features, allowing multiple themes to be deployed at the same time, as well as being able to host the documentation for multiple versions of a library at the same time.'
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

GitHub Pages Themes
-------------------