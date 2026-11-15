# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2026-05-03

### Added

- Initial release.
- Dependency cycle detection.
- Topological sort with level grouping for independent build stages.
- Parallel builds for projects within the same topological level.
- Mermaid graph generation for project dependencies and build levels.
- `dotnet clean` across the resolved dependency graph.
- Global .NET tool packaging under `deeplydep`.

[1.0.0]: https://github.com/Shaikh-Imran/DeeplyDepBuilder/releases/tag/1.0.0
