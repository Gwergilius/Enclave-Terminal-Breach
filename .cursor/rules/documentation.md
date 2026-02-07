# Documentation Standards

## Documentation Language

**All documentation must be written in English.**

This includes:
- README files
- Architecture documentation
- Setup guides
- API documentation
- Code comments in documentation files
- Markdown files in the `docs/` directory

## Hungarian Translation

**Alongside each official English documentation file, create a Hungarian version.**

- **Naming:** Use the same filename with `.hu.md` suffix (e.g. `README.md` → `README.hu.md`).
- **Content:** Translate the full content of the English document into Hungarian.
- **Cross-references:** In Hungarian documents, links to other documentation must point to the Hungarian version. Use `referenced-document.hu.md` (or the appropriate `.hu.md` path), not `referenced-document.md`.
- **Version control:** `.hu.md` files are excluded from version control via `.gitignore` (pattern: `*.hu.md`); do not add them to the repository.
- **Scope:** Apply to README files, setup guides, and other user-facing documentation where a localised version is useful.

## XML Documentation

- XML documentation for all public APIs
- `<summary>` for all classes and methods
- `<param>` for all parameters
- `<returns>` for all return values
- `<exception>` for all exceptions
- `<example>` for usage examples
- Each comment should be written in English

## Code Comments

- Explanation of complex logic
- Business rules in English
- Description of edge cases in English
- Notes on future developments

## Code Style

- Follow PowerShell best practices for runbooks
- Use consistent naming conventions as defined in the architecture proposal
- Maintain code-based documentation standards

## Markdown Link Style

**All external and internal links in markdown documentation must use reference-style links.**

Reference-style links keep the document body clean and make link management easier. The actual URLs are defined at the end of the document in a dedicated "References" section.

**Format:**
```markdown
## Section
- [Link Text][link-id] description

...

[//]: #References
[link-id]: https://example.com/url
```

**Example:**
```markdown
## Prerequisites
- [Postman] installed on your machine

...

[//]: #References
[Postman]: https://www.postman.com/downloads/
```

**Rules:**
- Use `[//]: #References` comment to mark the references section. There is no space between '#' and 'References' (Actually, it should follow a bookmark-reference format)
- Place all link references at the end of the document
- Use descriptive link IDs (e.g., `[postman]`, `[azure-functions]`, `[api-endpoints]`)
- Keep link IDs lowercase with hyphens for readability
- Include both external URLs and relative paths for internal documentation links

---
alwaysApply: true
---
